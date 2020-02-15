using System;
using System.Windows.Forms;

namespace PEDollController
{
    public partial class FDlgBrowsePID : Form
    {
        public int Result;

        public FDlgBrowsePID()
        {
            InitializeComponent();
        }

        private void FDlgBrowsePID_VisibleChanged(object sender, EventArgs e)
        {
            // Ignore the dialog hiding
            if (!this.Visible)
                return;

            // -1 means user cancelled the selection
            Result = -1;

            // Wait for `ps` to initialize the list
            this.UseWaitCursor = true;
            Threads.CmdEngine.theInstance.AddCommand("ps");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Result = Convert.ToInt32(lstPs.SelectedItems[0].Text);
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
