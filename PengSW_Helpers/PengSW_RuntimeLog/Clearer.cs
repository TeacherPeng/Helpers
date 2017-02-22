using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PengSW.RuntimeLog
{
    public class Clearer : Watcher
    {
        private DateTime _LastLogTime = DateTime.MinValue;
        public int ReserveDays
        {
            get { return _ReserveDays; }
            set { _ReserveDays = value < 1 ? 1 : value; }
        }
        private int _ReserveDays = 30;

        protected override void RuntimeLog_ClarifyLog(string aText)
        {
            DateTime aLogTime = DateTime.Now.Date;
            if (aLogTime != _LastLogTime.Date)
            {
                DateTime aRetainTime = aLogTime.AddDays(-ReserveDays);
                DirectoryInfo aDirectoryInfo = new DirectoryInfo(Path.GetDirectoryName(_RL.LogFileNamePrefix));
                foreach (FileInfo aFileInfo in aDirectoryInfo.GetFiles(Path.GetFileName(_RL.LogFileNamePrefix) + ".*.log"))
                {
                    Match aMatch = Regex.Match(aFileInfo.Name, @"\.(\d\d\d\d)(\d\d)(\d\d)\.log$", RegexOptions.IgnoreCase);
                    if (aMatch != null && aMatch.Success)
                    {
                        DateTime aFileTime;
                        if (DateTime.TryParse($"{aMatch.Groups[1].Value}-{aMatch.Groups[2].Value}-{aMatch.Groups[3].Value}", out aFileTime))
                        {
                            if (aFileTime < aRetainTime) aFileInfo.Delete();
                        }
                    }
                }
            }
            _LastLogTime = aLogTime;
        }
    }
}
