using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace PEDollController.Threads
{
    class Gui
    {
        public static Gui theInstance = new Gui();
        public static Task theTask = new Task(theInstance.TaskMain);

        // ----------

        FMain winMain;

        Gui()
        {
            // These must happen before the first Form class
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Program.OnProgramEnd += Program_OnProgramEnd;
            winMain = new FMain();
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
            InvokeOn((Form Me) => Me.Close());
        }

        // Prototype: void InvokeOnProc(T Me) / (T Me) => { ... }
        public void InvokeOn<T>(Action<T> method) where T : Form
        {
            foreach (T frm in Application.OpenForms.Cast<Form>().Where(frm => frm is T))
                frm.Invoke(method, frm);
        }
    }
}
