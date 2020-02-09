using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "kill": `kill`/`killall` equivalent.
    // kill PID
    // kill --all NAME

    class CmdKill : ICommand
    {
        public string HelpResId() => "Commands.Help.Kill";
        public string HelpShortResId() => "Commands.HelpShort.Kill";

        public Dictionary<string, object> Parse(string cmd)
        {
            bool killAll = false;
            string extras;

            OptionSet options = new OptionSet()
            {
                { "all", x => killAll = (x != null) }
            };
            extras = String.Join(" ", Util.ParseOptions(cmd, options));

            string name = null;
            int pid = 0;

            if (killAll)
            {
                name = extras;
            }
            else
            {
                try
                {
                    pid = Convert.ToInt32(extras);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.Message);
                }
            }

            return new Dictionary<string, object>()
            {
                { "verb", "kill" },
                { "killAll", killAll },
                { "name", name },
                { "pid", pid }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(true);

            // Send CMD_KILL
            Puppet.PACKET_CMD_KILL pktKill = new Puppet.PACKET_CMD_KILL(0);
            pktKill.pid = (UInt32)(int)options["pid"];
            client.Send(Puppet.Util.Serialize(pktKill));

            if ((bool)options["killAll"])
                client.Send(Puppet.Util.SerializeString((string)options["name"]));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));
        }
    }
}
