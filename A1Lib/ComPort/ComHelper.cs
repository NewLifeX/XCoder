using ALib.UsbHub;
using NewLife;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALib.ComPort
{
    public static class ComHelper
    {
        /// <summary>获取所有COM,多串口的复合设备COM顺序是按照子设备MI排序的</summary>
        /// <returns>ContainerId,Name</returns>
        public static Dictionary<string, string> GetAllCom()
        {
            var dic = new Dictionary<string, string>();
            var midic = new Dictionary<string, List<(int, string)>>();

            // 使用 WMI 查询 Win32_PnPEntity 中所有 COM 口
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%)'");
            foreach (ManagementObject device in searcher.Get())
            {
                var did = device["DeviceID"] as string;
                var key = UsbHelper.GetContainerId(did);
                var value = device["Name"] as string;

                if ((key == null) || (value == null)) continue;

                if (did.IndexOf("&MI_") > 0)
                {
                    var mi = -1;
                    var miStr = did.Split('&', '\\').FirstOrDefault(s => s.StartsWith("MI_"));
                    mi = miStr != null ? miStr.TrimStart("MI_").ToInt() : -1;

                    List<(int, string)> list;
                    if (midic.ContainsKey(key))
                    {
                        list = midic[key];
                        list.Add((mi, value));
                    }
                    else
                    {
                        list = new List<(int, string)>();
                        list.Add((mi, value));
                        midic[key] = list;
                    }
                }
                else
                {
                    if (dic.ContainsKey(key))
                    {
                        var oldStr = dic[key];
                        var newStr = oldStr + " + " + value;
                        dic[key] = newStr;
                    }
                    else
                    {
                        dic.Add(key, value);
                    }
                }
            }

            if (midic.Count > 0)
            {
                foreach (var item in midic)
                {
                    var list = item.Value.OrderBy(x => x.Item1).ToList();
                    var outstr = string.Join(" + ", list.Select(x => x.Item2));
                    dic[item.Key] = outstr;
                }
            }

            return dic;
        }

        /// <summary>获取串口名称</summary>
        /// <param name="idx">COM口顺序号，不是COM口的不计入</param>
        /// <returns>不是串口返回 ""</returns>
        public static String GetComName(this UsbDevice dev, int idx = 0)
        {
            var str = dev.ToString();

            if (str.IndexOf("(COM") < 0)
            {
                // FTDIBUS
                if (dev.VID == "0403")
                {
                    var all = GetAllCom();
                    if (all.ContainsKey(dev.ContainerId)) str = all[dev.ContainerId];
                }
                else
                {
                    return "";
                }
            }

            var strs = new List<string>();
            var offset = 0;
            while (true)
            {
                var head = str.IndexOf("(COM", offset);
                if (head < 0) break;

                var tail = str.IndexOf(")", head);

                var comName = str.Substring(head + 1, tail - head - 1);
                strs.Add(comName);
                offset = tail + 1;
            }

            if (strs.Count < 1) return "";

            return strs[idx];
        }

        /// <summary>从HUB端口，获取串口名称</summary>
        /// <param name="dev"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static String GetComName(this UsbHubPort dev)
        {
            if (dev == null) return "";
            if (!dev.Exists()) return "";
            return GetComName(dev.GetUsbDevice(), dev.SubDev);
        }

        /// <summary>从USB创建SerialTransport</summary>
        /// <param name="dev"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static SerialTransport CreateSerialTransport(this UsbDevice dev, int idx = 0)
        {
            var com = GetComName(dev, idx);
            if (!com.StartsWith("COM")) return null;

            var port = new SerialTransport();
            port.PortName = com;

            return port;
        }

        /// <summary>创建解码串口</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dev"></param>
        /// <param name="idx"></param>
        /// <param name="baudrate"></param>
        /// <returns></returns>
        public static T Create<T>(this UsbDevice dev, int idx = 0, int baudrate = 115200) where T : Com_WithUnpack, new()
        {
            var com = dev.GetComName(idx);
            if (!com.StartsWith("COM")) return null;

            var t = new T();
            t.Init(com, baudrate);

            return t;
        }

        /// <summary>从USB HUB端口创建解码对象</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dev"></param>
        /// <param name="baudrate"></param>
        /// <returns></returns>
        public static T Create<T>(this UsbHubPort dev, int baudrate = 115200) where T : Com_WithUnpack, new()
        {
            if (!dev.Exists()) return null;
            return Create<T>(dev.GetUsbDevice(), dev.SubDev, baudrate);
        }
    }
}
