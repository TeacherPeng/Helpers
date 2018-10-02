using System;
using System.IO;
using System.Threading;
using static PengSW.RuntimeLog.RL;
using static PengSW.TimerTaskHelper.TimerTaskManager;

namespace PengSW.FileSystemWatcherHelper
{
    /// <summary>
    /// 基于Sytem.IO.FileSystemWatcher的，针对指定文件的监测器。
    /// </summary>
    public class FileWatcherEx
    {
        public FileWatcherEx(string aFileName)
        {
            FileName = aFileName;
            FileWatcher = new FileSystemWatcher();
            FileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            FileWatcher.IncludeSubdirectories = false;
            FileWatcher.Path = Path.GetDirectoryName(aFileName);
            FileWatcher.Filter = Path.GetFileName(aFileName);
            FileWatcher.Changed += new FileSystemEventHandler(Watcher_FileChanged);
            FileWatcher.Created += new FileSystemEventHandler(Watcher_FileCreated);
            FileWatcher.Deleted += new FileSystemEventHandler(Watcher_FileDeleted);
            FileWatcher.Error += new ErrorEventHandler(Watcher_Error);
            FileWatcher.Renamed += new RenamedEventHandler(Watcher_FileRenamed);
        }

        #region 监视属性

        public string FileName { get; }

        public bool EnableRaisingEvents
        {
            get { return FileWatcher.EnableRaisingEvents; }
            set
            {
                FileWatcher.EnableRaisingEvents = value;
            }
        }

        #endregion

        #region 通知事件

        public event Action<string> FileCreated;
        public event Action<string> FileChanged;
        public event Action<string> FileCopied;
        public event Action<string> FileDeleted;
        public event RenamedEventHandler FileRenamed;
        public event ErrorEventHandler WatchError;

        #endregion

        #region 监视器响应

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            L($"Watcher Error: {e.GetException().Message}");
            WatchError?.Invoke(sender, e);
        }

        private void Watcher_FileRenamed(object sender, RenamedEventArgs e)
        {
            L($"File [{e.OldFullPath}] renamed [{e.FullPath}].");
            FileRenamed?.Invoke(sender, e);
        }

        private void Watcher_FileDeleted(object sender, FileSystemEventArgs e)
        {
            L($"File [{e.FullPath}] deleted.");
            FileDeleted?.Invoke(e.FullPath);
        }

        private void Watcher_FileCreated(object sender, FileSystemEventArgs e)
        {
            L($"File [{e.FullPath}] created.");
            FileCreated?.Invoke(e.FullPath);
            Watcher_FileChanged(sender, e);
        }

        private void Watcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                Thread.Sleep(200);
                FileInfo aFileInfo = new FileInfo(e.FullPath);
                using (FileStream aFileStream = aFileInfo.OpenRead())
                {
                    aFileStream.Close();
                }
                AddTimerTask(TimeSpan.FromMilliseconds(500), new Action(() => { L($"File [{e.FullPath}] Copied."); FileCopied?.Invoke(e.FullPath); }), $"File [{e.FullPath}] Copied.");
            }
            catch (Exception ex)
            {
                L($"File [{e.FullPath}] changing：[{ex.Message}]...");
                FileChanged?.Invoke(e.FullPath);
            }
        }

        private FileSystemWatcher FileWatcher;

        #endregion
    }
}
