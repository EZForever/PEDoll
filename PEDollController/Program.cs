using System;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

using PEDollController.Threads;

namespace PEDollController
{
    static class Program
    {
        #region GetResourceString()

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

        #endregion

        #region TabPage.My{Show|Hide}()

        // NOTE: Always MyHide() before MyShow()! Or the indexes recorded will be messed up

        // <page, <parent, index>>
        static Dictionary<TabPage, Tuple<TabControl, int>> tabLounge = new Dictionary<TabPage, Tuple<TabControl, int>>();

        public static void MyShow(this TabPage page)
        {
            if (!tabLounge.ContainsKey(page))
                return;
                //throw new InvalidOperationException();

            TabControl parent = tabLounge[page].Item1;
            parent.TabPages.Insert(tabLounge[page].Item2, page);
            tabLounge.Remove(page);
        }

        public static void MyHide(this TabPage page)
        {
            TabControl parent = page.Parent as TabControl;
            if (parent == null)
                return;
                //throw new InvalidOperationException();

            tabLounge.Add(page, new Tuple<TabControl, int>(parent, parent.TabPages.IndexOf(page)));
            parent.TabPages.Remove(page);
        }

        #endregion

        public static event Action OnProgramEnd;

        //[STAThread]
        static void Main()
        {
            Console.ResetColor();
            Logger.H(GetResourceString("UI.Cli.Banner", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));

            // Initialize CmdEngine, which receives and processes user commands
            CmdEngine.theTask.Start();

            // Initialize GUI
            // NOTE: In order to let OLE dialogs able to work, GUI must run on a dedicated STA thread
            Gui.theThread.SetApartmentState(ApartmentState.STA);
            Gui.theThread.Start();

            // Wait for CmdEngine to finish
            CmdEngine.theTask.Wait();

            // Then finialize anything
            OnProgramEnd();
        }
    }
}
