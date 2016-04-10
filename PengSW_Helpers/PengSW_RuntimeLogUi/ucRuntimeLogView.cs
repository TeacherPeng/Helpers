using System.Windows.Forms;

namespace PengSW.RuntimeLog
{
    public partial class ucRuntimeLogView : UserControl
    {
        public ucRuntimeLogView()
        {
            InitializeComponent();
            m_Watcher = new RuntimeLogToTextBox(txtLog);
        }

        private RuntimeLogToTextBox m_Watcher;

        public int MaxLength 
        { 
            get { return m_Watcher.MaxLength; }
            set { m_Watcher.MaxLength = value; }
        }

        public void Bind(RL aRuntimeLog)
        {
            m_Watcher.Bind(aRuntimeLog);
        }

        public RL RuntimeLog { get { return m_Watcher.RLBinded; } }

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
