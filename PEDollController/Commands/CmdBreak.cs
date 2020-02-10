using System;
using System.Collections.Generic;

namespace PEDollController.Commands
{

    // Command "break": Suspend/Resume all threads.
    // break

    class CmdBreak : ICommand
    {
        public string HelpResId() => "Commands.Help.Break";
        public string HelpShortResId() => "Commands.HelpShort.Break";

        public Dictionary<string, object> Parse(string cmd)
        {
            // Ignores any arguments

            return new Dictionary<string, object>()
            {
                { "verb", "break" }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(false);

            // Does not allow suspending a hooked process
            if(client.hookOep != 0)
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotApplicable"));

            // Send CMD_BREAK
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_CMD_BREAK(0)));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));
        }
    }
}
