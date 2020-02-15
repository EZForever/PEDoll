using System;
using System.Linq;
using System.Windows.Forms;

namespace PEDollController
{
    public partial class FDlgAddHook : Form
    {
        #region Form

        public FDlgAddHook()
        {
            InitializeComponent();
        }

        public void ShowTipError(Control control, string msg = "\n")
        {
            Threads.Gui.theInstance.InvokeOn((FMain Me) => Me.ShowTipError(control, msg));
        }

        private void ResetForm()
        {
            Threads.Client client;
            try
            {
                client = Threads.CmdEngine.theInstance.GetTargetClient(false);
            }
            catch (ArgumentException)
            {
                this.Visible = false;
                return;
            }

            // "Function" GroupBox

            cboAddrMode.SelectedIndex = 0;
            txtAddr.Text = String.Empty;

            cboConvention.Items.Clear();
            cboConvention.Items.AddRange(client.bits == 64 ? Commands.CmdHook.conventionsX64 : Commands.CmdHook.conventionsX86);
            cboConvention.SelectedIndex = 0;

            // "Stack" GroupBox

            chkStack.Checked = false;
            txtStackBytes.Text = (client.bits == 64) ? "32" : "0";
            txtStackRet.Text = "0";

            // "Before" GroupBox

            chkBefore.Checked = chkBeforeVerdict.Checked = false;
            txtBeforeAction.Text = String.Empty;
            cboBeforeVerdict.SelectedIndex = 0;

            // "After" GroupBox

            chkAfter.Checked = chkAfterVerdict.Checked = false;
            txtAfterAction.Text = String.Empty;
            cboAfterVerdict.SelectedIndex = 0;
        }

        private void FDlgAddHook_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
                ResetForm();
        }

        #endregion

        #region Checkboxes

        private void GroupBoxSetEnabled(GroupBox groupBox, Control source, bool value)
        {
            foreach (Control control in groupBox.Controls)
                if (control != source)
                    control.Enabled = value;
        }

        private void chkStack_CheckedChanged(object sender, EventArgs e)
        {
            GroupBoxSetEnabled(grpStack, chkStack, chkStack.Checked);
        }

        private void chkBefore_CheckedChanged(object sender, EventArgs e)
        {
            GroupBoxSetEnabled(grpBefore, chkBefore, chkBefore.Checked);
            cboBeforeVerdict.Enabled = chkBefore.Checked && chkBeforeVerdict.Checked;
        }

        private void chkBeforeVerdict_CheckedChanged(object sender, EventArgs e)
        {
            cboBeforeVerdict.Enabled = chkBefore.Checked && chkBeforeVerdict.Checked;
        }

        private void chkAfter_CheckedChanged(object sender, EventArgs e)
        {
            GroupBoxSetEnabled(grpAfter, chkAfter, chkAfter.Checked);
            cboAfterVerdict.Enabled = chkAfter.Checked && chkAfterVerdict.Checked;
        }

        private void chkAfterVerdict_CheckedChanged(object sender, EventArgs e)
        {
            cboAfterVerdict.Enabled = chkAfter.Checked && chkAfterVerdict.Checked;
        }

        #endregion

        #region Result buttons

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Check if addr is empty
            if(String.IsNullOrWhiteSpace(txtAddr.Text))
            {
                ShowTipError(txtAddr);
                return;
            }

            // Check the stack parameters
            if(chkStack.Checked)
            {
                try
                {
                    Convert.ToUInt64(txtStackBytes.Text);
                }
                catch
                {
                    ShowTipError(txtStackBytes);
                    return;
                }
                try
                {
                    Convert.ToUInt64(txtStackRet.Text);
                }
                catch
                {
                    ShowTipError(txtStackRet);
                    return;
                }
            }

            string argFunction;
            switch (cboAddrMode.SelectedIndex)
            {
                case 0: // Symbol
                {
                    argFunction = txtAddr.Text; 
                    break;
                }
                case 1: // Address
                {
                    if (!txtAddr.Text.StartsWith("0x") || !UInt64.TryParse(txtAddr.Text, System.Globalization.NumberStyles.HexNumber, null, out _))
                    {
                        ShowTipError(txtAddr);
                        return;
                    }
                    argFunction = txtAddr.Text;
                    break;
                }
                case 2: // Pattern
                {
                    if (!txtAddr.Text.All(x => "0123456789abcdefABCDEF".Contains(x)))
                    {
                        ShowTipError(txtAddr);
                        return;
                    }
                    argFunction = "*" + txtAddr.Text;
                    break;
                }
                default:
                {
                    ShowTipError(cboAddrMode);
                    return;
                }
            }
            argFunction = String.Format("{0} --convention={1}", argFunction, cboConvention.SelectedItem);

            string argStack = chkStack.Checked
                ? String.Format("--stack={0},{1}", txtStackBytes.Text, txtStackRet.Text)
                : String.Empty;

            string argBeforeVerdict;
            switch(cboBeforeVerdict.SelectedIndex)
            {
                case 0:
                    argBeforeVerdict = "approve"; break;
                case 1:
                    argBeforeVerdict = "reject"; break;
                case 2:
                    argBeforeVerdict = "terminate"; break;
                default:
                    ShowTipError(cboBeforeVerdict); return;
            }

            string argBefore = chkBefore.Checked
                ? String.Format("--before {0} {1}", txtBeforeAction.Text, chkBeforeVerdict.Checked ? ("--verdict=" + argBeforeVerdict) : String.Empty)
                : String.Empty;

            string argAfterVerdict;
            switch(cboAfterVerdict.SelectedIndex)
            {
                case 0:
                    argAfterVerdict = "approve"; break;
                case 1:
                    argAfterVerdict = "terminate"; break;
                default:
                    ShowTipError(cboAfterVerdict); return;
            }

            string argAfter = chkAfter.Checked
                ? String.Format("--After {0} {1}", txtAfterAction.Text, chkAfterVerdict.Checked ? ("--verdict=" + argAfterVerdict) : String.Empty)
                : String.Empty;

            string cmd = String.Format(
                "hook {0} {1} {2} {3}",
                argFunction,
                argStack,
                argBefore,
                argAfter
            );
            Threads.CmdEngine.theInstance.AddCommand(cmd);
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        #endregion
    }
}
