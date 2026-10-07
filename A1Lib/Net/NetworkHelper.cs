using ALib.UsbHub;
using NewLife.Configuration;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALib.Net
{
    public static class NetworkHelper
    {
        /*
        AI 修改前的版本，未测试。  此处留作备份，  AI 软件不许改。
         
        /// <summary>从USB获取网卡对象</summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public static ManagementObject GetNetworkCard(this UsbDevice dev)
        {
            var dst_cid = dev.ContainerId;

            // 查询所有网口适配器
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionStatus IS NOT NULL"))
            {
                foreach (ManagementObject adapter in searcher.Get())
                {
                    string pnpId = adapter["PNPDeviceID"]?.ToString();
                    var cid = UsbHelper.GetContainerId(pnpId);

                    if (dst_cid == cid) return adapter;
                }
            }

            return null;
        }

        /// <summary>设置静态IP</summary>
        /// <param name="card">网卡</param>
        /// <param name="ip">IP</param>
        /// <param name="mask">子网掩码</param>
        /// <param name="gw">网关</param>
        /// <returns></returns>
        public static bool SetStaticIp(this ManagementObject card, IPAddress ip, IPAddress mask, IPAddress gw)
        {
            if (card == null) return false;

            try
            {
                // 设置 IP
                ManagementBaseObject ipParams = card.GetMethodParameters("EnableStatic");
                ipParams["IPAddress"] = new string[] { ip.ToString() };
                ipParams["SubnetMask"] = new string[] { mask.ToString() };
                ManagementBaseObject ipResult = card.InvokeMethod("EnableStatic", ipParams, null);
                uint ipRes = Convert.ToUInt32(ipResult["ReturnValue"]);
                if (ipRes != 0)
                {
                    // XTrace.WriteLine($"设置IP失败, 错误码: {ipRes}");
                    return false;
                }

                // 设置网关
                ManagementBaseObject gatewayParams = card.GetMethodParameters("SetGateways");
                gatewayParams["DefaultIPGateway"] = new string[] { gw.ToString() };
                gatewayParams["GatewayCostMetric"] = new int[] { 1 };
                ManagementBaseObject gatewayResult = card.InvokeMethod("SetGateways", gatewayParams, null);
                uint gwRes = Convert.ToUInt32(gatewayResult["ReturnValue"]);
                if (gwRes != 0)
                {
                    // XTrace.WriteLine($"设置网关失败, 错误码: {gwRes}");
                    return false;
                }

                // XTrace.WriteLine($"设置静态IP成功: {ip}/{mask} GW:{gw}");
                return true;
            }
            catch { return false; }
        }

         */


        /// <summary>从USB获取网卡对象</summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public static ManagementObject GetNetworkCard(this UsbDevice dev)
        {
            var dst_cid = dev.ContainerId;

            // 查询所有网口适配器
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionStatus IS NOT NULL"))
            {
                foreach (ManagementObject adapter in searcher.Get())
                {
                    string pnpId = adapter["PNPDeviceID"]?.ToString();
                    var cid = UsbHelper.GetContainerId(pnpId);

                    if (dst_cid == cid) return adapter;
                }
            }

            return null;
        }

        /// <summary>设置静态IP</summary>
        /// <param name="card">网卡 (Win32_NetworkAdapter)</param>
        /// <param name="ip">IP</param>
        /// <param name="mask">子网掩码</param>
        /// <param name="gw">网关</param>
        /// <returns></returns>
        public static bool SetStaticIp(this ManagementObject card, IPAddress ip, IPAddress mask, IPAddress gw)
        {
            if (card == null) return false;

            try
            {
                // 通过关联关系直接获取 Configuration 对象
                using (var relatedConfigs = card.GetRelated("Win32_NetworkAdapterConfiguration"))
                {
                    foreach (ManagementObject config in relatedConfigs)
                    {
                        // 设置 IP
                        ManagementBaseObject ipParams = config.GetMethodParameters("EnableStatic");
                        ipParams["IPAddress"] = new string[] { ip.ToString() };
                        ipParams["SubnetMask"] = new string[] { mask.ToString() };
                        ManagementBaseObject ipResult = config.InvokeMethod("EnableStatic", ipParams, null);
                        uint ipRes = Convert.ToUInt32(ipResult["ReturnValue"]);
                        if (ipRes != 0)
                        {
                            // XTrace.WriteLine($"设置IP失败, 错误码: {ipRes}");
                            return false;
                        }

                        // 设置网关
                        ManagementBaseObject gatewayParams = config.GetMethodParameters("SetGateways");
                        gatewayParams["DefaultIPGateway"] = new string[] { gw.ToString() };
                        gatewayParams["GatewayCostMetric"] = new int[] { 1 };
                        ManagementBaseObject gatewayResult = config.InvokeMethod("SetGateways", gatewayParams, null);
                        uint gwRes = Convert.ToUInt32(gatewayResult["ReturnValue"]);
                        if (gwRes != 0)
                        {
                            // XTrace.WriteLine($"设置网关失败, 错误码: {gwRes}");
                            return false;
                        }

                        // XTrace.WriteLine($"设置静态IP成功: {ip}/{mask} GW:{gw}");
                        return true;
                    }
                }

                return false;
            }
            catch { return false; }
        }
    }
}
