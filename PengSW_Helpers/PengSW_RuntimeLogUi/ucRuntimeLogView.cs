using System.Windows.Forms;

namespace PengSW.RuntimeLog
{
    public partial class ucRuntimeLogView : UserControl
    {
        public ucRuntimeLogView()
        {
            InitializeComponent();
            _Watcher = new RuntimeLogToTextBox(txtLog);
        }

        private RuntimeLogToTextBox _Watcher;

        public int MaxLength 
        { 
            get { return _Watcher.MaxLength; }
            set { _Watcher.MaxLength = value; }
        }

        public void Bind(RuntimeLog aRuntimeLog)
        {
            _Watcher.Bind(aRuntimeLog);
        }

        public RuntimeLog RuntimeLog { get { return _Watcher.RLBinded; } }

        public void Clear()
        {
            txtLog.Clear();
        }
    }

    public class RuntimeLogToTextBox : Watcher
    {
        public RuntimeLogToTextBox(TextBox aTextBox)
        {
            txtLog = aTextBox;
            MaxLength = 4096;
        }

        public int MaxLength { get; set; }

        private TextBox txtLog;

        protected override void RuntimeLog_ClarifyLog(string aText)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new System.Action<string>(RuntimeLog_ClarifyLog), aText);
                return;
            }
            if (aText.Length > 256) aText = aText.Substring(0, 256) + "..." + System.Environment.NewLine;
            txtLog.AppendText(aText);
            if (txtLog.Text.Length > MaxLength)
            {
                txtLog.Text = txtLog.Text.Substring(txtLog.Text.Length - MaxLength);
            }
        }
    }
}
