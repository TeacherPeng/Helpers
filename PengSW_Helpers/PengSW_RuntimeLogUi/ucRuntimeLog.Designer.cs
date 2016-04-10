namespace PengSW.RuntimeLog
{
    partial class ucRuntimeLog
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucRuntimeLog));
            this.tbrToolbar = new System.Windows.Forms.ToolStrip();
            this.cmdClear = new System.Windows.Forms.ToolStripButton();
            this.cmdOpen = new System.Windows.Forms.ToolStripButton();
            this.cmdReserveDays = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuReserve03 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReserve07 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReserve10 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReserve30 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReserveDays = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtLevel = new System.Windows.Forms.ToolStripTextBox();
            this.uciRuntimeLogView = new PengSW.RuntimeLog.ucRuntimeLogView();
            this.tbrToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbrToolbar
            // 
            this.tbrToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdClear,
            this.cmdOpen,
            this.cmdReserveDays,
            this.toolStripLabel1,
            this.txtLevel});
            this.tbrToolbar.Location = new System.Drawing.Point(0, 0);
            this.tbrToolbar.Name = "tbrToolbar";
            this.tbrToolbar.Size = new System.Drawing.Size(442, 25);
            this.tbrToolbar.TabIndex = 0;
            // 
            // cmdClear
            // 
            this.cmdClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdClear.Image = ((System.Drawing.Image)(resources.GetObject("cmdClear.Image")));
            this.cmdClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(60, 22);
            this.cmdClear.Text = "清空显示";
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // cmdOpen
            // 
            this.cmdOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdOpen.Image = ((System.Drawing.Image)(resources.GetObject("cmdOpen.Image")));
            this.cmdOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdOpen.Name = "cmdOpen";
            this.cmdOpen.Size = new System.Drawing.Size(60, 22);
            this.cmdOpen.Text = "查看日志";
            this.cmdOpen.Click += new System.EventHandler(this.cmdOpen_Click);
            // 
            // cmdReserveDays
            // 
            this.cmdReserveDays.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdReserveDays.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuReserve03,
            this.mnuReserve07,
            this.mnuReserve10,
            this.mnuReserve30,
            this.mnuReserveDays});
            this.cmdReserveDays.Image = ((System.Drawing.Image)(resources.GetObject("cmdReserveDays.Image")));
            this.cmdReserveDays.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdReserveDays.Name = "cmdReserveDays";
            this.cmdReserveDays.Size = new System.Drawing.Size(69, 22);
            this.cmdReserveDays.Text = "保存天数";
            this.cmdReserveDays.Click += new System.EventHandler(this.cmdReserveDays_Click);
            // 
            // mnuReserve03
            // 
            this.mnuReserve03.Name = "mnuReserve03";
            this.mnuReserve03.Size = new System.Drawing.Size(112, 22);
            this.mnuReserve03.Text = "3天";
            this.mnuReserve03.Click += new System.EventHandler(this.mnuReserve03_Click);
            // 
            // mnuReserve07
            // 
            this.mnuReserve07.Name = "mnuReserve07";
            this.mnuReserve07.Size = new System.Drawing.Size(112, 22);
            this.mnuReserve07.Text = "7天";
            this.mnuReserve07.Click += new System.EventHandler(this.mnuReserve07_Click);
            // 
            // mnuReserve10
            // 
            this.mnuReserve10.Name = "mnuReserve10";
            this.mnuReserve10.Size = new System.Drawing.Size(112, 22);
            this.mnuReserve10.Text = "10天";
            this.mnuReserve10.Click += new System.EventHandler(this.mnuReserve10_Click);
            // 
            // mnuReserve30
            // 
            this.mnuReserve30.Name = "mnuReserve30";
            this.mnuReserve30.Size = new System.Drawing.Size(112, 22);
            this.mnuReserve30.Text = "30天";
            this.mnuReserve30.Click += new System.EventHandler(this.mnuReserve30_Click);
            // 
            // mnuReserveDays
            // 
            this.mnuReserveDays.Name = "mnuReserveDays";
            this.mnuReserveDays.Size = new System.Drawing.Size(112, 22);
            this.mnuReserveDays.Text = "自定义";
            this.mnuReserveDays.Click += new System.EventHandler(this.mnuReserveDays_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(68, 22);
            this.toolStripLabel1.Text = "显示级别：";
            // 
            // txtLevel
            // 
            this.txtLevel.Name = "txtLevel";
            this.txtLevel.Size = new System.Drawing.Size(100, 25);
            this.txtLevel.TextChanged += new System.EventHandler(this.txtLevel_TextChanged);
            // 
            // uciRuntimeLogView
            // 
            this.uciRuntimeLogView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uciRuntimeLogView.Location = new System.Drawing.Point(0, 25);
            this.uciRuntimeLogView.MaxLength = 0;
            this.uciRuntimeLogView.Name = "uciRuntimeLogView";
            this.uciRuntimeLogView.Size = new System.Drawing.Size(442, 321);
            this.uciRuntimeLogView.TabIndex = 1;
            // 
            // ucRuntimeLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uciRuntimeLogView);
            this.Controls.Add(this.tbrToolbar);
            this.Name = "ucRuntimeLog";
            this.Size = new System.Drawing.Size(442, 346);
            this.tbrToolbar.ResumeLayout(false);
            this.tbrToolbar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tbrToolbar;
        private ucRuntimeLogView uciRuntimeLogView;
        private System.Windows.Forms.ToolStripButton cmdClear;
        private System.Windows.Forms.ToolStripButton cmdOpen;
        private System.Windows.Forms.ToolStripDropDownButton cmdReserveDays;
        private System.Windows.Forms.ToolStripMenuItem mnuReserve03;
        private System.Windows.Forms.ToolStripMenuItem mnuReserve07;
        private System.Windows.Forms.ToolStripMenuItem mnuReserve10;
        private System.Windows.Forms.ToolStripMenuItem mnuReserve30;
        private System.Windows.Forms.ToolStripMenuItem mnuReserveDays;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtLevel;
    }
}
