using System;
using System.Reflection;

using PEDollController.Threads;

namespace PEDollController
{
    static class Program
    {
        public static event Action OnProgramEnd;

        public static string GetResourceString(string resId)
        {
            string ret = Properties.Resources.ResourceManager.GetString(resId);
            if (ret == null)
                ret = String.Format("[ Missing \"{0}\" ]", resId);
            return ret;
        }

        public static string GetResourceString(string resId, params object[] args)
        {
            return String.Format(GetResourceString(resId), args);
        }

        [STAThread]
        static void Main()
        {
            Console.ResetColor();
            Logger.H(GetResourceString("UI.Cli.Banner", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));

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
