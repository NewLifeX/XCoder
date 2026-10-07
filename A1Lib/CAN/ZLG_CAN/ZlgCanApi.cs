using System;
using System.Runtime.InteropServices;

namespace ALib.ZLG_CAN
{
    /// <summary>
    /// ZLG（广州致远）CAN 接口卡驱动 zlgcan.dll 的 P/Invoke 声明。
    /// 对应官方 SDK 头文件 zlgcan.h / canframe.h / typedef.h，x64 版本。
    /// 注意：本类只声明，不做任何托管包装；所有调用均由 <see cref="ZlgCanPort"/> 完成。
    /// </summary>
    public static class ZlgCanApi
    {
        #region 设备类型

        /// <summary>USBCAN-I 单通道。与 USBCAN2 共用 USBCAN.dll，枚举同一设备会重复，需去重</summary>
        public const UInt32 ZCAN_USBCAN1 = 3;

        /// <summary>USBCAN-II 双通道。与 USBCAN1 共用 USBCAN.dll，枚举同一设备会重复，需去重</summary>
        public const UInt32 ZCAN_USBCAN2 = 4;

        /// <summary>USBCAN-II 单通道（E-U 系列）</summary>
        public const UInt32 ZCAN_USBCAN_E_U = 20;

        /// <summary>USBCAN-II 双通道（2E-U 系列）</summary>
        public const UInt32 ZCAN_USBCAN_2E_U = 21;

        /// <summary>USBCANFD-200U</summary>
        public const UInt32 ZCAN_USBCANFD_200U = 41;

        /// <summary>USBCANFD-100U</summary>
        public const UInt32 ZCAN_USBCANFD_100U = 42;

        /// <summary>USBCANFD-MINI</summary>
        public const UInt32 ZCAN_USBCANFD_MINI = 43;

        /// <summary>虚拟设备，用于无硬件时自测</summary>
        public const UInt32 ZCAN_VIRTUAL_DEVICE = 99;

        #endregion

        #region 常量

        /// <summary>无效设备句柄</summary>
        public static readonly IntPtr INVALID_DEVICE_HANDLE = IntPtr.Zero;

        /// <summary>无效通道句柄</summary>
        public static readonly IntPtr INVALID_CHANNEL_HANDLE = IntPtr.Zero;

        /// <summary>CAN 帧</summary>
        public const byte TYPE_CAN = 0;

        /// <summary>CANFD 帧</summary>
        public const byte TYPE_CANFD = 1;

        /// <summary>发送类型：正常发送</summary>
        public const UInt32 TRANSMIT_NORMAL = 0;

        /// <summary>标准帧最大数据长度</summary>
        public const Int32 CAN_MAX_DLEN = 8;

        /// <summary>扩展帧格式标志（bit31）</summary>
        public const UInt32 CAN_EFF_FLAG = 0x80000000U;

        /// <summary>远程帧标志（bit30）</summary>
        public const UInt32 CAN_RTR_FLAG = 0x40000000U;

        /// <summary>错误帧标志（bit29）</summary>
        public const UInt32 CAN_ERR_FLAG = 0x20000000U;

        /// <summary>ID 掩码</summary>
        public const UInt32 CAN_ID_FLAG = 0x1FFFFFFFU;

        /// <summary>调用成功</summary>
        public const UInt32 STATUS_OK = 1;

        #endregion

        #region 结构体

        /// <summary>CAN 帧（对应 can_frame）</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct can_frame
        {
            /// <summary>32 位 ID，含 EFF/RTR/ERR 标志，见 MAKE_CAN_ID</summary>
            public UInt32 can_id;

            /// <summary>数据长度 0~8</summary>
            public byte can_dlc;

            /// <summary>填充</summary>
            public byte __pad;

            /// <summary>保留</summary>
            public byte __res0;

            /// <summary>保留</summary>
            public byte __res1;

            /// <summary>数据区，固定 8 字节</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CAN_MAX_DLEN)]
            public byte[] data;

            /// <summary>组装 32 位 ID（含标志位）</summary>
            /// <param name="id">CAN ID（不高于 29 位）</param>
            /// <param name="eff">是否扩展帧</param>
            /// <param name="rtr">是否远程帧</param>
            /// <param name="err">是否错误帧</param>
            /// <returns></returns>
            public static UInt32 MakeCanId(UInt32 id, bool eff, bool rtr, bool err)
            {
                var v = id & CAN_ID_FLAG;
                if (eff) v |= CAN_EFF_FLAG;
                if (rtr) v |= CAN_RTR_FLAG;
                if (err) v |= CAN_ERR_FLAG;
                return v;
            }

            /// <summary>是否扩展帧</summary>
            public bool IsExtend => (can_id & CAN_EFF_FLAG) != 0;

            /// <summary>是否远程帧</summary>
            public bool IsRemote => (can_id & CAN_RTR_FLAG) != 0;

            /// <summary>是否错误帧</summary>
            public bool IsError => (can_id & CAN_ERR_FLAG) != 0;

            /// <summary>纯 CAN ID（去除标志位）</summary>
            public UInt32 PureId => can_id & CAN_ID_FLAG;
        }

        /// <summary>CAN 通道初始化配置（对应 ZCAN_CHANNEL_INIT_CONFIG）</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ZCAN_CHANNEL_INIT_CONFIG
        {
            /// <summary>帧类型，TYPE_CAN 或 TYPE_CANFD</summary>
            public UInt32 can_type;

            // ---- union { struct can; struct canfd; } ----
            public UInt32 acc_code;
            public UInt32 acc_mask;
            public UInt32 reserved;

            /// <summary>滤波方式，1 为单滤波</summary>
            public byte filter;

            /// <summary>波特率定时器 0</summary>
            public byte timing0;

            /// <summary>波特率定时器 1</summary>
            public byte timing1;

            /// <summary>模式，0 为正常模式</summary>
            public byte mode;

            /// <summary>CANFD 仲裁域波特率（can_type = TYPE_CANFD 时使用）</summary>
            public UInt32 abit_timing;

            /// <summary>CANFD 数据域波特率（can_type = TYPE_CANFD 时使用）</summary>
            public UInt32 dbit_timing;

            /// <summary>CANFD 波特率分频</summary>
            public UInt32 brp;

            /// <summary>CANFD 填充</summary>
            public UInt16 pad;

            /// <summary>CANFD 保留</summary>
            public UInt32 reserved2;
        }

        /// <summary>设备信息（对应 ZCAN_DEVICE_INFO）</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ZCAN_DEVICE_INFO
        {
            /// <summary>硬件版本</summary>
            public UInt16 hw_Version;

            /// <summary>固件版本</summary>
            public UInt16 fw_Version;

            /// <summary>驱动版本</summary>
            public UInt16 dr_Version;

            /// <summary>动态库版本</summary>
            public UInt16 in_Version;

            /// <summary>中断号</summary>
            public UInt16 irq_Num;

            /// <summary>CAN 通道数</summary>
            public byte can_Num;

            /// <summary>序列号</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] str_Serial_Num;

            /// <summary>硬件类型</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] str_hw_Type;

            /// <summary>保留</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public UInt16[] reserved;
        }

        /// <summary>发送数据（对应 ZCAN_Transmit_Data）</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ZCAN_Transmit_Data
        {
            /// <summary>CAN 帧</summary>
            public can_frame frame;

            /// <summary>发送类型：0 正常发送、1 单次发送、2 自发自收</summary>
            public UInt32 transmit_type;
        }

        /// <summary>接收数据（对应 ZCAN_Receive_Data）</summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ZCAN_Receive_Data
        {
            /// <summary>CAN 帧</summary>
            public can_frame frame;

            /// <summary>时间戳，单位微秒</summary>
            public UInt64 timestamp;
        }

        #endregion

        #region 设备与通道

        /// <summary>打开设备</summary>
        /// <param name="device_type">设备类型，见 ZCAN_USBCAN* 常量</param>
        /// <param name="device_index">同类型设备的索引，从 0 开始</param>
        /// <param name="reserved">保留，传 0</param>
        /// <returns>设备句柄，失败返回 IntPtr.Zero</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ZCAN_OpenDevice(UInt32 device_type, UInt32 device_index, UInt32 reserved);

        /// <summary>按自定义名称打开设备</summary>
        /// <param name="device_type">设备类型</param>
        /// <param name="name">设备名称</param>
        /// <returns>设备句柄，失败返回 IntPtr.Zero</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ZCAN_OpenDeviceByName(UInt32 device_type, [MarshalAs(UnmanagedType.LPStr)] String name);

        /// <summary>关闭设备</summary>
        /// <param name="device_handle">设备句柄</param>
        /// <returns>STATUS_OK 表示成功</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_CloseDevice(IntPtr device_handle);

        /// <summary>获取设备信息</summary>
        /// <param name="device_handle">设备句柄</param>
        /// <param name="pInfo">输出设备信息</param>
        /// <returns>STATUS_OK 表示成功</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_GetDeviceInf(IntPtr device_handle, ref ZCAN_DEVICE_INFO pInfo);

        /// <summary>设备是否在线</summary>
        /// <param name="device_handle">设备句柄</param>
        /// <returns>非 0 表示在线</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_IsDeviceOnLine(IntPtr device_handle);

        /// <summary>初始化 CAN 通道</summary>
        /// <param name="device_handle">设备句柄</param>
        /// <param name="can_index">通道索引，从 0 开始</param>
        /// <param name="pInitConfig">通道初始化配置</param>
        /// <returns>通道句柄，失败返回 IntPtr.Zero</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ZCAN_InitCAN(IntPtr device_handle, UInt32 can_index, ref ZCAN_CHANNEL_INIT_CONFIG pInitConfig);

        /// <summary>启动 CAN 通道</summary>
        /// <param name="channel_handle">通道句柄</param>
        /// <returns>STATUS_OK 表示成功</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_StartCAN(IntPtr channel_handle);

        /// <summary>复位 CAN 通道</summary>
        /// <param name="channel_handle">通道句柄</param>
        /// <returns>STATUS_OK 表示成功</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_ResetCAN(IntPtr channel_handle);

        /// <summary>清空接收缓冲</summary>
        /// <param name="channel_handle">通道句柄</param>
        /// <returns>STATUS_OK 表示成功</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_ClearBuffer(IntPtr channel_handle);

        /// <summary>获取接收缓冲区中的帧数</summary>
        /// <param name="channel_handle">通道句柄</param>
        /// <param name="type">TYPE_CAN 或 TYPE_CANFD</param>
        /// <returns>帧数</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_GetReceiveNum(IntPtr channel_handle, byte type);

        #endregion

        #region 收发

        /// <summary>发送 CAN 帧</summary>
        /// <param name="channel_handle">通道句柄</param>
        /// <param name="pTransmit">待发送数据指针（指向首个元素的非托管地址，由调用方 fixed 后传入）</param>
        /// <param name="len">帧数</param>
        /// <returns>实际发送成功的帧数</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_Transmit(IntPtr channel_handle, IntPtr pTransmit, UInt32 len);

        /// <summary>接收 CAN 帧</summary>
        /// <param name="channel_handle">通道句柄</param>
        /// <param name="pReceive">接收缓冲指针（指向首个元素的非托管地址，由调用方 fixed 后传入）</param>
        /// <param name="len">缓冲可容纳的帧数</param>
        /// <param name="wait_time">等待超时（毫秒），-1 表示直到收到数据才返回</param>
        /// <returns>实际接收到的帧数</returns>
        [DllImport("zlgcan.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ZCAN_Receive(IntPtr channel_handle, IntPtr pReceive, UInt32 len, Int32 wait_time);

        #endregion

        #region 波特率

        // 波特率寄存器值取自官方应用手册《CAN 波特率计算》，timing0 为 BTR0、timing1 为 BTR1。
        // 参考时钟 36MHz（USBCAN 系列与 PCAN 一致）。

        /// <summary>按波特率取 (timing0, timing1)</summary>
        /// <param name="baudrate">波特率索引，与 CAN.Baudrate_e 取值一致</param>
        /// <param name="timing0">输出 timing0</param>
        /// <param name="timing1">输出 timing1</param>
        /// <returns>是否支持该波特率</returns>
        public static Boolean TryGetTiming(Int32 baudrate, out byte timing0, out byte timing1)
        {
            switch (baudrate)
            {
                case 0: timing0 = 0x00; timing1 = 0x14; return true;   // 1000K
                case 1: timing0 = 0x00; timing1 = 0x1C; return true;   // 500K
                case 2: timing0 = 0x01; timing1 = 0x1C; return true;   // 250K
                case 3: timing0 = 0x81; timing1 = 0xFA; return true;   // 200K
                case 4: timing0 = 0x03; timing1 = 0x1C; return true;   // 125K
                case 5: timing0 = 0x04; timing1 = 0x1C; return true;   // 100K
                default: timing0 = 0x01; timing1 = 0x1C; return false;  // 回退 250K
            }
        }

        #endregion
    }
}
