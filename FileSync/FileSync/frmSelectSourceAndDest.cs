using System;
using System.Windows.Forms;

namespace FileSync
{
    public partial class frmSelectSourceAndDest : Form
    {
        public frmSelectSourceAndDest()
        {
            InitializeComponent();
            SetOperationState();

            LoadHistory();
        }

        public string SourceFolder { get { return cboSource.Text; } }
        public string DestFolder { get { return cboDest.Text; } }

        private void SetOperationState()
        {
            cmdOk.Enabled = cboSource.Text.Trim().Length > 0 && cboDest.Text.Trim().Length > 0;
        }

        private void SelectFolderTo(System.Windows.Forms.ComboBox aComboBox)
        {
            System.Windows.Forms.FolderBrowserDialog aDlg = new FolderBrowserDialog();
            if (aDlg.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
            aComboBox.Text = aDlg.SelectedPath;
        }

        private void cmdSourceBrowse_Click(object sender, EventArgs e)
        {
            SelectFolderTo(cboSource);
        }

        private void cmdDestBrowse_Click(object sender, EventArgs e)
        {
            SelectFolderTo(cboDest);
        }

        private void cboSource_TextChanged(object sender, EventArgs e)
        {
            SetOperationState();
        }

        private void cboDest_TextChanged(object sender, EventArgs e)
        {
            SetOperationState();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            SaveHistory();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void LoadHistory()
        {
            string[] aSources = Properties.Settings.Default.Source.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] aDests = Properties.Settings.Default.Dest.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            cboSource.Items.AddRange(aSources);
            cboDest.Items.AddRange(aDests);
        }

        private void SaveHistory()
        {
            if (!cboSource.Items.Contains(cboSource.Text)) cboSource.Items.Add(cboSource.Text);
            if (!cboDest.Items.Contains(cboDest.Text)) cboDest.Items.Add(cboDest.Text);

            System.Text.StringBuilder aStringBuilder = new System.Text.StringBuilder();
            foreach (object aItem in cboSource.Items)
            {
                aStringBuilder.AppendLine(aItem.ToString());
            }
            Properties.Settings.Default.Source = aStringBuilder.ToString();

            aStringBuilder.Length = 0;
            foreach (object aItem in cboDest.Items)
            {
                aStringBuilder.AppendLine(aItem.ToString());
            }
            Properties.Settings.Default.Dest = aStringBuilder.ToString();

            Properties.Settings.Default.Save();
        }
    }
}
