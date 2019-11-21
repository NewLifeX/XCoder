using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NewLife;
using NewLife.Log;

namespace XCoder.Tools
{
    public class MachineInfo
    {
        #region 属性
        /// <summary>系统名称</summary>
        public String OSName { get; private set; }

        /// <summary>系统版本</summary>
        public String OSVersion { get; private set; }

        /// <summary>处理器序列号</summary>
        public String Processor { get; private set; }

        /// <summary>处理器序列号</summary>
        public String CpuID { get; private set; }

        /// <summary>唯一标识</summary>
        public String UUID { get; private set; }

        /// <summary>机器标识</summary>
        public String Guid { get; private set; }

        /// <summary>内存总量</summary>
        public String Memory { get; private set; }

#if __WIN__
        private ComputerInfo _cinfo;
        /// <summary>可用内存</summary>
        public UInt64 AvailableMemory => _cinfo.AvailablePhysicalMemory;

        private PerformanceCounter _cpuCounter;
        /// <summary>CPU占用率</summary>
        public Single CpuRate => _cpuCounter == null ? 0 : (_cpuCounter.NextValue() / 100);
#else
        /// <summary>可用内存</summary>
        public String AvailableMemory { get; private set; }

        /// <summary>CPU占用率</summary>
        public Single CpuRate { get; private set; }
#endif

        /// <summary>温度</summary>
        public Double Temperature { get; }
        #endregion

        #region 构造
        /// <summary>实例化机器信息</summary>
        public MachineInfo()
        {
            Refresh();
        }
        #endregion

        #region 方法
        /// <summary>刷新</summary>
        public void Refresh()
        {

            OSName = RuntimeInformation.OSDescription;
            OSVersion = Environment.OSVersion.ToString();

            if (Runtime.Windows)
            {
                Processor = GetInfo("Win32_Processor", "Name");
                CpuID = GetInfo("Win32_Processor", "ProcessorId");
                UUID = GetInfo("Win32_ComputerSystemProduct", "UUID");
                OSName = GetInfo("Win32_OperatingSystem", "Caption");
                AvailableMemory = GetInfo("Win32_OperatingSystem", "FreePhysicalMemory");
                //SerialNumber
                Memory = GetInfo("Win32_OperatingSystem", "TotalVisibleMemorySize");
            }
            else if (Runtime.Linux)
            {
                OSName = GetOSName();
                //Memory = GetMemoryInfo();
                SetMemoryInfo();
                Processor = GetCpuInfo();
            }
        }
        #endregion

        #region WMI辅助
        /// <summary>获取WMI信息</summary>
        /// <param name="path"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static String GetInfo(String path, String property)
        {
            // Linux Mono不支持WMI
            if (Runtime.Mono) return "";

            var bbs = new List<String>();
            try
            {
                var wql = String.Format("Select {0} From {1}", property, path);
                var cimobject = new ManagementObjectSearcher(wql);
                var moc = cimobject.Get();
                foreach (var mo in moc)
                {
                    if (mo != null &&
                        mo.Properties != null &&
                        mo.Properties[property] != null &&
                        mo.Properties[property].Value != null)
                        bbs.Add(mo.Properties[property].Value.ToString());
                }
            }
            catch (Exception ex)
            {
                //XTrace.WriteException(ex);
                XTrace.WriteLine("WMI.GetInfo({0})失败！{1}", path, ex.Message);
                return "";
            }

            bbs.Sort();

            return bbs.Distinct().Join();
        }
        #endregion

        #region Linux辅助
        /*
         * https://blog.csdn.net/u014518337/article/details/86291984
         */

        public static String GetOSName()
        {
            const String cpuFilePath = "/etc/issue";
            var s = File.ReadAllText(cpuFilePath);
            return s.Trim();
        }
        public static String GetCpuInfo()
        {
            const String cpuFilePath = "/proc/cpuinfo";
            var s = File.ReadAllText(cpuFilePath);
            var lines = s.Split(new[] { '\n' });
            s = String.Empty;

            foreach (var item in lines)
            {
                if (item.StartsWith("model name"))
                {
                    var temp = item.Split(new[] { ':' });
                    s = temp[1].Trim();
                    break;
                }
            }
            return s;
        }

        public void SetMemoryInfo()
        {
            const String cpuFilePath = "/proc/meminfo";
            var s = File.ReadAllText(cpuFilePath);
            var lines = s.Split(new[] { '\n' });
            s = String.Empty;

            foreach (var item in lines)
            {
                if (item.StartsWith("MemTotal"))
                {
                    var temp = item.Split(new[] { ':' });
                    Memory = temp[1].Trim();
                    //break;
                }
                else if (item.StartsWith("MemFree"))
                {
                    var temp = item.Split(new[] { ':' });
                    AvailableMemory = temp[1].Trim();
                    return; // MemFree位于MemTotal后，因此到这里之后直接返回
                }
            }
        }
        #endregion
    }
}
