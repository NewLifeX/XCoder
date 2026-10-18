using ALib.Audio;
using ALib.Camera;
using ALib.ComPort;
using ALib.Hid;
using ALib.Net;
using NewLife;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.UsbHub
{
    /// <summary>
    /// 本类描述 USB_HUB 物理插口上的 设备/子设备
    /// 作用：描述一个HUB的插口情况，多个插口一起描述的时候可以直接判断此HUB是否存在
    /// </summary>
    public class UsbHubPort
    {
        [Description("设备名")]
        public String Name { get; set; } = "USB";

        [Description("HUB编组")]
        public int HubGroup { get; set; } = 0;

        [Description("HUB物理端口号，插口号。通常从1开始，没有看到0的")]
        public int Port { get; set; } = 0;

        [Description("复合设备的子设备编号")]
        public int SubDev { get; set; } = 0;

        [Description("实际接入的HUB")]
        public int HubId = -1;

        /// <summary>构造</summary>
        /// <param name="grp">编组</param>
        /// <param name="port">插口号</param>
        /// <param name="type">类型</param>
        /// <param name="sub">子设备编号</param>
        public UsbHubPort(int grp, int port, DevType type, int sub = 0)
        {
            HubGroup = grp;
            Port = port;
            SubDev = sub;
            Type = type;
        }

        /// <summary>获取USB设备</summary>
        /// <returns></returns>
        public UsbDevice GetUsbDevice()
        {
            if(HubId < 0) return null;

            var all = UsbHelper.GetAllUsbInfo();
            if (!all.ContainsKey(HubId)) return null;

            var hubdevs = all[HubId];
            return hubdevs.FirstOrDefault(e => e.Port == Port);
        }

        #region 判定设备是否存在

        public enum DevType
        {
            Hid,
            Camera,
            Audio,
            Com,
            Network,
            Hub,
            // 判断 VID  PID
            Other
        };

        /// <summary>设备类型</summary>
        public DevType Type { get; set; } = DevType.Other;

        /// <summary>type=Other时判定</summary>
        public ushort VID { get; set; }
        /// <summary>type=Other时判定</summary>
        public ushort PID { get; set; }

        /// <summary>判断设备在这个位置是否存在</summary>
        /// <param name="devs">hub下所有设备集合</param>
        /// <returns></returns>
        public bool Exists(List<UsbDevice> devs)
        {
            return Exists(devs, Port, Type, SubDev, PID, VID);
        }

        /// <summary>判断设备在这个位置是否存在</summary>
        /// <param name="hubid">系统中hub的编号，默认直接用HubId判断</param>
        /// <returns></returns>
        public bool Exists(int hubid = -1)
        {
            if (hubid < 0) hubid = HubId;
            if (hubid < 0) return false;

            return Exists(hubid, Port, Type, SubDev, PID, VID);
        }

        /// <summary>判定设备是否存在</summary>
        /// <param name="hubdevs"></param>
        /// <param name="port"></param>
        /// <param name="type">类型</param>
        /// <param name="subIdx">复合设备的子设备编号，默认0</param>
        /// <param name="pid">type=Other时判定</param>
        /// <param name="vid">type=Other时判定</param>
        /// <returns></returns>
        public static bool Exists(List<UsbDevice> hubdevs, int port, DevType type, int subIdx = 0, ushort pid = 0, ushort vid = 0)
        {
            /*
            var devs = hubdevs;
            var dic = devs.ToDictionary(e => e.Port, e => e);
            // 端口没有插入设备
            if (!dic.ContainsKey(port)) return false;
            var usbdev = dic[port];
            */
            // 端口没有插入设备
            var usbdev = hubdevs.FirstOrDefault(e => e.Port == port);
            if (usbdev == null) return false;

            bool res = false;
            // 根据类型判断是否存在
            switch (type)
            {
                case DevType.Hid:
                    var hiddev = usbdev.GetHidDevice(subIdx);
                    if (hiddev != null) res = true;
                    break;
                case DevType.Camera:
                    var pnp = usbdev.GetCameraPnp();
                    if (!pnp.IsNullOrEmpty()) res = true;
                    break;
                case DevType.Audio:
                    var ad = usbdev.GetAudioDevice();
                    if (ad != null) res = true;
                    break;
                case DevType.Com:
                    var cname = usbdev.GetComName(subIdx);
                    if (cname.StartsWith("COM")) res = true;
                    break;
                case DevType.Network:
                    var netobj = usbdev.GetNetworkCard();
                    if (netobj != null) res = true;
                    break;
                case DevType.Hub:
                    if (usbdev.Name.Contains("集线器")) res = true;
                    break;
                case DevType.Other:
                    var _pid = BitConverter.ToUInt16(usbdev.PID.GetBytes(), 0);
                    var _vid = BitConverter.ToUInt16(usbdev.VID.GetBytes(), 0);
                    if ((_pid == pid) && (_vid == vid)) res = true;
                    break;
                default: break;
            }

            return res;
        }

        /// <summary>判定设备是否存在</summary>
        /// <param name="hubid"></param>
        /// <param name="port"></param>
        /// <param name="type">类型</param>
        /// <param name="subIdx">复合设备的子设备编号，默认0</param>
        /// <param name="pid">type=Other时判定</param>
        /// <param name="vid">type=Other时判定</param>
        /// <returns></returns>
        public static bool Exists(int hubid, int port, DevType type, int subIdx = 0, ushort pid = 0, ushort vid = 0)
        {
            if (hubid < 0) return false;

            var all = UsbHelper.GetAllUsbInfo();
            if (!all.ContainsKey(hubid)) return false;

            return Exists(all[hubid], port, type, subIdx, pid, vid);
        }

        #endregion

        #region 查找HUB

        /// <summary>获取检查有效的hub</summary>
        /// <param name="pdevs"></param>
        /// <param name="ignore">pdevs中不参与匹配的。 注意：成员和pdevs内的是同一个对象</param>
        /// <returns>key是group，value是满足条件的hubids</returns>
        public static Dictionary<int,List<int>> FindHubs(UsbHubPort[] pdevs, UsbHubPort[] ignore = null)
        {
            // 拿到所有去重后的 group id
            var gps = pdevs.Select(e => e.HubGroup).Distinct().ToList();

            // 满足hub筛查条件的hub集合
            var dic = new Dictionary<int, List<int>>();
            foreach (var hub in UsbHelper.GetAllUsbInfo())
            {
                // 分别判断是否满足分组
                foreach (var gp in gps)
                {
                    bool grpOk = true;
                    foreach (var udev in pdevs)
                    {
                        if ((ignore!=null) && ignore.Contains(udev)) continue;

                        // 是相关分组
                        if (udev.HubGroup != gp) continue;
                        if (!udev.Exists(hub.Value))
                        {
                            grpOk = false;
                            break;
                        }
                    }

                    if (grpOk)
                    {
                        if (!dic.ContainsKey(gp))
                        {
                            var list = new List<int>();
                            list.Add(hub.Key);
                            dic.Add(gp, list);
                        }
                        else
                        {
                            dic[gp].Add(hub.Key);
                        }
                    }
                }
            }

            return dic;
        }

        /// <summary>获取检查有效的hub,每个Group只且只有一个匹配项</summary>
        /// <param name="pdevs"></param>
        /// <param name="ignore"></param>
        /// <returns>key是group，value是满足条件的hubid</returns>
        public static Dictionary<int, int> FindHubs_JustOne(UsbHubPort[] pdevs, UsbHubPort[] ignore = null)
        {
            var dic = FindHubs(pdevs, ignore);

            if (dic == null || dic.Count < 1) return null;
            foreach (var item in dic)
            {
                if (item.Value.Count != 1) return null;
            }

            return dic.ToDictionary(e => e.Key, e => e.Value[0]);
        }

        #endregion
    }
}
