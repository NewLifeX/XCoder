using ALib.Can;
using ALib.CAN;
using NewLife;
using NewLife.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ALib.CX_CAN
{
    public static class CxControlCAN
    {
        /*------------兼容ZLG的数据类型---------------------------------*/

        //1.ZLGCAN系列接口卡信息的数据类型。
        //public struct VCI_BOARD_INFO 
        //{ 
        //    public UInt16 hw_Version;
        //    public UInt16 fw_Version;
        //    public UInt16 dr_Version;
        //    public UInt16 in_Version;
        //    public UInt16 irq_Num;
        //    public byte   can_Num;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst=20)] public byte []str_Serial_Num;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        //    public byte[] str_hw_Type;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        //    public byte[] Reserved;
        //}

        //以下为简易定义与调用方式，在项目属性->生成->勾选使用不安全代码即可
        unsafe public struct VCI_BOARD_INFO  //使用不安全代码
        {
            public UInt16 hw_Version;
            public UInt16 fw_Version;
            public UInt16 dr_Version;
            public UInt16 in_Version;
            public UInt16 irq_Num;
            public byte can_Num;

            public fixed byte str_Serial_Num[20];
            public fixed byte str_hw_Type[40];
            public fixed byte Reserved[8];
        }

        /////////////////////////////////////////////////////
        //2.定义CAN信息帧的数据类型。
        unsafe public struct VCI_CAN_OBJ  //使用不安全代码
        {
            public uint ID;               //标准帧11bit 。 canopen  高4bit 消息类型，低7bit节点地址
            public uint TimeStamp;        //时间标识
            public byte TimeFlag;         //是否使用时间标识
            public byte SendType;         //发送标志。保留，未用
            public byte RemoteFlag;       //是否是远程帧。canopen 用 0
            public byte ExternFlag;       //是否是扩展帧
            public byte DataLen;          //数据长度。 DLC   4bit
            public fixed byte Data[8];    //数据
            public fixed byte Reserved[3];//保留位


            /// <summary>fixed byte Data 转托管数组</summary>
            /// <returns></returns>
            public byte[] GetData()
            {
                var bs = new byte[8];

                unsafe
                {
                    for (int i = 0; i < 8; i++)
                    {
                        bs[i] = Data[i];
                    }
                }

                return bs;
            }

            public CanPacket ToCanPacket()
            {
                var pkt = new CanPacket();

                pkt.COB_ID = (int)ID;
                pkt.RTR = RemoteFlag > 0;
                if (DataLen > 0)
                {
                    pkt.Data = new byte[DataLen];
                    unsafe
                    {
                        for (int i = 0; i < DataLen; i++)
                        {
                            pkt.Data[i] = Data[i];
                        }
                    }
                }

                return pkt;
            }

            public static VCI_CAN_OBJ FromCanPacket(CanPacket pkt)
            {
                var obj = new VCI_CAN_OBJ();
                obj.ID = (uint)pkt.COB_ID;
                obj.RemoteFlag = (byte)(pkt.RTR ? 1 : 0);
                obj.ExternFlag = 0;
                obj.DataLen = (byte)pkt.DLC;
                if (pkt.DLC > 0)
                {
                    unsafe
                    {
                        for (int i = 0; i < pkt.DLC; i++)
                        {
                            obj.Data[i] = pkt.Data[i];
                        }
                    }
                }
                return obj;
            }

            /// <summary>VCI_CAN_OBJ -> CanMsg</summary>
            /// <returns></returns>
            public CanMsg ToCanMsg()
            {
                var len = DataLen > 8 ? 8 : DataLen;
                var data = new byte[len];
                unsafe
                {
                    for (int i = 0; i < len; i++)
                    {
                        data[i] = Data[i];
                    }
                }

                var pkt = new CanMsg((Int32)ID, data, RemoteFlag > 0, ExternFlag > 0);
                // ZLG 时间标识单位为 0.1ms，仅在使用时间标识时有效
                if (TimeFlag > 0) pkt.TimeStamp = TimeSpan.FromMilliseconds(TimeStamp / 10.0);
                return pkt;
            }


        }

        /// <summary>格式化字符串</summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static String MsgToString(VCI_CAN_OBJ msg)
        {
            var sb = Pool.StringBuilder.Get();

            var tsp = TimeSpan.FromMilliseconds(msg.TimeStamp / 10);
            sb.Append(tsp.ToString("hh\\:mm\\:ss\\.fff"));

            // var T = msg.TimeFlag > 0 ? " T " : "NT ";
            // sb.Append(T);

            if (msg.ExternFlag > 0)
            {
                var id = msg.ID.GetBytes(false).ReadBytes(0, 4).ToHex();
                sb.Append(" " + id + " ");
            }
            else
            {
                var id = msg.ID.GetBytes(false).ReadBytes(2, 2).ToHex();
                sb.Append("  " + id + " ");
            }

            var R = msg.RemoteFlag > 0 ? "R " : "  ";
            sb.Append(R);

            var E = msg.ExternFlag > 0 ? "E " : "  ";
            sb.Append(E);

            sb.Append("[" + msg.DataLen + "] ");
            sb.Append(" " + msg.GetData().ReadBytes(0, msg.DataLen).ToHex(" "));
            return sb.Return(true);
        }


        //3.定义初始化CAN的数据类型
        public struct VCI_INIT_CONFIG
        {
            public UInt32 AccCode;
            public UInt32 AccMask;
            public UInt32 Reserved;
            public byte Filter;   //0或1接收所有帧。2标准帧滤波，3是扩展帧滤波。
            public byte Timing0;  //波特率参数，具体配置，请查看二次开发库函数说明书。
            public byte Timing1;
            public byte Mode;     //模式，0表示正常模式，1表示只听模式,2自测模式
        }

        /*------------其他数据结构描述---------------------------------*/
        //4.USB-CAN总线适配器板卡信息的数据类型1，该类型为VCI_FindUsbDevice函数的返回参数。
        public struct VCI_BOARD_INFO1
        {
            public UInt16 hw_Version;
            public UInt16 fw_Version;
            public UInt16 dr_Version;
            public UInt16 in_Version;
            public UInt16 irq_Num;
            public byte can_Num;
            public byte Reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] str_Serial_Num;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] str_hw_Type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] str_Usb_Serial;
        }

        /*------------数据结构描述完成---------------------------------*/

        public struct CHGDESIPANDPORT
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] szpwd;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] szdesip;
            public Int32 desport;

            public void Init()
            {
                szpwd = new byte[10];
                szdesip = new byte[20];
            }
        }


        const int DEV_USBCAN = 3;
        const int DEV_USBCAN2 = 4;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="DeviceInd"></param>
        /// <param name="Reserved"></param>
        /// <returns></returns>
        /*------------兼容ZLG的函数描述---------------------------------*/
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);

        /*------------其他函数描述---------------------------------*/

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ConnectDevice(UInt32 DevType, UInt32 DevIndex);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_UsbDeviceReset(UInt32 DevType, UInt32 DevIndex, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_FindUsbDevice2(ref VCI_BOARD_INFO pInfo);
    }
}
