using DeviceId;
using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;

namespace Shannan.Core.Framework
{
    public class Client
    {
        private Client()
        {
            DeviceId = new DeviceIdBuilder().AddMachineName().OnWindows(windows => windows.AddProcessorId().AddMotherboardSerialNumber().AddSystemDriveSerialNumber()).ToString();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    NetType = adapter.Name;
                    break;
                }
            }

            #region 计算机信息

            ManagementClass mcComputer = new ManagementClass("Win32_ComputerSystem");
            foreach (ManagementObject mo in mcComputer.GetInstances())
            {
                ComputerManufacturer = mo.GetPropertyValue("Manufacturer").ToString();
                ComputerModel = mo.GetPropertyValue("Model").ToString();
                ComputerSystemType = mo.GetPropertyValue("SystemType").ToString();
                ComputerPhysicalMemory = long.Parse(mo.GetPropertyValue("TotalPhysicalMemory").ToString());
                break;
            }

            #endregion 计算机信息

            #region 物理内存

            PhysicalMemory = 0;
            ManagementClass mcPhysicalMemory = new ManagementClass("Win32_PhysicalMemory");
            foreach (ManagementObject mo in mcPhysicalMemory.GetInstances())
            {
                PhysicalMemory += long.Parse(mo.GetPropertyValue("Capacity").ToString());
            }

            #endregion 物理内存

            #region 操作系统信息

            ManagementClass mcOperating = new ManagementClass("Win32_OperatingSystem");
            foreach (ManagementObject mo in mcOperating.GetInstances())
            {
                OSName = mo.GetPropertyValue("Caption").ToString();
                OSVersion = mo.GetPropertyValue("Version").ToString();
                OSArchitecture = mo.GetPropertyValue("OSArchitecture").ToString();
                break;
            }

            #endregion 操作系统信息

            #region 处理器/CPU

            ManagementClass mcProcessor = new ManagementClass("Win32_Processor");
            foreach (ManagementObject mo in mcProcessor.GetInstances())
            {
                CPUName = mo.GetPropertyValue("Name").ToString();
                break;
            }

            #endregion 处理器/CPU

            #region 显卡

            ManagementClass mcVideo = new ManagementClass("Win32_VideoController");
            foreach (ManagementObject mo in mcVideo.GetInstances())
            {
                VideoName = mo.GetPropertyValue("Name").ToString();
                VideoDriverVersion = mo.GetPropertyValue("DriverVersion").ToString();
                VideoDriverDate = mo.GetPropertyValue("DriverDate").ToString();
                break;
            }

            #endregion 显卡

            #region 应用程序信息

            AssemblyName assemName = Assembly.GetEntryAssembly().GetName();
            AppName = assemName.Name;
            AppVersion = assemName.Version.ToString(4);

            #endregion 应用程序信息

            //GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            //watcher.PositionChanged += Watcher_PositionChanged;
            //watcher.StatusChanged += Watcher_StatusChanged;
            //watcher.Start(false);
        }

        //private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        //{
        //    ConsoleManager.Log(e.Status.ToString());
        //}

        //private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        //{
        //    ConsoleManager.Log(e.Position.Location.ToString());
        //    ConsoleManager.Log(e.Position.Timestamp.ToString());
        //}

        static Client()
        {
            Instance = new Client();
        }

        public static Client Instance
        {
            get;
            private set;
        }

        public string DeviceId { get; private set; } = "";

        /// <summary>
        /// 操作系统名称
        /// </summary>
        public string OSName { get; private set; } = "";

        /// <summary>
        /// 操作系统平台
        /// </summary>
        public string OSPlatform
        {
            get
            {
                return Environment.OSVersion.Platform.ToString();
            }
        }

        /// <summary>
        /// 操作系统版本号
        /// </summary>
        public string OSVersion { get; private set; } = "";

        /// <summary>
        /// 操作系统架构
        /// </summary>
        public string OSArchitecture { get; private set; } = "";

        /// <summary>
        /// 操作系统是否为64位
        /// </summary>
        public bool Is64BitOperatingSystem
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }

        /// <summary>
        /// 计算机制造商
        /// </summary>
        public string ComputerManufacturer { get; private set; } = "";

        /// <summary>
        /// 计算机型号
        /// </summary>
        public string ComputerModel { get; private set; } = "";

        /// <summary>
        /// 计算机系统类型
        /// </summary>
        public string ComputerSystemType { get; private set; } = "";

        /// <summary>
        /// 计算机可用物理内存
        /// </summary>
        public long ComputerPhysicalMemory { get; private set; } = 0;

        /// <summary>
        /// 物理内存
        /// </summary>
        public long PhysicalMemory { get; private set; } = 0;

        /// <summary>
        /// 处理器
        /// </summary>
        public string CPUName { get; private set; } = "";

        /// <summary>
        /// 显卡名称
        /// </summary>
        public string VideoName { get; private set; } = "";

        /// <summary>
        /// 视频驱动程序版本号
        /// </summary>
        public string VideoDriverVersion { get; private set; } = "";

        /// <summary>
        /// 视频驱动程序最后修改日期
        /// </summary>
        public string VideoDriverDate { get; private set; } = "";

        /// <summary>
        /// 分辨率
        /// </summary>
        public string Resolution
        {
            get
            {
                return string.Format("{0}*{1}", SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
            }
        }

        public string NetType { get; private set; } = "";

        public int CurrentTimestamp
        {
            get
            {
                return Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            }
        }

        public int ServerTimestampDiff { get; private set; } = 0;

        public void SetServerTimestamp(int serverTimestamp)
        {
            ServerTimestampDiff = CurrentTimestamp - serverTimestamp;
        }

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string AppName { get; private set; } = "";

        /// <summary>
        /// 应用程序版本号
        /// </summary>
        public string AppVersion { get; private set; } = "";

        public string CLRVersion
        {
            get
            {
                return Environment.Version.ToString(3);
            }
        }
    }
}
