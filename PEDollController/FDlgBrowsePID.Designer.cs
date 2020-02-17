namespace PEDollController
{
    partial class FDlgBrowsePID
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FDlgBrowsePID));
            this.lstPs = new System.Windows.Forms.ListView();
            this.clmPID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstPs
            // 
            resources.ApplyResources(this.lstPs, "lstPs");
            this.lstPs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmPID,
            this.clmName});
            this.lstPs.FullRowSelect = true;
            this.lstPs.GridLines = true;
            this.lstPs.HideSelection = false;
            this.lstPs.MultiSelect = false;
            this.lstPs.Name = "lstPs";
            this.lstPs.UseCompatibleStateImageBehavior = false;
            this.lstPs.View = System.Windows.Forms.View.Details;
            // 
            // clmPID
            // 
            resources.ApplyResources(this.clmPID, "clmPID");
            // 
            // clmName
            // 
            resources.ApplyResources(this.clmName, "clmName");
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FDlgBrowsePID
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lstPs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FDlgBrowsePID";
            this.ShowInTaskbar = false;
            this.VisibleChanged += new System.EventHandler(this.FDlgBrowsePID_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ListView lstPs;
        private System.Windows.Forms.ColumnHeader clmPID;
        private System.Windows.Forms.ColumnHeader clmName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}