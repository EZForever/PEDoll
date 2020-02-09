using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "doll": Creates a new Doll client.
    // doll CMDLINE
    // doll --attach PID

    class CmdDoll : ICommand
    {
        public string HelpResId() => "Commands.Help.Doll";
        public string HelpShortResId() => "Commands.HelpShort.Doll";

        public Dictionary<string, object> Parse(string cmd)
        {
            bool attach = false;
            string extras;

            OptionSet options = new OptionSet()
            {
                { "attach", x => attach = (x != null) }
            };
            extras = String.Join(" ", Util.ParseOptions(cmd, options));

            string cmdline = null;
            int pid = 0;

            if(attach)
            {
                try
                {
                    pid = Convert.ToInt32(extras);
                }
                catch(Exception e)
                {
                    throw new ArgumentException(e.Message);
                }
            }
            else
            {
                cmdline = extras;
            }

            return new Dictionary<string, object>()
            {
                { "verb", "doll" },
                { "attach", attach },
                { "cmdline", cmdline },
                { "pid", pid }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(true);

            // Send CMD_DOLL
            Puppet.PACKET_CMD_DOLL pktDoll = new Puppet.PACKET_CMD_DOLL(0);
            pktDoll.pid = (UInt32)(int)options["pid"];
            client.Send(Puppet.Util.Serialize(pktDoll));

            if(!(bool)options["attach"])
                client.Send(Puppet.Util.SerializeString((string)options["cmdline"]));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));
        }
    }
}
