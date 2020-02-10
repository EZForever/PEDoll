using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{
    static class Util
    {
        public static readonly Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>()
        {
            { "ps", new CmdPs() },
            { "rem", new CmdRem() },
            { "end", new CmdEnd() },
            { "help", new CmdHelp() },
            { "load", new CmdLoad() },
            { "exit", new CmdExit() },
            { "doll", new CmdDoll() },
            { "kill", new CmdKill() },
            { "hook", new CmdHook() },
            { "shell", new CmdShell() },
            { "break", new CmdBreak() },
            { "listen", new CmdListen() },
            { "target", new CmdTarget() },
            { "unhook", new CmdUnhook() },
            { "loaddll", new CmdLoadDll() },
        };

        public static void Invoke(string cmd)
        {
            // Remove prefix whitespaces
            cmd = cmd.Trim();

            string[] cmdExplode = cmd.Split(' ');
            string cmdVerb = cmdExplode[0]; // cmdExplode will have element as long as cmd is not null

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

        public static List<string> ParseOptions(string cmd, OptionSet options)
        {
            try
            {
                return options.Parse(CommandLine.ToArgs(cmd));
            }
            catch (OptionException e)
            {
                throw new ArgumentException(e.Message);
            }
        }
        
        public static string Win32ErrorToMessage(int code)
        {
            // TODO: "Commands.Win32Error"
            // "Win32 Error {0}: {1}"
            return Program.GetResourceString("Commands.Win32Error", code, new System.ComponentModel.Win32Exception(code).Message);
        }
    }
}
