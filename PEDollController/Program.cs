using System;
using System.Threading.Tasks;

using PEDollController.Threads;

namespace PEDollController
{
    static class Program
    {
        public static event Action OnProgramEnd;

        [STAThread]
        static void Main()
        {
            Console.WriteLine("PEDollController InDev");
            Console.WriteLine();

            // Initialize CmdEngine, which receives and processes user commands
            CmdEngine.theTask.Start();
            
            // Initialize GUI
            Gui.theTask.Start();

            // Wait for CmdEngine to finish
            CmdEngine.theTask.Wait();

            // Then finialize anything
            OnProgramEnd();
        }
    }
}
