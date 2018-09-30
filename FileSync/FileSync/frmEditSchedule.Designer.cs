namespace FileSync
{
    partial class frmEditSchedule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditSchedule));
            this.label1 = new System.Windows.Forms.Label();
            this.cboSource = new System.Windows.Forms.ComboBox();
            this.cmdSourceBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cboDest = new System.Windows.Forms.ComboBox();
            this.cmdDestBrowse = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "源文件夹：";
            // 
            // cboSource
            // 
            this.cboSource.FormattingEnabled = true;
            this.cboSource.Location = new System.Drawing.Point(99, 44);
            this.cboSource.Name = "cboSource";
            this.cboSource.Size = new System.Drawing.Size(362, 20);
            this.cboSource.TabIndex = 1;
            this.cboSource.TextChanged += new System.EventHandler(this.cboSource_TextChanged);
            // 
            // cmdSourceBrowse
            // 
            this.cmdSourceBrowse.Location = new System.Drawing.Point(463, 42);
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
            this.label2.Location = new System.Drawing.Point(16, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "目标文件夹：";
            // 
            // cboDest
            // 
            this.cboDest.FormattingEnabled = true;
            this.cboDest.Location = new System.Drawing.Point(99, 84);
            this.cboDest.Name = "cboDest";
            this.cboDest.Size = new System.Drawing.Size(362, 20);
            this.cboDest.TabIndex = 4;
            this.cboDest.TextChanged += new System.EventHandler(this.cboDest_TextChanged);
            // 
            // cmdDestBrowse
            // 
            this.cmdDestBrowse.Location = new System.Drawing.Point(463, 82);
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
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(517, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(107, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(107, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(107, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            // 
            // frmEditSchedule
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(517, 392);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.cmdDestBrowse);
            this.Controls.Add(this.cmdSourceBrowse);
            this.Controls.Add(this.cboDest);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboSource);
            this.Controls.Add(this.label1);
            this.Name = "frmEditSchedule";
            this.Text = "编辑同步计划";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
    }
}