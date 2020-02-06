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

        // Parse a line of command to its options, return null on error
        Dictionary<string, object> Parse(string cmd);

        // Invoke the command with given options
        // Options are guaranteed to be sanitized but not verifyed
        void Invoke(Dictionary<string, object> options);
    }
}
