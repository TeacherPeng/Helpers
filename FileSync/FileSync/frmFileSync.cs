using System.Windows.Forms;

namespace FileSync
{
    public partial class frmFileSync : Form
    {
        public frmFileSync()
        {
            InitializeComponent();
        }

        private void cmdEditSchedule_Click(object sender, System.EventArgs e)
        {
            frmEditSchedule aSelecter = new frmEditSchedule();
            if (aSelecter.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                FileSyncTaskCreator aTaskCreator = new FileSyncTaskCreator(aSelecter.SourceFolder, aSelecter.DestFolder);
                System.Collections.Generic.List<ISyncTask> aTasks = aTaskCreator.GetTasks();
                foreach (ISyncTask aTask in aTasks)
                {
                    int aIndex = gridTasks.Rows.Add(aTask.TaskName, aTask.Source, aTask.SourceTime, aTask.SourceSize, aTask.Dest, aTask.DestTime, aTask.DestSize, "");
                    gridTasks.Rows[aIndex].Tag = aTask;
                }
                this.Cursor = Cursors.Default;
            }
            catch (System.Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(this, ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdClearAllTasks_Click(object sender, System.EventArgs e)
        {
            gridTasks.Rows.Clear();
        }

        private void cmdStartSync_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            foreach (System.Windows.Forms.DataGridViewRow aRow in gridTasks.Rows)
            {
                aRow.Cells["colResult"].Value = "开始同步";
                ISyncTask aTask = aRow.Tag as ISyncTask;
                try
                {
                    aTask.Execute();
                    aRow.Cells["colResult"].Value = "同步结束";
                }
                catch (System.Exception ex)
                {
                    aRow.Cells["colResult"].Value = ex.Message;
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void cmdOpenSchedule_Click(object sender, System.EventArgs e)
        {

        }
    }
}
