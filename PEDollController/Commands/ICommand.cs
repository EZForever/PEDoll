using System;
using System.Collections.Generic;

namespace PEDollController.Commands
{
    interface ICommand
    {
        // Resource ID of full help message (`help cmd`)
        string HelpResId();

        // Resource ID of shortened (one-line) help message (`help`)
        string HelpShortResId();

        // Parses a line of command to its options, throws ArgumentException on error
        Dictionary<string, object> Parse(string cmd);

        // Invokes the command with given options, throws ArgumentException on error
        // Options are guaranteed to be sanitized but not verifyed
        void Invoke(Dictionary<string, object> options);
    }
}
