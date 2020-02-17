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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.mnuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSpr1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileMonitorX86 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileMonitorX64 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSpr2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpCommands = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpSpr1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPanels = new System.Windows.Forms.TabControl();
            this.tabPageListener = new System.Windows.Forms.TabPage();
            this.pnlListenerStart = new System.Windows.Forms.Panel();
            this.lblListener3 = new System.Windows.Forms.Label();
            this.txtListenerStartPort = new System.Windows.Forms.TextBox();
            this.chkListenerStartIPv6 = new System.Windows.Forms.CheckBox();
            this.btnListenerStart = new System.Windows.Forms.Button();
            this.grpListenerTarget = new System.Windows.Forms.GroupBox();
            this.lstListenerTargets = new System.Windows.Forms.ListView();
            this.clmTargetsSelected = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmTargetsID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmTargetsName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmTargetsType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmTargetsStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmTargetsPID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmTargetsBits = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblListener1 = new System.Windows.Forms.Label();
            this.lblListener2 = new System.Windows.Forms.Label();
            this.tabPageDumps = new System.Windows.Forms.TabPage();
            this.lblDumps1 = new System.Windows.Forms.Label();
            this.cboDumpFormats = new System.Windows.Forms.ComboBox();
            this.lstDumps = new System.Windows.Forms.ListView();
            this.clmDumpsID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDumpsSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDumpsSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDumpSave = new System.Windows.Forms.Button();
            this.btnDumpShow = new System.Windows.Forms.Button();
            this.txtDumpContent = new System.Windows.Forms.TextBox();
            this.tabPageMonitor = new System.Windows.Forms.TabPage();
            this.grpMonitorShell = new System.Windows.Forms.GroupBox();
            this.chkMonitorShellKeep = new System.Windows.Forms.CheckBox();
            this.lblMonitor2 = new System.Windows.Forms.Label();
            this.btnMonitorShell = new System.Windows.Forms.Button();
            this.txtMonitorShell = new System.Windows.Forms.TextBox();
            this.grpMonitorKill = new System.Windows.Forms.GroupBox();
            this.btnKillInvoke = new System.Windows.Forms.Button();
            this.btnKillBrowse = new System.Windows.Forms.Button();
            this.txtKillPID = new System.Windows.Forms.TextBox();
            this.optKillPID = new System.Windows.Forms.RadioButton();
            this.txtKillName = new System.Windows.Forms.TextBox();
            this.optKillName = new System.Windows.Forms.RadioButton();
            this.lblMonitor1 = new System.Windows.Forms.Label();
            this.lblMonitorCurrent = new System.Windows.Forms.Label();
            this.btnMonitorCurrent = new System.Windows.Forms.Button();
            this.grpMonitorDoll = new System.Windows.Forms.GroupBox();
            this.btnDollInvoke = new System.Windows.Forms.Button();
            this.btnDollBrowse = new System.Windows.Forms.Button();
            this.txtDollPID = new System.Windows.Forms.TextBox();
            this.optDollAttach = new System.Windows.Forms.RadioButton();
            this.txtDollCmdline = new System.Windows.Forms.TextBox();
            this.optDollLaunch = new System.Windows.Forms.RadioButton();
            this.lblMonitor0 = new System.Windows.Forms.Label();
            this.tabPageDoll = new System.Windows.Forms.TabPage();
            this.grpDollBreak = new System.Windows.Forms.GroupBox();
            this.btnDollBreak = new System.Windows.Forms.Button();
            this.grpDollLoaddll = new System.Windows.Forms.GroupBox();
            this.lblDoll2 = new System.Windows.Forms.Label();
            this.btnDollLoaddll = new System.Windows.Forms.Button();
            this.txtDollLoaddll = new System.Windows.Forms.TextBox();
            this.grpDollHooks = new System.Windows.Forms.GroupBox();
            this.btnHooksAdd = new System.Windows.Forms.Button();
            this.btnHooksRemove = new System.Windows.Forms.Button();
            this.lstDollHooks = new System.Windows.Forms.ListView();
            this.clmHooksID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmHooksOep = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmHooksName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblDoll1 = new System.Windows.Forms.Label();
            this.lblDollCurrent = new System.Windows.Forms.Label();
            this.btnDollCurrent = new System.Windows.Forms.Button();
            this.lblDoll0 = new System.Windows.Forms.Label();
            this.tabPageHooked = new System.Windows.Forms.TabPage();
            this.grpHookedEval = new System.Windows.Forms.GroupBox();
            this.txtHookedEval = new System.Windows.Forms.TextBox();
            this.btnHookedEval = new System.Windows.Forms.Button();
            this.grpHookedResults = new System.Windows.Forms.GroupBox();
            this.txtHookedResults = new System.Windows.Forms.TextBox();
            this.pnlHookVerdict = new System.Windows.Forms.Panel();
            this.btnVerdictApprove = new System.Windows.Forms.Button();
            this.btnVerdictReject = new System.Windows.Forms.Button();
            this.btnVerdictTerminate = new System.Windows.Forms.Button();
            this.lblHooked1 = new System.Windows.Forms.Label();
            this.lblHookedCurrent = new System.Windows.Forms.Label();
            this.lblHooked0 = new System.Windows.Forms.Label();
            this.grpCLI = new System.Windows.Forms.GroupBox();
            this.txtCLICommand = new System.Windows.Forms.TextBox();
            this.btnCLIExecute = new System.Windows.Forms.Button();
            this.tipError = new System.Windows.Forms.ToolTip(this.components);
            this.dlgDumpSave = new System.Windows.Forms.SaveFileDialog();
            this.dlgFileLoadOpen = new System.Windows.Forms.OpenFileDialog();
            this.mnuStrip.SuspendLayout();
            this.tabPanels.SuspendLayout();
            this.tabPageListener.SuspendLayout();
            this.pnlListenerStart.SuspendLayout();
            this.grpListenerTarget.SuspendLayout();
            this.tabPageDumps.SuspendLayout();
            this.tabPageMonitor.SuspendLayout();
            this.grpMonitorShell.SuspendLayout();
            this.grpMonitorKill.SuspendLayout();
            this.grpMonitorDoll.SuspendLayout();
            this.tabPageDoll.SuspendLayout();
            this.grpDollBreak.SuspendLayout();
            this.grpDollLoaddll.SuspendLayout();
            this.grpDollHooks.SuspendLayout();
            this.tabPageHooked.SuspendLayout();
            this.grpHookedEval.SuspendLayout();
            this.grpHookedResults.SuspendLayout();
            this.pnlHookVerdict.SuspendLayout();
            this.grpCLI.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuStrip
            // 
            resources.ApplyResources(this.mnuStrip, "mnuStrip");
            this.mnuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuHelp});
            this.mnuStrip.Name = "mnuStrip";
            this.mnuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.tipError.SetToolTip(this.mnuStrip, resources.GetString("mnuStrip.ToolTip"));
            // 
            // mnuFile
            // 
            resources.ApplyResources(this.mnuFile, "mnuFile");
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileLoad,
            this.mnuFileSpr1,
            this.mnuFileMonitorX86,
            this.mnuFileMonitorX64,
            this.mnuFileSpr2,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            // 
            // mnuFileLoad
            // 
            resources.ApplyResources(this.mnuFileLoad, "mnuFileLoad");
            this.mnuFileLoad.Name = "mnuFileLoad";
            this.mnuFileLoad.Click += new System.EventHandler(this.mnuFileLoad_Click);
            // 
            // mnuFileSpr1
            // 
            resources.ApplyResources(this.mnuFileSpr1, "mnuFileSpr1");
            this.mnuFileSpr1.Name = "mnuFileSpr1";
            // 
            // mnuFileMonitorX86
            // 
            resources.ApplyResources(this.mnuFileMonitorX86, "mnuFileMonitorX86");
            this.mnuFileMonitorX86.Name = "mnuFileMonitorX86";
            this.mnuFileMonitorX86.Click += new System.EventHandler(this.mnuFileMonitorX86_Click);
            // 
            // mnuFileMonitorX64
            // 
            resources.ApplyResources(this.mnuFileMonitorX64, "mnuFileMonitorX64");
            this.mnuFileMonitorX64.Name = "mnuFileMonitorX64";
            this.mnuFileMonitorX64.Click += new System.EventHandler(this.mnuFileMonitorX64_Click);
            // 
            // mnuFileSpr2
            // 
            resources.ApplyResources(this.mnuFileSpr2, "mnuFileSpr2");
            this.mnuFileSpr2.Name = "mnuFileSpr2";
            // 
            // mnuFileExit
            // 
            resources.ApplyResources(this.mnuFileExit, "mnuFileExit");
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuHelp
            // 
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpCommands,
            this.mnuHelpSpr1,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            // 
            // mnuHelpCommands
            // 
            resources.ApplyResources(this.mnuHelpCommands, "mnuHelpCommands");
            this.mnuHelpCommands.Name = "mnuHelpCommands";
            this.mnuHelpCommands.Click += new System.EventHandler(this.mnuHelpCommands_Click);
            // 
            // mnuHelpSpr1
            // 
            resources.ApplyResources(this.mnuHelpSpr1, "mnuHelpSpr1");
            this.mnuHelpSpr1.Name = "mnuHelpSpr1";
            // 
            // mnuHelpAbout
            // 
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // tabPanels
            // 
            resources.ApplyResources(this.tabPanels, "tabPanels");
            this.tabPanels.Controls.Add(this.tabPageListener);
            this.tabPanels.Controls.Add(this.tabPageDumps);
            this.tabPanels.Controls.Add(this.tabPageMonitor);
            this.tabPanels.Controls.Add(this.tabPageDoll);
            this.tabPanels.Controls.Add(this.tabPageHooked);
            this.tabPanels.Multiline = true;
            this.tabPanels.Name = "tabPanels";
            this.tabPanels.SelectedIndex = 0;
            this.tipError.SetToolTip(this.tabPanels, resources.GetString("tabPanels.ToolTip"));
            // 
            // tabPageListener
            // 
            resources.ApplyResources(this.tabPageListener, "tabPageListener");
            this.tabPageListener.Controls.Add(this.pnlListenerStart);
            this.tabPageListener.Controls.Add(this.grpListenerTarget);
            this.tabPageListener.Controls.Add(this.lblListener1);
            this.tabPageListener.Controls.Add(this.lblListener2);
            this.tabPageListener.Name = "tabPageListener";
            this.tipError.SetToolTip(this.tabPageListener, resources.GetString("tabPageListener.ToolTip"));
            this.tabPageListener.UseVisualStyleBackColor = true;
            // 
            // pnlListenerStart
            // 
            resources.ApplyResources(this.pnlListenerStart, "pnlListenerStart");
            this.pnlListenerStart.Controls.Add(this.lblListener3);
            this.pnlListenerStart.Controls.Add(this.txtListenerStartPort);
            this.pnlListenerStart.Controls.Add(this.chkListenerStartIPv6);
            this.pnlListenerStart.Controls.Add(this.btnListenerStart);
            this.pnlListenerStart.Name = "pnlListenerStart";
            this.tipError.SetToolTip(this.pnlListenerStart, resources.GetString("pnlListenerStart.ToolTip"));
            // 
            // lblListener3
            // 
            resources.ApplyResources(this.lblListener3, "lblListener3");
            this.lblListener3.Name = "lblListener3";
            this.tipError.SetToolTip(this.lblListener3, resources.GetString("lblListener3.ToolTip"));
            // 
            // txtListenerStartPort
            // 
            resources.ApplyResources(this.txtListenerStartPort, "txtListenerStartPort");
            this.txtListenerStartPort.Name = "txtListenerStartPort";
            this.tipError.SetToolTip(this.txtListenerStartPort, resources.GetString("txtListenerStartPort.ToolTip"));
            // 
            // chkListenerStartIPv6
            // 
            resources.ApplyResources(this.chkListenerStartIPv6, "chkListenerStartIPv6");
            this.chkListenerStartIPv6.Name = "chkListenerStartIPv6";
            this.tipError.SetToolTip(this.chkListenerStartIPv6, resources.GetString("chkListenerStartIPv6.ToolTip"));
            this.chkListenerStartIPv6.UseVisualStyleBackColor = true;
            // 
            // btnListenerStart
            // 
            resources.ApplyResources(this.btnListenerStart, "btnListenerStart");
            this.btnListenerStart.Name = "btnListenerStart";
            this.tipError.SetToolTip(this.btnListenerStart, resources.GetString("btnListenerStart.ToolTip"));
            this.btnListenerStart.UseVisualStyleBackColor = true;
            this.btnListenerStart.Click += new System.EventHandler(this.btnListenerStart_Click);
            // 
            // grpListenerTarget
            // 
            resources.ApplyResources(this.grpListenerTarget, "grpListenerTarget");
            this.grpListenerTarget.Controls.Add(this.lstListenerTargets);
            this.grpListenerTarget.Name = "grpListenerTarget";
            this.grpListenerTarget.TabStop = false;
            this.tipError.SetToolTip(this.grpListenerTarget, resources.GetString("grpListenerTarget.ToolTip"));
            // 
            // lstListenerTargets
            // 
            resources.ApplyResources(this.lstListenerTargets, "lstListenerTargets");
            this.lstListenerTargets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmTargetsSelected,
            this.clmTargetsID,
            this.clmTargetsName,
            this.clmTargetsType,
            this.clmTargetsStatus,
            this.clmTargetsPID,
            this.clmTargetsBits});
            this.lstListenerTargets.FullRowSelect = true;
            this.lstListenerTargets.GridLines = true;
            this.lstListenerTargets.HideSelection = false;
            this.lstListenerTargets.MultiSelect = false;
            this.lstListenerTargets.Name = "lstListenerTargets";
            this.tipError.SetToolTip(this.lstListenerTargets, resources.GetString("lstListenerTargets.ToolTip"));
            this.lstListenerTargets.UseCompatibleStateImageBehavior = false;
            this.lstListenerTargets.View = System.Windows.Forms.View.Details;
            this.lstListenerTargets.DoubleClick += new System.EventHandler(this.lstListenerTargets_DoubleClick);
            // 
            // clmTargetsSelected
            // 
            resources.ApplyResources(this.clmTargetsSelected, "clmTargetsSelected");
            // 
            // clmTargetsID
            // 
            resources.ApplyResources(this.clmTargetsID, "clmTargetsID");
            // 
            // clmTargetsName
            // 
            resources.ApplyResources(this.clmTargetsName, "clmTargetsName");
            // 
            // clmTargetsType
            // 
            resources.ApplyResources(this.clmTargetsType, "clmTargetsType");
            // 
            // clmTargetsStatus
            // 
            resources.ApplyResources(this.clmTargetsStatus, "clmTargetsStatus");
            // 
            // clmTargetsPID
            // 
            resources.ApplyResources(this.clmTargetsPID, "clmTargetsPID");
            // 
            // clmTargetsBits
            // 
            resources.ApplyResources(this.clmTargetsBits, "clmTargetsBits");
            // 
            // lblListener1
            // 
            resources.ApplyResources(this.lblListener1, "lblListener1");
            this.lblListener1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblListener1.Name = "lblListener1";
            this.tipError.SetToolTip(this.lblListener1, resources.GetString("lblListener1.ToolTip"));
            // 
            // lblListener2
            // 
            resources.ApplyResources(this.lblListener2, "lblListener2");
            this.lblListener2.Name = "lblListener2";
            this.tipError.SetToolTip(this.lblListener2, resources.GetString("lblListener2.ToolTip"));
            // 
            // tabPageDumps
            // 
            resources.ApplyResources(this.tabPageDumps, "tabPageDumps");
            this.tabPageDumps.Controls.Add(this.lblDumps1);
            this.tabPageDumps.Controls.Add(this.cboDumpFormats);
            this.tabPageDumps.Controls.Add(this.lstDumps);
            this.tabPageDumps.Controls.Add(this.btnDumpSave);
            this.tabPageDumps.Controls.Add(this.btnDumpShow);
            this.tabPageDumps.Controls.Add(this.txtDumpContent);
            this.tabPageDumps.Name = "tabPageDumps";
            this.tipError.SetToolTip(this.tabPageDumps, resources.GetString("tabPageDumps.ToolTip"));
            this.tabPageDumps.UseVisualStyleBackColor = true;
            // 
            // lblDumps1
            // 
            resources.ApplyResources(this.lblDumps1, "lblDumps1");
            this.lblDumps1.Name = "lblDumps1";
            this.tipError.SetToolTip(this.lblDumps1, resources.GetString("lblDumps1.ToolTip"));
            // 
            // cboDumpFormats
            // 
            resources.ApplyResources(this.cboDumpFormats, "cboDumpFormats");
            this.cboDumpFormats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDumpFormats.FormattingEnabled = true;
            this.cboDumpFormats.Name = "cboDumpFormats";
            this.tipError.SetToolTip(this.cboDumpFormats, resources.GetString("cboDumpFormats.ToolTip"));
            // 
            // lstDumps
            // 
            resources.ApplyResources(this.lstDumps, "lstDumps");
            this.lstDumps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmDumpsID,
            this.clmDumpsSize,
            this.clmDumpsSource});
            this.lstDumps.FullRowSelect = true;
            this.lstDumps.GridLines = true;
            this.lstDumps.HideSelection = false;
            this.lstDumps.MultiSelect = false;
            this.lstDumps.Name = "lstDumps";
            this.tipError.SetToolTip(this.lstDumps, resources.GetString("lstDumps.ToolTip"));
            this.lstDumps.UseCompatibleStateImageBehavior = false;
            this.lstDumps.View = System.Windows.Forms.View.Details;
            this.lstDumps.DoubleClick += new System.EventHandler(this.lstDumps_DoubleClick);
            // 
            // clmDumpsID
            // 
            resources.ApplyResources(this.clmDumpsID, "clmDumpsID");
            // 
            // clmDumpsSize
            // 
            resources.ApplyResources(this.clmDumpsSize, "clmDumpsSize");
            // 
            // clmDumpsSource
            // 
            resources.ApplyResources(this.clmDumpsSource, "clmDumpsSource");
            // 
            // btnDumpSave
            // 
            resources.ApplyResources(this.btnDumpSave, "btnDumpSave");
            this.btnDumpSave.Name = "btnDumpSave";
            this.tipError.SetToolTip(this.btnDumpSave, resources.GetString("btnDumpSave.ToolTip"));
            this.btnDumpSave.UseVisualStyleBackColor = true;
            this.btnDumpSave.Click += new System.EventHandler(this.btnDumpSave_Click);
            // 
            // btnDumpShow
            // 
            resources.ApplyResources(this.btnDumpShow, "btnDumpShow");
            this.btnDumpShow.Name = "btnDumpShow";
            this.tipError.SetToolTip(this.btnDumpShow, resources.GetString("btnDumpShow.ToolTip"));
            this.btnDumpShow.UseVisualStyleBackColor = true;
            this.btnDumpShow.Click += new System.EventHandler(this.btnDumpShow_Click);
            // 
            // txtDumpContent
            // 
            resources.ApplyResources(this.txtDumpContent, "txtDumpContent");
            this.txtDumpContent.Name = "txtDumpContent";
            this.txtDumpContent.ReadOnly = true;
            this.tipError.SetToolTip(this.txtDumpContent, resources.GetString("txtDumpContent.ToolTip"));
            // 
            // tabPageMonitor
            // 
            resources.ApplyResources(this.tabPageMonitor, "tabPageMonitor");
            this.tabPageMonitor.Controls.Add(this.grpMonitorShell);
            this.tabPageMonitor.Controls.Add(this.grpMonitorKill);
            this.tabPageMonitor.Controls.Add(this.lblMonitor1);
            this.tabPageMonitor.Controls.Add(this.lblMonitorCurrent);
            this.tabPageMonitor.Controls.Add(this.btnMonitorCurrent);
            this.tabPageMonitor.Controls.Add(this.grpMonitorDoll);
            this.tabPageMonitor.Controls.Add(this.lblMonitor0);
            this.tabPageMonitor.Name = "tabPageMonitor";
            this.tipError.SetToolTip(this.tabPageMonitor, resources.GetString("tabPageMonitor.ToolTip"));
            this.tabPageMonitor.UseVisualStyleBackColor = true;
            // 
            // grpMonitorShell
            // 
            resources.ApplyResources(this.grpMonitorShell, "grpMonitorShell");
            this.grpMonitorShell.Controls.Add(this.chkMonitorShellKeep);
            this.grpMonitorShell.Controls.Add(this.lblMonitor2);
            this.grpMonitorShell.Controls.Add(this.btnMonitorShell);
            this.grpMonitorShell.Controls.Add(this.txtMonitorShell);
            this.grpMonitorShell.Name = "grpMonitorShell";
            this.grpMonitorShell.TabStop = false;
            this.tipError.SetToolTip(this.grpMonitorShell, resources.GetString("grpMonitorShell.ToolTip"));
            // 
            // chkMonitorShellKeep
            // 
            resources.ApplyResources(this.chkMonitorShellKeep, "chkMonitorShellKeep");
            this.chkMonitorShellKeep.Name = "chkMonitorShellKeep";
            this.tipError.SetToolTip(this.chkMonitorShellKeep, resources.GetString("chkMonitorShellKeep.ToolTip"));
            this.chkMonitorShellKeep.UseVisualStyleBackColor = true;
            // 
            // lblMonitor2
            // 
            resources.ApplyResources(this.lblMonitor2, "lblMonitor2");
            this.lblMonitor2.Name = "lblMonitor2";
            this.tipError.SetToolTip(this.lblMonitor2, resources.GetString("lblMonitor2.ToolTip"));
            // 
            // btnMonitorShell
            // 
            resources.ApplyResources(this.btnMonitorShell, "btnMonitorShell");
            this.btnMonitorShell.Name = "btnMonitorShell";
            this.tipError.SetToolTip(this.btnMonitorShell, resources.GetString("btnMonitorShell.ToolTip"));
            this.btnMonitorShell.UseVisualStyleBackColor = true;
            this.btnMonitorShell.Click += new System.EventHandler(this.btnMonitorShell_Click);
            // 
            // txtMonitorShell
            // 
            resources.ApplyResources(this.txtMonitorShell, "txtMonitorShell");
            this.txtMonitorShell.Name = "txtMonitorShell";
            this.tipError.SetToolTip(this.txtMonitorShell, resources.GetString("txtMonitorShell.ToolTip"));
            // 
            // grpMonitorKill
            // 
            resources.ApplyResources(this.grpMonitorKill, "grpMonitorKill");
            this.grpMonitorKill.Controls.Add(this.btnKillInvoke);
            this.grpMonitorKill.Controls.Add(this.btnKillBrowse);
            this.grpMonitorKill.Controls.Add(this.txtKillPID);
            this.grpMonitorKill.Controls.Add(this.optKillPID);
            this.grpMonitorKill.Controls.Add(this.txtKillName);
            this.grpMonitorKill.Controls.Add(this.optKillName);
            this.grpMonitorKill.Name = "grpMonitorKill";
            this.grpMonitorKill.TabStop = false;
            this.tipError.SetToolTip(this.grpMonitorKill, resources.GetString("grpMonitorKill.ToolTip"));
            // 
            // btnKillInvoke
            // 
            resources.ApplyResources(this.btnKillInvoke, "btnKillInvoke");
            this.btnKillInvoke.Name = "btnKillInvoke";
            this.tipError.SetToolTip(this.btnKillInvoke, resources.GetString("btnKillInvoke.ToolTip"));
            this.btnKillInvoke.UseVisualStyleBackColor = true;
            this.btnKillInvoke.Click += new System.EventHandler(this.btnKillInvoke_Click);
            // 
            // btnKillBrowse
            // 
            resources.ApplyResources(this.btnKillBrowse, "btnKillBrowse");
            this.btnKillBrowse.Name = "btnKillBrowse";
            this.tipError.SetToolTip(this.btnKillBrowse, resources.GetString("btnKillBrowse.ToolTip"));
            this.btnKillBrowse.UseVisualStyleBackColor = true;
            this.btnKillBrowse.Click += new System.EventHandler(this.btnKillBrowse_Click);
            // 
            // txtKillPID
            // 
            resources.ApplyResources(this.txtKillPID, "txtKillPID");
            this.txtKillPID.Name = "txtKillPID";
            this.tipError.SetToolTip(this.txtKillPID, resources.GetString("txtKillPID.ToolTip"));
            // 
            // optKillPID
            // 
            resources.ApplyResources(this.optKillPID, "optKillPID");
            this.optKillPID.Name = "optKillPID";
            this.tipError.SetToolTip(this.optKillPID, resources.GetString("optKillPID.ToolTip"));
            this.optKillPID.UseVisualStyleBackColor = true;
            // 
            // txtKillName
            // 
            resources.ApplyResources(this.txtKillName, "txtKillName");
            this.txtKillName.Name = "txtKillName";
            this.tipError.SetToolTip(this.txtKillName, resources.GetString("txtKillName.ToolTip"));
            // 
            // optKillName
            // 
            resources.ApplyResources(this.optKillName, "optKillName");
            this.optKillName.Checked = true;
            this.optKillName.Name = "optKillName";
            this.optKillName.TabStop = true;
            this.tipError.SetToolTip(this.optKillName, resources.GetString("optKillName.ToolTip"));
            this.optKillName.UseVisualStyleBackColor = true;
            this.optKillName.CheckedChanged += new System.EventHandler(this.optKillName_CheckedChanged);
            // 
            // lblMonitor1
            // 
            resources.ApplyResources(this.lblMonitor1, "lblMonitor1");
            this.lblMonitor1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMonitor1.Name = "lblMonitor1";
            this.tipError.SetToolTip(this.lblMonitor1, resources.GetString("lblMonitor1.ToolTip"));
            // 
            // lblMonitorCurrent
            // 
            resources.ApplyResources(this.lblMonitorCurrent, "lblMonitorCurrent");
            this.lblMonitorCurrent.Name = "lblMonitorCurrent";
            this.tipError.SetToolTip(this.lblMonitorCurrent, resources.GetString("lblMonitorCurrent.ToolTip"));
            // 
            // btnMonitorCurrent
            // 
            resources.ApplyResources(this.btnMonitorCurrent, "btnMonitorCurrent");
            this.btnMonitorCurrent.Name = "btnMonitorCurrent";
            this.tipError.SetToolTip(this.btnMonitorCurrent, resources.GetString("btnMonitorCurrent.ToolTip"));
            this.btnMonitorCurrent.UseVisualStyleBackColor = true;
            this.btnMonitorCurrent.Click += new System.EventHandler(this.btnMonitorCurrent_Click);
            // 
            // grpMonitorDoll
            // 
            resources.ApplyResources(this.grpMonitorDoll, "grpMonitorDoll");
            this.grpMonitorDoll.Controls.Add(this.btnDollInvoke);
            this.grpMonitorDoll.Controls.Add(this.btnDollBrowse);
            this.grpMonitorDoll.Controls.Add(this.txtDollPID);
            this.grpMonitorDoll.Controls.Add(this.optDollAttach);
            this.grpMonitorDoll.Controls.Add(this.txtDollCmdline);
            this.grpMonitorDoll.Controls.Add(this.optDollLaunch);
            this.grpMonitorDoll.Name = "grpMonitorDoll";
            this.grpMonitorDoll.TabStop = false;
            this.tipError.SetToolTip(this.grpMonitorDoll, resources.GetString("grpMonitorDoll.ToolTip"));
            // 
            // btnDollInvoke
            // 
            resources.ApplyResources(this.btnDollInvoke, "btnDollInvoke");
            this.btnDollInvoke.Name = "btnDollInvoke";
            this.tipError.SetToolTip(this.btnDollInvoke, resources.GetString("btnDollInvoke.ToolTip"));
            this.btnDollInvoke.UseVisualStyleBackColor = true;
            this.btnDollInvoke.Click += new System.EventHandler(this.btnDollInvoke_Click);
            // 
            // btnDollBrowse
            // 
            resources.ApplyResources(this.btnDollBrowse, "btnDollBrowse");
            this.btnDollBrowse.Name = "btnDollBrowse";
            this.tipError.SetToolTip(this.btnDollBrowse, resources.GetString("btnDollBrowse.ToolTip"));
            this.btnDollBrowse.UseVisualStyleBackColor = true;
            this.btnDollBrowse.Click += new System.EventHandler(this.btnDollBrowse_Click);
            // 
            // txtDollPID
            // 
            resources.ApplyResources(this.txtDollPID, "txtDollPID");
            this.txtDollPID.Name = "txtDollPID";
            this.tipError.SetToolTip(this.txtDollPID, resources.GetString("txtDollPID.ToolTip"));
            // 
            // optDollAttach
            // 
            resources.ApplyResources(this.optDollAttach, "optDollAttach");
            this.optDollAttach.Name = "optDollAttach";
            this.tipError.SetToolTip(this.optDollAttach, resources.GetString("optDollAttach.ToolTip"));
            this.optDollAttach.UseVisualStyleBackColor = true;
            // 
            // txtDollCmdline
            // 
            resources.ApplyResources(this.txtDollCmdline, "txtDollCmdline");
            this.txtDollCmdline.Name = "txtDollCmdline";
            this.tipError.SetToolTip(this.txtDollCmdline, resources.GetString("txtDollCmdline.ToolTip"));
            // 
            // optDollLaunch
            // 
            resources.ApplyResources(this.optDollLaunch, "optDollLaunch");
            this.optDollLaunch.Checked = true;
            this.optDollLaunch.Name = "optDollLaunch";
            this.optDollLaunch.TabStop = true;
            this.tipError.SetToolTip(this.optDollLaunch, resources.GetString("optDollLaunch.ToolTip"));
            this.optDollLaunch.UseVisualStyleBackColor = true;
            this.optDollLaunch.CheckedChanged += new System.EventHandler(this.optDollLaunch_CheckedChanged);
            // 
            // lblMonitor0
            // 
            resources.ApplyResources(this.lblMonitor0, "lblMonitor0");
            this.lblMonitor0.Name = "lblMonitor0";
            this.tipError.SetToolTip(this.lblMonitor0, resources.GetString("lblMonitor0.ToolTip"));
            // 
            // tabPageDoll
            // 
            resources.ApplyResources(this.tabPageDoll, "tabPageDoll");
            this.tabPageDoll.Controls.Add(this.grpDollBreak);
            this.tabPageDoll.Controls.Add(this.grpDollLoaddll);
            this.tabPageDoll.Controls.Add(this.grpDollHooks);
            this.tabPageDoll.Controls.Add(this.lblDoll1);
            this.tabPageDoll.Controls.Add(this.lblDollCurrent);
            this.tabPageDoll.Controls.Add(this.btnDollCurrent);
            this.tabPageDoll.Controls.Add(this.lblDoll0);
            this.tabPageDoll.Name = "tabPageDoll";
            this.tipError.SetToolTip(this.tabPageDoll, resources.GetString("tabPageDoll.ToolTip"));
            this.tabPageDoll.UseVisualStyleBackColor = true;
            // 
            // grpDollBreak
            // 
            resources.ApplyResources(this.grpDollBreak, "grpDollBreak");
            this.grpDollBreak.Controls.Add(this.btnDollBreak);
            this.grpDollBreak.Name = "grpDollBreak";
            this.grpDollBreak.TabStop = false;
            this.tipError.SetToolTip(this.grpDollBreak, resources.GetString("grpDollBreak.ToolTip"));
            // 
            // btnDollBreak
            // 
            resources.ApplyResources(this.btnDollBreak, "btnDollBreak");
            this.btnDollBreak.Name = "btnDollBreak";
            this.tipError.SetToolTip(this.btnDollBreak, resources.GetString("btnDollBreak.ToolTip"));
            this.btnDollBreak.UseVisualStyleBackColor = true;
            this.btnDollBreak.Click += new System.EventHandler(this.btnDollBreak_Click);
            // 
            // grpDollLoaddll
            // 
            resources.ApplyResources(this.grpDollLoaddll, "grpDollLoaddll");
            this.grpDollLoaddll.Controls.Add(this.lblDoll2);
            this.grpDollLoaddll.Controls.Add(this.btnDollLoaddll);
            this.grpDollLoaddll.Controls.Add(this.txtDollLoaddll);
            this.grpDollLoaddll.Name = "grpDollLoaddll";
            this.grpDollLoaddll.TabStop = false;
            this.tipError.SetToolTip(this.grpDollLoaddll, resources.GetString("grpDollLoaddll.ToolTip"));
            // 
            // lblDoll2
            // 
            resources.ApplyResources(this.lblDoll2, "lblDoll2");
            this.lblDoll2.Name = "lblDoll2";
            this.tipError.SetToolTip(this.lblDoll2, resources.GetString("lblDoll2.ToolTip"));
            // 
            // btnDollLoaddll
            // 
            resources.ApplyResources(this.btnDollLoaddll, "btnDollLoaddll");
            this.btnDollLoaddll.Name = "btnDollLoaddll";
            this.tipError.SetToolTip(this.btnDollLoaddll, resources.GetString("btnDollLoaddll.ToolTip"));
            this.btnDollLoaddll.UseVisualStyleBackColor = true;
            this.btnDollLoaddll.Click += new System.EventHandler(this.btnDollLoaddll_Click);
            // 
            // txtDollLoaddll
            // 
            resources.ApplyResources(this.txtDollLoaddll, "txtDollLoaddll");
            this.txtDollLoaddll.Name = "txtDollLoaddll";
            this.tipError.SetToolTip(this.txtDollLoaddll, resources.GetString("txtDollLoaddll.ToolTip"));
            // 
            // grpDollHooks
            // 
            resources.ApplyResources(this.grpDollHooks, "grpDollHooks");
            this.grpDollHooks.Controls.Add(this.btnHooksAdd);
            this.grpDollHooks.Controls.Add(this.btnHooksRemove);
            this.grpDollHooks.Controls.Add(this.lstDollHooks);
            this.grpDollHooks.Name = "grpDollHooks";
            this.grpDollHooks.TabStop = false;
            this.tipError.SetToolTip(this.grpDollHooks, resources.GetString("grpDollHooks.ToolTip"));
            // 
            // btnHooksAdd
            // 
            resources.ApplyResources(this.btnHooksAdd, "btnHooksAdd");
            this.btnHooksAdd.Name = "btnHooksAdd";
            this.tipError.SetToolTip(this.btnHooksAdd, resources.GetString("btnHooksAdd.ToolTip"));
            this.btnHooksAdd.UseVisualStyleBackColor = true;
            this.btnHooksAdd.Click += new System.EventHandler(this.btnHooksAdd_Click);
            // 
            // btnHooksRemove
            // 
            resources.ApplyResources(this.btnHooksRemove, "btnHooksRemove");
            this.btnHooksRemove.Name = "btnHooksRemove";
            this.tipError.SetToolTip(this.btnHooksRemove, resources.GetString("btnHooksRemove.ToolTip"));
            this.btnHooksRemove.UseVisualStyleBackColor = true;
            this.btnHooksRemove.Click += new System.EventHandler(this.btnHooksRemove_Click);
            // 
            // lstDollHooks
            // 
            resources.ApplyResources(this.lstDollHooks, "lstDollHooks");
            this.lstDollHooks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmHooksID,
            this.clmHooksOep,
            this.clmHooksName});
            this.lstDollHooks.FullRowSelect = true;
            this.lstDollHooks.GridLines = true;
            this.lstDollHooks.HideSelection = false;
            this.lstDollHooks.MultiSelect = false;
            this.lstDollHooks.Name = "lstDollHooks";
            this.tipError.SetToolTip(this.lstDollHooks, resources.GetString("lstDollHooks.ToolTip"));
            this.lstDollHooks.UseCompatibleStateImageBehavior = false;
            this.lstDollHooks.View = System.Windows.Forms.View.Details;
            // 
            // clmHooksID
            // 
            resources.ApplyResources(this.clmHooksID, "clmHooksID");
            // 
            // clmHooksOep
            // 
            resources.ApplyResources(this.clmHooksOep, "clmHooksOep");
            // 
            // clmHooksName
            // 
            resources.ApplyResources(this.clmHooksName, "clmHooksName");
            // 
            // lblDoll1
            // 
            resources.ApplyResources(this.lblDoll1, "lblDoll1");
            this.lblDoll1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDoll1.Name = "lblDoll1";
            this.tipError.SetToolTip(this.lblDoll1, resources.GetString("lblDoll1.ToolTip"));
            // 
            // lblDollCurrent
            // 
            resources.ApplyResources(this.lblDollCurrent, "lblDollCurrent");
            this.lblDollCurrent.Name = "lblDollCurrent";
            this.tipError.SetToolTip(this.lblDollCurrent, resources.GetString("lblDollCurrent.ToolTip"));
            // 
            // btnDollCurrent
            // 
            resources.ApplyResources(this.btnDollCurrent, "btnDollCurrent");
            this.btnDollCurrent.Name = "btnDollCurrent";
            this.tipError.SetToolTip(this.btnDollCurrent, resources.GetString("btnDollCurrent.ToolTip"));
            this.btnDollCurrent.UseVisualStyleBackColor = true;
            this.btnDollCurrent.Click += new System.EventHandler(this.btnDollCurrent_Click);
            // 
            // lblDoll0
            // 
            resources.ApplyResources(this.lblDoll0, "lblDoll0");
            this.lblDoll0.Name = "lblDoll0";
            this.tipError.SetToolTip(this.lblDoll0, resources.GetString("lblDoll0.ToolTip"));
            // 
            // tabPageHooked
            // 
            resources.ApplyResources(this.tabPageHooked, "tabPageHooked");
            this.tabPageHooked.Controls.Add(this.grpHookedEval);
            this.tabPageHooked.Controls.Add(this.grpHookedResults);
            this.tabPageHooked.Controls.Add(this.pnlHookVerdict);
            this.tabPageHooked.Controls.Add(this.lblHooked1);
            this.tabPageHooked.Controls.Add(this.lblHookedCurrent);
            this.tabPageHooked.Controls.Add(this.lblHooked0);
            this.tabPageHooked.Name = "tabPageHooked";
            this.tipError.SetToolTip(this.tabPageHooked, resources.GetString("tabPageHooked.ToolTip"));
            this.tabPageHooked.UseVisualStyleBackColor = true;
            // 
            // grpHookedEval
            // 
            resources.ApplyResources(this.grpHookedEval, "grpHookedEval");
            this.grpHookedEval.Controls.Add(this.txtHookedEval);
            this.grpHookedEval.Controls.Add(this.btnHookedEval);
            this.grpHookedEval.Name = "grpHookedEval";
            this.grpHookedEval.TabStop = false;
            this.tipError.SetToolTip(this.grpHookedEval, resources.GetString("grpHookedEval.ToolTip"));
            // 
            // txtHookedEval
            // 
            resources.ApplyResources(this.txtHookedEval, "txtHookedEval");
            this.txtHookedEval.Name = "txtHookedEval";
            this.tipError.SetToolTip(this.txtHookedEval, resources.GetString("txtHookedEval.ToolTip"));
            this.txtHookedEval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtHookedEval_KeyPress);
            // 
            // btnHookedEval
            // 
            resources.ApplyResources(this.btnHookedEval, "btnHookedEval");
            this.btnHookedEval.Name = "btnHookedEval";
            this.tipError.SetToolTip(this.btnHookedEval, resources.GetString("btnHookedEval.ToolTip"));
            this.btnHookedEval.UseVisualStyleBackColor = true;
            this.btnHookedEval.Click += new System.EventHandler(this.btnHookedEval_Click);
            // 
            // grpHookedResults
            // 
            resources.ApplyResources(this.grpHookedResults, "grpHookedResults");
            this.grpHookedResults.Controls.Add(this.txtHookedResults);
            this.grpHookedResults.Name = "grpHookedResults";
            this.grpHookedResults.TabStop = false;
            this.tipError.SetToolTip(this.grpHookedResults, resources.GetString("grpHookedResults.ToolTip"));
            // 
            // txtHookedResults
            // 
            resources.ApplyResources(this.txtHookedResults, "txtHookedResults");
            this.txtHookedResults.Name = "txtHookedResults";
            this.txtHookedResults.ReadOnly = true;
            this.tipError.SetToolTip(this.txtHookedResults, resources.GetString("txtHookedResults.ToolTip"));
            // 
            // pnlHookVerdict
            // 
            resources.ApplyResources(this.pnlHookVerdict, "pnlHookVerdict");
            this.pnlHookVerdict.Controls.Add(this.btnVerdictApprove);
            this.pnlHookVerdict.Controls.Add(this.btnVerdictReject);
            this.pnlHookVerdict.Controls.Add(this.btnVerdictTerminate);
            this.pnlHookVerdict.Name = "pnlHookVerdict";
            this.tipError.SetToolTip(this.pnlHookVerdict, resources.GetString("pnlHookVerdict.ToolTip"));
            // 
            // btnVerdictApprove
            // 
            resources.ApplyResources(this.btnVerdictApprove, "btnVerdictApprove");
            this.btnVerdictApprove.Name = "btnVerdictApprove";
            this.tipError.SetToolTip(this.btnVerdictApprove, resources.GetString("btnVerdictApprove.ToolTip"));
            this.btnVerdictApprove.UseVisualStyleBackColor = true;
            this.btnVerdictApprove.Click += new System.EventHandler(this.btnVerdictApprove_Click);
            // 
            // btnVerdictReject
            // 
            resources.ApplyResources(this.btnVerdictReject, "btnVerdictReject");
            this.btnVerdictReject.Name = "btnVerdictReject";
            this.tipError.SetToolTip(this.btnVerdictReject, resources.GetString("btnVerdictReject.ToolTip"));
            this.btnVerdictReject.UseVisualStyleBackColor = true;
            this.btnVerdictReject.Click += new System.EventHandler(this.btnVerdictReject_Click);
            // 
            // btnVerdictTerminate
            // 
            resources.ApplyResources(this.btnVerdictTerminate, "btnVerdictTerminate");
            this.btnVerdictTerminate.Name = "btnVerdictTerminate";
            this.tipError.SetToolTip(this.btnVerdictTerminate, resources.GetString("btnVerdictTerminate.ToolTip"));
            this.btnVerdictTerminate.UseVisualStyleBackColor = true;
            this.btnVerdictTerminate.Click += new System.EventHandler(this.btnVerdictTerminate_Click);
            // 
            // lblHooked1
            // 
            resources.ApplyResources(this.lblHooked1, "lblHooked1");
            this.lblHooked1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblHooked1.Name = "lblHooked1";
            this.tipError.SetToolTip(this.lblHooked1, resources.GetString("lblHooked1.ToolTip"));
            // 
            // lblHookedCurrent
            // 
            resources.ApplyResources(this.lblHookedCurrent, "lblHookedCurrent");
            this.lblHookedCurrent.Name = "lblHookedCurrent";
            this.tipError.SetToolTip(this.lblHookedCurrent, resources.GetString("lblHookedCurrent.ToolTip"));
            // 
            // lblHooked0
            // 
            resources.ApplyResources(this.lblHooked0, "lblHooked0");
            this.lblHooked0.Name = "lblHooked0";
            this.tipError.SetToolTip(this.lblHooked0, resources.GetString("lblHooked0.ToolTip"));
            // 
            // grpCLI
            // 
            resources.ApplyResources(this.grpCLI, "grpCLI");
            this.grpCLI.Controls.Add(this.txtCLICommand);
            this.grpCLI.Controls.Add(this.btnCLIExecute);
            this.grpCLI.Name = "grpCLI";
            this.grpCLI.TabStop = false;
            this.tipError.SetToolTip(this.grpCLI, resources.GetString("grpCLI.ToolTip"));
            // 
            // txtCLICommand
            // 
            resources.ApplyResources(this.txtCLICommand, "txtCLICommand");
            this.txtCLICommand.Name = "txtCLICommand";
            this.tipError.SetToolTip(this.txtCLICommand, resources.GetString("txtCLICommand.ToolTip"));
            this.txtCLICommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCLICommand_KeyPress);
            // 
            // btnCLIExecute
            // 
            resources.ApplyResources(this.btnCLIExecute, "btnCLIExecute");
            this.btnCLIExecute.Name = "btnCLIExecute";
            this.tipError.SetToolTip(this.btnCLIExecute, resources.GetString("btnCLIExecute.ToolTip"));
            this.btnCLIExecute.UseVisualStyleBackColor = true;
            this.btnCLIExecute.Click += new System.EventHandler(this.btnCLIExecute_Click);
            // 
            // tipError
            // 
            this.tipError.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.tipError.ToolTipTitle = "Invalid parameter";
            // 
            // dlgDumpSave
            // 
            resources.ApplyResources(this.dlgDumpSave, "dlgDumpSave");
            // 
            // dlgFileLoadOpen
            // 
            resources.ApplyResources(this.dlgFileLoadOpen, "dlgFileLoadOpen");
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
            this.tipError.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FMain_FormClosed);
            this.Shown += new System.EventHandler(this.FMain_Shown);
            this.mnuStrip.ResumeLayout(false);
            this.mnuStrip.PerformLayout();
            this.tabPanels.ResumeLayout(false);
            this.tabPageListener.ResumeLayout(false);
            this.pnlListenerStart.ResumeLayout(false);
            this.pnlListenerStart.PerformLayout();
            this.grpListenerTarget.ResumeLayout(false);
            this.tabPageDumps.ResumeLayout(false);
            this.tabPageDumps.PerformLayout();
            this.tabPageMonitor.ResumeLayout(false);
            this.grpMonitorShell.ResumeLayout(false);
            this.grpMonitorShell.PerformLayout();
            this.grpMonitorKill.ResumeLayout(false);
            this.grpMonitorKill.PerformLayout();
            this.grpMonitorDoll.ResumeLayout(false);
            this.grpMonitorDoll.PerformLayout();
            this.tabPageDoll.ResumeLayout(false);
            this.grpDollBreak.ResumeLayout(false);
            this.grpDollLoaddll.ResumeLayout(false);
            this.grpDollLoaddll.PerformLayout();
            this.grpDollHooks.ResumeLayout(false);
            this.tabPageHooked.ResumeLayout(false);
            this.grpHookedEval.ResumeLayout(false);
            this.grpHookedEval.PerformLayout();
            this.grpHookedResults.ResumeLayout(false);
            this.grpHookedResults.PerformLayout();
            this.pnlHookVerdict.ResumeLayout(false);
            this.grpCLI.ResumeLayout(false);
            this.grpCLI.PerformLayout();
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
        private System.Windows.Forms.TabPage tabPageListener;
        private System.Windows.Forms.TabPage tabPageDumps;
        private System.Windows.Forms.GroupBox grpCLI;
        private System.Windows.Forms.TextBox txtCLICommand;
        private System.Windows.Forms.Button btnCLIExecute;
        private System.Windows.Forms.Button btnMonitorCurrent;
        private System.Windows.Forms.ColumnHeader clmDumpsID;
        private System.Windows.Forms.ColumnHeader clmDumpsSize;
        private System.Windows.Forms.ColumnHeader clmDumpsSource;
        private System.Windows.Forms.ComboBox cboDumpFormats;
        private System.Windows.Forms.Label lblDumps1;
        private System.Windows.Forms.GroupBox grpMonitorDoll;
        private System.Windows.Forms.RadioButton optDollAttach;
        private System.Windows.Forms.TextBox txtDollCmdline;
        private System.Windows.Forms.RadioButton optDollLaunch;
        private System.Windows.Forms.Button btnDollInvoke;
        private System.Windows.Forms.Button btnDollBrowse;
        private System.Windows.Forms.TextBox txtDollPID;
        private System.Windows.Forms.Label lblMonitor1;
        private System.Windows.Forms.Label lblDoll1;
        private System.Windows.Forms.Button btnDollCurrent;
        private System.Windows.Forms.Label lblListener1;
        private System.Windows.Forms.Label lblListener2;
        private System.Windows.Forms.GroupBox grpListenerTarget;
        private System.Windows.Forms.ColumnHeader clmTargetsID;
        private System.Windows.Forms.ColumnHeader clmTargetsName;
        private System.Windows.Forms.ColumnHeader clmTargetsType;
        private System.Windows.Forms.ColumnHeader clmTargetsStatus;
        private System.Windows.Forms.ColumnHeader clmTargetsPID;
        private System.Windows.Forms.ColumnHeader clmTargetsBits;
        private System.Windows.Forms.Button btnListenerStart;
        private System.Windows.Forms.CheckBox chkListenerStartIPv6;
        private System.Windows.Forms.TextBox txtListenerStartPort;
        private System.Windows.Forms.Label lblListener3;
        private System.Windows.Forms.GroupBox grpDollHooks;
        private System.Windows.Forms.Button btnHooksAdd;
        private System.Windows.Forms.Button btnHooksRemove;
        private System.Windows.Forms.ListView lstDollHooks;
        private System.Windows.Forms.ColumnHeader clmHooksID;
        private System.Windows.Forms.ColumnHeader clmHooksOep;
        private System.Windows.Forms.ColumnHeader clmHooksName;
        private System.Windows.Forms.Label lblHooked1;
        private System.Windows.Forms.GroupBox grpMonitorKill;
        private System.Windows.Forms.Button btnKillInvoke;
        private System.Windows.Forms.Button btnKillBrowse;
        private System.Windows.Forms.TextBox txtKillPID;
        private System.Windows.Forms.RadioButton optKillPID;
        private System.Windows.Forms.TextBox txtKillName;
        private System.Windows.Forms.RadioButton optKillName;
        private System.Windows.Forms.GroupBox grpMonitorShell;
        private System.Windows.Forms.Label lblMonitor2;
        private System.Windows.Forms.Button btnMonitorShell;
        private System.Windows.Forms.TextBox txtMonitorShell;
        private System.Windows.Forms.GroupBox grpDollLoaddll;
        private System.Windows.Forms.Label lblDoll2;
        private System.Windows.Forms.Button btnDollLoaddll;
        private System.Windows.Forms.TextBox txtDollLoaddll;
        private System.Windows.Forms.GroupBox grpDollBreak;
        private System.Windows.Forms.Button btnDollBreak;
        private System.Windows.Forms.Panel pnlHookVerdict;
        private System.Windows.Forms.Button btnVerdictApprove;
        private System.Windows.Forms.Button btnVerdictReject;
        private System.Windows.Forms.Button btnVerdictTerminate;
        private System.Windows.Forms.Label lblMonitor0;
        private System.Windows.Forms.Label lblDoll0;
        private System.Windows.Forms.Label lblHooked0;
        private System.Windows.Forms.GroupBox grpHookedResults;
        private System.Windows.Forms.GroupBox grpHookedEval;
        private System.Windows.Forms.Button btnHookedEval;
        private System.Windows.Forms.TextBox txtHookedEval;
        private System.Windows.Forms.ToolTip tipError;
        internal System.Windows.Forms.Panel pnlListenerStart;
        internal System.Windows.Forms.ListView lstListenerTargets;
        private System.Windows.Forms.ColumnHeader clmTargetsSelected;
        internal System.Windows.Forms.TabPage tabPageMonitor;
        internal System.Windows.Forms.TabPage tabPageDoll;
        internal System.Windows.Forms.TabPage tabPageHooked;
        internal System.Windows.Forms.Label lblMonitorCurrent;
        internal System.Windows.Forms.Label lblDollCurrent;
        private System.Windows.Forms.TabControl tabPanels;
        internal System.Windows.Forms.Label lblHookedCurrent;
        internal System.Windows.Forms.ListView lstDumps;
        internal System.Windows.Forms.TextBox txtDumpContent;
        internal System.Windows.Forms.Button btnDumpSave;
        internal System.Windows.Forms.Button btnDumpShow;
        private System.Windows.Forms.SaveFileDialog dlgDumpSave;
        private System.Windows.Forms.OpenFileDialog dlgFileLoadOpen;
        private System.Windows.Forms.CheckBox chkMonitorShellKeep;
        internal System.Windows.Forms.TextBox txtHookedResults;
        private System.Windows.Forms.ToolStripMenuItem mnuFileMonitorX86;
        private System.Windows.Forms.ToolStripMenuItem mnuFileMonitorX64;
        private System.Windows.Forms.ToolStripSeparator mnuFileSpr2;
    }
}

