using System;
using System.Collections.Generic;

namespace PEDollController.Commands
{

    // Command "rem": Comment. Do nothing.
    // rem [anything]...
    // #[anything]...

    class CmdRem : ICommand
    {
        public string HelpResId() => "Commands.Help.Rem";
        public string HelpShortResId() => "Commands.HelpShort.Rem";

        public Dictionary<string, object> Parse(string cmd)
        {
            // Ignores any arguments

            return new Dictionary<string, object>()
            {
                { "verb", "rem" }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            // Do nothing
        }
    }
}
