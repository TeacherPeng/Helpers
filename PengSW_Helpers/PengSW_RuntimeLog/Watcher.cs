using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PengSW.RuntimeLog
{
    public abstract class Watcher : IDisposable
    {
        public Watcher()
        {
            // 默认绑定到全局共享RL.
            Bind(null);
        }

        protected void RegisteEvent()
        {
            if (_RL != null) _RL.ClarifyLog += RuntimeLog_ClarifyLog;
        }

        protected void UnregisteEvent()
        {
            if (_RL != null) _RL.ClarifyLog -= RuntimeLog_ClarifyLog;
        }

        public void Bind(RuntimeLog aRL)
        {
            RLBinded = aRL;
        }

        public RuntimeLog RLBinded
        {
            get { return _RL; }
            set
            {
                UnregisteEvent();
                _RL = value ?? RL.GlobalRL;
                RegisteEvent();
            }
        }
        protected RuntimeLog _RL = null;

        protected abstract void RuntimeLog_ClarifyLog(string aText);

        public void Dispose()
        {
            if (_RL != null) UnregisteEvent();
            _RL = null;
        }
    }
}
