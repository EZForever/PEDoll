using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "hook": Installs a hook.
    // hook
    // hook {[MODULE!]NAME|@ADDR|*PATTERN}
    //      [--convention=CONVENTION[, STACK[, RETURN]]]
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
                throw new ArgumentException("before/after");
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
            string addrMode = null; // {null|"name"|"addr"|"pattern"}
            string name = null;
            UInt64 addr = 0;
            byte[] pattern = null;

            string convention = null; // {"stdcall"|"cdecl"|"fastcall"|"msvc"|"gcc"}, default value set on Invoke()
            UInt64 stack = 0;
            UInt64 ret = 0;

            List<Dictionary<string, object>> beforeActions = new List<Dictionary<string, object>>();
            string beforeVerdict = null; // {null|"approve"|"reject"|"terminate"}

            List<Dictionary<string, object>> afterActions = new List<Dictionary<string, object>>();
            string afterVerdict = null; // {null|"approve"|"terminate"}

            int state = 0; // FSM on option parsing; 0 - Begin, 1 - Before, 2 - After

            OptionSet options = new OptionSet()
            {
                { "before", x => state = (x != null) ? 1 : state },
                { "after", x => state = (x != null) ? 2 : state },
                {
                    "convention=",
                    x =>
                    {
                        FSMTest(0, state);

                        if(Array.IndexOf(conventionsX86, x) >= 0 || Array.IndexOf(conventionsX64, x) >= 0)
                            convention = x;
                        else
                            throw new ArgumentException("convention");
                    }
                },
                {
                    "stack:,",
                    (x, y) =>
                    {
                        FSMTest(0, state);

                        try
                        {
                            stack = Convert.ToUInt64(x);
                            if(y != null)
                                ret = Convert.ToUInt64(y);
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
                        FSMTest(1, state);

                        (state == 1 ? beforeActions : afterActions).Add(new Dictionary<string, object>() {
                            { "verb", "echo" },
                            { "echo", RemoveQuotes(x) }
                        });
                    }
                },
                {
                    "dump=,",
                    (x, y) =>
                    {
                        FSMTest(1, state);

                        if(x[0] != '{' || x[x.Length - 1] != '}' || y[0] != '{' || y[y.Length - 1] != '}')
                            throw new ArgumentException("dump");

                        (state == 1 ? beforeActions : afterActions).Add(new Dictionary<string, object>() {
                            { "verb", "dump" },
                            { "addr", x },
                            { "size", y }
                        });
                    }
                },
                {
                    "ctx=,",
                    (x, y) =>
                    {
                        FSMTest(1, state);

                        (state == 1 ? beforeActions : afterActions).Add(new Dictionary<string, object>() {
                            { "verb", "ctx" },
                            { "key", RemoveQuotes(x) }, // TODO: Remove embraced quotes
                            { "value", RemoveQuotes(y) }
                        });
                    }
                },
                {
                    "verdict=",
                    x =>
                    {
                        FSMTest(1, state);

                        if(Array.IndexOf(state == 1 ? verdictsBefore : verdictsAfter, x) < 0)
                            throw new ArgumentException("verdict");

                        if(state == 1)
                            beforeVerdict = x;
                        else
                            afterVerdict = x;
                    }
                },
                {
                    "<>",
                    x =>
                    {
                        FSMTest(0, state);

                        if(x.StartsWith("0x"))
                        {
                            addrMode = "addr";
                            try
                            {
                                addr = Convert.ToUInt64(x);
                            }
                            catch
                            {
                                throw new ArgumentException("addr");
                            }
                        }
                        else if(x[0] == '*')
                        {
                            addrMode = "pattern";
                            try
                            {
                                pattern = PatternToBinary(x);
                            }
                            catch
                            {
                                throw new ArgumentException("pattern");
                            }
                        }
                        else
                        {
                            addrMode = "name";
                            name = x;
                        }
                    } 
                }
            };
            Util.ParseOptions(cmd, options);

            return new Dictionary<string, object>()
            {
                { "verb", "hook" },

                { "addrMode", addrMode },
                { "name", name },
                { "addr", addr },
                { "pattern", pattern },

                { "convention", convention },
                { "stack", stack },
                { "ret", ret },

                { "beforeActions", beforeActions },
                { "beforeVerdict", beforeVerdict },

                { "afterActions", afterActions },
                { "afterVerdict", afterVerdict }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            // TODO: CmdHook.Invoke()
        }
    }
}
