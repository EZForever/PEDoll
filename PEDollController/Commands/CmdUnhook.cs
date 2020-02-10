using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "unhook": Uninstalls a hook.
    // unhook 0xOEP

    class CmdUnhook : ICommand
    {
        public string HelpResId() => "Commands.Help.Unhook";
        public string HelpShortResId() => "Commands.HelpShort.Unhook";

        public Dictionary<string, object> Parse(string cmd)
        {
            UInt64 oep = 0;

            OptionSet options = new OptionSet()
            {
                {
                    "<>",
                    x =>
                    {
                        if(!x.StartsWith("0x"))
                            throw new ArgumentException("oep");
                        try
                        {
                            oep = UInt64.Parse(x.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        }
                        catch
                        {
                            throw new ArgumentException("oep");
                        }

                    }
                }
            };
            Util.ParseOptions(cmd, options);

            return new Dictionary<string, object>()
            {
                { "verb", "unhook" },
                { "oep", oep }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(false);

            UInt64 oep = (UInt64)options["oep"];
            Threads.HookEntry entry = new Threads.HookEntry();
            foreach(Threads.HookEntry hook in client.hooks)
            {
                if(hook.oep == oep)
                {
                    // If client is under the current hook, cancel the operation with TargetNotApplicable
                    if (client.hookOep == oep)
                        throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotApplicable"));

                    entry = hook;
                    break;
                }
            }
            if (entry.name == null) // Uninitialized
                throw new ArgumentException("Commands.Unhook.NotFound");
            // TODO: "Commands.Unhook.NotFound"

            // Prepare & send CMD_UNHOOK packets
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_CMD_UNHOOK(0)));
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(oep)));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));

            // Remove entry from client's hooks
            client.hooks.Remove(entry);
            // TODO: Refresh hook list

            // TODO: "Commands.Unhook.Uninstalled"
            // "Hook \"{0}\" at {1} removed"
            // FIXME: Client.OEPString() ?
            Logger.I(Program.GetResourceString("Commands.Unhook.Uninstalled", entry.name, entry.oep));
        }
    }
}
