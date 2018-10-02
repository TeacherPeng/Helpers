using System;
using System.IO;
using System.Threading;
using static PengSW.RuntimeLog.RL;
using static PengSW.TimerTaskHelper.TimerTaskManager;

namespace PengSW.FileSystemWatcherHelper
{
    /// <summary>
    /// 对System.IO.FileSystemWatcher进行鉴别处理，反馈更为精确的监视事件。
    /// </summary>
    public class FileSystemWatcherEx
    {
        #region 初始化

        public FileSystemWatcherEx()
        {
            FileWatcher = new FileSystemWatcher();
            FileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            FileWatcher.IncludeSubdirectories = true;
            FileWatcher.Changed += new FileSystemEventHandler(Watcher_FileChanged);
            FileWatcher.Created += new FileSystemEventHandler(Watcher_FileCreated);
            FileWatcher.Deleted += new FileSystemEventHandler(Watcher_FileDeleted);
            FileWatcher.Error += new ErrorEventHandler(Watcher_Error);
            FileWatcher.Renamed += new RenamedEventHandler(Watcher_FileRenamed);

            FolderWatcher = new FileSystemWatcher();
            FolderWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName;
            FolderWatcher.IncludeSubdirectories = true;
            FolderWatcher.Changed += new FileSystemEventHandler(Watcher_FileChanged);
            FolderWatcher.Created += new FileSystemEventHandler(Watcher_FolderCreated);
            FolderWatcher.Deleted += new FileSystemEventHandler(Watcher_FolderDeleted);
            FolderWatcher.Error += new ErrorEventHandler(Watcher_Error);
            FolderWatcher.Renamed += new RenamedEventHandler(Watcher_FolderRenamed);
        }

        #endregion

        #region 监视属性

        public string Path
        {
            get { return FileWatcher.Path; }
            set
            {
                FileWatcher.Path = value;
                FolderWatcher.Path = value;
            }
        }

        public bool EnableRaisingEvents
        {
            get { return FileWatcher.EnableRaisingEvents; }
            set
            {
                FileWatcher.IncludeSubdirectories = IncludeSubdirectories;
                FileWatcher.EnableRaisingEvents = value;
                FolderWatcher.IncludeSubdirectories = IncludeSubdirectories;
                FolderWatcher.EnableRaisingEvents = value;
            }
        }

        public bool IncludeSubdirectories
        {
            get { return FileWatcher.IncludeSubdirectories; }
            set
            {
                FileWatcher.IncludeSubdirectories = value;
                FolderWatcher.IncludeSubdirectories = value;
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

        public event Action<string> FolderCreated;
        public event Action<string> FolderChanged;
        public event Action<string> FolderCopied;
        public event Action<string> FolderDeleted;
        public event RenamedEventHandler FolderRenamed;

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
            if (Directory.Exists(e.FullPath))
            {
                Watcher_FolderChanged(sender, e);
            }
            else
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
        }

        private void Watcher_FolderRenamed(object sender, RenamedEventArgs e)
        {
            L($"Folder [{e.OldFullPath}] renamed [{e.FullPath}].");
            FolderRenamed?.Invoke(sender, e);
        }

        private void Watcher_FolderDeleted(object sender, FileSystemEventArgs e)
        {
            L($"Folder [{e.FullPath}] deleted.");
            FolderDeleted?.Invoke(e.FullPath);
        }

        private void Watcher_FolderCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                L($"Folder [{e.FullPath}] created.");
                FolderCreated?.Invoke(e.FullPath);
                AddTimerTask(TimeSpan.FromMilliseconds(1000), new Action(() => { L($"Folder [{e.FullPath}] Copied."); FolderCopied?.Invoke(e.FullPath); }), $"Folder [{e.FullPath}] Copied.");
            }
            catch (Exception ex)
            {
                E(ex, "Watcher_FolderCreated");
            }
        }

        private void Watcher_FolderChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                StopTask($"Folder [{e.FullPath}] Copied.");
                L($"Folder [{e.FullPath}] changed.");
                FolderChanged?.Invoke(e.FullPath);
            }
            catch (Exception ex)
            {
                E(ex, "Watcher_FolderChanged");
            }
        }

        private FileSystemWatcher FileWatcher;
        private FileSystemWatcher FolderWatcher;

        #endregion
    }
}
