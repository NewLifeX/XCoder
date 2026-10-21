using System;
using System.Collections.Generic;

namespace ALib.Can_Uds
{
    #region 诊断服务 SID

    /// <summary>
    /// UDS 诊断服务标识符（Service Identifier，ISO 14229-1）。
    /// 肯定响应 = SID + 0x40。
    /// </summary>
    public enum UdsService_e : byte
    {
        #region 诊断和通信管理

        /// <summary>诊断会话控制 DiagnosticSessionControl</summary>
        DiagnosticSessionControl = 0x10,

        /// <summary>ECU 复位 ECUReset</summary>
        ECUReset = 0x11,

        /// <summary>安全访问 SecurityAccess</summary>
        SecurityAccess = 0x27,

        /// <summary>通信控制 CommunicationControl</summary>
        CommunicationControl = 0x28,

        /// <summary>链路保活 TesterPresent</summary>
        TesterPresent = 0x3E,

        /// <summary>访问时间参数 AccessTimingParameter</summary>
        AccessTimingParameter = 0x83,

        /// <summary>安全数据传输 SecuredDataTransmission</summary>
        SecuredDataTransmission = 0x84,

        /// <summary>DTC 设置控制 ControlDTCSetting</summary>
        ControlDTCSetting = 0x85,

        /// <summary>事件响应 ResponseOnEvent</summary>
        ResponseOnEvent = 0x86,

        /// <summary>链路控制 LinkControl</summary>
        LinkControl = 0x87,

        #endregion

        #region 数据传输

        /// <summary>按标识符读数据 ReadDataByIdentifier</summary>
        ReadDataByIdentifier = 0x22,

        /// <summary>按地址读内存 ReadMemoryByAddress</summary>
        ReadMemoryByAddress = 0x23,

        /// <summary>读缩放数据 ReadScalingDataByIdentifier</summary>
        ReadScalingDataByIdentifier = 0x24,

        /// <summary>周期读数据 ReadDataByPeriodicIdentifier</summary>
        ReadDataByPeriodicIdentifier = 0x2A,

        /// <summary>动态定义数据标识符 DynamicallyDefineDataIdentifier</summary>
        DynamicallyDefineDataIdentifier = 0x2C,

        /// <summary>按标识符写数据 WriteDataByIdentifier</summary>
        WriteDataByIdentifier = 0x2E,

        /// <summary>按标识符控制输入输出 InputOutputControlByIdentifier</summary>
        InputOutputControlByIdentifier = 0x2F,

        /// <summary>按地址写内存 WriteMemoryByAddress</summary>
        WriteMemoryByAddress = 0x3D,

        #endregion

        #region 存储数据传输

        /// <summary>清除诊断信息 ClearDiagnosticInformation</summary>
        ClearDiagnosticInformation = 0x14,

        /// <summary>读取 DTC 信息 ReadDTCInformation</summary>
        ReadDTCInformation = 0x19,

        #endregion

        #region 例程控制

        /// <summary>例程控制 RoutineControl</summary>
        RoutineControl = 0x31,

        #endregion

        #region 上传下载

        /// <summary>请求下载 RequestDownload</summary>
        RequestDownload = 0x34,

        /// <summary>请求上传 RequestUpload</summary>
        RequestUpload = 0x35,

        /// <summary>传输数据 TransferData</summary>
        TransferData = 0x36,

        /// <summary>请求结束传输 RequestTransferExit</summary>
        RequestTransferExit = 0x37,

        #endregion

        #region 否定响应

        /// <summary>否定响应（固定值 0x7F）</summary>
        NegativeResponse = 0x7F,

        #endregion
    }

    #endregion

    #region 否定响应码 NRC

    /// <summary>
    /// 否定响应码 Negative Response Code（ISO 14229-1 附录 A）
    /// </summary>
    public enum UdsNrc_e : byte
    {
        /// <summary>一般拒绝 GeneralReject</summary>
        GeneralReject = 0x10,

        /// <summary>服务不支持 ServiceNotSupported</summary>
        ServiceNotSupported = 0x11,

        /// <summary>子功能不支持 SubFunctionNotSupported</summary>
        SubFunctionNotSupported = 0x12,

        /// <summary>报文长度或格式错误 IncorrectMessageLengthOrInvalidFormat</summary>
        IncorrectMessageLengthOrInvalidFormat = 0x13,

        /// <summary>响应过长 ResponseTooLong</summary>
        ResponseTooLong = 0x14,

        /// <summary>忙，请重发 BusyRepeatRequest</summary>
        BusyRepeatRequest = 0x21,

        /// <summary>条件不满足 ConditionsNotCorrect</summary>
        ConditionsNotCorrect = 0x22,

        /// <summary>请求顺序错误 RequestSequenceError</summary>
        RequestSequenceError = 0x24,

        /// <summary>请求超出范围 RequestOutOfRange</summary>
        RequestOutOfRange = 0x31,

        /// <summary>安全访问被拒绝 SecurityAccessDenied</summary>
        SecurityAccessDenied = 0x33,

        /// <summary>密钥无效 InvalidKey</summary>
        InvalidKey = 0x35,

        /// <summary>尝试次数超限 ExceedNumberOfAttempts</summary>
        ExceedNumberOfAttempts = 0x36,

        /// <summary>要求的延时未到 RequiredTimeDelayNotExpired</summary>
        RequiredTimeDelayNotExpired = 0x37,

        /// <summary>已正确接收，处理中（需继续等待）ResponsePending</summary>
        ResponsePending = 0x78,

        /// <summary>当前会话不支持该子功能 SubFunctionNotSupportedInActiveSession</summary>
        SubFunctionNotSupportedInActiveSession = 0x7E,

        /// <summary>当前会话不支持该服务 ServiceNotSupportedInActiveSession</summary>
        ServiceNotSupportedInActiveSession = 0x7F,

        /// <summary>升档/降档不支持（传输场景）</summary>
        UploadDownloadNotAccepted = 0x70,

        /// <summary>因数据传输中止 TransferDataSuspended</summary>
        TransferDataSuspended = 0x71,

        /// <summary>通用编程校验失败 GeneralProgrammingFailure</summary>
        GeneralProgrammingFailure = 0x72,

        /// <summary>错误的块序号 WrongBlockSequenceCounter</summary>
        WrongBlockSequenceCounter = 0x73,

        /// <summary>服务在给定会话中不受支持（扩展）</summary>
        ServiceNotSupportedInActiveSessionExt = 0x7F,

        /// <summary>车辆速度过快导致刷写被拒</summary>
        VehicleSpeedTooHigh = 0x88,

        /// <summary>发动机运转中导致刷写被拒</summary>
        EngineRunning = 0x89,
    }

    #endregion

    #region 会话与复位

    /// <summary>
    /// 诊断会话类型（0x10 子功能）
    /// </summary>
    public enum UdsSession_e : byte
    {
        /// <summary>默认会话 Default</summary>
        Default = 0x01,

        /// <summary>编程会话 Programming</summary>
        Programming = 0x02,

        /// <summary>扩展会话 Extended</summary>
        Extended = 0x03,

        /// <summary>安全系统会话 SafetySystem</summary>
        SafetySystem = 0x04,
    }

    /// <summary>
    /// ECU 复位类型（0x11 子功能）
    /// </summary>
    public enum UdsReset_e : byte
    {
        /// <summary>硬复位 HardReset</summary>
        HardReset = 0x01,

        /// <summary>点火开关复位 KeyOffOnReset</summary>
        KeyOffOnReset = 0x02,

        /// <summary>软复位 SoftReset</summary>
        SoftReset = 0x03,

        /// <summary>使能快速关机 EnableRapidPowerShutDown</summary>
        EnableRapidPowerShutDown = 0x04,

        /// <summary>禁用快速关机 DisableRapidPowerShutDown</summary>
        DisableRapidPowerShutDown = 0x05,
    }

    #endregion

    #region DTC 相关

    /// <summary>
    /// DTC 状态字节位定义（ISO 14229-1，8 位）
    /// </summary>
    [Flags]
    public enum UdsDtcStatus_e : byte
    {
        /// <summary>TestFailed 测试失败</summary>
        TestFailed = 0x01,

        /// <summary>TestFailedThisOperationCycle 本操作周期内失败</summary>
        TestFailedThisOperationCycle = 0x02,

        /// <summary>PendingDTC 待定 DTC</summary>
        PendingDTC = 0x04,

        /// <summary>ConfirmedDTC 已确认 DTC</summary>
        ConfirmedDTC = 0x08,

        /// <summary>TestNotCompletedSinceLastClear 自上次清除后未完成测试</summary>
        TestNotCompletedSinceLastClear = 0x10,

        /// <summary>TestFailedSinceLastClear 自上次清除后失败</summary>
        TestFailedSinceLastClear = 0x20,

        /// <summary>TestNotCompletedThisOperationCycle 本操作周期未完成测试</summary>
        TestNotCompletedThisOperationCycle = 0x40,

        /// <summary>WarningIndicatorRequested 请求报警指示</summary>
        WarningIndicatorRequested = 0x80,
    }

    /// <summary>
    /// 0x19 ReadDTCInformation 子功能
    /// </summary>
    public enum UdsReadDtcSubFunction_e : byte
    {
        /// <summary>按状态掩码报告 DTC 数量</summary>
        ReportNumberOfDTCByStatusMask = 0x01,

        /// <summary>按状态掩码报告 DTC 列表</summary>
        ReportDTCByStatusMask = 0x02,

        /// <summary>报告 DTC 快照记录标识</summary>
        ReportDTCSnapshotIdentification = 0x03,

        /// <summary>按 DTC 报告快照记录</summary>
        ReportDTCSnapshotRecordByDTCNumber = 0x04,

        /// <summary>报告 DTC 扩展数据记录标识</summary>
        ReportDTCExtDataRecordByDTCNumber = 0x06,

        /// <summary>报告支持的 DTC 数量</summary>
        ReportSupportedDTC = 0x0A,

        /// <summary>报告首个测试失败 DTC</summary>
        ReportFirstTestFailedDTC = 0x0B,

        /// <summary>报告首个已确认 DTC</summary>
        ReportFirstConfirmedDTC = 0x0C,

        /// <summary>报告应用内最旧 DTC</summary>
        ReportMostRecentTestFailedDTC = 0x0D,

        /// <summary>报告 ECU 镜像标识</summary>
        ReportDTCFaultDetectionCounter = 0x14,

        /// <summary>报告 DTC 状态可用性掩码</summary>
        ReportDTCStatusAvailabilityMask = 0x07,

        /// <summary>报告 DTC 抑制状态</summary>
        ReportDTCBySeverity = 0x09,
    }

    /// <summary>
    /// DTC 记录（3 字节 DTC + 1 字节状态）
    /// </summary>
    public class UdsDtc
    {
        /// <summary>DTC 三个字节（高→低）</summary>
        public byte[] Dtc = new byte[3];

        /// <summary>状态字节</summary>
        public UdsDtcStatus_e Status;

        /// <summary>DTC 数值（24bit）</summary>
        public UInt32 DtcValue => (UInt32)((Dtc[0] << 16) | (Dtc[1] << 8) | Dtc[2]);

        public UdsDtc() { }

        public UdsDtc(UInt32 dtc, UdsDtcStatus_e status = 0)
        {
            Dtc[0] = (byte)(dtc >> 16);
            Dtc[1] = (byte)(dtc >> 8);
            Dtc[2] = (byte)dtc;
            Status = status;
        }

        public override String ToString() => $"DTC=0x{DtcValue:X6} Status=0x{(byte)Status:X2}";
    }

    #endregion

    #region 例程与安全访问

    /// <summary>
    /// 0x31 RoutineControl 子功能
    /// </summary>
    public enum UdsRoutineControl_e : byte
    {
        /// <summary>启动例程</summary>
        StartRoutine = 0x01,

        /// <summary>停止例程</summary>
        StopRoutine = 0x02,

        /// <summary>请求例程结果</summary>
        RequestRoutineResults = 0x03,
    }

    /// <summary>
    /// 0x27 SecurityAccess 子功能。
    /// 奇数 = 请求种子，偶数 = 发送密钥，两两成对。
    /// </summary>
    public enum UdsSecurityLevel_e : byte
    {
        /// <summary>请求种子（等级 1）</summary>
        RequestSeedLevel1 = 0x01,

        /// <summary>发送密钥（等级 1）</summary>
        SendKeyLevel1 = 0x02,

        /// <summary>请求种子（等级 3）</summary>
        RequestSeedLevel3 = 0x03,

        /// <summary>发送密钥（等级 3）</summary>
        SendKeyLevel3 = 0x04,

        /// <summary>请求种子（等级 5）</summary>
        RequestSeedLevel5 = 0x05,

        /// <summary>发送密钥（等级 5）</summary>
        SendKeyLevel5 = 0x06,

        /// <summary>请求种子（等级 7）</summary>
        RequestSeedLevel7 = 0x07,

        /// <summary>发送密钥（等级 7）</summary>
        SendKeyLevel7 = 0x08,

        /// <summary>请求种子（等级 9）</summary>
        RequestSeedLevel9 = 0x09,

        /// <summary>发送密钥（等级 9）</summary>
        SendKeyLevel9 = 0x0A,

        /// <summary>请求种子（等级 11）</summary>
        RequestSeedLevel11 = 0x0B,

        /// <summary>发送密钥（等级 11）</summary>
        SendKeyLevel11 = 0x0C,

        /// <summary>请求种子（等级 13）</summary>
        RequestSeedLevel13 = 0x0D,

        /// <summary>发送密钥（等级 13）</summary>
        SendKeyLevel13 = 0x0E,

        /// <summary>请求种子（等级 15）</summary>
        RequestSeedLevel15 = 0x0F,

        /// <summary>发送密钥（等级 15）</summary>
        SendKeyLevel15 = 0x10,
    }

    #endregion

    #region 通信与 DTC 控制

    /// <summary>
    /// 0x28 CommunicationControl 子功能
    /// </summary>
    public enum UdsCommunicationControl_e : byte
    {
        /// <summary>使能接收与发送</summary>
        EnableRxAndTx = 0x00,

        /// <summary>使能接收，禁用发送</summary>
        EnableRxAndDisableTx = 0x01,

        /// <summary>禁用接收，使能发送</summary>
        DisableRxAndEnableTx = 0x02,

        /// <summary>禁用接收与发送</summary>
        DisableRxAndTx = 0x03,

        /// <summary>使能接收并禁用发送（带增强地址）</summary>
        EnableRxAndDisableTxWithEnhancedAddressInformation = 0x04,

        /// <summary>使能接收与发送（带增强地址）</summary>
        EnableRxAndTxWithEnhancedAddressInformation = 0x05,
    }

    /// <summary>
    /// 通信类型（0x28 byte2）
    /// </summary>
    public enum UdsCommunicationType_e : byte
    {
        /// <summary>普通应用报文</summary>
        NormalCommunicationMessages = 0x01,

        /// <summary>网络管理报文</summary>
        NetworkManagementCommunicationMessages = 0x02,

        /// <summary>普通 + 网络管理</summary>
        NormalAndNetworkManagementMessages = 0x03,
    }

    /// <summary>
    /// 0x85 ControlDTCSetting 子功能
    /// </summary>
    public enum UdsDtcSetting_e : byte
    {
        /// <summary>打开 DTC 设置</summary>
        On = 0x01,

        /// <summary>关闭 DTC 设置</summary>
        Off = 0x02,
    }

    /// <summary>
    /// 0x2F InputOutputControlByIdentifier 控制参数
    /// </summary>
    public enum UdsIoControl_e : byte
    {
        /// <summary>返回控制到 ECU</summary>
        ReturnControlToECU = 0x00,

        /// <summary>复位为默认值</summary>
        ResetToDefault = 0x01,

        /// <summary>冻结当前状态</summary>
        FreezeCurrentState = 0x02,

        /// <summary>短时调整为指定值</summary>
        ShortTermAdjustment = 0x03,
    }

    #endregion

    #region 常用 DID

    /// <summary>
    /// 常用数据标识符 DID（Data Identifier），按 ISO 14229-1 附录 C.
    /// 各 OEM 会在 0xF000 以上自定义。
    /// </summary>
    public static class UdsDid
    {
        #region 通用

        /// <summary>车辆识别号 VIN（17 字节 ASCII）</summary>
        public const UInt16 Vin = 0xF190;

        /// <summary>系统供应商标识</summary>
        public const UInt16 SystemSupplierIdentifier = 0xF18A;

        /// <summary>ECU 软件版本</summary>
        public const UInt16 ECUSoftwareNumber = 0xF181;

        /// <summary>ECU 软件版本号</summary>
        public const UInt16 ECUSoftwareVersionNumber = 0xF182;

        /// <summary>ECU 硬件版本号</summary>
        public const UInt16 ECUHardwareVersionNumber = 0xF192;

        /// <summary>应用软件标识</summary>
        public const UInt16 ApplicationSoftwareIdentification = 0xF195;

        /// <summary>应用数据标识</summary>
        public const UInt16 ApplicationDataIdentification = 0xF196;

        /// <summary>ECU 序列号</summary>
        public const UInt16 ECUSerialNumber = 0xF18C;

        /// <summary>诊断会话的周期数据标识</summary>
        public const UInt16 BootSoftwareIdentification = 0xF180;

        #endregion

        #region 刷写相关

        /// <summary>指纹（写指纹，厂家自定义长度）</summary>
        public const UInt16 Fingerprint = 0xF15A;

        /// <summary>编程日期</summary>
        public const UInt16 ProgrammingDate = 0xF199;

        /// <summary>激活的刷写预编程条件</summary>
        public const UInt16 ActiveDiagnosticSession = 0xF186;

        /// <summary>车辆制造商备件号</summary>
        public const UInt16 VehicleManufacturerSparePartNumber = 0xF187;

        /// <summary>车辆制造商 ECU 软件号</summary>
        public const UInt16 VehicleManufacturerECUSoftwareNumber = 0xF188;

        /// <summary>车辆制造商 ECU 软件版本号</summary>
        public const UInt16 VehicleManufacturerECUSoftwareVersionNumber = 0xF189;

        #endregion

        #region 地址与标识

        /// <summary>ECU 装配日期</summary>
        public const UInt16 ECUManufacturingDate = 0xF18B;

        /// <summary>诊断地址</summary>
        public const UInt16 DiagnosticAddress = 0xF18E;

        /// <summary>VIN 消息计数器</summary>
        public const UInt16 VINMessageCount = 0xF191;

        #endregion
    }

    #endregion

    #region 常用的服务/参数常量

    /// <summary>
    /// UDS 协议固定常量
    /// </summary>
    public static class UdsConstants
    {
        /// <summary>肯定响应偏移：响应 SID = 请求 SID + 0x40</summary>
        public const byte PositiveResponseOffset = 0x40;

        /// <summary>否定响应 SID</summary>
        public const byte NegativeResponseSid = 0x7F;

        /// <summary>0x3E 抑制肯定响应位（子功能 bit7）</summary>
        public const byte SuppressPositiveResponseMask = 0x80;

        /// <summary>诊断 ID 基址：物理请求 0x7E0</summary>
        public const UInt32 PhysRequestBase = 0x7E0;

        /// <summary>诊断 ID 基址：物理响应 0x7E8</summary>
        public const UInt32 PhysResponseBase = 0x7E8;

        /// <summary>功能寻址 ID 0x7DF</summary>
        public const UInt32 FunctionId = 0x7DF;

        /// <summary>默认 P2 服务器超时（毫秒）</summary>
        public const int DefaultP2Server = 50;

        /// <summary>默认 P2* 服务器超时（毫秒）</summary>
        public const int DefaultP2StarServer = 5000;

        /// <summary>默认 S3 服务器会话超时（毫秒）</summary>
        public const int DefaultS3Server = 5000;
    }

    #endregion
}
