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
            if (m_RL == null)
            {
                RL.ClarifyShareLog += new System.Action<string>(RuntimeLog_ClarifyLog);
            }
            else
            {
                m_RL.ClarifyLog += new System.Action<string>(RuntimeLog_ClarifyLog);
            }
        }

        protected void UnregisteEvent()
        {
            if (m_RL == null)
            {
                RL.ClarifyShareLog -= RuntimeLog_ClarifyLog;
            }
            else
            {
                m_RL.ClarifyLog -= RuntimeLog_ClarifyLog;
            }
        }

        public void Bind(RL aRL)
        {
            UnregisteEvent();
            m_RL = aRL;
            RegisteEvent();
        }

        public RL RLBinded { get { return m_RL; } }

        protected abstract void RuntimeLog_ClarifyLog(string aText);

        protected RL m_RL = null;

        protected virtual void Dispose(bool aMode)
        {
            UnregisteEvent();
            m_RL = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
