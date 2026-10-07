using ALib.UsbHub;
using HidSharp;
using Microsoft.Win32;
using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Hid
{
    public static class UsbHidHelper
    {
        /// <summary>获取所有USB HID,不考虑复合设备</summary>
        /// <remarks>注意，只包含HidDevice支持的HID设备</remarks>
        /// <param name="devs">从这些设备里面获取，没有就从所有设备里面筛选</param>
        /// <returns>ContainerId,HidDevice</returns>
        public static Dictionary<string, List<HidDevice>> GetAllUsbHid(List<UsbDevice> devs = null)
        {
            // 筛选范围
            if (devs == null)
            {
                var all = UsbHelper.GetAllUsbInfo();
                devs = new List<UsbDevice>();
                foreach (var item in all)
                {
                    devs.AddRange(item.Value);
                }
            }

            // 所有有效 usb hid 设备 cid
            var allcid = GetAllHidCid();
            // 转换为小写
            var temp = new Dictionary<string, string>();
            foreach (var item in allcid)
            {
                temp.Add(item.Key.ToLower(), item.Value);
            }
            allcid = temp;

            // 使用 HidSharp 获取所有 HID 设备
            var deviceList = DeviceList.Local;
            var hidDevices = deviceList.GetHidDevices().ToArray();

            var dic = new Dictionary<string, List<HidDevice>>();
            foreach (var hid in hidDevices)
            {
                // path = @"\\?\hid#vid_09da&pid_c10a&col01#9&54744d2&1&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"
                var path = hid.DevicePath;
                // 去掉 @"\\?\"
                // path = @"hid#vid_09da&pid_c10a&col01#9&54744d2&1&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"
                path = path.Replace(@"\\?\", "");
                // # 变成 \
                // path = @"hid\vid_09da&pid_c10a&col01\9&54744d2&1&0000\{4d1e55b2-f16f-11cf-88cb-001111000030}"
                path = path.Replace("#", @"\");
                // 去掉最后面的 \ 和后面的值
                // path = @"hid\vid_09da&pid_c10a&col01\9&54744d2&1&0000"
                path = path.Substring(0, path.LastIndexOf('\\'));
                // 拿到cid
                var cid = GetCid(allcid, path);
                if (string.IsNullOrEmpty(cid)) continue;
                // 再从 devs 里面找对应的设备
                var dev = devs.Find(d => d.ContainerId == cid);
                if (dev == null) continue;

                // 添加到字典
                if (dic.ContainsKey(cid)) dic[cid].Add(hid);
                else dic.Add(cid, new List<HidDevice> { hid });
            }

            return dic;
        }

        /// <summary>从字典里面拿到cid</summary>
        /// <param name="path_cid">注册表路径（纯小写），CID</param>
        /// <param name="path">需要的路径（纯小写）</param>
        /// <returns></returns>
        private static string GetCid(Dictionary<string, string> path_cid, string path)
        {
            foreach (var item in path_cid)
            {
                var dicpath = item.Key;
                if (dicpath.IndexOf(path) != -1) return item.Value;
            }

            return "";
        }

        /// <summary>从注册表获取所有hid设备的ContainerId</summary>
        /// <remarks>注意，包含了键盘鼠标等标准设备</remarks>
        /// <param name="onlyUsb">只拿usb的hid设备</param>
        /// <returns>注册表路径，ContainerId</returns>        
        public static Dictionary<string, string> GetAllHidCid(bool onlyUsb = true)
        {
            var dic = new Dictionary<string, string>();

            // Registry.LocalMachine 注册表搜索 @"SYSTEM\CurrentControlSet\Enum\HID\" 目录下 ContainerID 字段
            // 目录 和 ContainerID 值作为 dic 的 key 和 value
            using (var hidKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\HID"))
            {
                if (hidKey == null) return dic;

                // 遍历所有 VID_xxxx&PID_xxxx 子项
                foreach (var vidPidName in hidKey.GetSubKeyNames())
                {
                    using (var vidPidKey = hidKey.OpenSubKey(vidPidName))
                    {
                        if (vidPidKey == null) continue;

                        // 遍历具体设备实例
                        foreach (var instanceName in vidPidKey.GetSubKeyNames())
                        {
                            var fullPath = $"HID\\{vidPidName}\\{instanceName}";

                            using (var instanceKey = vidPidKey.OpenSubKey(instanceName))
                            {
                                if (instanceKey == null) continue;

                                // 只拿 USB HID 设备
                                // if (onlyUsb)
                                // {
                                //     if (fullPath.IndexOf("PID") < 0) continue;
                                // }

                                // 获取 ContainerID
                                var containerId = instanceKey.GetValue("ContainerID") as string;
                                if (containerId == null) continue;
                                // 打开注册表看到的，这个值的都不是 USB 设备
                                if (onlyUsb)
                                {
                                    if (containerId == @"{00000000-0000-0000-ffff-ffffffffffff}") continue;
                                }

                                if (!string.IsNullOrEmpty(containerId))
                                {
                                    dic[fullPath] = containerId;
                                }
                            }
                        }
                    }
                }
            }

            return dic;
        }

        /// <summary>获取HID设备</summary>
        /// <param name="dev">USB设备</param>
        /// <param name="idx">符合设备序号,按照注册表顺序,mi_col一起排序的</param>
        /// <remarks>注意，idx下标是只是支持HidDevice的HID设备的排序</remarks>
        /// <returns></returns>
        public static HidDevice GetHidDevice(this UsbDevice dev, int idx = 0)
        {
            var dic = GetAllUsbHid(new List<UsbDevice> { dev });
            if(dic.Count < idx)return null;

            return dic[dev.ContainerId][idx];
        }
    }
}
