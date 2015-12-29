using System.Diagnostics;
using System.IO;

namespace PengSW.Application
{
    public static class ApplicationHelper
    {
        // 检测是否已经有另一个例程在运行
        public static bool HasRunning()
        {
            Process aCurrentProcess = Process.GetCurrentProcess();
            foreach (Process aProcess in Process.GetProcessesByName(aCurrentProcess.ProcessName))
            {
                if (aProcess.Id != aCurrentProcess.Id && aProcess.MainModule.FileName == aCurrentProcess.MainModule.FileName)
                    return true;
            }
            return false;
        }

        // 返回Application所在目录
        public static DirectoryInfo LocalDirectory()
        {
            FileInfo aFileInfo = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
            return aFileInfo.Directory;
        }

        // 设置Application所在目录为当前目录
        public static void SetWorkingDirectory()
        {
            Directory.SetCurrentDirectory(LocalDirectory().FullName);
        }

        // 根据文件名生成含Application所在目录的完整文件名
        public static string LocalFileName(string aFileName)
        {
            return Path.Combine(LocalDirectory().FullName, aFileName);
        }
    }
}
