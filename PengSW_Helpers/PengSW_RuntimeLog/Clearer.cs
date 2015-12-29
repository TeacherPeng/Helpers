using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PengSW.RuntimeLog
{
    public class Clearer : Watcher
    {
        private System.DateTime m_LastLogTime = System.DateTime.MinValue;
        private int m_ReserveDays = 30;
        public int ReserveDays
        {
            get { return m_ReserveDays; }
            set { m_ReserveDays = value < 1 ? 1 : value; }
        }

        protected override void RuntimeLog_ClarifyLog(string aText)
        {
            System.DateTime aLogTime = System.DateTime.Now.Date;
            if (aLogTime != m_LastLogTime.Date)
            {
                System.DateTime aRetainTime = aLogTime.AddDays(-ReserveDays);
                if (this.RLBinded == null)
                {
                    RL.RemoveLogFileBefore(aRetainTime);
                }
                else
                {
                    this.RLBinded.RemoveBefore(aRetainTime);
                }
            }
            m_LastLogTime = aLogTime;
        }
    }
}
