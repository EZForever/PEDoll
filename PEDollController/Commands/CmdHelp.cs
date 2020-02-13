using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "help": Show help message for all commands / specified command
    // help [command]

    class CmdHelp : ICommand
    {
        public string HelpResId() => "Commands.Help.Help";
        public string HelpShortResId() => "Commands.HelpShort.Help";

        public Dictionary<string, object> Parse(string cmd)
        {
            string command = null;

            OptionSet options = new OptionSet()
            {
                { "<>", x => command = x }
            };
            Util.ParseOptions(cmd, options);

            return new Dictionary<string, object>()
            {
                { "verb", "help" },
                { "command", command }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            string command = (string)options["command"];
            if (String.IsNullOrEmpty(command))
                ShowHelpScreen();
            else if(Util.Commands.ContainsKey(command))
                Logger.I(Program.GetResourceString(Util.Commands[command].HelpResId()));
            else
                throw new ArgumentException(Program.GetResourceString("Commands.Unknown", command));
        }

        void ShowHelpScreen()
        {
            Logger.I(Program.GetResourceString("Commands.HelpShort.Header"));

            // A sorted list of help messages is more readable
            SortedSet<string> commandNames = new SortedSet<string>(Util.Commands.Keys);
            foreach (string commandName in commandNames)
                Logger.I(Program.GetResourceString(Util.Commands[commandName].HelpShortResId()));
        }
    }
}
