using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "loaddll": Call LoadLibrary() in a new thread.
    // loaddll MODULE

    class CmdLoadDll : ICommand
    {
        public string HelpResId() => "Commands.Help.LoadDll";
        public string HelpShortResId() => "Commands.HelpShort.LoadDll";

        public Dictionary<string, object> Parse(string cmd)
        {
            string module = null;

            OptionSet options = new OptionSet()
            {
                { "<>", x => module = x }
            };
            Util.ParseOptions(cmd, options);

            if (module == null)
                throw new ArgumentException("module");

            return new Dictionary<string, object>()
            {
                { "verb", "loaddll" },
                { "module", module }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(false);

            // Send CMD_LOADDLL packets
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_CMD_LOADDLL(0)));
            client.Send(Puppet.Util.SerializeString((string)options["module"]));

            // Expect ACK(0)
            Puppet.PACKET_ACK pktAck;
            pktAck = Puppet.Util.Deserialize<Puppet.PACKET_ACK>(client.Expect(Puppet.PACKET_TYPE.ACK));
            if (pktAck.status != 0)
                throw new ArgumentException(Util.Win32ErrorToMessage((int)pktAck.status));
        }
    }
}
