using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Microsoft.Win32;
using NewLife;
using NewLife.Collections;
using NewLife.Data;
using NewLife.Reflection;
using NewLife.Security;
using NewLife.Serialization;
using NewLife.Web;
using XCoder.Common;

namespace XCoder.Tools;

[Category("安全工具")]
[DisplayName("加密解密")]
public partial class FrmSecurity : Form, IXForm
{
    private ControlConfig _config;

    #region 窗体初始化
    public FrmSecurity()
    {
        InitializeComponent();

        // 动态调节宽度高度，兼容高DPI
        this.FixDpi();
    }

    private void FrmSecurity_Load(Object sender, EventArgs e)
    {
        _config = new ControlConfig { Control = this, FileName = "security.json" };
        _config.Load();
    }
    #endregion

    #region 辅助
    /// <summary>从字符串中获取字节数组</summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private Byte[] GetBytes(String str)
    {
        if (str.IsNullOrEmpty()) return new Byte[0];

        try
        {
            if (str.Contains("-")) return str.ToHex();
        }
        catch { }

        try
        {
            return str.ToBase64();
        }
        catch { }

        return str.GetBytes();
    }

    /// <summary>从原文中获取字节数组</summary>
    /// <returns></returns>
    private Byte[] GetSource()
    {
        var v = rtSource.Text;

        if (rbString.Checked) return v.GetBytes();
        if (rbHex.Checked) return v.ToHex();
        if (rbBase64.Checked) return v.ToBase64();

        return null;
    }

    private void rtSource_TextChanged(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        if (v.IsNullOrEmpty()) return;

        // 单字节
        var enc = Encoding.UTF8;
        if (enc.GetByteCount(v) != v.Length)
        {
            rbHex.Enabled = false;
            rbBase64.Enabled = false;
            return;
        }

        try
        {
            rbHex.Enabled = v.ToHex().Length > 0;
        }
        catch
        {
            rbHex.Enabled = false;
        }

        try
        {
            rbBase64.Enabled = v.ToBase64().Length > 0;
        }
        catch
        {
            rbBase64.Enabled = false;
        }
    }

    private Byte[] GetPass()
    {
        var v = rtPass.Text;

        if (rbString2.Checked) return v.GetBytes();
        if (rbHex2.Checked) return v.ToHex();
        if (rbBase642.Checked) return v.ToBase64();

        return null;
    }

    private void SetResult(params String[] rs)
    {
        var sb = new StringBuilder();
        foreach (var item in rs)
        {
            if (sb.Length > 0) sb.AppendLine();
            sb.Append(item);
        }
        rtResult.Text = sb.ToString();

        _config.Save();
    }

    private List<String> SetResult(Byte[] data)
    {
        //SetResult("/*HEX编码、Base64编码、Url改进Base64编码*/", data.ToHex("-"), data.ToBase64(), data.ToUrlBase64());

        var list = new List<String>();
        if (cbString.Checked) list.Add(data.ToStr());
        if (cbHex.Checked)
        {
            list.Add(data.ToHex().ToUpper());
            list.Add(data.ToHex().ToLower());
            list.Add(data.ToHex("-"));
            list.Add(data.ToHex(" "));
        }
        if (cbBase64.Checked)
        {
            list.Add(data.ToBase64());
            list.Add(data.ToUrlBase64());
        }

        SetResult(list.ToArray());

        return list;
    }

    //private void SetResult2(Byte[] data)
    //{
    //    SetResult("/*字符串、HEX编码、Base64编码*/", data.ToStr(), data.ToHex("-"), data.ToBase64());
    //}
    #endregion

    #region 功能
    private void btnExchange_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        var v2 = rtResult.Text;
        // 结果区只要第一行
        if (!v2.IsNullOrEmpty())
        {
            var ss = v2.Split("\n");
            var n = 0;
            if (ss.Length > n + 1 && ss[n].StartsWith("/*") && ss[n].EndsWith("*/")) n++;
            v2 = ss[n];
        }
        rtSource.Text = v2;
        rtResult.Text = v;
    }

    private void btnHex_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        //rtResult.Text = buf.ToHex(" ", 32);
        SetResult(buf.ToHex(), buf.ToHex(" ", 32), buf.ToHex("-", 32));
    }

    private void btnHex2_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        rtResult.Text = v?.Trim().ToHex().ToStr();
    }

    private void btnB64_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        //rtResult.Text = buf.ToBase64();
        SetResult(buf.ToBase64(), buf.ToUrlBase64());
    }

    private void btnB642_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;

        var vs = v.Split(".");
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

    private void btnMD5_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        buf = buf.MD5();
        SetResult(buf);
    }

    private void btnMD52_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        buf = buf.MD5().Take(8).ToArray();
        SetResult(buf);
    }

    private void btnSHA1_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var key = GetPass();

        buf = buf.SHA1(key);
        var rs = SetResult(buf);
        rs.Add($"sha1${key.ToStr()}${buf.ToBase64()}");

        SetResult(rs.ToArray());
    }

    private void btnSHA256_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var key = GetPass();

        buf = buf.SHA256(key);
        var rs = SetResult(buf);
        rs.Add($"sha256${key.ToStr()}${buf.ToBase64()}");

        SetResult(rs.ToArray());
    }

    private void btnSHA384_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var key = GetPass();

        buf = buf.SHA384(key);
        var rs = SetResult(buf);
        rs.Add($"sha384${key.ToStr()}${buf.ToBase64()}");

        SetResult(rs.ToArray());
    }

    private void btnSHA512_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var key = GetPass();

        buf = buf.SHA512(key);
        var rs = SetResult(buf);
        rs.Add($"sha512${key.ToStr()}${buf.ToBase64()}");

        SetResult(rs.ToArray());
    }

    private void btnCRC_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        //rtResult.Text = "{0:X8}\r\n{0}".F(buf.Crc());
        var rs = buf.Crc();
        buf = rs.GetBytes(false);
        SetResult("/*数字、HEX编码、Base64编码*/", rs + "", buf.ToHex(), buf.ToBase64());
    }

    private void btnCRC2_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        //rtResult.Text = "{0:X4}\r\n{0}".F(buf.Crc16());
        var rs = buf.Crc16();
        var data = rs.GetBytes(false);
        var mcrc = Modbus_CRC(buf, 0, buf.Length);
        SetResult("/*数字、HEX编码、Base64编码、Modbus-Crc*/", rs + "", data.ToHex(), data.ToBase64(), mcrc.GetBytes().ToHex("-"));
    }

    private void btnRSA_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var key = rtPass.Text;

        if (key.Length < 100)
        {
            key = RSAHelper.GenerateKey().First();
            rtPass.Text = key;
        }

        buf = RSAHelper.Encrypt(buf, key);

        SetResult(buf);
    }

    private void btnRSA2_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = rtPass.Text;

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

    private void btnDSA_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var key = rtPass.Text;

        if (key.Length < 100)
        {
            key = DSAHelper.GenerateKey().First();
            rtPass.Text = key;
        }

        buf = DSAHelper.Sign(buf, key);

        SetResult(buf);
    }

    private void btnDSA2_Click(Object sender, EventArgs e)
    {
        var buf = GetSource();
        var pass = rtPass.Text;

        var v = rtResult.Text;
        if (v.Contains("\n\n")) v = v.Substring(null, "\n\n");
        var sign = GetBytes(v);

        var rs = DSAHelper.Verify(buf, pass, sign);
        if (rs)
            MessageBox.Show("验证通过", "DSA数字签名", MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
            MessageBox.Show("验证失败", "DSA数字签名", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void btnUrl_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        v = HttpUtility.UrlEncode(v);
        rtResult.Text = v;
    }

    private void btnUrl2_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        v = HttpUtility.UrlDecode(v);
        rtResult.Text = v;
    }

    private void btnHtml_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        v = HttpUtility.HtmlEncode(v);
        rtResult.Text = v;
    }

    private void btnHtml2_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        v = HttpUtility.HtmlDecode(v);
        rtResult.Text = v;
    }

    private void btnTime_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        if (v.IsNullOrEmpty()) return;

        var sb = Pool.StringBuilder.Get();

        DateTime.TryParse(rtPass.Text, out var baseTime);
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
            // 当前未指定时区，为指定为UTC，需要先转为本地时间
            dt = dt.ToLocalTime().ToUniversalTime();
            if (baseTime > DateTime.MinValue) dt = dt.Add(baseTime - utc);
            if (dt.Year > 1000 && dt.Year < 3000)
            {
                sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss.fff} (Unix毫秒) {1}\r\n", dt, dt.Kind);
                dt = dt.ToLocalTime();
                sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss.fff} (现在) {1}\r\n", dt, dt.Kind);
            }

            //sb.AppendFormat("过去：{0:yyyy-MM-dd HH:mm:ss.fff}\r\n", now.AddMilliseconds(-n));
            //sb.AppendFormat("未来：{0:yyyy-MM-dd HH:mm:ss.fff}\r\n", now.AddMilliseconds(n));
        }
        else if (n > 0)
        {
            dt = v.ToInt().ToDateTime();
            // 当前未指定时区，为指定为UTC，需要先转为本地时间
            dt = dt.ToLocalTime().ToUniversalTime();
            if (baseTime > DateTime.MinValue) dt = dt.Add(baseTime - utc);
            if (dt.Year > 1000 && dt.Year < 3000)
            {
                sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss} (Unix秒) {1}\r\n", dt, dt.Kind);
                dt = dt.ToLocalTime();
                sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss} (现在) {1}\r\n", dt, dt.Kind);
            }

            //sb.AppendFormat("过去：{0:yyyy-MM-dd HH:mm:ss}\r\n", now.AddSeconds(-n));
            //sb.AppendFormat("未来：{0:yyyy-MM-dd HH:mm:ss}\r\n", now.AddSeconds(n));
        }

        // 有可能是过去时间或者未来时间戳
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

        rtResult.Text = sb.Put(true);
    }

    private void btnVersion_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        if (v.IsNullOrEmpty()) return;

        //if (!Version.TryParse(v, out var version)) return;

        var dt = AssemblyX.GetCompileTime(v);

        rtResult.Text = dt.ToFullString();
    }

    private void btnSnowflake_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text.ToLong();
        if (v <= 0) return;

        var snow = new Snowflake();

        // 指定基准时间
        if (!rtPass.Text.IsNullOrEmpty())
        {
            var baseTime = rtPass.Text.ToDateTime();
            if (baseTime.Year > 1000) snow.StartTimestamp = baseTime;
        }

        // 计算结果
        {
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
    }

    private void btnJWT_Click(Object sender, EventArgs e)
    {
        var v = rtSource.Text;
        if (v.IsNullOrEmpty()) return;

        var pass = rtPass.Text?.Trim();

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

    private void btnTrace_Click(Object sender, EventArgs e)
    {
        var str = rtSource.Text.Trim();
        if (str.IsNullOrEmpty()) return;

        var rs = new List<String>();
        foreach (var item in str.Split("\n"))
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

    #region 机器信息
    private void BtnComputerInfo_Click(Object sender, EventArgs e)
    {
        var sb = Pool.StringBuilder.Get();

        var mi = MachineInfo.Current;
        mi.Refresh();
        sb.AppendLine(mi.ToJson(true));
        sb.AppendLine();

        var osName = GetInfo("Win32_OperatingSystem", "Caption");
        sb.AppendFormat("OSName:\t{0}\t(Win32_OperatingSystem.Caption)\r\n", osName);

        var osVersion = GetInfo("Win32_OperatingSystem", "Version");
        sb.AppendFormat("OSVersion:\t{0}\t(Win32_OperatingSystem.Version)\r\n", osVersion);

        var macs = GetMacs().ToList();
        if (macs.Count > 0) sb.AppendFormat("MAC:\t{0}\r\n", macs.Join(",", x => x.ToHex("-")));

        var processor = GetInfo("Win32_Processor", "Name");
        sb.AppendFormat("Processor:\t{0}\t(Win32_Processor.Name)\r\n", processor);

        var cpuID = GetInfo("Win32_Processor", "ProcessorId");
        sb.AppendFormat("ProcessorId:\t{0}\t(Win32_Processor.ProcessorId)\r\n", cpuID);

        var uuid = GetInfo("Win32_ComputerSystemProduct", "UUID");
        sb.AppendFormat("UUID:\t{0}\t(Win32_ComputerSystemProduct.UUID)\r\n", uuid);

        var id = GetInfo("Win32_ComputerSystemProduct", "IdentifyingNumber");
        sb.AppendFormat("IdentifyingNumber:\t{0}\t(Win32_ComputerSystemProduct.IdentifyingNumber)\r\n", id);

        var bios = GetInfo("Win32_BIOS", "SerialNumber");
        sb.AppendFormat("BIOS:\t{0}\t(Win32_BIOS.SerialNumber)\r\n", bios);

        var baseBoard = GetInfo("Win32_BaseBoard", "SerialNumber");
        sb.AppendFormat("BaseBoard:\t{0}\t(Win32_BaseBoard.SerialNumber)\r\n", baseBoard);

        var serialNumber = GetInfo("Win32_DiskDrive", "SerialNumber");
        sb.AppendFormat("DiskSerial:\t{0}\t(Win32_DiskDrive.SerialNumber)\r\n", serialNumber);

        var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
        if (reg != null)
        {
            var guid = reg.GetValue("MachineGuid") + "";
            /*if (!guid.IsNullOrEmpty())*/
            sb.AppendFormat("MachineGuid:\t{0}\t(SOFTWARE\\Microsoft\\Cryptography)\r\n", guid);
        }

        sb.AppendLine();
        var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
        foreach (var pi in ci.GetType().GetProperties())
        {
            //if (sb.Length > 0) sb.AppendLine();
            sb.AppendFormat("{0}:\t{1:n0}\r\n", pi.Name, ci.GetValue(pi));
        }

        rtResult.Text = sb.Put(true);
    }

    private static String[] _Excludes = new[] { "Loopback", "VMware", "VBox", "Virtual", "Teredo", "Microsoft", "VPN", "VNIC", "IEEE" };
    /// <summary>获取所有网卡MAC地址</summary>
    /// <returns></returns>
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

    #endregion

    #region WMI辅助
    /// <summary>获取WMI信息</summary>
    /// <param name="path"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    public static String GetInfo(String path, String property)
    {
        // Linux Mono不支持WMI
        if (Runtime.Mono) return "";

        var bbs = new List<String>();
        try
        {
            var wql = String.Format("Select {0} From {1}", property, path);
            var cimobject = new ManagementObjectSearcher(wql);
            var moc = cimobject.Get();
            foreach (var mo in moc)
            {
                if (mo != null &&
                    mo.Properties != null &&
                    mo.Properties[property] != null &&
                    mo.Properties[property].Value != null)
                    bbs.Add(mo.Properties[property].Value.ToString());
            }
        }
        catch
        {
            return "";
        }

        bbs.Sort();
        return bbs.Join(",");
    }
    #endregion

    #region Modbus_CRC
    private static readonly UInt16[] crc_ta = new UInt16[16] { 0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400, };

    /// <summary>Crc校验</summary>
    /// <param name="data"></param>
    /// <param name="offset">偏移</param>
    /// <param name="count">数量</param>
    /// <returns></returns>
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