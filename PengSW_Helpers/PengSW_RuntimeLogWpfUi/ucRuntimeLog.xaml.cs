using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PengSW.RuntimeLog
{
    /// <summary>
    /// ucRL.xaml 的交互逻辑
    /// </summary>
    public partial class ucRuntimeLog : UserControl
    {
        public ucRuntimeLog()
        {
            InitializeComponent();
            Bind(null);
        }

        private RLModel m_Model;

        public void Bind(RL aRL)
        {
            m_Model = new RLModel(aRL, this.Dispatcher);
            m_Model.Clarify += RL_ClarifyLog;
            this.DataContext = m_Model;
        }

        protected void UpdateLog(string aText)
        {
            try
            {
                if (aText.Length > 256) aText = aText.Substring(0, 256) + "...\n";
                txtLog.AppendText(aText);
                if (txtLog.Text.Length > m_Model.MaxLines)
                {
                    txtLog.Text = txtLog.Text.Substring(txtLog.Text.Length - m_Model.MaxLines);
                }
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToEnd();
            }
            catch
            {
            }
        }

        private void RL_ClarifyLog(string aText)
        {
            Dispatcher.BeginInvoke(new Action<string>(UpdateLog), aText);
        }

        private void OnClear_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
        }

        private void OnOpenLogFile_Click(object sender, RoutedEventArgs e)
        {
            m_Model.OpenLog();
        }
    }
}
