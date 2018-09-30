using System.Runtime.InteropServices;
using System.IO;
namespace FileSync
{
    public interface ISyncTask
    {
        string TaskName { get; }
        string Source { get; }
        string SourceTime { get; }
        string SourceSize { get; }
        string Dest { get; }
        string DestTime { get; }
        string DestSize { get; }
        void Execute();
    }

    public class CopyFileTask : ISyncTask
    {
        private System.IO.FileInfo m_SourceFileInfo;
        private System.IO.FileInfo m_DestFileInfo;

        public CopyFileTask(System.IO.FileInfo aSourceFileInfo, System.IO.FileInfo aDestFileInfo)
        {
            m_SourceFileInfo = aSourceFileInfo;
            m_DestFileInfo = aDestFileInfo;
        }

        #region ISyncTask 成员

        public string TaskName
        {
            get { return m_DestFileInfo.Exists ? "更新文件" : "复制文件"; }
        }

        public string Source
        {
            get { return m_SourceFileInfo.FullName; }
        }

        public string SourceTime
        {
            get { return m_SourceFileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public string SourceSize
        {
            get { return m_SourceFileInfo.Length.ToString(); }
        }

        public string Dest
        {
            get { return m_DestFileInfo.FullName; }
        }

        public string DestTime
        {
            get { return m_DestFileInfo.Exists ? m_DestFileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") : ""; }
        }

        public string DestSize
        {
            get { return m_DestFileInfo.Exists ? m_DestFileInfo.Length.ToString() : ""; }
        }

        public void Execute()
        {
            m_SourceFileInfo.CopyTo(m_DestFileInfo.FullName, true);
        }

        #endregion
    }

    public class CopyFolderTask : ISyncTask
    {
        private System.IO.DirectoryInfo m_SourceDirectoryInfo;
        private System.IO.DirectoryInfo m_DestDirectoryInfo;

        public CopyFolderTask(System.IO.DirectoryInfo aSourceDirectoryInfo, System.IO.DirectoryInfo aDestDirectoryInfo)
        {
            m_SourceDirectoryInfo = aSourceDirectoryInfo;
            m_DestDirectoryInfo = aDestDirectoryInfo;
        }

        #region ISyncTask 成员

        public string TaskName
        {
            get { return "复制文件夹"; }
        }

        public string Source
        {
            get { return m_SourceDirectoryInfo.FullName; }
        }

        public string SourceTime
        {
            get { return m_SourceDirectoryInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public string SourceSize
        {
            get { return ""; }
        }

        public string Dest
        {
            get { return m_DestDirectoryInfo.FullName; }
        }

        public string DestTime
        {
            get { return ""; }
        }

        public string DestSize
        {
            get { return ""; }
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto, Pack = 1)]
        public struct SHFILEOPSTRUCT
        {
            public System.IntPtr hwnd;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public System.IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll ", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
        const int FO_DELETE = 3;
        const int FO_COPY = 2;
        const int FOF_ALLOWUNDO = 0x40;
        const int FOF_NOCONFIRMATION = 0x10;     //Don 't     prompt     the     user.;    
        const int FOF_SIMPLEPROGRESS = 0x100;
        const int FOF_NORECURSION = 0x1000;
        const int FOF_NOCONFIRMMKDIR = 0x0200;

        public void SendToRecyclyBin(string path)
        {
            SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT();
            shf.wFunc = FO_DELETE;
            shf.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
            shf.pFrom = path;
            SHFileOperation(ref shf);
        }

        public void Execute()
        {
            SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT();
            shf.wFunc = FO_COPY;
            shf.fFlags = FOF_NOCONFIRMMKDIR;
            shf.pFrom = m_SourceDirectoryInfo.FullName + "\0\0";
            shf.pTo = m_DestDirectoryInfo.FullName + "\0\0";
            SHFileOperation(ref shf);
        }

        #endregion
    }

    public class FileSyncTaskCreator
    {
        public FileSyncTaskCreator(string aSourceFolder, string aDestFolder)
        {
            m_SourceFolder = aSourceFolder;
            m_DestFolder = aDestFolder;
        }

        private string m_SourceFolder;
        private string m_DestFolder;

        public System.Collections.Generic.List<ISyncTask> GetTasks()
        {
            System.Collections.Generic.List<ISyncTask> aTasks = new System.Collections.Generic.List<ISyncTask>();
            
            System.IO.DirectoryInfo aSourceDirectoryInfo = new System.IO.DirectoryInfo(m_SourceFolder);
            System.IO.DirectoryInfo aDestDirectoryInfo = new System.IO.DirectoryInfo(m_DestFolder);

            if (!aSourceDirectoryInfo.Exists) throw new System.ApplicationException(string.Format("源文件夹[{0}]不存在！", m_SourceFolder));
            GetTasks(aTasks, aSourceDirectoryInfo, aDestDirectoryInfo);

            return aTasks;
        }

        private void GetTasks(System.Collections.Generic.List<ISyncTask> aTasks, System.IO.DirectoryInfo aSourceDirectoryInfo, System.IO.DirectoryInfo aDestDirectoryInfo)
        {
            if (!aDestDirectoryInfo.Exists)
            {
                aTasks.Add(new CopyFolderTask(aSourceDirectoryInfo, aDestDirectoryInfo));
                return;
            }

            var aSourceFileInfos = aSourceDirectoryInfo.GetFiles();
            foreach (var aSourceFileInfo in aSourceFileInfos)
            {
                System.IO.FileInfo aDestFileInfo = new System.IO.FileInfo(System.IO.Path.Combine(aDestDirectoryInfo.FullName, aSourceFileInfo.Name));
                if (!aDestFileInfo.Exists || aDestFileInfo.LastWriteTime < aSourceFileInfo.LastWriteTime)
                {
                    aTasks.Add(new CopyFileTask(aSourceFileInfo, aDestFileInfo));
                }
            }

            var aChildFolders = aSourceDirectoryInfo.GetDirectories();
            foreach (var aChildFolder in aChildFolders)
            {
                System.IO.DirectoryInfo aDestChildFolder = new System.IO.DirectoryInfo(System.IO.Path.Combine(aDestDirectoryInfo.FullName, aChildFolder.Name));
                GetTasks(aTasks, aChildFolder, aDestChildFolder);
            }
        }
    }
}
