using System;
using System.Windows.Forms;

namespace PEDollController
{
    public partial class FDlgAbout : Form
    {
        public FDlgAbout()
        {
            InitializeComponent();
            lblBanner.Text = Program.GetResourceString("UI.Gui.Banner", Program.GetVersionString());
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
