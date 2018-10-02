using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

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

        public static string LocalFileName(Type aHostType, string aFileName)
        {
            return Path.Combine(Path.GetDirectoryName(aHostType.Assembly.Location), aFileName);
        }

        /// <summary>
        /// 在程序集中检索出指定基类的所有可创建实例的派生类
        ///     可用于工厂模式中查找指定类系。
        /// </summary>
        /// <param name="aBaseType">基类</param>
        /// <returns>可创建实例的派生类的类型集合</returns>
        public static IEnumerable<Type> GetSubclassTypes(Type aBaseType)
        {
            return from r in Assembly.GetAssembly(aBaseType).GetTypes() where !r.IsAbstract && r.IsSubclassOf(aBaseType) select r;
        }

        /// <summary>
        /// 在程序集中检索出指定基类的所有可创建实例的派生类，并创建相应实例。
        ///     可用于原型模式中，创建原型实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetSubclassInstances<T>(Type aHostType) where T : class
        {
            Assembly aAssembly = Assembly.GetAssembly(aHostType);
            if (typeof(T).IsInterface)
                return from r in aAssembly.GetTypes() where !r.IsAbstract && r.GetInterface(typeof(T).FullName) != null select aAssembly.CreateInstance(r.FullName) as T;
            else
                return from r in aAssembly.GetTypes() where !r.IsAbstract && r.IsSubclassOf(typeof(T)) select aAssembly.CreateInstance(r.FullName) as T;
        }
    }
}
