using ALib.UsbHub;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Camera
{
    public static class  CameraHelper
    {
        /// <summary>获取所有相机PNPID,不区分是否是USB摄像头</summary>
        /// <returns></returns>
        public static string[] GetAllCameraPnpId()
        {
            List<string> cameras = new List<string>();
            try
            {
                // 查询Win32_PnPEntity中所有摄像头设备
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')");
                foreach (ManagementObject device in searcher.Get())
                {
                    string name = device["PNPDeviceID"]?.ToString();
                    if (!string.IsNullOrEmpty(name))
                    {
                        cameras.Add(name);
                    }
                }
            }
            catch { }

            return cameras.ToArray();
        }

        /*
        /// <summary>获取相机编号</summary>
        /// <returns>不是相机返回 -1</returns>
        public static int GetCameraIndex(this UsbDevice dev)
        {
            if (dev == null) return -1;
            var cans = GetAllCameraPnpId();
            if (cans == null || cans.Length < 1) return -1;

            for (int i = 0; i < cans.Length; i++)
            {
                var can = cans[i];
                var cid = UsbHelper.GetContainerId(can);
                if (cid == null) continue;

                if (cid.Equals(dev.ContainerId, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
        */

        public static string GetCameraPnp(this UsbDevice dev)
        {
            if (dev == null) return null;
            var cans = GetAllCameraPnpId();
            if (cans == null || cans.Length < 1) return null;

            for (int i = 0; i < cans.Length; i++)
            {
                var can = cans[i];
                var cid = UsbHelper.GetContainerId(can);
                if (cid == null) continue;

                if (cid.Equals(dev.ContainerId, StringComparison.OrdinalIgnoreCase))
                {
                    return cans[i];
                }
            }

            return null;
        }
    }
}
