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

        public void ShowTipError(Control control, string msg = "\n")
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

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            Threads.Gui.theInstance.dlgAbout.ShowDialog();
        }

        #endregion

        #region CLI Bar

        private void SendCmd()
        {
            Threads.CmdEngine.theInstance.AddCommand(txtCLICommand.Text);
            txtCLICommand.Text = String.Empty;
        }

        private void btnCLIExecute_Click(object sender, EventArgs e)
        {
            SendCmd();
        }

        private void txtCLICommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                SendCmd();
                e.Handled = true;
            }
        }

        #endregion

        #region "Listener" Panel

        public void RefreshGuiTargets()
        {
            lstListenerTargets.Items.Clear();

            for (int i = 0; i < Threads.Client.theInstances.Count; i++)
            {
                Threads.Client instance = Threads.Client.theInstances[i];

                lstListenerTargets.Items.Add(new ListViewItem(new string[] {
                        (i == Threads.CmdEngine.theInstance.target) ? "*" : " ",
                        i.ToString(),
                        instance.clientName,
                        instance.GetTypeString(),
                        instance.GetStatusString(),
                        instance.pid.ToString(),
                        instance.bits.ToString()
                    }));

                // Mark dead clients
                if (instance.isDead)
                    lstListenerTargets.Items[i].ForeColor = Color.Gray;
            }

            tabPageMonitor.MyHide();
            tabPageHooked.MyHide();
            tabPageDoll.MyHide();

            Threads.Client instanceCurr = Threads.Client.theInstances[Threads.CmdEngine.theInstance.target];
            if (!instanceCurr.isDead)
            {
                if (instanceCurr.isMonitor)
                {
                    lblMonitorCurrent.Text = Program.GetResourceString(
                        "UI.Gui.Title.Monitor",
                        Threads.CmdEngine.theInstance.target,
                        instanceCurr.clientName
                    );
                    tabPageMonitor.MyShow();
                }
                else
                {
                    lblDollCurrent.Text = Program.GetResourceString(
                        "UI.Gui.Title.Doll",
                        Threads.CmdEngine.theInstance.target,
                        instanceCurr.clientName
                    );
                    this.RefreshGuiHooks();
                    tabPageDoll.MyShow();

                    if (instanceCurr.hookOep != 0)
                    {
                        int idx = instanceCurr.hooks.FindIndex(x => x.oep == instanceCurr.hookOep);
                        Threads.HookEntry entry = instanceCurr.hooks[idx];

                        lblHookedCurrent.Text = Program.GetResourceString(
                            "UI.Gui.Title.Hooked",
                            idx,
                            entry.name,
                            (instanceCurr.hookPhase == 0) ? "before" : "after"
                        );
                        btnVerdictReject.Enabled = (instanceCurr.hookPhase == 0);
                        tabPageHooked.MyShow();
                    }
                }
            }
        }

        private void InitListenerStartPort()
        {
            txtListenerStartPort.Text = Puppet.Util.DEFAULT_PORT.ToString();
        }

        private void btnListenerStart_Click(object sender, EventArgs e)
        {
            ushort port;
            try
            {
                port = Convert.ToUInt16(txtListenerStartPort.Text);
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

        private void btnMonitorCurrent_Click(object sender, EventArgs e)
        {
            Threads.CmdEngine.theInstance.AddCommand("end");
        }

        private void optDollLaunch_CheckedChanged(object sender, EventArgs e)
        {
            txtDollCmdline.Enabled = optDollLaunch.Checked;
            txtDollPID.Enabled = btnDollBrowse.Enabled = !optDollLaunch.Checked;
        }

        private void btnDollBrowse_Click(object sender, EventArgs e)
        {
            Threads.Gui.theInstance.dlgBrowsePID.ShowDialog();

            int result = Threads.Gui.theInstance.dlgBrowsePID.Result;
            if (result < 0)
                return;

            txtDollPID.Text = result.ToString();
        }

        private void btnDollInvoke_Click(object sender, EventArgs e)
        {
            string arg;
            if(optDollLaunch.Checked)
            {
                arg = txtDollCmdline.Text;
            }
            else
            {
                try
                {
                    Convert.ToUInt32(txtDollPID.Text);
                }
                catch
                {
                    ShowTipError(txtDollPID);
                    return;
                }
                arg = txtDollPID.Text;
            }

            string cmd = String.Format("doll {0} {1}",
                optDollLaunch.Checked ? "" : "--attach",
                arg
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);

            txtDollCmdline.Text = txtDollPID.Text = String.Empty;
        }

        private void optKillName_CheckedChanged(object sender, EventArgs e)
        {
            txtKillName.Enabled = optKillName.Checked;
            txtKillPID.Enabled = btnKillBrowse.Enabled = !optKillName.Checked;
        }

        private void btnKillBrowse_Click(object sender, EventArgs e)
        {
            Threads.Gui.theInstance.dlgBrowsePID.ShowDialog();

            int result = Threads.Gui.theInstance.dlgBrowsePID.Result;
            if (result < 0)
                return;

            txtKillPID.Text = result.ToString();
        }

        private void btnKillInvoke_Click(object sender, EventArgs e)
        {
            string arg;
            if (optKillName.Checked)
            {
                arg = txtKillName.Text;
            }
            else
            {
                try
                {
                    Convert.ToUInt32(txtKillPID.Text);
                }
                catch
                {
                    ShowTipError(txtKillPID);
                    return;
                }
                arg = txtKillPID.Text;
            }

            string cmd = String.Format("kill {0} {1}",
                optKillName.Checked ? "--all" : "",
                arg
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);

            txtKillName.Text = txtKillPID.Text = String.Empty;
        }

        private void btnMonitorShell_Click(object sender, EventArgs e)
        {
            string cmd = String.Format("shell {0} {1}",
                chkMonitorShellKeep.Checked ? "/k" : "/c",
                txtMonitorShell.Text
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);

            txtMonitorShell.Text = String.Empty;
        }

        #endregion

        #region "Doll" Panel

        public void RefreshGuiHooks()
        {
            Threads.Client client;
            try
            {
                client = Threads.CmdEngine.theInstance.GetTargetClient(false);
            }
            catch (ArgumentException)
            {
                return;
            }

            lstDollHooks.Items.Clear();

            for(int i = 0; i < client.hooks.Count; i++)
            {
                Threads.HookEntry hook = client.hooks[i];

                // Skip the removed hooks
                if (hook.name == null)
                    continue;

                lstDollHooks.Items.Add(new ListViewItem(new string[]
                {
                    i.ToString(),
                    client.OepToString(hook.oep),
                    hook.name
                }));
            }

            if(lstDollHooks.Items.Count > 0)
            {
                lstDollHooks.Items[0].Selected = true;
                btnHooksRemove.Enabled = true;
            }
            else
            {
                btnHooksRemove.Enabled = false;
            }
        }

        private void btnDollCurrent_Click(object sender, EventArgs e)
        {
            Threads.CmdEngine.theInstance.AddCommand("end");
        }

        private void btnHooksAdd_Click(object sender, EventArgs e)
        {
            Threads.Gui.theInstance.dlgAddHook.ShowDialog();
        }

        private void btnHooksRemove_Click(object sender, EventArgs e)
        {
            string cmd = String.Format("unhook {0}",
                lstDollHooks.SelectedIndices[0]
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
        }

        private void btnDollBreak_Click(object sender, EventArgs e)
        {
            Threads.CmdEngine.theInstance.AddCommand("break");
        }

        private void btnDollLoaddll_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(txtDollLoaddll.Text))
            {
                ShowTipError(txtDollLoaddll);
                return;
            }

            string cmd = String.Format("loaddll {0}",
                txtDollLoaddll.Text
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
        }

        #endregion

        #region "Hooked" Panel

        private void SendEval()
        {
            string cmd = String.Format("eval \"{0}\"",
                txtHookedEval.Text
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
            txtHookedEval.Text = String.Empty;
        }

        private void btnVerdictApprove_Click(object sender, EventArgs e)
        {
            Threads.CmdEngine.theInstance.AddCommand("verdict approve");
        }

        private void btnVerdictReject_Click(object sender, EventArgs e)
        {
            Threads.CmdEngine.theInstance.AddCommand("verdict reject");
        }

        private void btnVerdictTerminate_Click(object sender, EventArgs e)
        {
            Threads.CmdEngine.theInstance.AddCommand("verdict terminate");
        }
        
        private void txtHookedEval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendEval();
                e.Handled = true;
            }
        }

        private void btnHookedEval_Click(object sender, EventArgs e)
        {
            SendEval();
        }

        #endregion
    }
}
