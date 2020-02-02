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
