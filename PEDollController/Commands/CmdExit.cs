using System;
using System.Collections.Generic;

namespace PEDollController.Commands
{
    // Command "exit": Quit Controller.
    // exit

    class CmdExit : ICommand
    {
        public string HelpResId() => "Commands.Help.Exit";
        public string HelpShortResId() => "Commands.HelpShort.Exit";

        public Dictionary<string, object> Parse(string cmd)
        {
            // Ignores any arguments

            return new Dictionary<string, object>()
            {
                { "verb", "exit" }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.CmdEngine.theInstance.stopTaskEvent.Set();
        }
    }
}
