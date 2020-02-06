using System;
using System.Resources;
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
        };

        public static void Invoke(string cmd)
        {
            // Remove prefix whitespaces
            cmd = cmd.Trim();

            string[] cmdExplode = cmd.Split(' ');
            string cmdVerb;

            // Any command that's empty or start with a # is considered as a comment
            if (cmdExplode.Length < 1 || cmdExplode[0].StartsWith("#"))
                cmdVerb = "rem";
            else
                cmdVerb = cmdExplode[0].ToLower();

            if(!Commands.ContainsKey(cmdVerb))
            {
                Console.WriteLine(Program.GetResourceString("Commands.Unknown"), cmdVerb);
                return;
            }

            Dictionary<string, object> options = Commands[cmdVerb].Parse(cmd);
            if(options == null)
            {
                Console.WriteLine(Program.GetResourceString("Commands.Invalid"), cmd, cmdVerb);
                return;
            }

            Commands[cmdVerb].Invoke(options);
        }
    }
}
