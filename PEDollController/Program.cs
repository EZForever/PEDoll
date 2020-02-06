using System;
using System.Threading.Tasks;

using PEDollController.Threads;

namespace PEDollController
{
    static class Program
    {
        public static event Action OnProgramEnd;

        public static string GetResourceString(string resId)
        {
            string ret = Properties.Resources.ResourceManager.GetString(resId);
            if (ret == null) // String not found in current culture; use en-US as a fallback
                ret = Properties.Resources.ResourceManager.GetString(resId, System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            if (ret == null) // Still not found? Well, something's going wrong in the resource file
                ret = String.Format("[ Missing \"{0}\" ]", resId);
            return ret;
        }

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
