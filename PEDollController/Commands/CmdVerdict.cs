using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "verdict": Verdicts a activated hook.
    // verdict {approve|reject|terminate}

    class CmdVerdict : ICommand
    {
        static readonly string[] verdicts = {
            "approve", "reject", "terminate"
        };

        // ----------

        public string HelpResId() => "Commands.Help.Verdict";
        public string HelpShortResId() => "Commands.HelpShort.Verdict";

        public Dictionary<string, object> Parse(string cmd)
        {
            string verdict = null;

            OptionSet options = new OptionSet()
            {
                {
                    "<>",
                    x => {
                        if(Array.IndexOf(verdicts, x) < 0)
                            throw new ArgumentException("verdict");
                        else
                            verdict = x;
                    }
                }
            };
            Util.ParseOptions(cmd, options);

            if (verdict == null)
                throw new ArgumentException("verdict");

            return new Dictionary<string, object>()
            {
                { "verb", "verdict" },
                { "verdict", verdict }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            string verdict = (string)options["verdict"];

            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(false);
            if (client.hookOep == 0 || (client.hookPhase != 0 && verdict == "reject"))
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotApplicable"));

            client.SendVerdict(verdict);
        }
    }
}
