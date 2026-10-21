using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALib.Can_Uds
{
    #region 诊断会话上下文

    /// <summary>
    /// 一次诊断会话的上下文：当前会话类型、是否已解锁安全等级、DTC 是否允许设置等。
    /// 由 <see cref="UdsClient"/> 维护，服务层据其判断条件是否满足。
    /// </summary>
    public class UdsSession
    {
        /// <summary>当前会话类型</summary>
        public UdsSession_e SessionType = UdsSession_e.Default;

        /// <summary>是否处于编程会话</summary>
        public Boolean IsProgramming => SessionType == UdsSession_e.Programming;

        /// <summary>是否处于扩展会话</summary>
        public Boolean IsExtended => SessionType == UdsSession_e.Extended;

        /// <summary>是否处于默认会话</summary>
        public Boolean IsDefault => SessionType == UdsSession_e.Default;

        /// <summary>已解锁的安全等级（0 表示未解锁）</summary>
        public Byte SecurityLevel = 0;

        /// <summary>安全访问密钥算法，由使用者设置（种子 → 密钥）</summary>
        public Func<Byte[], Byte[]> KeyAlgorithm;

        /// <summary>DTC 设置是否使能</summary>
        public Boolean DtcSettingEnabled = true;

        /// <summary>通信控制状态（普通报文是否可收发）</summary>
        public Boolean CommunicationEnabled = true;

        /// <summary>最近一次活动时刻，用于 S3 超时判定</summary>
        public DateTime LastActive = DateTime.Now;

        /// <summary>会话切换时重置部分状态</summary>
        public void SwitchTo(UdsSession_e session)
        {
            SessionType = session;
            LastActive = DateTime.Now;

            // 回到默认会话或换会话时，安全状态失效
            SecurityLevel = 0;
        }
    }

    #endregion

    #region 请求/响应报文体

    /// <summary>
    /// UDS 请求报文封装。用于构造各诊断服务的请求数据（不含 ISO-TP 与 CAN ID）。
    /// </summary>
    public class UdsRequest
    {
        /// <summary>服务标识 SID</summary>
        public UdsService_e Service;

        /// <summary>子功能（部分服务无）</summary>
        public Byte SubFunction;

        /// <summary>是否抑制肯定响应（子功能 bit7）</summary>
        public Boolean SuppressResponse;

        /// <summary>数据标识符 DID（0x22/0x2E/0x2F 等）</summary>
        public UInt16 Did;

        /// <summary>参数负载</summary>
        public Byte[] Data = Array.Empty<Byte>();

        /// <summary>完整原始字节（构造完毕后由 ToArray 生成）</summary>
        public Byte[] Raw;

        public UdsRequest() { }

        public UdsRequest(UdsService_e service, Byte[] data = null)
        {
            Service = service;
            Data = data ?? Array.Empty<Byte>();
        }

        /// <summary>序列化为字节数组</summary>
        public Byte[] ToArray()
        {
            var list = new List<Byte> { (Byte)Service };

            switch (Service)
            {
                case UdsService_e.DiagnosticSessionControl:
                case UdsService_e.ECUReset:
                case UdsService_e.SecurityAccess:
                case UdsService_e.CommunicationControl:
                case UdsService_e.ControlDTCSetting:
                case UdsService_e.RoutineControl:
                case UdsService_e.ReadDTCInformation:
                case UdsService_e.InputOutputControlByIdentifier:
                case UdsService_e.TesterPresent:
                    list.Add((Byte)(SubFunction | (SuppressResponse ? UdsConstants.SuppressPositiveResponseMask : 0)));
                    break;

                case UdsService_e.ReadDataByIdentifier:
                case UdsService_e.ReadScalingDataByIdentifier:
                case UdsService_e.WriteDataByIdentifier:
                    list.Add((Byte)(Did >> 8));
                    list.Add((Byte)Did);
                    break;

                case UdsService_e.DynamicallyDefineDataIdentifier:
                    list.Add((Byte)SubFunction);
                    list.Add((Byte)(Did >> 8));
                    list.Add((Byte)Did);
                    break;
            }

            if (Data != null && Data.Length > 0) list.AddRange(Data);

            Raw = list.ToArray();
            return Raw;
        }

        public override String ToString()
        {
            var bs = Raw ?? ToArray();
            return $"UDS 请求 SID=0x{(Byte)Service:X2} [{bs.Length}] {bs.ToHex(" ")}";
        }
    }

    /// <summary>
    /// UDS 响应报文封装：区分肯定响应与否定响应（含 NRC）。
    /// </summary>
    public class UdsResponse
    {
        /// <summary>原始响应字节</summary>
        public Byte[] Raw;

        /// <summary>请求的服务 SID</summary>
        public UdsService_e Service;

        /// <summary>响应服务 SID（否定响应时为 0x7F）</summary>
        public Byte ResponseSid;

        /// <summary>是否为否定响应</summary>
        public Boolean IsNegative;

        /// <summary>否定响应码（仅否定响应有效）</summary>
        public UdsNrc_e Nrc;

        /// <summary>是否收到了 0x78（响应挂起）</summary>
        public Boolean ResponsePending;

        /// <summary>响应数据（跳过 SID 及其后的固定头）</summary>
        public Byte[] Data = Array.Empty<Byte>();

        /// <summary>是否为肯定响应</summary>
        public Boolean IsPositive => !IsNegative;

        /// <summary>响应长度</summary>
        public Int32 Length => Raw?.Length ?? 0;

        /// <summary>解析原始响应字节</summary>
        /// <param name="raw">完整响应数据</param>
        /// <param name="requestSid">对应的请求 SID，用于校验</param>
        public static UdsResponse Parse(Byte[] raw, UdsService_e requestSid)
        {
            if (raw == null || raw.Length == 0) return null;

            var res = new UdsResponse
            {
                Raw = raw,
                Service = requestSid,
                ResponseSid = raw[0],
            };

            if (raw[0] == UdsConstants.NegativeResponseSid)
            {
                res.IsNegative = true;
                if (raw.Length >= 3)
                {
                    res.Nrc = (UdsNrc_e)raw[2];
                    res.ResponsePending = res.Nrc == UdsNrc_e.ResponsePending;
                }
                else if (raw.Length >= 2)
                {
                    res.Nrc = UdsNrc_e.GeneralReject;
                }

                res.Data = Array.Empty<Byte>();
                return res;
            }

            // 肯定响应：SID+0x40
            if (raw.Length >= 2) res.Data = raw.ReadBytes(1, raw.Length - 1);
            return res;
        }

        /// <summary>取响应数据的指定偏移处字节</summary>
        public Byte GetByte(Int32 index, Byte def = 0)
        {
            if (Data == null || index < 0 || index >= Data.Length) return def;
            return Data[index];
        }

        /// <summary>取响应数据中指定偏移处的 16bit 值（大端）</summary>
        public UInt16 GetUInt16(Int32 index, UInt16 def = 0)
        {
            if (Data == null || index < 0 || index + 1 >= Data.Length) return def;
            return (UInt16)((Data[index] << 8) | Data[index + 1]);
        }

        /// <summary>取响应数据中指定偏移处的 32bit 值（大端）</summary>
        public UInt32 GetUInt32(Int32 index, UInt32 def = 0)
        {
            if (Data == null || index < 0 || index + 3 >= Data.Length) return def;
            return (UInt32)((Data[index] << 24) | (Data[index + 1] << 16) | (Data[index + 2] << 8) | Data[index + 3]);
        }

        public override String ToString()
        {
            var hex = Raw == null ? "" : Raw.ToHex(" ");
            if (IsNegative) return $"UDS 否定响应 SID=0x{(Byte)Service:X2} NRC=0x{(Byte)Nrc:X2}({Nrc}) [{hex}]";
            return $"UDS 肯定响应 SID=0x{(Byte)Service:X2} [{hex}]";
        }
    }

    #endregion

    #region 异常

    /// <summary>
    /// UDS 诊断异常：包含否定响应码，便于上层按 NRC 分类处理。
    /// </summary>
    public class UdsException : Exception
    {
        /// <summary>否定响应码</summary>
        public UdsNrc_e Nrc { get; }

        /// <summary>请求的服务</summary>
        public UdsService_e Service { get; }

        public UdsException(UdsService_e service, UdsNrc_e nrc)
            : base($"UDS 服务 0x{(Byte)service:X2} 被否定：NRC=0x{(Byte)nrc:X2}（{DescribeNrc(nrc)}）")
        {
            Service = service;
            Nrc = nrc;
        }

        /// <summary>NRC 中文描述</summary>
        public static String DescribeNrc(UdsNrc_e nrc)
        {
            switch (nrc)
            {
                case UdsNrc_e.GeneralReject: return "一般拒绝";
                case UdsNrc_e.ServiceNotSupported: return "服务不支持";
                case UdsNrc_e.SubFunctionNotSupported: return "子功能不支持";
                case UdsNrc_e.IncorrectMessageLengthOrInvalidFormat: return "长度或格式错误";
                case UdsNrc_e.ResponseTooLong: return "响应过长";
                case UdsNrc_e.BusyRepeatRequest: return "忙，请重发";
                case UdsNrc_e.ConditionsNotCorrect: return "条件不满足";
                case UdsNrc_e.RequestSequenceError: return "请求顺序错误";
                case UdsNrc_e.RequestOutOfRange: return "请求超出范围";
                case UdsNrc_e.SecurityAccessDenied: return "安全访问被拒绝";
                case UdsNrc_e.InvalidKey: return "密钥无效";
                case UdsNrc_e.ExceedNumberOfAttempts: return "尝试次数超限";
                case UdsNrc_e.RequiredTimeDelayNotExpired: return "延时未到";
                case UdsNrc_e.ResponsePending: return "收到，处理中";
                case UdsNrc_e.SubFunctionNotSupportedInActiveSession: return "当前会话不支持该子功能";
                case UdsNrc_e.ServiceNotSupportedInActiveSession: return "当前会话不支持该服务";
                case UdsNrc_e.UploadDownloadNotAccepted: return "不接受上传下载";
                case UdsNrc_e.TransferDataSuspended: return "传输已中止";
                case UdsNrc_e.GeneralProgrammingFailure: return "编程校验失败";
                case UdsNrc_e.WrongBlockSequenceCounter: return "块序号错误";
                case UdsNrc_e.VehicleSpeedTooHigh: return "车速过高";
                case UdsNrc_e.EngineRunning: return "发动机运转中";
                default: return nrc.ToString();
            }
        }
    }

    #endregion
}
