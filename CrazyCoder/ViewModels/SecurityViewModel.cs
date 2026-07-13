using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NewLife;
using NewLife.Collections;
using NewLife.Data;
using NewLife.Reflection;
using NewLife.Security;
using NewLife.Serialization;
using NewLife.Web;

namespace CrazyCoder.ViewModels;

/// <summary>加密解密工具 ViewModel</summary>
public partial class SecurityViewModel : ObservableObject
{
    #region 属性
    /// <summary>原文</summary>
    [ObservableProperty]
    private String _sourceText = "学无先后达者为师";

    /// <summary>密码</summary>
    [ObservableProperty]
    private String _passText = "NewLife";

    /// <summary>结果</summary>
    [ObservableProperty]
    private String _resultText = "";

    // 原文格式单选
    /// <summary>原文格式-字符串</summary>
    [ObservableProperty]
    private Boolean _isSourceString = true;

    /// <summary>原文格式-HEX</summary>
    [ObservableProperty]
    private Boolean _isSourceHex;

    /// <summary>原文格式-Base64</summary>
    [ObservableProperty]
    private Boolean _isSourceBase64;

    /// <summary>HEX单选可用</summary>
    [ObservableProperty]
    private Boolean _sourceHexEnabled = true;

    /// <summary>Base64单选可用</summary>
    [ObservableProperty]
    private Boolean _sourceBase64Enabled = true;

    // 密码格式单选
    /// <summary>密码格式-字符串</summary>
    [ObservableProperty]
    private Boolean _isPassString = true;

    /// <summary>密码格式-HEX</summary>
    [ObservableProperty]
    private Boolean _isPassHex;

    /// <summary>密码格式-Base64</summary>
    [ObservableProperty]
    private Boolean _isPassBase64;

    // 结果格式多选
    /// <summary>结果格式-字符串</summary>
    [ObservableProperty]
    private Boolean _isResultString = true;

    /// <summary>结果格式-HEX</summary>
    [ObservableProperty]
    private Boolean _isResultHex = true;

    /// <summary>结果格式-Base64</summary>
    [ObservableProperty]
    private Boolean _isResultBase64 = true;
    #endregion

    #region 源文本变更 - 检测HEX/Base64可用性
    partial void OnSourceTextChanged(String value)
    {
        if (value.IsNullOrEmpty()) return;

        // 非纯ASCII（多字节字符）时禁用HEX/Base64单选
        var enc = Encoding.UTF8;
        if (enc.GetByteCount(value) != value.Length)
        {
            SourceHexEnabled = false;
            SourceBase64Enabled = false;
            return;
        }

        // 检测是否为合法HEX
        try
        {
            SourceHexEnabled = value.ToHex().Length > 0;
        }
        catch
        {
            SourceHexEnabled = false;
        }

        // 检测是否为合法Base64
        try
        {
            SourceBase64Enabled = value.ToBase64().Length > 0;
        }
        catch
        {
            SourceBase64Enabled = false;
        }
    }
    #endregion

    #region 辅助方法
    /// <summary>从字符串中智能获取字节数组。自动识别 HEX(含-) → Base64 → UTF8</summary>
    /// <param name="str">字符串</param>
    /// <returns>字节数组</returns>
    private static Byte[] GetBytes(String str)
    {
        if (str.IsNullOrEmpty()) return [];

        try
        {
            if (str.Contains('-')) return str.ToHex();
        }
        catch { }

        try
        {
            return str.ToBase64();
        }
        catch { }

        return str.GetBytes();
    }

    /// <summary>按单选按钮状态从原文取字节</summary>
    /// <returns>字节数组</returns>
    private Byte[] GetSource()
    {
        var v = SourceText;

        if (IsSourceString) return v.GetBytes();
        if (IsSourceHex) return v.ToHex();
        if (IsSourceBase64) return v.ToBase64();

        return null;
    }

    /// <summary>按单选按钮状态从密码区取字节</summary>
    /// <returns>字节数组</returns>
    private Byte[] GetPass()
    {
        var v = PassText;

        if (IsPassString) return v.GetBytes();
        if (IsPassHex) return v.ToHex();
        if (IsPassBase64) return v.ToBase64();

        return null;
    }

    /// <summary>多行输出结果</summary>
    /// <param name="rs">字符串数组</param>
    private void SetResult(params String[] rs)
    {
        var sb = new StringBuilder();
        foreach (var item in rs)
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.Append(item);
        }
        ResultText = sb.ToString();
    }

    /// <summary>按输出格式复选框输出字节数组</summary>
    /// <param name="data">字节数组</param>
    /// <returns>输出字符串列表</returns>
    private List<String> SetResult(Byte[] data)
    {
        var list = new List<String>();
        if (IsResultString) list.Add(data.ToStr());
        if (IsResultHex)
        {
            list.Add(data.ToHex().ToUpper());
            list.Add(data.ToHex().ToLower());
            list.Add(data.ToHex("-"));
            list.Add(data.ToHex(" "));
        }
        if (IsResultBase64)
        {
            list.Add(data.ToBase64());
            list.Add(data.ToUrlBase64());
        }

        SetResult(list.ToArray());

        return list;
    }
    #endregion

    #region 功能命令
    /// <summary>上下互换：交换原文区和结果区内容</summary>
    [RelayCommand]
    private void Exchange()
    {
        var v = SourceText;
        var v2 = ResultText;
        // 结果区只要第一行
        if (!v2.IsNullOrEmpty())
        {
            var ss = v2.Split('\n');
            var n = 0;
            if (ss.Length > n + 1 && ss[n].StartsWith("/*") && ss[n].EndsWith("*/")) n++;
            v2 = ss[n];
        }
        SourceText = v2;
        ResultText = v;
    }

    /// <summary>HEX编码</summary>
    [RelayCommand]
    private void HexEncode()
    {
        var buf = GetSource();
        SetResult(buf.ToHex(), buf.ToHex(" ", 32), buf.ToHex("-", 32));
    }

    /// <summary>HEX解码</summary>
    [RelayCommand]
    private void HexDecode()
    {
        var v = SourceText;
        ResultText = v?.Trim().ToHex().ToStr();
    }

    /// <summary>Base64编码</summary>
    [RelayCommand]
    private void Base64Encode()
    {
        var buf = GetSource();
        SetResult(buf.ToBase64(), buf.ToUrlBase64());
    }

    /// <summary>Base64解码</summary>
    [RelayCommand]
    private void Base64Decode()
    {
        var v = SourceText;

        var vs = v.Split('.');
        if (vs.Length <= 1)
        {
            var buf = v.Trim().ToBase64();
            SetResult(buf);
        }
        else
        {
            SetResult(vs.Select(e2 => e2.Trim().ToBase64().ToStr()).ToArray());
        }
    }

    /// <summary>MD5_32位</summary>
    [RelayCommand]
    private void MD5_32()
    {
        var buf = GetSource();
        buf = buf.MD5();
        SetResult(buf);
    }

    /// <summary>MD5_16位</summary>
    [RelayCommand]
    private void MD5_16()
    {
        var buf = GetSource();
        buf = buf.MD5().Take(8).ToArray();
        SetResult(buf);
    }

    /// <summary>SHA1</summary>
    [RelayCommand]
    private void SHA1()
    {
        var buf = GetSource();
        var key = GetPass();

        buf = buf.SHA1(key);
        var rs = SetResult(buf);
        rs.Add($"sha1${key.ToStr()}${buf.ToBase64()}");

        SetResult(rs.ToArray());
    }

    /// <summary>SHA256</summary>
    [RelayCommand]
    private void SHA256()
    {
        var buf = GetSource();
        var key = GetPass();

        buf = buf.SHA256(key);
        var rs = SetResult(buf);
        rs.Add($"sha256${key.ToStr()}${buf.ToBase64()}");

        SetResult(rs.ToArray());
    }

    /// <summary>SHA384</summary>
    [RelayCommand]
    private void SHA384()
    {
        var buf = GetSource();
        var key = GetPass();

        buf = buf.SHA384(key);
        var rs = SetResult(buf);
        rs.Add($"sha384${key.ToStr()}${buf.ToBase64()}");

        SetResult(rs.ToArray());
    }

    /// <summary>SHA512</summary>
    [RelayCommand]
    private void SHA512()
    {
        var buf = GetSource();
        var key = GetPass();

        buf = buf.SHA512(key);
        var rs = SetResult(buf);
        rs.Add($"sha512${key.ToStr()}${buf.ToBase64()}");

        SetResult(rs.ToArray());
    }

    /// <summary>CRC_32</summary>
    [RelayCommand]
    private void CRC_32()
    {
        var buf = GetSource();
        var rs = buf.Crc();
        var data = rs.GetBytes(false);
        SetResult("/*数字、HEX编码、Base64编码*/", rs + "", data.ToHex(), data.ToBase64());
    }

    /// <summary>CRC_16</summary>
    [RelayCommand]
    private void CRC_16()
    {
        var buf = GetSource();
        var rs = buf.Crc16();
        var data = rs.GetBytes(false);
        var mcrc = Modbus_CRC(buf, 0, buf.Length);
        SetResult("/*数字、HEX编码、Base64编码、Modbus-Crc*/", rs + "", data.ToHex(), data.ToBase64(), mcrc.GetBytes().ToHex("-"));
    }

    /// <summary>RSA加密</summary>
    [RelayCommand]
    private void RSAEncrypt()
    {
        var buf = GetSource();
        var key = PassText;

        if (key.Length < 100)
        {
            key = RSAHelper.GenerateKey().First();
            PassText = key;
        }

        buf = RSAHelper.Encrypt(buf, key);

        SetResult(buf);
    }

    /// <summary>RSA解密</summary>
    [RelayCommand]
    private void RSADecrypt()
    {
        var buf = GetSource();
        var pass = PassText;

        try
        {
            buf = RSAHelper.Decrypt(buf, pass, true);
        }
        catch (CryptographicException)
        {
            // 换一种填充方式
            buf = RSAHelper.Decrypt(buf, pass, false);
        }

        SetResult(buf);
    }

    /// <summary>DSA签名</summary>
    [RelayCommand]
    private void DSASign()
    {
        var buf = GetSource();
        var key = PassText;

        if (key.Length < 100)
        {
            key = DSAHelper.GenerateKey().First();
            PassText = key;
        }

        buf = DSAHelper.Sign(buf, key);

        SetResult(buf);
    }

    /// <summary>DSA验证</summary>
    [RelayCommand]
    private void DSAVerify()
    {
        var buf = GetSource();
        var pass = PassText;

        var v = ResultText;
        if (v.Contains("\n\n")) v = v.Substring(null, "\n\n");
        var sign = GetBytes(v);

        var rs = DSAHelper.Verify(buf, pass, sign);
        SetResult(rs ? "验证通过" : "验证失败");
    }

    /// <summary>Url编码</summary>
    [RelayCommand]
    private void UrlEncode()
    {
        var v = SourceText;
        v = WebUtility.UrlEncode(v);
        ResultText = v;
    }

    /// <summary>Url解码</summary>
    [RelayCommand]
    private void UrlDecode()
    {
        var v = SourceText;
        v = WebUtility.UrlDecode(v);
        ResultText = v;
    }

    /// <summary>Html编码</summary>
    [RelayCommand]
    private void HtmlEncode()
    {
        var v = SourceText;
        v = WebUtility.HtmlEncode(v);
        ResultText = v;
    }

    /// <summary>Html解码</summary>
    [RelayCommand]
    private void HtmlDecode()
    {
        var v = SourceText;
        v = WebUtility.HtmlDecode(v);
        ResultText = v;
    }

    /// <summary>时间戳</summary>
    [RelayCommand]
    private void Timestamp()
    {
        var v = SourceText;
        if (v.IsNullOrEmpty()) return;

        var sb = Pool.StringBuilder.Get();

        DateTime.TryParse(PassText, out var baseTime);
        var utc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        if (baseTime > DateTime.MinValue) sb.AppendFormat("基准：{0:yyyy-MM-dd HH:mm:ss.fff} {1}\r\n", baseTime, baseTime.Kind);

        var dt = v.ToDateTime();
        if (dt.Year > 1 && dt.Year < 3000)
        {
            var s = dt.ToInt();
            var m = dt.ToLong();

            if (baseTime > DateTime.MinValue)
            {
                s -= baseTime.ToInt();
                m -= baseTime.ToLong();
            }

            sb.AppendLine("Unix秒：" + s);
            sb.AppendLine("Unix毫秒：" + m);
        }

        var now = DateTime.Now;
        var n = v.ToLong();
        if (n >= Int32.MaxValue)
        {
            dt = n.ToDateTime();
            dt = dt.ToLocalTime().ToUniversalTime();
            if (baseTime > DateTime.MinValue) dt = dt.Add(baseTime - utc);
            if (dt.Year > 1000 && dt.Year < 3000)
            {
                sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss.fff} (Unix毫秒) {1}\r\n", dt, dt.Kind);
                dt = dt.ToLocalTime();
                sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss.fff} (现在) {1}\r\n", dt, dt.Kind);
            }
        }
        else if (n > 0)
        {
            dt = v.ToInt().ToDateTime();
            dt = dt.ToLocalTime().ToUniversalTime();
            if (baseTime > DateTime.MinValue) dt = dt.Add(baseTime - utc);
            if (dt.Year > 1000 && dt.Year < 3000)
            {
                sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss} (Unix秒) {1}\r\n", dt, dt.Kind);
                dt = dt.ToLocalTime();
                sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss} (现在) {1}\r\n", dt, dt.Kind);
            }
        }

        // 过去/未来时间戳
        if (n > 0 && n < 1000L * 365 * 24 * 3600 * 1000)
        {
            sb.AppendFormat("过去：{0:yyyy-MM-dd HH:mm:ss.fff} (now.AddMilliseconds(-n))\r\n", now.AddMilliseconds(-n));
            sb.AppendFormat("未来：{0:yyyy-MM-dd HH:mm:ss.fff} (now.AddMilliseconds(n))\r\n", now.AddMilliseconds(n));

            if (n < Int32.MaxValue)
            {
                sb.AppendFormat("过去：{0:yyyy-MM-dd HH:mm:ss} (now.AddSeconds(-n))\r\n", now.AddSeconds(-n));
                sb.AppendFormat("未来：{0:yyyy-MM-dd HH:mm:ss} (now.AddSeconds(n))\r\n", now.AddSeconds(n));
            }
        }

        ResultText = sb.Return(true);
    }

    /// <summary>机器信息</summary>
    [RelayCommand]
    private void ComputerInfo()
    {
        var sb = Pool.StringBuilder.Get();

        var mi = MachineInfo.Current;
        mi.Refresh();
        sb.AppendLine(mi.ToJson(true));
        sb.AppendLine();

        var osName = GetWmiInfo("Win32_OperatingSystem", "Caption");
        sb.AppendFormat("OSName:\t{0}\t(Win32_OperatingSystem.Caption)\r\n", osName);

        var osVersion = GetWmiInfo("Win32_OperatingSystem", "Version");
        sb.AppendFormat("OSVersion:\t{0}\t(Win32_OperatingSystem.Version)\r\n", osVersion);

        var macs = GetMacs().ToList();
        if (macs.Count > 0) sb.AppendFormat("MAC:\t{0}\r\n", macs.Join(",", x => x.ToHex("-")));

        var processor = GetWmiInfo("Win32_Processor", "Name");
        sb.AppendFormat("Processor:\t{0}\t(Win32_Processor.Name)\r\n", processor);

        var cpuID = GetWmiInfo("Win32_Processor", "ProcessorId");
        sb.AppendFormat("ProcessorId:\t{0}\t(Win32_Processor.ProcessorId)\r\n", cpuID);

        var uuid = GetWmiInfo("Win32_ComputerSystemProduct", "UUID");
        sb.AppendFormat("UUID:\t{0}\t(Win32_ComputerSystemProduct.UUID)\r\n", uuid);

        var id = GetWmiInfo("Win32_ComputerSystemProduct", "IdentifyingNumber");
        sb.AppendFormat("IdentifyingNumber:\t{0}\t(Win32_ComputerSystemProduct.IdentifyingNumber)\r\n", id);

        var bios = GetWmiInfo("Win32_BIOS", "SerialNumber");
        sb.AppendFormat("BIOS:\t{0}\t(Win32_BIOS.SerialNumber)\r\n", bios);

        var baseBoard = GetWmiInfo("Win32_BaseBoard", "SerialNumber");
        sb.AppendFormat("BaseBoard:\t{0}\t(Win32_BaseBoard.SerialNumber)\r\n", baseBoard);

        var serialNumber = GetWmiInfo("Win32_DiskDrive", "SerialNumber");
        sb.AppendFormat("DiskSerial:\t{0}\t(Win32_DiskDrive.SerialNumber)\r\n", serialNumber);

        var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
        if (reg != null)
        {
            var guid = reg.GetValue("MachineGuid") + "";
            sb.AppendFormat("MachineGuid:\t{0}\t(SOFTWARE\\Microsoft\\Cryptography)\r\n", guid);
        }

        ResultText = sb.Return(true);
    }

    private static readonly String[] _Excludes = ["Loopback", "VMware", "VBox", "Virtual", "Teredo", "Microsoft", "VPN", "VNIC", "IEEE"];

    /// <summary>获取所有网卡MAC地址</summary>
    /// <returns>MAC地址集合</returns>
    public static IEnumerable<Byte[]> GetMacs()
    {
        foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (_Excludes.Any(e => item.Description.Contains(e))) continue;
            if (item.Speed < 1_000_000) continue;

            var addrs = item.GetIPProperties().UnicastAddresses.Where(e => e.Address.AddressFamily == AddressFamily.InterNetwork).ToArray();
            if (addrs.All(e => IPAddress.IsLoopback(e.Address))) continue;

            var mac = item.GetPhysicalAddress()?.GetAddressBytes();
            if (mac != null && mac.Length == 6) yield return mac;
        }
    }

    /// <summary>获取WMI信息</summary>
    /// <param name="path">WMI路径</param>
    /// <param name="property">属性名</param>
    /// <returns>属性值</returns>
    public static String GetWmiInfo(String path, String property)
    {
        try
        {
            var wql = $"Select {property} From {path}";
            var searcher = new ManagementObjectSearcher(wql);
            var bbs = new List<String>();
            foreach (var mo in searcher.Get())
            {
                if (mo != null && mo.Properties != null && mo.Properties[property] != null && mo.Properties[property].Value != null)
                    bbs.Add(mo.Properties[property].Value.ToString());
            }
            bbs.Sort();
            return bbs.Join(",");
        }
        catch
        {
            return "";
        }
    }

    /// <summary>雪花Id</summary>
    [RelayCommand]
    private void Snowflake()
    {
        var v = SourceText.ToLong();
        if (v <= 0) return;

        var snow = new Snowflake();

        // 指定基准时间
        if (!PassText.IsNullOrEmpty())
        {
            var baseTime = PassText.ToDateTime();
            if (baseTime.Year > 1000) snow.StartTimestamp = baseTime;
        }

        if (!snow.TryParse(v, out var time, out var workerId, out var sequence)) throw new Exception("解码失败！");

        // 初始化一次，用于获取本机workerId
        snow.NewId();

        var t = (Int64)(time - snow.StartTimestamp).TotalMilliseconds;
        SetResult(
            $"十六：{v:X16}",
            $"编码：{(t << 2):X8} {workerId:X2} {sequence:X3}",
            $"基准：{snow.StartTimestamp.ToFullString()} {snow.StartTimestamp.Kind}",
            $"时间：{time:yyyy-MM-dd HH:mm:ss.fff} {time.Kind} ({t} / {(t << 22):X8})",
            $"节点：{workerId} ({workerId:X4})",
            $"序号：{sequence} ({sequence:X4})",
            $"本机：{snow.WorkerId} ({snow.WorkerId:X4})");
    }

    /// <summary>JWT令牌</summary>
    [RelayCommand]
    private void JwtToken()
    {
        var v = SourceText;
        if (v.IsNullOrEmpty()) return;

        var pass = PassText?.Trim();

        var vs = v.Split('.');
        if (vs.Length == 3)
        {
            var jwt = new JwtBuilder { Secret = "abcd" };

            var ss = pass?.Split(':');
            if (ss != null && ss.Length >= 2)
            {
                jwt.Algorithm = ss[0];
                jwt.Secret = ss[1];
            }

            var rs = jwt.TryDecode(v, out var message);

            SetResult($"验证结果：{rs}", jwt.ToJson(true));
        }
        else if (vs.Length == 2)
        {
            var prv = new TokenProvider
            {
                Key = pass
            };

            var rs = prv.TryDecode(v, out var user, out var expire);

            SetResult($"验证结果：{rs}", new { user, expire }.ToJson(true));
        }
        else
        {
            SetResult(vs.Select(e2 => e2.ToBase64().ToStr()).ToArray());
        }
    }

    /// <summary>版本号</summary>
    [RelayCommand]
    private void Version()
    {
        var v = SourceText;
        if (v.IsNullOrEmpty()) return;

        var dt = AssemblyX.GetCompileTime(v);

        ResultText = dt.ToFullString();
    }

    /// <summary>TraceId</summary>
    [RelayCommand]
    private void TraceId()
    {
        var str = SourceText.Trim();
        if (str.IsNullOrEmpty()) return;

        var rs = new List<String>();
        foreach (var item in str.Split('\n'))
        {
            var v = item.Trim();
            if (v.IsNullOrEmpty()) continue;

            // TraceId
            if (v.Length >= 30)
            {
                if (v.Length >= 8)
                    rs.Add($"地址：{v[..8]} {new IPAddress(v[..8].ToHex())}");

                if (v.Length >= 8 + 13)
                {
                    var time = v.Substring(8, 13).ToLong();
                    rs.Add($"时间：{time}（{time.ToDateTime().ToLocalTime().ToFullString()}）");
                }

                if (v.Length >= 21 + 4)
                    rs.Add($"序列：{v[21..25]} {v[21..25].ToHex().ToUInt16()}");

                if (v.Length >= 26 + 4)
                    rs.Add($"进程：{v[26..30]} {v[26..30].ToHex().ToUInt16()}");
            }
            else if (v.Length >= 16)
            {
                if (v.Length >= 8)
                    rs.Add($"地址：{v[..8]} {new IPAddress(v[..8].ToHex())}");

                if (v.Length >= 8 + 4)
                    rs.Add($"进程：{v[8..12]} {v[8..12].ToHex().ToUInt16()}");

                if (v.Length >= 12 + 4)
                    rs.Add($"序列：{v[12..16]} {v[12..16].ToHex().ToUInt16()}");
            }
            rs.Add("");
        }

        SetResult(rs.ToArray());
    }
    #endregion

    #region Modbus_CRC
    private static readonly UInt16[] crc_ta = [0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400];

    /// <summary>Crc校验</summary>
    /// <param name="data">数据</param>
    /// <param name="offset">偏移</param>
    /// <param name="count">数量</param>
    /// <returns>Crc结果</returns>
    public static UInt16 Modbus_CRC(Byte[] data, Int32 offset, Int32 count = -1)
    {
        if (data == null || data.Length < 1) return 0;

        UInt16 u = 0xFFFF;
        Byte b;

        if (count == 0) count = data.Length - offset;

        for (var i = offset; i < count; i++)
        {
            b = data[i];
            u = (UInt16)(crc_ta[(b ^ u) & 15] ^ (u >> 4));
            u = (UInt16)(crc_ta[((b >> 4) ^ u) & 15] ^ (u >> 4));
        }

        return u;
    }
    #endregion
}
