using System;
using System.Collections.Generic;

namespace PEDollController.Commands
{

    // Command "ps": `ps` equivalent.
    // ps

    class CmdPs : ICommand
    {
        public string HelpResId() => "Commands.Help.Ps";
        public string HelpShortResId() => "Commands.HelpShort.Ps";

        public Dictionary<string, object> Parse(string cmd)
        {
            // Ignores any arguments

            return new Dictionary<string, object>()
            {
                { "verb", "ps" }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(true);

            // Send CMD_PS
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_CMD_PS(0)));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));

            Logger.I(Program.GetResourceString("Commands.Ps.Header"));

            // Obtain entries
            Puppet.PACKET_INTEGER pktInt;
            while(true)
            {
                pktInt = Puppet.Util.Deserialize<Puppet.PACKET_INTEGER>(client.Expect(Puppet.PACKET_TYPE.INTEGER));
                if ((Int64)pktInt.data == -1)
                    break;

                string name = Puppet.Util.DeserializeString(client.Expect(Puppet.PACKET_TYPE.STRING));
                Logger.I(Program.GetResourceString("Commands.Ps.Format", pktInt.data, name));
            }
        }
    }
}
