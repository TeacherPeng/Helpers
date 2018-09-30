namespace FileSync
{
    partial class frmSelectSourceAndDest
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
            this.label1 = new System.Windows.Forms.Label();
            this.cboSource = new System.Windows.Forms.ComboBox();
            this.cmdSourceBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cboDest = new System.Windows.Forms.ComboBox();
            this.cmdDestBrowse = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "源文件夹：";
            // 
            // cboSource
            // 
            this.cboSource.FormattingEnabled = true;
            this.cboSource.Location = new System.Drawing.Point(103, 25);
            this.cboSource.Name = "cboSource";
            this.cboSource.Size = new System.Drawing.Size(362, 20);
            this.cboSource.TabIndex = 1;
            this.cboSource.TextChanged += new System.EventHandler(this.cboSource_TextChanged);
            // 
            // cmdSourceBrowse
            // 
            this.cmdSourceBrowse.Location = new System.Drawing.Point(467, 23);
            this.cmdSourceBrowse.Name = "cmdSourceBrowse";
            this.cmdSourceBrowse.Size = new System.Drawing.Size(29, 23);
            this.cmdSourceBrowse.TabIndex = 2;
            this.cmdSourceBrowse.Text = "..";
            this.cmdSourceBrowse.UseVisualStyleBackColor = true;
            this.cmdSourceBrowse.Click += new System.EventHandler(this.cmdSourceBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "目标文件夹：";
            // 
            // cboDest
            // 
            this.cboDest.FormattingEnabled = true;
            this.cboDest.Location = new System.Drawing.Point(103, 65);
            this.cboDest.Name = "cboDest";
            this.cboDest.Size = new System.Drawing.Size(362, 20);
            this.cboDest.TabIndex = 4;
            this.cboDest.TextChanged += new System.EventHandler(this.cboDest_TextChanged);
            // 
            // cmdDestBrowse
            // 
            this.cmdDestBrowse.Location = new System.Drawing.Point(467, 63);
            this.cmdDestBrowse.Name = "cmdDestBrowse";
            this.cmdDestBrowse.Size = new System.Drawing.Size(29, 23);
            this.cmdDestBrowse.TabIndex = 5;
            this.cmdDestBrowse.Text = "..";
            this.cmdDestBrowse.UseVisualStyleBackColor = true;
            this.cmdDestBrowse.Click += new System.EventHandler(this.cmdDestBrowse_Click);
            // 
            // cmdOk
            // 
            this.cmdOk.Location = new System.Drawing.Point(159, 125);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(75, 23);
            this.cmdOk.TabIndex = 6;
            this.cmdOk.Text = "确定(&O)";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(281, 125);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "取消(&C)";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // frmSelectSourceAndDest
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(514, 172);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.cmdDestBrowse);
            this.Controls.Add(this.cmdSourceBrowse);
            this.Controls.Add(this.cboDest);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboSource);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectSourceAndDest";
            this.Text = "选择同步源和目标";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboSource;
        private System.Windows.Forms.Button cmdSourceBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboDest;
        private System.Windows.Forms.Button cmdDestBrowse;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;
    }
}