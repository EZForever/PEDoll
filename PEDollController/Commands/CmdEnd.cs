using System;
using System.Collections.Generic;

namespace PEDollController.Commands
{

    // Command "end": Ends the client, stopping its process.
    // end

    class CmdEnd : ICommand
    {
        public string HelpResId() => "Commands.Help.End";
        public string HelpShortResId() => "Commands.HelpShort.End";

        public Dictionary<string, object> Parse(string cmd)
        {
            // Ignores any arguments

            return new Dictionary<string, object>()
            {
                { "verb", "end" }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient();

            // Send CMD_END
            client.Send(Puppet.Util.Serialize(new Puppet.PACKET_CMD_END(0)));

            // Expect ACK
            client.Expect(Puppet.PACKET_TYPE.ACK);
        }
    }
}
