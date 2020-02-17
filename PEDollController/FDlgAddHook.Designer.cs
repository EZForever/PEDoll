namespace PEDollController
{
    partial class FDlgAddHook
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FDlgAddHook));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpFunction = new System.Windows.Forms.GroupBox();
            this.cboConvention = new System.Windows.Forms.ComboBox();
            this.lbl1 = new System.Windows.Forms.Label();
            this.txtAddr = new System.Windows.Forms.TextBox();
            this.cboAddrMode = new System.Windows.Forms.ComboBox();
            this.grpStack = new System.Windows.Forms.GroupBox();
            this.lbl3 = new System.Windows.Forms.Label();
            this.txtStackRet = new System.Windows.Forms.TextBox();
            this.chkStack = new System.Windows.Forms.CheckBox();
            this.lbl2 = new System.Windows.Forms.Label();
            this.txtStackBytes = new System.Windows.Forms.TextBox();
            this.chkBefore = new System.Windows.Forms.CheckBox();
            this.grpBefore = new System.Windows.Forms.GroupBox();
            this.chkBeforeVerdict = new System.Windows.Forms.CheckBox();
            this.cboBeforeVerdict = new System.Windows.Forms.ComboBox();
            this.txtBeforeAction = new System.Windows.Forms.TextBox();
            this.grpAfter = new System.Windows.Forms.GroupBox();
            this.chkAfterVerdict = new System.Windows.Forms.CheckBox();
            this.cboAfterVerdict = new System.Windows.Forms.ComboBox();
            this.txtAfterAction = new System.Windows.Forms.TextBox();
            this.chkAfter = new System.Windows.Forms.CheckBox();
            this.grpFunction.SuspendLayout();
            this.grpStack.SuspendLayout();
            this.grpBefore.SuspendLayout();
            this.grpAfter.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpFunction
            // 
            this.grpFunction.Controls.Add(this.cboConvention);
            this.grpFunction.Controls.Add(this.lbl1);
            this.grpFunction.Controls.Add(this.txtAddr);
            this.grpFunction.Controls.Add(this.cboAddrMode);
            resources.ApplyResources(this.grpFunction, "grpFunction");
            this.grpFunction.Name = "grpFunction";
            this.grpFunction.TabStop = false;
            // 
            // cboConvention
            // 
            this.cboConvention.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConvention.FormattingEnabled = true;
            resources.ApplyResources(this.cboConvention, "cboConvention");
            this.cboConvention.Name = "cboConvention";
            // 
            // lbl1
            // 
            resources.ApplyResources(this.lbl1, "lbl1");
            this.lbl1.Name = "lbl1";
            // 
            // txtAddr
            // 
            resources.ApplyResources(this.txtAddr, "txtAddr");
            this.txtAddr.Name = "txtAddr";
            // 
            // cboAddrMode
            // 
            this.cboAddrMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAddrMode.FormattingEnabled = true;
            this.cboAddrMode.Items.AddRange(new object[] {
            resources.GetString("cboAddrMode.Items"),
            resources.GetString("cboAddrMode.Items1"),
            resources.GetString("cboAddrMode.Items2")});
            resources.ApplyResources(this.cboAddrMode, "cboAddrMode");
            this.cboAddrMode.Name = "cboAddrMode";
            // 
            // grpStack
            // 
            this.grpStack.Controls.Add(this.lbl3);
            this.grpStack.Controls.Add(this.txtStackRet);
            this.grpStack.Controls.Add(this.chkStack);
            this.grpStack.Controls.Add(this.lbl2);
            this.grpStack.Controls.Add(this.txtStackBytes);
            resources.ApplyResources(this.grpStack, "grpStack");
            this.grpStack.Name = "grpStack";
            this.grpStack.TabStop = false;
            // 
            // lbl3
            // 
            resources.ApplyResources(this.lbl3, "lbl3");
            this.lbl3.Name = "lbl3";
            // 
            // txtStackRet
            // 
            resources.ApplyResources(this.txtStackRet, "txtStackRet");
            this.txtStackRet.Name = "txtStackRet";
            // 
            // chkStack
            // 
            resources.ApplyResources(this.chkStack, "chkStack");
            this.chkStack.Name = "chkStack";
            this.chkStack.UseVisualStyleBackColor = true;
            this.chkStack.CheckedChanged += new System.EventHandler(this.chkStack_CheckedChanged);
            // 
            // lbl2
            // 
            resources.ApplyResources(this.lbl2, "lbl2");
            this.lbl2.Name = "lbl2";
            // 
            // txtStackBytes
            // 
            resources.ApplyResources(this.txtStackBytes, "txtStackBytes");
            this.txtStackBytes.Name = "txtStackBytes";
            // 
            // chkBefore
            // 
            resources.ApplyResources(this.chkBefore, "chkBefore");
            this.chkBefore.Name = "chkBefore";
            this.chkBefore.UseVisualStyleBackColor = true;
            this.chkBefore.CheckedChanged += new System.EventHandler(this.chkBefore_CheckedChanged);
            // 
            // grpBefore
            // 
            this.grpBefore.Controls.Add(this.chkBeforeVerdict);
            this.grpBefore.Controls.Add(this.cboBeforeVerdict);
            this.grpBefore.Controls.Add(this.txtBeforeAction);
            this.grpBefore.Controls.Add(this.chkBefore);
            resources.ApplyResources(this.grpBefore, "grpBefore");
            this.grpBefore.Name = "grpBefore";
            this.grpBefore.TabStop = false;
            // 
            // chkBeforeVerdict
            // 
            resources.ApplyResources(this.chkBeforeVerdict, "chkBeforeVerdict");
            this.chkBeforeVerdict.Name = "chkBeforeVerdict";
            this.chkBeforeVerdict.UseVisualStyleBackColor = true;
            this.chkBeforeVerdict.CheckedChanged += new System.EventHandler(this.chkBeforeVerdict_CheckedChanged);
            // 
            // cboBeforeVerdict
            // 
            this.cboBeforeVerdict.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cboBeforeVerdict, "cboBeforeVerdict");
            this.cboBeforeVerdict.FormattingEnabled = true;
            this.cboBeforeVerdict.Items.AddRange(new object[] {
            resources.GetString("cboBeforeVerdict.Items"),
            resources.GetString("cboBeforeVerdict.Items1"),
            resources.GetString("cboBeforeVerdict.Items2")});
            this.cboBeforeVerdict.Name = "cboBeforeVerdict";
            // 
            // txtBeforeAction
            // 
            resources.ApplyResources(this.txtBeforeAction, "txtBeforeAction");
            this.txtBeforeAction.Name = "txtBeforeAction";
            // 
            // grpAfter
            // 
            this.grpAfter.Controls.Add(this.chkAfterVerdict);
            this.grpAfter.Controls.Add(this.cboAfterVerdict);
            this.grpAfter.Controls.Add(this.txtAfterAction);
            this.grpAfter.Controls.Add(this.chkAfter);
            resources.ApplyResources(this.grpAfter, "grpAfter");
            this.grpAfter.Name = "grpAfter";
            this.grpAfter.TabStop = false;
            // 
            // chkAfterVerdict
            // 
            resources.ApplyResources(this.chkAfterVerdict, "chkAfterVerdict");
            this.chkAfterVerdict.Name = "chkAfterVerdict";
            this.chkAfterVerdict.UseVisualStyleBackColor = true;
            this.chkAfterVerdict.CheckedChanged += new System.EventHandler(this.chkAfterVerdict_CheckedChanged);
            // 
            // cboAfterVerdict
            // 
            this.cboAfterVerdict.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cboAfterVerdict, "cboAfterVerdict");
            this.cboAfterVerdict.FormattingEnabled = true;
            this.cboAfterVerdict.Items.AddRange(new object[] {
            resources.GetString("cboAfterVerdict.Items"),
            resources.GetString("cboAfterVerdict.Items1")});
            this.cboAfterVerdict.Name = "cboAfterVerdict";
            // 
            // txtAfterAction
            // 
            resources.ApplyResources(this.txtAfterAction, "txtAfterAction");
            this.txtAfterAction.Name = "txtAfterAction";
            // 
            // chkAfter
            // 
            resources.ApplyResources(this.chkAfter, "chkAfter");
            this.chkAfter.Name = "chkAfter";
            this.chkAfter.UseVisualStyleBackColor = true;
            this.chkAfter.CheckedChanged += new System.EventHandler(this.chkAfter_CheckedChanged);
            // 
            // FDlgAddHook
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.grpAfter);
            this.Controls.Add(this.grpBefore);
            this.Controls.Add(this.grpStack);
            this.Controls.Add(this.grpFunction);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FDlgAddHook";
            this.ShowInTaskbar = false;
            this.VisibleChanged += new System.EventHandler(this.FDlgAddHook_VisibleChanged);
            this.grpFunction.ResumeLayout(false);
            this.grpFunction.PerformLayout();
            this.grpStack.ResumeLayout(false);
            this.grpStack.PerformLayout();
            this.grpBefore.ResumeLayout(false);
            this.grpBefore.PerformLayout();
            this.grpAfter.ResumeLayout(false);
            this.grpAfter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpFunction;
        private System.Windows.Forms.ComboBox cboAddrMode;
        private System.Windows.Forms.TextBox txtAddr;
        private System.Windows.Forms.ComboBox cboConvention;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.GroupBox grpStack;
        private System.Windows.Forms.Label lbl3;
        private System.Windows.Forms.TextBox txtStackRet;
        private System.Windows.Forms.CheckBox chkStack;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.TextBox txtStackBytes;
        private System.Windows.Forms.CheckBox chkBefore;
        private System.Windows.Forms.GroupBox grpBefore;
        private System.Windows.Forms.CheckBox chkBeforeVerdict;
        private System.Windows.Forms.ComboBox cboBeforeVerdict;
        private System.Windows.Forms.TextBox txtBeforeAction;
        private System.Windows.Forms.GroupBox grpAfter;
        private System.Windows.Forms.CheckBox chkAfterVerdict;
        private System.Windows.Forms.ComboBox cboAfterVerdict;
        private System.Windows.Forms.TextBox txtAfterAction;
        private System.Windows.Forms.CheckBox chkAfter;
    }
}