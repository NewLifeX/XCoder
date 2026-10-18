using System;

namespace ALib.Can_SAEJ1939
{
    /// <summary>
    /// J1939 常用/固定结构 PGN 常量（18bit 值，已含数据页位）。
    /// 按 PDU1 / PDU2 分组，便于查找。
    /// </summary>
    public static class J1939Pgn
    {
        #region 网络管理 / 传输层（PDU1，0xEA/0xEB/0xEC 段）

        /// <summary>请求 Request，PDU1</summary>
        public const UInt32 Request = 0x00EA00;         // 59904

        /// <summary>应答 Acknowledgement，PDU1</summary>
        public const UInt32 Acknowledge = 0x00E800;     // 59392

        /// <summary>地址声明 Address Claimed / 无法声明，PDU2</summary>
        public const UInt32 AddressClaimed = 0x00EE00;  // 60928

        /// <summary>TP.CM 连接管理，PDU2</summary>
        public const UInt32 TpCm = 0x00EC00;            // 60416

        /// <summary>TP.DT 数据传输，PDU2</summary>
        public const UInt32 TpDt = 0x00EB00;            // 60160

        /// <summary>TP.CM 连接管理（定时参数/唯一编号等扩展），PDU1</summary>
        public const UInt32 TpCmLimits = 0x00C800;      // 51200

        #endregion

        #region 诊断消息（PDU2）

        /// <summary>DM1 活动故障码</summary>
        public const UInt32 DM1 = 0x00FECA;             // 65226

        /// <summary>DM2 历史故障码</summary>
        public const UInt32 DM2 = 0x00FECB;             // 65227

        /// <summary>DM3 历史故障码清除（数据区为空）</summary>
        public const UInt32 DM3 = 0x00FECC;             // 65228

        /// <summary>DM11 清除活动故障码</summary>
        public const UInt32 DM11 = 0x00FED3;            // 65235

        /// <summary>DM13 停止/开始广播</summary>
        public const UInt32 DM13 = 0x00FED5;            // 65237

        #endregion

        #region 应用层 PG（标准定义布局，常用）

        /// <summary>发动机转速 / 控制器1（Electronic Engine Controller 1）</summary>
        public const UInt32 EEC1 = 0x00F004;            // 61444

        /// <summary>整车总里程 / 小时计（Vehicle Distance）</summary>
        public const UInt32 VDS = 0x00FEE0;             // 65248

        /// <summary>发动机温度1（Engine Temperature 1）</summary>
        public const UInt32 ET1 = 0x00FEEE;             // 65262

        /// <summary>发动机流体液位/压力（Engine Fluid Level/Pressure 1）</summary>
        public const UInt32 EFLP1 = 0x00FEEF;           // 65263

        /// <summary>发动机转速/车速等（EC2，含车速）</summary>
        public const UInt32 CCVS = 0x00FEF1;            // 65265

        /// <summary>专有 A 报文（PDU1 点对点，语义厂家自定义）</summary>
        public const UInt32 PropA = 0x00EF00;           // 61184

        #endregion
    }

    /// <summary>
    /// J1939 全局/特定地址常量
    /// </summary>
    public static class J1939Addr
    {
        /// <summary>全局广播地址</summary>
        public const byte Global = 0xFF;

        /// <summary>空地址（未声明）</summary>
        public const byte Null = 0xFE;
    }
}
