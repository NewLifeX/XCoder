using ALib.ComPort;
using ALib.UsbHub;
using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib
{
    /// <summary>
    /// 京东条码商城的扫码枪，命令模式读码封装，注意不是刘强东的京东
    /// 型号 U100/U200 等
    /// 参数：虚拟串口模式 115200-8n1，命令模式
    /// </summary>
    public class QrScan : Com_WithUnpack
    {
        public QrScan() { }

        public QrScan(String comName) : base(comName) { }

        /// <summary>读 二维码/条码</summary>
        /// <returns>返回条码</returns>
        public String ReadQr()
        {
            RxCache.Clear();
            RxMsgClear();

            // 发送识读指令
            // 7E 00 08 01 00 02 01 AB CD
            var cmd = new byte[] { 0x7E, 0x00, 0x08, 0x01, 0x00, 0x02, 0x01, 0xAB, 0xCD };
            Send(cmd);

            var tend = DateTime.Now.AddSeconds(2);
            while (true)
            {
                if (DateTime.Now > tend) break;

                Task.Delay(50).Wait();
                if (RxMsg.Count() > 0)
                {
                    return Encoding.ASCII.GetString(RxMsg.Dequeue());
                }
            }

            return null;
        }

        /// <summary>解包</summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public override byte[] TryUnpack(CircularQueue<byte> queue)
        {
            // 02 00 00 01 00 33 31
            var reply = new byte[] { 0x02, 0x00, 0x00, 0x01, 0x00, 0x33, 0x31 };

            while (true)
            {
                var allrx = queue.ToArray();
                // var idx = allrx.IndexOf(reply);
                // var idx = MemoryExtensions.IndexOf(allrx.AsSpan(), reply.AsSpan());
                var idx = MemoryExtensions.LastIndexOf(allrx.AsSpan(), reply.AsSpan());

                if (idx < 0) return null;
                if (idx > 0)
                {
                    queue.Seek(idx);
                    continue;
                }

                if (allrx[allrx.Length - 1] == 0x0d)
                {
                    queue.Seek(allrx.Length);
                    return allrx.ReadBytes(reply.Length, -1);
                }

                return null;
            }
        }
    }
}
