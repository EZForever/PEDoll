using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "hook": Installs a hook.
    // hook
    // hook {[MODULE!]NAME|@ADDR|*PATTERN}
    //      [--convention=CONVENTION] [--stack=STACK[,RETURN]]
    //      [--before [ACTION ...]] [--after [ACTION ...]]

    class CmdHook : ICommand
    {
        // These are only for validating inputs at parse-time

        static readonly string[] conventionsX86 = {
            "stdcall", "cdecl", "fastcall"
        };

        static readonly string[] conventionsX64 = {
            "msvc", "gcc"
        };

        static readonly string[] verdictsBefore = {
            "approve", "reject", "terminate"
        };

        static readonly string[] verdictsAfter = {
            "approve", "terminate"
        };

        static void FSMTest(int target, int current)
        {
            if (target > current)
                throw new ArgumentException("symbol/before/after");
        }

        static string RemoveQuotes(string x)
        {
            return (x[0] == '"' && x[x.Length - 1] == '"') ? x.Substring(1, x.Length - 2) : x;
        }

        // "*8B4C240885D2" => (byte[]){ 0x8B, 0x4C, 0x24, 0x08, 0x85, 0xD2 }
        static byte[] PatternToBinary(string pattern)
        {
            List<byte> ret = new List<byte>();

            // NOTE: This ignores a hex digit if they come in odds
            for(int i = 1; i < pattern.Length; i += 2)
                ret.Add(Convert.ToByte(pattern.Substring(i, 2), 16));

            return ret.ToArray();
        }

        // ----------

        public string HelpResId() => "Commands.Help.Hook";
        public string HelpShortResId() => "Commands.HelpShort.Hook";

        public Dictionary<string, object> Parse(string cmd)
        {
            Threads.HookEntry entry = new Threads.HookEntry();
            entry.beforeActions = new List<Dictionary<string, object>>();
            entry.afterActions = new List<Dictionary<string, object>>();

            int state = 0; // FSM on option parsing; 0 - Begin, 1 - Convention/Stack, 2 - Before, 3 - After

            OptionSet options = new OptionSet()
            {
                {
                    "before",
                    x =>
                    {
                        FSMTest(1, state);

                        if(x != null)
                            state = 2;
                    }
                },
                {
                    "after",
                    x =>
                    {
                        FSMTest(1, state);

                        if(x != null)
                            state = 3;
                    }
                },
                {
                    "convention=",
                    x =>
                    {
                        FSMTest(1, state);

                        if(Array.IndexOf(conventionsX86, x) >= 0 || Array.IndexOf(conventionsX64, x) >= 0)
                            entry.convention = x;
                        else
                            throw new ArgumentException("convention");
                    }
                },
                {
                    "stack:,",
                    (x, y) =>
                    {
                        FSMTest(1, state);

                        try
                        {
                            entry.stack = Convert.ToUInt64(x);
                            if(y != null)
                                entry.ret = Convert.ToUInt64(y);
                        }
                        catch
                        {
                            throw new ArgumentException("stack");
                        }
                    }
                },
                {
                    "echo=",
                    x =>
                    {
                        FSMTest(2, state);

                        (state == 2 ? entry.beforeActions : entry.afterActions).Add(new Dictionary<string, object>() {
                            { "verb", "echo" },
                            { "echo", RemoveQuotes(x) }
                        });
                    }
                },
                {
                    "dump=,",
                    (x, y) =>
                    {
                        FSMTest(2, state);

                        if(x[0] != '{' || x[x.Length - 1] != '}' || y[0] != '{' || y[y.Length - 1] != '}')
                            throw new ArgumentException("dump");

                        (state == 2 ? entry.beforeActions : entry.afterActions).Add(new Dictionary<string, object>() {
                            { "verb", "dump" },
                            { "addr", x.Substring(1, x.Length - 2) },
                            { "size", y.Substring(1, y.Length - 2) }
                        });
                    }
                },
                {
                    "ctx=,",
                    (x, y) =>
                    {
                        FSMTest(2, state);

                        (state == 2 ? entry.beforeActions : entry.afterActions).Add(new Dictionary<string, object>() {
                            { "verb", "ctx" },
                            { "key", RemoveQuotes(x) },
                            { "value", RemoveQuotes(y) }
                        });
                    }
                },
                {
                    "verdict=",
                    x =>
                    {
                        FSMTest(2, state);

                        if(Array.IndexOf(state == 2 ? verdictsBefore : verdictsAfter, x) < 0)
                            throw new ArgumentException("verdict");

                        if(state == 1)
                            entry.beforeVerdict = x;
                        else
                            entry.afterVerdict = x;
                    }
                },
                {
                    "<>",
                    x =>
                    {
                        FSMTest(0, state);

                        if(x.StartsWith("0x"))
                        {
                            entry.addrMode = "addr";
                            try
                            {
                                entry.addr = UInt64.Parse(x.Substring(2), System.Globalization.NumberStyles.HexNumber);
                            }
                            catch
                            {
                                throw new ArgumentException("addr");
                            }
                        }
                        else if(x[0] == '*')
                        {
                            entry.addrMode = "pattern";
                            try
                            {
                                entry.pattern = PatternToBinary(x);
                            }
                            catch
                            {
                                throw new ArgumentException("pattern");
                            }
                        }
                        else
                        {
                            entry.addrMode = "symbol";
                            entry.symbol = x;
                        }
                        entry.name = x; // Save hook expression as hook's name
                        state = 1;
                    } 
                }
            };
            Util.ParseOptions(cmd, options);

            return new Dictionary<string, object>()
            {
                { "verb", "hook" },
                { "entry", entry }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.HookEntry entry = (Threads.HookEntry)options["entry"];

            if(entry.name == null)
            {
                // TODO: Show a list of hooks
                return;
            }

            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(false);

            // Check convention or fill in default values
            string[] conventions = (client.bits == 32 ? conventionsX86 : conventionsX64);
            if (entry.convention == null)
                entry.convention = conventions[0];
            else if (Array.IndexOf(conventions, entry.convention) < 0)
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotApplicable"));

            // If the hook already exists, overwrite anything but OEP instead
            foreach(Threads.HookEntry hook in client.hooks)
            {
                if(hook.name == entry.name)
                {
                    // TODO: "Commands.Hook.HookExists"
                    // "Warning: Hook under name \"{0}\" already exists. Overwriting its settings."
                    Logger.W(Program.GetResourceString("Commands.Hook.HookExists", entry.name));
                    entry.oep = hook.oep;
                    client.hooks[client.hooks.IndexOf(hook)] = entry;
                    return;
                }
            }

            // Prepare CMD_HOOK packets
            Puppet.PACKET_CMD_HOOK pktHook = new Puppet.PACKET_CMD_HOOK(0);
            byte[] bufMethod;

            switch(entry.addrMode)
            {
                case "symbol":
                    pktHook.method = 0;
                    bufMethod = Puppet.Util.SerializeString(entry.symbol);
                    break;
                case "pattern":
                    pktHook.method = 1;
                    bufMethod = Puppet.Util.SerializeBinary(entry.pattern);
                    break;
                case "addr":
                    pktHook.method = 2;
                    bufMethod = Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(entry.addr));
                    break;
                default:
                    // Input is sanitized so this should not happen
                    throw new ArgumentException();
            }

            // Send packets
            client.Send(Puppet.Util.Serialize(pktHook));
            client.Send(bufMethod);

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));

            // Fill OEP into entry
            Puppet.PACKET_INTEGER pktOep;
            pktOep = Puppet.Util.Deserialize<Puppet.PACKET_INTEGER>(client.Expect(Puppet.PACKET_TYPE.INTEGER));
            entry.oep = pktOep.data;

            // Add entry to client's hooks
            client.hooks.Add(entry);
            // TODO: Refresh hook list

            // TODO: "Commands.Hook.Installed"
            // "Hook \"{0}\" installed at {1}"
            // FIXME: Client.OEPString() ?
            Logger.I(Program.GetResourceString("Commands.Hook.Installed", entry.name, entry.oep));
        }
    }
}
