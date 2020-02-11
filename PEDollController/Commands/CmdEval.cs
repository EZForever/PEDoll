using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "eval": Manually call EvalEngine.
    // eval EXPR

    class CmdEval : ICommand
    {
        public string HelpResId() => "Commands.Help.Eval";
        public string HelpShortResId() => "Commands.HelpShort.Eval";

        public Dictionary<string, object> Parse(string cmd)
        {
            string expr = null;

            OptionSet options = new OptionSet()
            {
                { "<>", x => expr = x }
            };
            Util.ParseOptions(cmd, options);

            if (expr == null)
                throw new ArgumentException("expr");

            return new Dictionary<string, object>()
            {
                { "verb", "eval" },
                { "expr", expr }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(false);
            if(client.hookOep == 0)
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotApplicable"));

            Logger.I(Threads.EvalEngine.EvalString(client, (string)options["expr"]));
        }
    }
}
