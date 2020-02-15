using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "eval": Manually call EvalEngine.
    // eval EXPR

    class CmdEval : ICommand
    {

        static string RemoveQuotes(string x)
        {
            return (x[0] == '"' && x[x.Length - 1] == '"') ? x.Substring(1, x.Length - 2) : x;
        }

        // ----------

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
                { "expr", RemoveQuotes(expr) }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient(false);
            if(client.hookOep == 0)
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotApplicable"));

            string expr = (string)options["expr"];
            string result = Program.GetResourceString(
                "Threads.Client.Eval",
                expr,
                Threads.EvalEngine.EvalString(client, expr)
            );
            Logger.I(result);
            Threads.Gui.theInstance.InvokeOn((FMain Me) => Me.txtHookedResults.Text += (result + Environment.NewLine));
        }
    }
}
