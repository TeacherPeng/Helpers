using System.Windows.Controls;
using PengSW.NotifyPropertyChanged;
using PengSW.RuntimeLog;
using System.Windows.Threading;
using System;
using System.Text.RegularExpressions;

namespace PengSW.RuntimeLog
{
    public class RLModel : NotifyPropertyChangedObject, IDisposable
    {
        public RLModel(RL aRL, Dispatcher aDispatcher)
        {
            _RL = aRL;
            _Dispatcher = aDispatcher;
            RegisteEvent();
            m_Clearer = new Clearer();
            m_Clearer.Bind(_RL);
        }

        public event System.Action<string> Clarify;
        private Dispatcher _Dispatcher;

        protected void RegisteEvent()
        {
            if (_RL == null)
            {
                RL.ClarifyShareLog += new System.Action<string>(RL_ClarifyLog);
            }
            else
            {
                _RL.ClarifyLog += new System.Action<string>(RL_ClarifyLog);
            }
        }

        private void RL_ClarifyLog(string aText)
        {
            if (Clarify == null) return;
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
            if (_Dispatcher == null)
                Clarify(aText);
            else
                _Dispatcher.BeginInvoke(new Action<string>(Clarify), aText);
        }

        protected void UnregisteEvent()
        {
            if (_RL == null)
            {
                RL.ClarifyShareLog -= RL_ClarifyLog;
            }
            else
            {
                _RL.ClarifyLog -= RL_ClarifyLog;
            }
        }

        public int ReserveDays
        {
            get { return m_Clearer.ReserveDays; }
            set 
            {
                if (m_Clearer.ReserveDays == value) return;
                m_Clearer.ReserveDays = value;
                OnPropertyChanged(nameof(ReserveDays));
            }
        }

        public int MaxLines { get { return m_MaxLines; } set { SetValue(ref m_MaxLines, value, nameof(MaxLines)); } }
        private int m_MaxLines = 4096;

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
            if (_RL == null) RL.OpenLogFile(); else _RL.Open();
        }

        public void Dispose()
        {
            UnregisteEvent();
        }

        private RL _RL = null;
        public RL RL { get { return _RL; } }

        private Clearer m_Clearer;
    }
}
