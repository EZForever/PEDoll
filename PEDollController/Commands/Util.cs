using System;
using System.Text;
using System.Collections.Generic;

namespace PEDollController.Commands
{
    static class Util
    {
        public static readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>()
        {
            { "rem" , new CmdRem() },
            { "help", new CmdHelp() },
            { "load", new CmdLoad() },
            { "exit", new CmdExit() },
            { "listen", new CmdListen() },
        };

        public static void Invoke(string cmd)
        {
            // Remove prefix whitespaces
            cmd = cmd.Trim();

            string[] cmdExplode = cmd.Split(' ');
            string cmdVerb = cmdExplode[0].ToLower(); // cmdExplode will have element as long as cmd is not null

            // Any command that's empty or start with a # is considered as a comment
            if (String.IsNullOrEmpty(cmdVerb) || cmdVerb.StartsWith("#"))
                cmdVerb = "rem";

            if (!Commands.ContainsKey(cmdVerb))
                throw new ArgumentException(Program.GetResourceString("Commands.Unknown", cmdVerb));

            Dictionary<string, object> options;
            try
            {
                options = Commands[cmdVerb].Parse(cmd);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(Program.GetResourceString("Commands.Invalid", cmd, cmdVerb), e.Message);
            }

            Commands[cmdVerb].Invoke(options);
        }
    }
}
