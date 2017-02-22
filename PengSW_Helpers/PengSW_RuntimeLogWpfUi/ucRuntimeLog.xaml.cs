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
            _Timer.Interval = TimeSpan.FromMilliseconds(500);
            _Timer.Tick += OnTimer_Tick;
        }

        private string _LogText;
        private void OnTimer_Tick(object sender, EventArgs e)
        {
            txtLog.Text = _LogText;
            txtLog.SelectionStart = _LogText.Length;
            txtLog.ScrollToEnd();
            _Timer.Stop();
        }

        private RLModel _Model;
        private DispatcherTimer _Timer = new DispatcherTimer();

        public void Bind(RuntimeLog aRL)
        {
            _Model?.Dispose();
            _Model = new RLModel(aRL, this.Dispatcher);
            _Model.Clarify += RL_ClarifyLog;
            this.DataContext = _Model;
        }

        protected void UpdateLog(string aText)
        {
            try
            {
                _LogText = aText;
                if (!_Timer.IsEnabled) _Timer.Start();
            }
            catch
            {
            }
        }

        private void RL_ClarifyLog(string aText)
        {
            UpdateLog(aText);
        }

        private void OnClear_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
        }

        private void OnOpenLogFile_Click(object sender, RoutedEventArgs e)
        {
            _Model.OpenLog();
        }
    }
}
