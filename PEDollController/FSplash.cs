using System;
using System.Windows.Forms;

namespace PEDollController
{
    public partial class FSplash : Form
    {
        public FSplash()
        {
            InitializeComponent();
            lblBanner.Text = Program.GetResourceString("UI.Gui.Banner", Program.Version);
        }

        private void tmrClose_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
