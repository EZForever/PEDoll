using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace PEDollController
{
    public partial class FMain : Form
    {
        #region Form

        public FMain()
        {
            InitializeComponent();

            InitFileLoadOpen();
            InitListenerStartPort();
            InitDumpsFormats();

            // Hide target-specific pages
            // NOTE: The sequence here is important
            tabPageMonitor.MyHide();
            tabPageHooked.MyHide();
            tabPageDoll.MyHide();
        }

        private void ShowTipError(Control control, string msg = "\n")
        {
            tipError.Show(msg, control, new Point(0, control.Height), 1200);
        }

        private void FMain_Shown(object sender, EventArgs e)
        {
            // Show the initialize complete message
            Logger.H(Program.GetResourceString("UI.Gui.Ready"));
        }

        private void FMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Threads.CmdEngine.theInstance.stopTaskEvent.Set();
        }

        #endregion

        #region Menu Bar

        private void InitFileLoadOpen()
        {
            dlgFileLoadOpen.InitialDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Scripts");
        }

        private void mnuFileLoad_Click(object sender, EventArgs e)
        {
            if (dlgFileLoadOpen.ShowDialog() != DialogResult.OK)
                return;

            string cmd = String.Format("load \"{0}\"",
                dlgFileLoadOpen.FileName
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuHelpCommands_Click(object sender, EventArgs e)
        {
            Threads.CmdEngine.theInstance.AddCommand("help");
        }

        #endregion

        #region CLI Bar

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

        #endregion

        #region "Listener" Panel

        private void InitListenerStartPort()
        {
            txtListenerStartPort.Text = Puppet.Util.DEFAULT_PORT.ToString();
        }

        private void btnListenerStart_Click(object sender, EventArgs e)
        {
            int port;
            try
            {
                port = Convert.ToInt32(txtListenerStartPort.Text);
            }
            catch
            {
                ShowTipError(txtListenerStartPort);
                return;
            }

            string cmd = String.Format("listen {0} {1}",
                chkListenerStartIPv6.Checked ? "--ipv6" : "",
                port
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
        }

        private void lstListenerTargets_DoubleClick(object sender, EventArgs e)
        {
            int idx = lstListenerTargets.SelectedIndices[0];

            // Ignore DblClicking on current target
            if (idx == Threads.CmdEngine.theInstance.target)
                return;

            // Issue `target`
            string cmd = String.Format("target {0}",
                idx
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
        }

        #endregion

        #region "Dumps" Panel

        private void InitDumpsFormats()
        {
            cboDumpFormats.Items.AddRange(BlobFormatters.Util.Formatters.Keys.ToArray());
            cboDumpFormats.SelectedIndex = 0;
        }

        private void ShowDump()
        {
            string cmd = String.Format("dump {0} --format={1}",
                lstDumps.SelectedIndices[0],
                cboDumpFormats.SelectedItem
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
        }

        private void SaveDump()
        {
            int idx = lstDumps.SelectedIndices[0];
            Threads.DumpEntry entry = Threads.CmdEngine.theInstance.dumps[idx];

            dlgDumpSave.FileName = Program.GetResourceString(
                "UI.Gui.Dump.Format",
                idx,
                entry.Source
            );

            if (dlgDumpSave.ShowDialog() != DialogResult.OK)
                return;

            string cmd = String.Format("dump {0} --format={1} --save=\"{2}\"",
                lstDumps.SelectedIndices[0],
                cboDumpFormats.SelectedItem,
                dlgDumpSave.FileName
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
        }

        private void lstDumps_DoubleClick(object sender, EventArgs e)
        {
            ShowDump();
        }

        private void btnDumpShow_Click(object sender, EventArgs e)
        {
            ShowDump();
        }

        private void btnDumpSave_Click(object sender, EventArgs e)
        {
            SaveDump();
        }

        #endregion

        #region "Monitor" Panel

        private void optDollLaunch_CheckedChanged(object sender, EventArgs e)
        {
            txtDollCmdline.Enabled = optDollLaunch.Checked;
            txtDollPID.Enabled = btnDollBrowse.Enabled = !optDollLaunch.Checked;
        }

        private void optKillName_CheckedChanged(object sender, EventArgs e)
        {
            txtKillName.Enabled = optKillName.Checked;
            txtKillPID.Enabled = btnKillBrowse.Enabled = !optKillName.Checked;
        }

        #endregion

        #region "Doll" Panel



        #endregion

        #region "Hooked" Panel



        #endregion
    }
}
