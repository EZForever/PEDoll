using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PEDollController.Threads
{
    class Gui
    {
        public static Gui theInstance = new Gui();
        public static Task theTask = new Task(theInstance.TaskMain);

        static Action<Form> CloseForm = new Action<Form>(frm => frm.Close());

        public FMain winMain;

        Gui()
        {
            Program.OnProgramEnd += Program_OnProgramEnd;
            winMain = new FMain();
        }

        ~Gui()
        {
            Program.OnProgramEnd -= Program_OnProgramEnd;
        }

        void TaskMain()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new FSplash());
            Application.Run(winMain);
        }

        void Program_OnProgramEnd()
        {
            List<Form> openedForms = new List<Form>();
            foreach (Form frm in Application.OpenForms)
                openedForms.Add(frm);
            foreach (Form frm in openedForms)
                frm.Invoke(CloseForm, frm);
        }
    }
}
