using System;
using System.Windows.Forms;

namespace PEDollController
{
    public partial class FSplash : Form
    {
        public FSplash()
        {
            InitializeComponent();
        }

        private void tmrClose_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
