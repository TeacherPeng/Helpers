using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace PengSW.RuntimeLog
{
    /// <summary>
    /// 运行时日志
    /// </summary>
    public class RuntimeLog : IDisposable
    {
        /// <summary>
        /// 创建一个日志记录器
        /// </summary>
        /// <param name="aFolderName">指定日志文件夹名，日志文件夹会建在dll所在文件夹下。</param>
        /// <param name="aFilename">指定日志文件名，不含后缀，约定后缀为.yyyyMMdd.log</param>
        public RuntimeLog(string aFolderName = null, string aLogPrefix = null)
        {
            SetLog(aFolderName, aLogPrefix);
            _WriteThread = new Thread(new ThreadStart(this.WriteThread))
            {
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            _WriteThread.Start();
        }

        public void SetLog(string aFolderName = null, string aLogPrefix = null)
        {
            string aAssemblyFolder = Path.GetDirectoryName(GetType().Assembly.Location);
            string aLogFolderName = string.IsNullOrWhiteSpace(aFolderName) ? aAssemblyFolder : Path.IsPathRooted(aFolderName) ? aFolderName : Path.Combine(aAssemblyFolder, aFolderName);
            if (!Directory.Exists(aLogFolderName)) Directory.CreateDirectory(aLogFolderName);
            string aPrefix = string.IsNullOrWhiteSpace(aLogPrefix) ? Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) : aLogPrefix;
            LogFileNamePrefix = Path.Combine(aLogFolderName, aPrefix);
        }

        public void OpenLog() => Process.Start(LogFileName);

        public void L(string aLog, int aLevel = 4, bool aShowDialog = false, bool aClarify = true, string aTag = null)
        {
            _Write(aLog, aLog, aLog, aShowDialog, aClarify, aLevel, aTag);
        }
        public void L(string aLog, bool aShowDialog, bool aClarify = true, int aLevel = 4, string aTag = null) => L(aLog, aLevel, aShowDialog, aClarify, aTag);
        public void E(Exception ex, string aTitle = null, bool aShowDialog = false, bool aClarify = true, bool aStackTrace = true, string aTag = null)
        {
            StringBuilder aLogBuilder = new StringBuilder();
            StringBuilder aClarifyBuilder = new StringBuilder();
            aLogBuilder.AppendLine("Exception catched.");
            if (!string.IsNullOrEmpty(aTitle))
            {
                aLogBuilder.Append("Exception Title: ");
                aLogBuilder.AppendLine(aTitle);
                aClarifyBuilder.Append(aTitle);
                aClarifyBuilder.Append(":");
            }
            aLogBuilder.Append("Exception Message: ");
            aLogBuilder.AppendLine(ex.Message);
            aClarifyBuilder.Append(ex.Message);
            aLogBuilder.Append("Exception Type: ");
            aLogBuilder.AppendLine(ex.GetType().FullName);
            if (aStackTrace)
            {
                aLogBuilder.AppendLine("Stack Trace:");
                aLogBuilder.AppendLine(ex.StackTrace);
            }

            _Write(aLogBuilder.ToString(), aClarifyBuilder.ToString(), aClarifyBuilder.ToString(), aShowDialog, aClarify, 0, aTag);
        }
        public void Dispose()
        {
            _WriteThread?.Abort();
            _WriteThread = null;
        }

        public string LogFileNamePrefix { get; set; }
        public string LogFileName => LogFileNamePrefix + "." + DateTime.Now.ToString("yyyyMMdd") + ".log";
        public int MinLevelToSave { get; set; } = 9;
        public int MinLevelToClarify { get; set; } = 9;

        public event Action<string> ClarifyLog;

        #region 日志记录

        class LogTask
        {
            public LogTask(string aFileName, string aLog, string aClarify)
            {
                FileName = aFileName;
                Log = aLog;
                Clarify = aClarify;
            }
            public string FileName;
            public string Log;
            public string Clarify;
        }
        private ConcurrentQueue<LogTask> _LogQueue = new ConcurrentQueue<LogTask>();

        private void _Write(string aWriteLog, string aClarifyLog, string aDialogLog, bool aShowDialog, bool aClarify, int aLevel, string aTag)
        {
            string aFileName = LogFileName;
            StringBuilder aLogBuilder = new StringBuilder();

            string aLogWillWrite = null;
            if (!string.IsNullOrWhiteSpace(aFileName) && aLevel <= MinLevelToSave)
            {
                aLogBuilder.Append(string.Format("[{0:yyyy-MM-dd HH:mm:ss}]", DateTime.Now));
                if (string.IsNullOrWhiteSpace(aTag))
                {
                    StackTrace aStackTrace = new StackTrace();
                    Type aType = (from r in aStackTrace.GetFrames() let t = r.GetMethod().ReflectedType where t != typeof(RL) && t != typeof(RuntimeLog) select t).FirstOrDefault();
                    if (aType != null) aLogBuilder.Append($"[{aType.Name}]");
                }
                else
                {
                    aLogBuilder.Append($"[{aTag}]");
                }
                aLogBuilder.Append($"[{aLevel}] ");
                aLogBuilder.AppendLine(aWriteLog);
                aLogWillWrite = aLogBuilder.ToString();
            }

            string aLogWillClarify = null;
            if (aClarify && aLevel <= MinLevelToClarify)
            {
                aLogBuilder.Length = 0;
                aLogBuilder.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
                aLogBuilder.AppendLine(aClarifyLog);
                aLogWillClarify = aLogBuilder.ToString();
            }

            _LogQueue.Enqueue(new LogTask(aFileName, aLogWillWrite, aLogWillClarify));

            if (aShowDialog)
            {
                MessageBox.Show(aDialogLog, "Runtime Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void WriteThread()
        {
            try
            {
                Dictionary<string, StringBuilder> aRecords = new Dictionary<string, StringBuilder>();
                StringBuilder aClarify = new StringBuilder();
                while (true)
                {
                    LogTask aLogTask;
                    while (_LogQueue.TryDequeue(out aLogTask))
                    {
                        try
                        {
                            if (aLogTask.Log != null)
                            {
                                if (!aRecords.ContainsKey(aLogTask.FileName))
                                    aRecords.Add(aLogTask.FileName, new StringBuilder());
                                aRecords[aLogTask.FileName].Append(aLogTask.Log);
                            }
                            if (aLogTask.Clarify != null) aClarify.Append(aLogTask.Clarify);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message);
                        }
                    }
                    foreach (var aLog in aRecords)
                    {
                        if (aLog.Value.Length > 0)
                        {
                            try { File.AppendAllText(aLog.Key, aLog.Value.ToString(), Encoding.Unicode); } catch (Exception ex) { Trace.WriteLine(ex.Message); }
                            aLog.Value.Length = 0;
                        }
                    }
                    if (aClarify.Length > 0)
                    {
                        try { ClarifyLog?.Invoke(aClarify.ToString()); } catch (Exception ex) { Trace.WriteLine(ex.Message); }
                        aClarify.Length = 0;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private Thread _WriteThread;
        
        #endregion
    }

    public static class RL
    {
        public static RuntimeLog GlobalRL
        {
            get
            {
                if (_GlobalRL == null) _GlobalRL = new RuntimeLog();
                return _GlobalRL;
            }
        }
        private static RuntimeLog _GlobalRL;

        public static void SetLog(string aFolderName = null, string aLogPrefix = null) => GlobalRL.SetLog(aFolderName, aLogPrefix);
        public static void OpenLog() => GlobalRL.OpenLog();
        public static void L(string aLog, int aLevel = 4, bool aShowDialog = false, bool aClarify = true, string aTag = null) => GlobalRL.L(aLog, aLevel, aShowDialog, aClarify, aTag);
        public static void L(string aLog, bool aShowDialog, bool aClarify = true, int aLevel = 4, string aTag = null) => GlobalRL.L(aLog, aLevel, aShowDialog, aClarify, aTag);
        public static void E(Exception ex, string aTitle = null, bool aShowDialog = false, bool aClarify = true, bool aStackTrace = true, string aTag = null) => GlobalRL.E(ex, aTitle, aShowDialog, aClarify, aStackTrace, aTag);
        public static void E(Exception ex, bool aShowDialog, bool aClarify = true, string aTitle = null) => GlobalRL.E(ex, null, aShowDialog, aClarify, true, null);
        public static int MinLevelToSave { get { return GlobalRL.MinLevelToSave; } set { GlobalRL.MinLevelToSave = value; } }
        public static int MinLevelToClarify { get { return GlobalRL.MinLevelToClarify; } set { GlobalRL.MinLevelToClarify = value; } }
    }
}
