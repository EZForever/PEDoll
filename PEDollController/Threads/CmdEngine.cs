using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PEDollController.Threads
{
    struct DumpEntry
    {
        public string Source;
        public byte[] Data;
    }

    class CmdEngine
    {
        public static CmdEngine theInstance = new CmdEngine();
        public static Task theTask = new Task(theInstance.TaskMain);

        // ----------

        BlockingQueue<string> cmdQueue;
        AsyncDataProvider<string> cmdProvider;

        // Stop engine & all tasks created by CmdEngine (Listener, Client*)
        public ManualResetEvent stopTaskEvent;
        public Task stopTaskAsync;

        public int target = -1;
        public int targetLastDoll = -1;
        public int targetLastMonitor = -1;

        public List<DumpEntry> dumps;

        CmdEngine()
        {
            dumps = new List<DumpEntry>();
            cmdQueue = new BlockingQueue<string>();
            cmdProvider = new AsyncDataProvider<string>(cmdQueue.BlockingDequeue);

            stopTaskEvent = new ManualResetEvent(false);
            stopTaskAsync = new Task(() => stopTaskEvent.WaitOne());
            stopTaskAsync.Start();

            // Register to Program's stop event
            Program.OnProgramEnd += Program_OnProgramEnd;

            // Register Ctrl-C event, so that when we received a Ctrl-C from the console, we stop the engine
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        ~CmdEngine()
        {
            Console.CancelKeyPress -= Console_CancelKeyPress;
            Program.OnProgramEnd -= Program_OnProgramEnd;
        }

        void TaskMain()
        {
            while(true)
            {
                // XXX: Also wait for commands from Console.ReadLine()?
                Task<string> taskCmd = cmdProvider.Get();
                int idx = Task.WaitAny(stopTaskAsync, taskCmd);

                if (idx == 0)
                {
                    // stopTaskAsync triggered
                    break;
                }
                else
                {
                    string cmd = taskCmd.Result;

                    Console.ForegroundColor = Logger.colorCmd;
                    Console.WriteLine("> " + cmd);
                    Console.ResetColor();
                    OnCommand(cmd);
                }
            }
        }

        void OnCommand(string cmd)
        {
            try
            {
                Commands.Util.Invoke(cmd);
            }
            catch(ArgumentException e)
            {
                Logger.E(e.Message); // NOTE: e.ParamName is followed
            }
        }

        void Program_OnProgramEnd()
        {
            stopTaskEvent.Set();
        }

        void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            // Do not just stop the process
            args.Cancel = true;

            // TODO: "UI.CtrlC"
            // "\n[ Ctrl-C received, press any key to stop engine... ]"
            Logger.W(Program.GetResourceString("UI.CtrlC"));
            Console.ReadKey();

            // Tell TaskMain() about ending task
            stopTaskEvent.Set();
        }

        public void AddCommand(string cmd)
        {
            cmdQueue.BlockingEnqueue(cmd);
        }

        public Client GetTargetClient()
        {
            if (target < 0 || Client.theInstances[target].isDead)
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotAvailable"));
            // TODO: "Threads.CmdEngine.TargetNotAvailable"

            return Client.theInstances[target];
        }

        public Client GetTargetClient(bool isMonitor)
        {
            Client client = GetTargetClient();
            if (client.isMonitor != isMonitor)
                throw new ArgumentException(Program.GetResourceString("Threads.CmdEngine.TargetNotApplicable"));
            // TODO: "Threads.CmdEngine.TargetNotApplicable"
            // "Requested operation is not applicable to the target."

            return client;
        }
    }
}
