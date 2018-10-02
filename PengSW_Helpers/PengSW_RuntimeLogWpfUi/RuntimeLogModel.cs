using PengSW.NotifyPropertyChanged;
using System.Windows.Threading;
using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows;

namespace PengSW.RuntimeLog
{
    public class RLModel : NotifyPropertyChangedObject, IDisposable
    {
        public RLModel(RuntimeLog aRL, Dispatcher aDispatcher)
        {
            rl = aRL ?? RL.GlobalRL;
            RegisteEvent();
            _Clearer = new Clearer();
            _Clearer.Bind(rl);
        }

        public event Action<string> Clarify;

        public bool Enabled { get { return _Enabled; } set { SetValue(ref _Enabled, value, nameof(Enabled)); } }
        private bool _Enabled = true;

        protected void RegisteEvent()
        {
            rl.ClarifyLog += new Action<string>(RL_ClarifyLog);
        }

        private StringBuilder _StringBuilder = new StringBuilder();
        private void RL_ClarifyLog(string aText)
        {
            if (!Enabled) return;
            if (_ShowRegex != null)
            {
                try
                {
                    if (!_ShowRegex.IsMatch(aText)) return;
                }
                catch
                {
                }
            }
            if (_UnshowRegex != null)
            {
                try
                {
                    if (_UnshowRegex.IsMatch(aText)) return;
                }
                catch
                {
                }
            }

            // _StringBuilder.Append(aText.Length > 256 ? aText = aText.Substring(0, 256) + "...\n" : aText);
            _StringBuilder.Append(aText);
            if (_StringBuilder.Length > MaxLength)
            {
                _StringBuilder.Remove(0, _StringBuilder.Length - MaxLength);
            }
            Clarify?.Invoke(_StringBuilder.ToString());
        }

        public void Clear()
        {
            _StringBuilder.Length = 0;
        }

        protected void UnregisteEvent()
        {
            rl.ClarifyLog -= RL_ClarifyLog;
        }

        public int ReserveDays
        {
            get { return _Clearer.ReserveDays; }
            set 
            {
                if (_Clearer.ReserveDays == value) return;
                _Clearer.ReserveDays = value;
                OnPropertyChanged(nameof(ReserveDays));
            }
        }

        public int MaxLength { get { return m_MaxLength; } set { SetValue(ref m_MaxLength, value, nameof(MaxLength)); } }
        private int m_MaxLength = 4096;

        public int SaveLevel
        {
            get
            {
                return RL.MinLevelToSave;
            }
            set
            {
                if (RL.MinLevelToSave == value) return;
                RL.MinLevelToSave = value;
                OnPropertyChanged(nameof(SaveLevel));
            }
        }

        public int ClarifyLevel
        {
            get
            {
                return RL.MinLevelToClarify;
            }
            set
            {
                if (RL.MinLevelToClarify == value) return;
                RL.MinLevelToClarify = value;
                OnPropertyChanged(nameof(ClarifyLevel));
            }
        }
        
        public string ShowPattern
        {
            get { return _ShowPattern; }
            set
            {
                if (_ShowPattern == value) return;
                _ShowPattern = value;
                try
                {
                    _ShowRegex = string.IsNullOrWhiteSpace(_ShowPattern) ? null : new Regex(_ShowPattern);
                }
                catch
                {
                    _ShowRegex = null;
                }
            }
        }
        private string _ShowPattern = null;
        private Regex _ShowRegex = null;

        public string UnshowPattern
        {
            get { return _UnshowPattern; }
            set
            {
                if (_UnshowPattern == value) return;
                _UnshowPattern = value;
                try
                {
                    _UnshowRegex = string.IsNullOrWhiteSpace(_UnshowPattern) ? null : new Regex(_UnshowPattern);
                }
                catch
                {
                    _UnshowRegex = null;
                }
            }
        }
        private string _UnshowPattern = null;
        private Regex _UnshowRegex = null;

        public void OpenLog()
        {
            try
            {
                rl.OpenLog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Dispose()
        {
            UnregisteEvent();
            _Clearer?.Dispose();
        }

        public RuntimeLog rl { get; }

        private Clearer _Clearer;
    }
}
