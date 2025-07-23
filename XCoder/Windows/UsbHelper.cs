
using Microsoft.Win32;
using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace XCoder
{
    /// <summary>USB复合设备</summary>
    public class UsbSubDev
    {
        /// <summary>MI_00, MI_01, MI_02</summary>
        public int MI { get; set; }

        /// <summary>名称</summary>
        public string Name { get; set; }


        public override string ToString()
        {
            return $"[{MI}] {Name}";
        }
    }

    public class UsbDevice
    {
        /// <summary>端口号</summary>
        public int Port { get; set; }

        /// <summary>设备名称</summary>
        public string Name { get; set; }

        /// <summary>容器ID，USB设备唯一ID，复合设备中子设备共有(相同)</summary>
        public string ContainerId { get; set; }

        /// <summary>PNPDeviceID，USB设备注册表路径，复合设备中子设备加 &MI_xx</summary>
        public string PNPDeviceID { get; set; }

        public string VID { get; set; }
        public string PID { get; set; }

        /// <summary>复合设备，设备名称</summary>
        public List<UsbSubDev> SubDev { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"{Port,2} VID_{VID} PID_{PID} {ContainerId}  ");
            // sb.Append($"{CurrentIdPrefix,16}  ");
            // sb.Append($"{ParentIdPrefix,16}  ");
            if (SubDev != null && SubDev.Count > 0)
            {
                sb.Append(string.Join(" + ", SubDev));
            }
            else
            {
                sb.Append($"{Name}");
            }

            return sb.ToString();
        }
    }

    public class UsbHelper
    {
        /// <summary>获取USB集线集信息</summary>
        /// <remarks>集线集编号 & 设备信息</remarks>
        /// <returns></returns>
        public static Dictionary<int, List<UsbDevice>> GetAllUsbInfo()
        {
            var roots = new Dictionary<int, List<UsbDevice>>();

            // 查询所有 USB 设备
            using (var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE 'USB%'"))
            {
                var devices = searcher.Get();
                foreach (var device in devices)
                {
                    string name = device["Name"]?.ToString();
                    string pnpId = device["PNPDeviceID"]?.ToString();
                    // XTrace.WriteLine($"设备名称: {name}");
                    // XTrace.WriteLine($"PNPDeviceID: {pnpId}");

                    // 复合设备中的子设备
                    if (pnpId.IndexOf("&MI_") >= 0) continue;

                    // 获取注册表中的 LocationInformation
                    string location = GetLocationInformation(pnpId);
                    if (location == null) continue;
                    // XTrace.WriteLine($"位置信息: {location}");
                    // XTrace.WriteLine(new string('-', 50));

                    var phead = "Port_#";
                    var hhead = "Hub_#";
                    if (location.IndexOf(phead) < 0) continue;
                    var strs = location.Split('.');
                    var port = strs[0].TrimStart(phead).ToInt();
                    var hub = strs[1].TrimStart(hhead).ToInt();
                    var cid = GetContainerId(pnpId);
                    var (vid, pid) = GetVidPid(pnpId);

                    var dev = new UsbDevice
                    {
                        Port = port,
                        Name = name,
                        ContainerId = cid,
                        VID = vid,
                        PID = pid,
                        PNPDeviceID = pnpId,
                        // CurrentIdPrefix = pnpId.Split('\\').LastOrDefault(),
                        // ParentIdPrefix = GetParentIdPrefix(pnpId),
                    };

                    if (roots.ContainsKey(hub)) { roots[hub].Add(dev); }
                    else { roots[hub] = new List<UsbDevice> { dev }; }
                }

                // 处理复合设备
                foreach (var device in devices)
                {
                    string name = device["Name"]?.ToString();
                    string pnpId = device["PNPDeviceID"]?.ToString();
                    // XTrace.WriteLine($"设备名称: {name}");
                    // XTrace.WriteLine($"PNPDeviceID: {pnpId}");

                    // 复合设备中的子设备
                    if (pnpId.IndexOf("&MI_") < 0) continue;

                    var mi = -1;
                    var miStr = pnpId.Split('&', '\\').FirstOrDefault(s => s.StartsWith("MI_"));
                    mi = miStr != null ? miStr.TrimStart("MI_").ToInt() : -1;

                    // 容器ID
                    var cid = GetContainerId(pnpId);

                    // 找到对应的物理设备
                    UsbDevice dev = null;
                    foreach (var kv in roots)
                    {
                        // 查找对应的集线集
                        var usbDev = kv.Value.FirstOrDefault(d => d.ContainerId == cid);
                        if (usbDev != null)
                        {
                            dev = usbDev;
                            break;
                        }
                    }
                    if (dev == null) continue;

                    // 复合设备的子设备名称
                    if (dev.SubDev == null) dev.SubDev = new List<UsbSubDev>();
                    dev.SubDev.Add(new UsbSubDev { MI = mi, Name = name });
                }
            }

            // 整理数据，友好输出
            foreach (var kv in roots)
            {
                // 按端口号排序
                if (kv.Value == null || kv.Value.Count > 1)
                {
                    kv.Value.Sort((x, y) => x.Port.CompareTo(y.Port));
                }

                foreach (var dev in kv.Value)
                {
                    // 按复合设备的MI排序
                    if (dev.SubDev != null && dev.SubDev.Count > 1)
                    {
                        dev.SubDev.Sort((x, y) => x.MI.CompareTo(y.MI));
                    }
                }
            }

            return roots;
        }

        /// <summary>打印USB设备</summary>
        /// <param name="usbInfo"></param>
        /// <returns></returns>
        public static string UsbInfoString(Dictionary<int, List<UsbDevice>> usbInfo)
        {
            var sb = new StringBuilder();
            foreach (var kv in usbInfo)
            {
                sb.AppendLine("");
                sb.AppendLine($"--  Hub_{kv.Key}  --------------------");
                foreach (var dev in kv.Value)
                {
                    sb.AppendLine(dev.ToString());
                }
            }

            return sb.ToString();
        }

        // 从注册表中读取 LocationInformation
        static string GetLocationInformation(string devInstPath)
        {
            if (string.IsNullOrEmpty(devInstPath)) return null;

            using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\" + devInstPath))
            {
                return key?.GetValue("LocationInformation") as string;
            }
        }

        // 从注册表中读取 ContainerId
        public static string GetContainerId(string devInstPath)
        {
            if (string.IsNullOrEmpty(devInstPath)) return null;

            using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\" + devInstPath))
            {
                return key?.GetValue("ContainerID") as string;
            }
        }

        /// <summary>提取VID PID</summary>
        /// <param name="pnpId"></param>
        /// <returns></returns>
        static (string, string) GetVidPid(string pnpId)
        {
            if (pnpId.IsNullOrEmpty()) return ("", "");
            // "USB\\VID_0408&PID_5365\\7&2C27FCE3&0&0000"
            // "USB\\VID_0408&PID_5365&MI_00\\7&2C27FCE3&0&0000"
            var strs = pnpId.Split('\\');
            if (strs.Length < 3) return ("", "");
            // 取第二段，包含 VID 和 PID
            // VID_0408&PID_5365
            // VID_0408&PID_5365&MI_00
            var vpid = strs[1];
            if (vpid.IsNullOrEmpty()) return ("", "");

            strs = vpid.Split('&');
            if (strs.Length < 2) return ("", "");
            var vid = strs[0].Split('_').LastOrDefault();
            var pid = strs[1].Split('_').LastOrDefault();

            return (vid, pid);
        }
    }
}
