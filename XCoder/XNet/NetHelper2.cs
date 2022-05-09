using System.Management;
using System.Security.AccessControl;
using Microsoft.Win32;
using NewLife;
using NewLife.Log;

namespace XCoder.XNet
{
    static class NetHelper2
    {
        #region Tcp参数
        /// <summary>设置最大Tcp连接数</summary>
        public static void SetTcpMax()
        {
            TcpNumConnections = 0x00FFFFFE;
            MaxUserPort = 65534;
            MaxFreeTcbs = 16000;
            MaxHashTableSize = 65536;
            TcpTimedWaitDelay = 30;
            KeepAliveTime = 30 * 60 * 1000;
            EnableConnectionRateLimiting = 0;
        }

        /// <summary>显示Tcp参数</summary>
        public static void ShowTcpParameters()
        {
            XTrace.WriteLine("{0,-17}: {1,10:n0} Max : {2,10:n0}", "TcpNumConnections", TcpNumConnections, 0x00FFFFFE);
            XTrace.WriteLine("{0,-17}: {1,10:n0} Max : {2,10:n0}", "MaxUserPort", MaxUserPort, 65534);
            XTrace.WriteLine("{0,-17}: {1,10:n0} Max : {2,10:n0}", "MaxFreeTcbs", MaxFreeTcbs, 16000);
            XTrace.WriteLine("{0,-17}: {1,10:n0} Max : {2,10:n0}", "MaxHashTableSize", MaxHashTableSize, 65536);
            XTrace.WriteLine("{0,-17}: {1,10:n0} Best: {2,10:n0}", "TcpTimedWaitDelay", TcpTimedWaitDelay, 30);
            XTrace.WriteLine("{0,-17}: {1,10:n0} Best: {2,10:n0}", "KeepAliveTime", KeepAliveTime, 30 * 60 * 1000);
            XTrace.WriteLine("{0,-17}: {1,10:n0} Best: {2,10:n0}", "EnableConnectionRateLimiting", EnableConnectionRateLimiting, 0);
        }

        /// <summary>最大TCP连接数。默认16M</summary>
        public static Int32 TcpNumConnections { get { return GetReg("TcpNumConnections", 0x00FFFFFE); } set { SetReg("TcpNumConnections", value); } }

        /// <summary>最大用户端口数。默认5000</summary>
        /// <remarks>
        /// TCP客户端和服务器连接时，客户端必须分配一个动态端口，默认情况下这个动态端口的分配范围为 1024-5000 ，也就是说默认情况下，客户端最多可以同时发起3977 个Socket 连接
        /// </remarks>
        public static Int32 MaxUserPort { get { return GetReg("MaxUserPort", 5000); } set { SetReg("MaxUserPort", value); } }

        /// <summary>最大TCB 数量。默认2000</summary>
        /// <remarks>
        /// 系统为每个TCP 连接分配一个TCP 控制块(TCP control block or TCB)，这个控制块用于缓存TCP连接的一些参数，每个TCB需要分配 0.5 KB的pagepool 和 0.5KB 的Non-pagepool，也就说，每个TCP连接会占用 1KB 的系统内存
        /// </remarks>
        public static Int32 MaxFreeTcbs { get { return GetReg("MaxFreeTcbs", 2000); } set { SetReg("MaxFreeTcbs", value); } }

        /// <summary>最大TCP连接数。默认16M</summary>
        /// <remarks>
        /// 这个值指明分配 pagepool 内存的数量，也就是说，如果MaxFreeTcbs = 1000 , 则 pagepool 的内存数量为 500KB
        /// 那么 MaxHashTableSize 应大于 500 才行。这个数量越大，则Hash table 的冗余度就越高，每次分配和查找 TCP  连接用时就越少。这个值必须是2的幂，且最大为65536.
        /// </remarks>
        public static Int32 MaxHashTableSize { get { return GetReg("MaxHashTableSize", 512); } set { SetReg("MaxHashTableSize", value); } }

        /// <summary>系统释放已关闭的TCP连接并复用其资源之前，必须等待的时间。默认240</summary>
        /// <remarks>
        /// 这段时间间隔就是TIME_WAIT状态（2MSL，数据包最长生命周期的两倍状态）。
        /// 如果系统显示大量连接处于TIME_WAIT状态，则会导致并发量与吞吐量的严重下降，通过减小该项的值，系统可以更快地释放已关闭的连接，
        /// 从而为新连接提供更多的资源，特别是对于高并发短连接的Server具有积极的意义。
        /// </remarks>
        public static Int32 TcpTimedWaitDelay { get { return GetReg("TcpTimedWaitDelay", 240); } set { SetReg("TcpTimedWaitDelay", value); } }

        /// <summary>控制系统尝试验证空闲连接是否仍然完好的频率。默认2小时</summary>
        /// <remarks>
        /// 如果该连接在一段时间内没有活动，那么系统会发送保持连接的信号，如果网络正常并且接收方是活动的，它就会响应。如果需要对丢失接收方的情况敏感，也就是说需要更快地发现是否丢失了接收方，请考虑减小该值。而如果长期不活动的空闲连接的出现次数较多，但丢失接收方的情况出现较少，那么可能需要增大该值以减少开销。
        /// </remarks>
        public static Int32 KeepAliveTime { get { return GetReg("KeepAliveTime", 2 * 60 * 60 * 1000); } set { SetReg("KeepAliveTime", value); } }

        /// <summary>半开连接数限制。默认0</summary>
        public static Int32 EnableConnectionRateLimiting { get { return GetReg("EnableConnectionRateLimiting", 0); } set { SetReg("EnableConnectionRateLimiting", value); } }

        private static Int32 GetReg(String key, Int32 defvalue = 0)
        {
            using (var rkey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\Tcpip\Parameters"))
            {
                //var sub = rkey.OpenSubKey(key);
                //if (sub == null) return defvalue;

                return rkey.GetValue(key).ToInt(defvalue);
            }
        }

        private static void SetReg(String key, Int32 value)
        {
            using (var rkey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\Tcpip\Parameters", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
            {
                //var sub = rkey.CreateSubKey(key);
                rkey.SetValue(key, value);
            }
        }
        #endregion

        #region 设置适配器信息
        static private ManagementObjectCollection GetInstances()
        {
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            return mc.GetInstances();
        }

        /// <summary>设置IP，默认掩码255.255.255.0</summary>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static Boolean SetIP(String ip, String mask = "255.255.255.0")
        {
            var moc = GetInstances();
            if (moc == null) return false;

            foreach (ManagementObject mo in moc)
            {
                if (!(Boolean)mo["IPEnabled"]) continue;

                // 设置IP和掩码
                var inPar = mo.GetMethodParameters("EnableStatic");
                inPar["IPAddress"] = new String[] { ip };
                inPar["SubnetMask"] = new String[] { mask };
                var outPar = mo.InvokeMethod("EnableStatic", inPar, null);
            }

            var ips = NetHelper.GetIPs().ToList();
            var pp = ips.Find(e => e.ToString() == ip);
            return pp != null;
        }

        /// <summary>设置默认网关</summary>
        /// <param name="address"></param>
        /// <returns></returns>
        static public Boolean SetGateway(String address)
        {
            var moc = GetInstances();
            if (moc == null) return false;

            foreach (ManagementObject mo in moc)
            {
                if (!(Boolean)mo["IPEnabled"]) continue;

                // 设置网关 
                var inPar = mo.GetMethodParameters("SetGateways");
                inPar["DefaultIPGateway"] = new String[] { address };
                var outPar = mo.InvokeMethod("SetGateways", inPar, null);
            }

            var ips = NetHelper.GetGateways().ToList();
            var pp = ips.Find(e => e.ToString() == address);
            return pp != null;
        }

        /// <summary>启动DHCP</summary>
        static public void StartDHCP()
        {
            var moc = GetInstances();
            if (moc == null) return;

            foreach (ManagementObject mo in moc)
            {
                if (!(Boolean)mo["IPEnabled"]) continue;

                mo.InvokeMethod("SetDNSServerSearchOrder", null);
                // 开启DHCP
                mo.InvokeMethod("EnableDHCP", null);
            }
        }
        #endregion
    }
}