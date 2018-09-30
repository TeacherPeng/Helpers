namespace FileSync
{
    partial class frmFileSync
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFileSync));
            this.tbrToolbar = new System.Windows.Forms.ToolStrip();
            this.cmdOpenSchedule = new System.Windows.Forms.ToolStripButton();
            this.cmdEditSchedule = new System.Windows.Forms.ToolStripButton();
            this.cmdClearAllTasks = new System.Windows.Forms.ToolStripButton();
            this.cmdStartSync = new System.Windows.Forms.ToolStripButton();
            this.gridTasks = new System.Windows.Forms.DataGridView();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSourceTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSourceSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDest = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDestTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDestSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbrToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // tbrToolbar
            // 
            this.tbrToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdOpenSchedule,
            this.cmdEditSchedule,
            this.cmdClearAllTasks,
            this.cmdStartSync});
            this.tbrToolbar.Location = new System.Drawing.Point(0, 0);
            this.tbrToolbar.Name = "tbrToolbar";
            this.tbrToolbar.Size = new System.Drawing.Size(861, 25);
            this.tbrToolbar.TabIndex = 0;
            // 
            // cmdOpenSchedule
            // 
            this.cmdOpenSchedule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdOpenSchedule.Image = ((System.Drawing.Image)(resources.GetObject("cmdOpenSchedule.Image")));
            this.cmdOpenSchedule.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdOpenSchedule.Name = "cmdOpenSchedule";
            this.cmdOpenSchedule.Size = new System.Drawing.Size(84, 22);
            this.cmdOpenSchedule.Text = "打开同步计划";
            this.cmdOpenSchedule.Click += new System.EventHandler(this.cmdOpenSchedule_Click);
            // 
            // cmdEditSchedule
            // 
            this.cmdEditSchedule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdEditSchedule.Image = ((System.Drawing.Image)(resources.GetObject("cmdEditSchedule.Image")));
            this.cmdEditSchedule.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdEditSchedule.Name = "cmdEditSchedule";
            this.cmdEditSchedule.Size = new System.Drawing.Size(84, 22);
            this.cmdEditSchedule.Text = "编辑同步计划";
            this.cmdEditSchedule.Click += new System.EventHandler(this.cmdEditSchedule_Click);
            // 
            // cmdClearAllTasks
            // 
            this.cmdClearAllTasks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdClearAllTasks.Image = ((System.Drawing.Image)(resources.GetObject("cmdClearAllTasks.Image")));
            this.cmdClearAllTasks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdClearAllTasks.Name = "cmdClearAllTasks";
            this.cmdClearAllTasks.Size = new System.Drawing.Size(108, 22);
            this.cmdClearAllTasks.Text = "清除所有同步任务";
            this.cmdClearAllTasks.Click += new System.EventHandler(this.cmdClearAllTasks_Click);
            // 
            // cmdStartSync
            // 
            this.cmdStartSync.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdStartSync.Image = ((System.Drawing.Image)(resources.GetObject("cmdStartSync.Image")));
            this.cmdStartSync.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdStartSync.Name = "cmdStartSync";
            this.cmdStartSync.Size = new System.Drawing.Size(60, 22);
            this.cmdStartSync.Text = "开始同步";
            this.cmdStartSync.Click += new System.EventHandler(this.cmdStartSync_Click);
            // 
            // gridTasks
            // 
            this.gridTasks.AllowUserToAddRows = false;
            this.gridTasks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colType,
            this.colSource,
            this.colSourceTime,
            this.colSourceSize,
            this.colDest,
            this.colDestTime,
            this.colDestSize,
            this.colResult});
            this.gridTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTasks.Location = new System.Drawing.Point(0, 25);
            this.gridTasks.Name = "gridTasks";
            this.gridTasks.ReadOnly = true;
            this.gridTasks.RowTemplate.Height = 23;
            this.gridTasks.Size = new System.Drawing.Size(861, 563);
            this.gridTasks.TabIndex = 1;
            // 
            // colType
            // 
            this.colType.HeaderText = "类型";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colSource
            // 
            this.colSource.HeaderText = "源";
            this.colSource.Name = "colSource";
            this.colSource.ReadOnly = true;
            // 
            // colSourceTime
            // 
            this.colSourceTime.HeaderText = "源时间";
            this.colSourceTime.Name = "colSourceTime";
            this.colSourceTime.ReadOnly = true;
            // 
            // colSourceSize
            // 
            this.colSourceSize.HeaderText = "源大小";
            this.colSourceSize.Name = "colSourceSize";
            this.colSourceSize.ReadOnly = true;
            // 
            // colDest
            // 
            this.colDest.HeaderText = "目标";
            this.colDest.Name = "colDest";
            this.colDest.ReadOnly = true;
            // 
            // colDestTime
            // 
            this.colDestTime.HeaderText = "目标时间";
            this.colDestTime.Name = "colDestTime";
            this.colDestTime.ReadOnly = true;
            // 
            // colDestSize
            // 
            this.colDestSize.HeaderText = "目标大小";
            this.colDestSize.Name = "colDestSize";
            this.colDestSize.ReadOnly = true;
            // 
            // colResult
            // 
            this.colResult.HeaderText = "同步结果";
            this.colResult.Name = "colResult";
            this.colResult.ReadOnly = true;
            // 
            // frmFileSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 588);
            this.Controls.Add(this.gridTasks);
            this.Controls.Add(this.tbrToolbar);
            this.Name = "frmFileSync";
            this.Text = "文件同步工具";
            this.tbrToolbar.ResumeLayout(false);
            this.tbrToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tbrToolbar;
        private System.Windows.Forms.ToolStripButton cmdEditSchedule;
        private System.Windows.Forms.ToolStripButton cmdClearAllTasks;
        private System.Windows.Forms.ToolStripButton cmdStartSync;
        private System.Windows.Forms.DataGridView gridTasks;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSourceTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSourceSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDest;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDestTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDestSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResult;
        private System.Windows.Forms.ToolStripButton cmdOpenSchedule;
    }
}

