using System;
using System.Linq;
using System.Collections.Generic;

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
            List<string> cmdExplode = new List<string>(cmd.Split(' '));
            cmdExplode.RemoveAll(String.IsNullOrWhiteSpace);

            string command;
            if (cmdExplode.Count == 1)
                command = null; // Help screen
            else if (cmdExplode.Count == 2)
                command = cmdExplode[1]; // Help for a command
            else
                throw new ArgumentException();

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
            {
                ShowHelpScreen();
                return;
            }
            if(Util.Commands.ContainsKey(command))
                Logger.I(Program.GetResourceString(Util.Commands[command].HelpResId()));
            else
                throw new ArgumentException(Program.GetResourceString("Commands.Unknown", command));
        }

        void ShowHelpScreen()
        {
            Logger.I(Program.GetResourceString("Commands.HelpShort._Header"));

            // A sorted list of help messages is more readable
            SortedSet<string> commandNames = new SortedSet<string>(Util.Commands.Keys);
            foreach (string commandName in commandNames)
                Logger.I(Program.GetResourceString(Util.Commands[commandName].HelpShortResId()));
        }
    }
}
