using System;
using System.Diagnostics;
using System.IO;

namespace Shannan.Core.Utils
{
    public static class ProcessUtils
    {
        public static void OpenFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
#if DEBUG
                throw new Exception("打开文件失败，文件或已丢失");
#else
                return;
#endif
            }
            try
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = fileName,
                    Verb = "Open",
                    CreateNoWindow = true
                };
                process.Start();
            }
            catch (Exception)
            {
                OpenDirectory(Path.GetDirectoryName(fileName));
            }
        }

        public static void OpenDirectory(string directoryName)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = directoryName
            };
            process.Start();
        }

        /// <summary>
        /// 调用外部程序
        /// </summary>
        /// <param name="path">外部程序路径</param>
        public static void Run(string path)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.Start();
        }
    }
}
