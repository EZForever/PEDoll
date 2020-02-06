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
                command = ""; // Help screen
            else if (cmdExplode.Count == 2)
                command = cmdExplode[1]; // Help for a command
            else
                return null;

            return new Dictionary<string, object>()
            {
                { "verb", "help" },
                { "command", command }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            string command = (string)options["command"];
            if (command == "")
            {
                ShowHelpScreen();
                return;
            }
            if(Util.Commands.ContainsKey(command))
            {
                Console.WriteLine(Program.GetResourceString(Util.Commands[command].HelpResId()));
            }
            else
            {
                Console.WriteLine(Program.GetResourceString("Commands.Unknown"), command);
            }
        }

        void ShowHelpScreen()
        {
            Console.WriteLine(Program.GetResourceString("Commands.HelpShort._Header"));

            // A sorted list of help messages is more readable
            SortedSet<string> commandNames = new SortedSet<string>(Util.Commands.Keys);
            foreach (string commandName in commandNames)
                Console.WriteLine(Program.GetResourceString(Util.Commands[commandName].HelpShortResId()));
        }
    }
}
