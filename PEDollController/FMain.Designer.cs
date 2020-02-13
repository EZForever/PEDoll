namespace PEDollController
{
    partial class FMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.mnuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSpr1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpCommands = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpSpr1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPanels = new System.Windows.Forms.TabControl();
            this.tabPageListener = new System.Windows.Forms.TabPage();
            this.tabPageMonitor = new System.Windows.Forms.TabPage();
            this.tabPageDoll = new System.Windows.Forms.TabPage();
            this.tabPageHooked = new System.Windows.Forms.TabPage();
            this.tabPageDumps = new System.Windows.Forms.TabPage();
            this.lstListenerTargets = new System.Windows.Forms.ListView();
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmPID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmBits = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpCLI = new System.Windows.Forms.GroupBox();
            this.btnCLIExecute = new System.Windows.Forms.Button();
            this.txtCLICommand = new System.Windows.Forms.TextBox();
            this.grpListenerStart = new System.Windows.Forms.GroupBox();
            this.lblListener1 = new System.Windows.Forms.Label();
            this.txtListenerStartPort = new System.Windows.Forms.TextBox();
            this.chkListenerStartIPv6 = new System.Windows.Forms.CheckBox();
            this.btnListenerStart = new System.Windows.Forms.Button();
            this.lblListener2 = new System.Windows.Forms.Label();
            this.grpMonitorCurrent = new System.Windows.Forms.GroupBox();
            this.lblMonitorCurrent = new System.Windows.Forms.Label();
            this.btnMonitorCurrent = new System.Windows.Forms.Button();
            this.grpDollCurrent = new System.Windows.Forms.GroupBox();
            this.btnDollCurrent = new System.Windows.Forms.Button();
            this.lblDollCurrent = new System.Windows.Forms.Label();
            this.mnuStrip.SuspendLayout();
            this.tabPanels.SuspendLayout();
            this.tabPageListener.SuspendLayout();
            this.tabPageMonitor.SuspendLayout();
            this.tabPageDoll.SuspendLayout();
            this.grpCLI.SuspendLayout();
            this.grpListenerStart.SuspendLayout();
            this.grpMonitorCurrent.SuspendLayout();
            this.grpDollCurrent.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuStrip
            // 
            this.mnuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuHelp});
            resources.ApplyResources(this.mnuStrip, "mnuStrip");
            this.mnuStrip.Name = "mnuStrip";
            this.mnuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileLoad,
            this.mnuFileSpr1,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            resources.ApplyResources(this.mnuFile, "mnuFile");
            // 
            // mnuFileLoad
            // 
            this.mnuFileLoad.Name = "mnuFileLoad";
            resources.ApplyResources(this.mnuFileLoad, "mnuFileLoad");
            // 
            // mnuFileSpr1
            // 
            this.mnuFileSpr1.Name = "mnuFileSpr1";
            resources.ApplyResources(this.mnuFileSpr1, "mnuFileSpr1");
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            resources.ApplyResources(this.mnuFileExit, "mnuFileExit");
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpCommands,
            this.mnuHelpSpr1,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            // 
            // mnuHelpCommands
            // 
            this.mnuHelpCommands.Name = "mnuHelpCommands";
            resources.ApplyResources(this.mnuHelpCommands, "mnuHelpCommands");
            // 
            // mnuHelpSpr1
            // 
            this.mnuHelpSpr1.Name = "mnuHelpSpr1";
            resources.ApplyResources(this.mnuHelpSpr1, "mnuHelpSpr1");
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            // 
            // tabPanels
            // 
            this.tabPanels.Controls.Add(this.tabPageListener);
            this.tabPanels.Controls.Add(this.tabPageMonitor);
            this.tabPanels.Controls.Add(this.tabPageDoll);
            this.tabPanels.Controls.Add(this.tabPageHooked);
            this.tabPanels.Controls.Add(this.tabPageDumps);
            resources.ApplyResources(this.tabPanels, "tabPanels");
            this.tabPanels.Multiline = true;
            this.tabPanels.Name = "tabPanels";
            this.tabPanels.SelectedIndex = 0;
            // 
            // tabPageListener
            // 
            this.tabPageListener.Controls.Add(this.lblListener2);
            this.tabPageListener.Controls.Add(this.grpListenerStart);
            this.tabPageListener.Controls.Add(this.lstListenerTargets);
            resources.ApplyResources(this.tabPageListener, "tabPageListener");
            this.tabPageListener.Name = "tabPageListener";
            this.tabPageListener.UseVisualStyleBackColor = true;
            // 
            // tabPageMonitor
            // 
            this.tabPageMonitor.Controls.Add(this.grpMonitorCurrent);
            resources.ApplyResources(this.tabPageMonitor, "tabPageMonitor");
            this.tabPageMonitor.Name = "tabPageMonitor";
            this.tabPageMonitor.UseVisualStyleBackColor = true;
            // 
            // tabPageDoll
            // 
            this.tabPageDoll.Controls.Add(this.grpDollCurrent);
            resources.ApplyResources(this.tabPageDoll, "tabPageDoll");
            this.tabPageDoll.Name = "tabPageDoll";
            this.tabPageDoll.UseVisualStyleBackColor = true;
            // 
            // tabPageHooked
            // 
            resources.ApplyResources(this.tabPageHooked, "tabPageHooked");
            this.tabPageHooked.Name = "tabPageHooked";
            this.tabPageHooked.UseVisualStyleBackColor = true;
            // 
            // tabPageDumps
            // 
            resources.ApplyResources(this.tabPageDumps, "tabPageDumps");
            this.tabPageDumps.Name = "tabPageDumps";
            this.tabPageDumps.UseVisualStyleBackColor = true;
            // 
            // lstListenerTargets
            // 
            this.lstListenerTargets.CheckBoxes = true;
            this.lstListenerTargets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName,
            this.clmType,
            this.clmStatus,
            this.clmPID,
            this.clmBits});
            this.lstListenerTargets.FullRowSelect = true;
            this.lstListenerTargets.GridLines = true;
            this.lstListenerTargets.HideSelection = false;
            resources.ApplyResources(this.lstListenerTargets, "lstListenerTargets");
            this.lstListenerTargets.MultiSelect = false;
            this.lstListenerTargets.Name = "lstListenerTargets";
            this.lstListenerTargets.UseCompatibleStateImageBehavior = false;
            this.lstListenerTargets.View = System.Windows.Forms.View.Details;
            // 
            // clmName
            // 
            resources.ApplyResources(this.clmName, "clmName");
            // 
            // clmType
            // 
            resources.ApplyResources(this.clmType, "clmType");
            // 
            // clmStatus
            // 
            resources.ApplyResources(this.clmStatus, "clmStatus");
            // 
            // clmPID
            // 
            resources.ApplyResources(this.clmPID, "clmPID");
            // 
            // clmBits
            // 
            resources.ApplyResources(this.clmBits, "clmBits");
            // 
            // grpCLI
            // 
            this.grpCLI.Controls.Add(this.txtCLICommand);
            this.grpCLI.Controls.Add(this.btnCLIExecute);
            resources.ApplyResources(this.grpCLI, "grpCLI");
            this.grpCLI.Name = "grpCLI";
            this.grpCLI.TabStop = false;
            // 
            // btnCLIExecute
            // 
            resources.ApplyResources(this.btnCLIExecute, "btnCLIExecute");
            this.btnCLIExecute.Name = "btnCLIExecute";
            this.btnCLIExecute.UseVisualStyleBackColor = true;
            this.btnCLIExecute.Click += new System.EventHandler(this.btnCLIExecute_Click);
            // 
            // txtCLICommand
            // 
            resources.ApplyResources(this.txtCLICommand, "txtCLICommand");
            this.txtCLICommand.Name = "txtCLICommand";
            this.txtCLICommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCLICommand_KeyPress);
            // 
            // grpListenerStart
            // 
            this.grpListenerStart.Controls.Add(this.btnListenerStart);
            this.grpListenerStart.Controls.Add(this.chkListenerStartIPv6);
            this.grpListenerStart.Controls.Add(this.txtListenerStartPort);
            this.grpListenerStart.Controls.Add(this.lblListener1);
            resources.ApplyResources(this.grpListenerStart, "grpListenerStart");
            this.grpListenerStart.Name = "grpListenerStart";
            this.grpListenerStart.TabStop = false;
            // 
            // lblListener1
            // 
            resources.ApplyResources(this.lblListener1, "lblListener1");
            this.lblListener1.Name = "lblListener1";
            // 
            // txtListenerStartPort
            // 
            resources.ApplyResources(this.txtListenerStartPort, "txtListenerStartPort");
            this.txtListenerStartPort.Name = "txtListenerStartPort";
            // 
            // chkListenerStartIPv6
            // 
            resources.ApplyResources(this.chkListenerStartIPv6, "chkListenerStartIPv6");
            this.chkListenerStartIPv6.Name = "chkListenerStartIPv6";
            this.chkListenerStartIPv6.UseVisualStyleBackColor = true;
            // 
            // btnListenerStart
            // 
            resources.ApplyResources(this.btnListenerStart, "btnListenerStart");
            this.btnListenerStart.Name = "btnListenerStart";
            this.btnListenerStart.UseVisualStyleBackColor = true;
            // 
            // lblListener2
            // 
            resources.ApplyResources(this.lblListener2, "lblListener2");
            this.lblListener2.Name = "lblListener2";
            // 
            // grpMonitorCurrent
            // 
            this.grpMonitorCurrent.Controls.Add(this.btnMonitorCurrent);
            this.grpMonitorCurrent.Controls.Add(this.lblMonitorCurrent);
            resources.ApplyResources(this.grpMonitorCurrent, "grpMonitorCurrent");
            this.grpMonitorCurrent.Name = "grpMonitorCurrent";
            this.grpMonitorCurrent.TabStop = false;
            // 
            // lblMonitorCurrent
            // 
            resources.ApplyResources(this.lblMonitorCurrent, "lblMonitorCurrent");
            this.lblMonitorCurrent.Name = "lblMonitorCurrent";
            // 
            // btnMonitorCurrent
            // 
            resources.ApplyResources(this.btnMonitorCurrent, "btnMonitorCurrent");
            this.btnMonitorCurrent.Name = "btnMonitorCurrent";
            this.btnMonitorCurrent.UseVisualStyleBackColor = true;
            // 
            // grpDollCurrent
            // 
            this.grpDollCurrent.Controls.Add(this.btnDollCurrent);
            this.grpDollCurrent.Controls.Add(this.lblDollCurrent);
            resources.ApplyResources(this.grpDollCurrent, "grpDollCurrent");
            this.grpDollCurrent.Name = "grpDollCurrent";
            this.grpDollCurrent.TabStop = false;
            // 
            // btnDollCurrent
            // 
            resources.ApplyResources(this.btnDollCurrent, "btnDollCurrent");
            this.btnDollCurrent.Name = "btnDollCurrent";
            this.btnDollCurrent.UseVisualStyleBackColor = true;
            // 
            // lblDollCurrent
            // 
            resources.ApplyResources(this.lblDollCurrent, "lblDollCurrent");
            this.lblDollCurrent.Name = "lblDollCurrent";
            // 
            // FMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpCLI);
            this.Controls.Add(this.tabPanels);
            this.Controls.Add(this.mnuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.mnuStrip;
            this.MaximizeBox = false;
            this.Name = "FMain";
            this.mnuStrip.ResumeLayout(false);
            this.mnuStrip.PerformLayout();
            this.tabPanels.ResumeLayout(false);
            this.tabPageListener.ResumeLayout(false);
            this.tabPageListener.PerformLayout();
            this.tabPageMonitor.ResumeLayout(false);
            this.tabPageDoll.ResumeLayout(false);
            this.grpCLI.ResumeLayout(false);
            this.grpCLI.PerformLayout();
            this.grpListenerStart.ResumeLayout(false);
            this.grpListenerStart.PerformLayout();
            this.grpMonitorCurrent.ResumeLayout(false);
            this.grpDollCurrent.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileLoad;
        private System.Windows.Forms.ToolStripSeparator mnuFileSpr1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpCommands;
        private System.Windows.Forms.ToolStripSeparator mnuHelpSpr1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.TabControl tabPanels;
        private System.Windows.Forms.TabPage tabPageListener;
        private System.Windows.Forms.TabPage tabPageMonitor;
        private System.Windows.Forms.TabPage tabPageDoll;
        private System.Windows.Forms.TabPage tabPageHooked;
        private System.Windows.Forms.TabPage tabPageDumps;
        private System.Windows.Forms.ListView lstListenerTargets;
        private System.Windows.Forms.ColumnHeader clmName;
        private System.Windows.Forms.ColumnHeader clmType;
        private System.Windows.Forms.ColumnHeader clmStatus;
        private System.Windows.Forms.ColumnHeader clmPID;
        private System.Windows.Forms.ColumnHeader clmBits;
        private System.Windows.Forms.GroupBox grpCLI;
        private System.Windows.Forms.TextBox txtCLICommand;
        private System.Windows.Forms.Button btnCLIExecute;
        private System.Windows.Forms.GroupBox grpListenerStart;
        private System.Windows.Forms.Button btnListenerStart;
        private System.Windows.Forms.CheckBox chkListenerStartIPv6;
        private System.Windows.Forms.TextBox txtListenerStartPort;
        private System.Windows.Forms.Label lblListener1;
        private System.Windows.Forms.Label lblListener2;
        private System.Windows.Forms.GroupBox grpMonitorCurrent;
        private System.Windows.Forms.Button btnMonitorCurrent;
        private System.Windows.Forms.Label lblMonitorCurrent;
        private System.Windows.Forms.GroupBox grpDollCurrent;
        private System.Windows.Forms.Button btnDollCurrent;
        private System.Windows.Forms.Label lblDollCurrent;
    }
}

