using System;
using System.Collections.Generic;

using Mono.Options;

namespace PEDollController.Commands
{

    // Command "target": Shows/switches target client.
    // target[ID]
    // target --lastDoll
    // target --lastMonitor

    class CmdTarget : ICommand
    {
        public string HelpResId() => "Commands.Help.Target";
        public string HelpShortResId() => "Commands.HelpShort.Target";

        public Dictionary<string, object> Parse(string cmd)
        {
            int target = -1;
            bool lastDoll = false;
            bool lastMonitor = false;

            OptionSet options = new OptionSet()
            {
                { "lastDoll", x => lastDoll = (x != null) },
                { "lastMonitor", x => lastMonitor = (x != null) },
                { "<>", (int x) => target = x }
            };
            Util.ParseOptions(cmd, options);

            if (lastDoll && lastMonitor)
                throw new ArgumentException();

            return new Dictionary<string, object>()
            {
                { "verb", "target" },
                { "target", target },
                { "lastDoll", lastDoll },
                { "lastMonitor", lastMonitor }
            };
        }

        public void Invoke(Dictionary<string, object> options)
        {
            int target = (int)options["target"];
            bool lastDoll = (bool)options["lastDoll"];
            bool lastMonitor = (bool)options["lastMonitor"];

            if(target == -1 && !lastDoll && !lastMonitor)
            {
                Logger.I(Program.GetResourceString("Commands.Target.Header"));

                for(int i = 0; i < Threads.Client.theInstances.Count; i++)
                {
                    Threads.Client instance = Threads.Client.theInstances[i];

                    Logger.I(Program.GetResourceString("Commands.Target.Format",
                        (i == Threads.CmdEngine.theInstance.target) ? '*' : ' ',
                        i,
                        instance.clientName,
                        instance.GetTypeString(),
                        instance.GetStatusString(),
                        instance.pid,
                        instance.bits
                    ));
                }
                return;
            }

            if(lastDoll)
                target = Threads.CmdEngine.theInstance.targetLastDoll;
            else if(lastMonitor)
                target = Threads.CmdEngine.theInstance.targetLastMonitor;

            if(target < 0 || target >= Threads.Client.theInstances.Count)
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotAvailable"));

            int targetLast = Threads.CmdEngine.theInstance.target;
            Threads.CmdEngine.theInstance.target = target;
            if (Threads.Client.theInstances[targetLast].isMonitor)
                Threads.CmdEngine.theInstance.targetLastMonitor = targetLast;
            else
                Threads.CmdEngine.theInstance.targetLastDoll = targetLast;

            Threads.Client client = Threads.CmdEngine.theInstance.GetTargetClient();

            Logger.I(Program.GetResourceString("Commands.Target.CurrentTarget",
                target,
                client.clientName,
                client.GetTypeString(),
                client.GetStatusString()
            ));

            // TODO: Refresh GUI info pages (e.g. enable verdict page on a Hooked client)
        }
    }

}
