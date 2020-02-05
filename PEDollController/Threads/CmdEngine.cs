using System;
using System.Threading;
using System.Threading.Tasks;

namespace PEDollController.Threads
{
    class CmdEngine
    {
        public static CmdEngine theInstance = new CmdEngine();
        public static Task theTask = new Task(theInstance.TaskMain);

        // Stop engine & all tasks created by CmdEngine (Listener, Client*)
        ManualResetEvent stopTaskEvent;
        public Task stopTaskAsync;

        CmdEngine()
        {
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
            // TODO
            Listener.CreateInstance(false, Puppet.Util.DEFAULT_PORT);
            while(true)
            {
                int idx = Task.WaitAny(stopTaskAsync);
                if (idx == 0)
                    break;
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

            Console.WriteLine("\n< Ctrl-C received, press any key to stop engine... >");
            Console.ReadKey();

            // Tell TaskMain() about ending task
            stopTaskEvent.Set();
        }
    }
}
