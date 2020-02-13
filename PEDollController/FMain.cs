using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PEDollController
{
    public partial class FMain : Form
    {
        public FMain()
        {
            InitializeComponent();
        }
        
        void SendCmd()
        {
            Threads.CmdEngine.theInstance.AddCommand(txtCLICommand.Text);
            txtCLICommand.Text = String.Empty;
        }

        private void txtCLICommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                SendCmd();
                e.Handled = true;
            }
        }

        private void btnCLIExecute_Click(object sender, EventArgs e)
        {
            SendCmd();
        }
    }
}
