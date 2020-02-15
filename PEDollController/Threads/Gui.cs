using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace PEDollController.Threads
{
    class Gui
    {
        public static Gui theInstance = new Gui();
        public static Thread theThread = new Thread(theInstance.TaskMain);

        // ----------

        FMain winMain;
        public FDlgAbout dlgAbout;
        public FDlgAddHook dlgAddHook;
        public FDlgBrowsePID dlgBrowsePID;

        Gui()
        {
            // These must happen before the first Form class
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Program.OnProgramEnd += Program_OnProgramEnd;
            winMain = new FMain();
            dlgAbout = new FDlgAbout();
            dlgAddHook = new FDlgAddHook();
            dlgBrowsePID = new FDlgBrowsePID();
        }

        ~Gui()
        {
            Program.OnProgramEnd -= Program_OnProgramEnd;
        }

        void TaskMain()
        {
            Application.Run(new FSplash());
            Application.Run(winMain);
        }

        void Program_OnProgramEnd()
        {
            Action<Form> actionCloseForm = new Action<Form>((Form Me) => Me.Close());

            foreach (Form frm in Application.OpenForms.Cast<Form>().ToArray())
                frm.Invoke(actionCloseForm, frm);
        }

        // Prototype: void InvokeOnProc(T Me) / (T Me) => { ... }
        // Threads.Gui.InvokeOn((FMain Me) => { ... });
        // NOTE: Nothing will happen if the selected form is not visible
        public void InvokeOn<T>(Action<T> method) where T : Form
        {
            foreach (T frm in Application.OpenForms.Cast<Form>().Where(frm => frm is T))
                frm.Invoke(method, frm);
        }
    }
}
