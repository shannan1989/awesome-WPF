using System;
using System.Diagnostics;
using System.IO;

namespace Shannan.Core.Utils
{
    public class ProcessUtils
    {
        public static void OpenFile(string fileName)
        {
            if (File.Exists(fileName))
            {
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
            else
            {
                throw new Exception("打开文件失败，文件或已丢失");
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
    }
}
