using System;
using System.Collections.Generic;

namespace PEDollController.Commands
{

    // Command "shell": `system()` equivalent.
    // shell [ARGUMENTS ...]

    class CmdShell : ICommand
    {
        public string HelpResId() => "Commands.Help.Shell";
        public string HelpShortResId() => "Commands.HelpShort.Shell";

        public Dictionary<string, object> Parse(string cmd)
        {
            string arguments = String.Join(" ", CommandLine.ToArgs(cmd));

            return new Dictionary<string, object>()
            {
                { "verb", "shell" },
                { "arguments", arguments }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(true);

            // Send CMD_SHELL
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_CMD_SHELL(0)));

            // Send arguments
            client.Send(Puppet.Util.SerializeString((string)options["arguments"]));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));
        }
    }
}
