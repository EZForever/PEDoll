using System;
using System.Linq;
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
            int id = -1;

            OptionSet options = new OptionSet()
            {
                { "<>", (uint x) => id = (int)x }
            };
            Util.ParseOptions(cmd, options);

            return new Dictionary<string, object>()
            {
                { "verb", "unhook" },
                { "id", id }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            int id = (int)options["id"];
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(false);

            if(id >= client.hooks.Count || client.hooks[id].name == null)
                throw new ArgumentException(Program.GetResourceString("Commands.Unhook.NotFound"));

            Threads.HookEntry entry = client.hooks[id];
            // If client is under the current hook, cancel the operation with TargetNotApplicable
            if (client.hookOep == entry.oep)
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotApplicable"));

            // Prepare & send CMD_UNHOOK packets
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_CMD_UNHOOK(0)));
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_INTEGER(entry.oep)));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));

            // Remove entry from client's hooks
            //client.hooks.Remove(entry); // This will cause following hooks' IDs change
            client.hooks[id] = new Threads.HookEntry();

            Logger.I(Program.GetResourceString("Commands.Unhook.Uninstalled", id, entry.name));
            Threads.Gui.theInstance.InvokeOn((FMain Me) => Me.RefreshGuiHooks());
        }
    }
}
