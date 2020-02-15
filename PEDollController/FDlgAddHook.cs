using System;
using System.Windows.Forms;

namespace PEDollController
{
    public partial class FDlgAddHook : Form
    {
        public FDlgAddHook()
        {
            InitializeComponent();
        }

        private void FDlgAddHook_VisibleChanged(object sender, EventArgs e)
        {
            // Ignore the dialog hiding
            if (!this.Visible)
                return;

            Threads.Client client;
            try
            {
                client = Threads.CmdEngine.theInstance.GetTargetClient(false);
            }
            catch(ArgumentException)
            {
                this.Visible = false;
                return;
            }

            // TODO: Initialize cboConvention with client-releated values

            // Release client
            client = null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
