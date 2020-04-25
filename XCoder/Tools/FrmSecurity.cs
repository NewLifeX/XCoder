using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using NewLife.Log;
using NewLife.Reflection;
using NewLife.Security;
using NewLife.Serialization;

namespace XCoder.Tools
{
    [DisplayName("加密解密")]
    public partial class FrmSecurity : Form, IXForm
    {
        #region 窗体初始化
        public FrmSecurity()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();
        }

        private void FrmSecurity_Load(Object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            var dataPath = Setting.Current.DataPath;
            var file = dataPath.CombinePath("security.json").GetFullPath();
            if (File.Exists(file))
            {
                var dic = new JsonParser(File.ReadAllText(file)).Decode() as IDictionary<String, Object>;
                LoadConfig(dic, this);
            }
        }

        private void LoadConfig(IDictionary<String, Object> dic, Control control)
        {
            foreach (Control item in control.Controls)
            {
                switch (item)
                {
                    case RadioButton rb:
                        if (dic.TryGetValue(item.Name, out var v)) rb.Checked = v.ToBoolean();
                        break;
                    case CheckBox cb:
                        if (dic.TryGetValue(item.Name, out v)) cb.Checked = v.ToBoolean();
                        break;
                    case RichTextBox rtb:
                        if (dic.TryGetValue(item.Name, out v)) rtb.Text = v + "";
                        break;
                    default:
                        if (item.Controls.Count > 0) LoadConfig(dic, item);
                        break;
                }
            }
        }

        private void SaveConfig()
        {
            var dic = new Dictionary<String, Object>();
            SaveConfig(dic, this);

            var dataPath = Setting.Current.DataPath;
            var file = dataPath.CombinePath("security.json").GetFullPath();
            file.EnsureDirectory(true);
            File.WriteAllText(file, dic.ToJson(true));
        }

        private void SaveConfig(IDictionary<String, Object> dic, Control control)
        {
            foreach (Control item in control.Controls)
            {
                switch (item)
                {
                    case RadioButton rb: dic[item.Name] = rb.Checked; break;
                    case CheckBox cb: dic[item.Name] = cb.Checked; break;
                    case RichTextBox rtb: dic[item.Name] = rtb.Text; break;
                    default:
                        if (item.Controls.Count > 0) SaveConfig(dic, item);
                        break;
                }
            }
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

            SaveConfig();
        }

        private void SetResult(Byte[] data)
        {
            //SetResult("/*HEX编码、Base64编码、Url改进Base64编码*/", data.ToHex("-"), data.ToBase64(), data.ToUrlBase64());

            var list = new List<String>();
            if (cbString.Checked) list.Add(data.ToStr());
            if (cbHex.Checked)
            {
                list.Add(data.ToHex("-"));
                list.Add(data.ToHex(" "));
            }
            if (cbBase64.Checked)
            {
                list.Add(data.ToBase64());
                list.Add(data.ToUrlBase64());
            }

            SetResult(list.ToArray());
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
            rtResult.Text = v.ToHex().ToStr();
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
            //rtResult.Text = v.ToBase64().ToStr();
            var buf = v.ToBase64();
            //rtResult.Text = buf.ToStr() + Environment.NewLine + buf.ToHex();
            SetResult(buf.ToStr(), buf.ToHex());
        }

        private void btnMD5_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var str = buf.MD5().ToHex();
            rtResult.Text = str.ToUpper() + Environment.NewLine + str.ToLower();
        }

        private void btnMD52_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var str = buf.MD5().ToHex(0, 8);
            rtResult.Text = str.ToUpper() + Environment.NewLine + str.ToLower();
        }

        private void btnSHA1_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var key = GetPass();

            buf = buf.SHA1(key);
            SetResult(buf);
        }

        private void btnSHA256_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var key = GetPass();

            buf = buf.SHA256(key);
            SetResult(buf);
        }

        private void btnSHA384_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var key = GetPass();

            buf = buf.SHA384(key);
            SetResult(buf);
        }

        private void btnSHA512_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var key = GetPass();

            buf = buf.SHA512(key);
            SetResult(buf);
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
            buf = rs.GetBytes(false);
            SetResult("/*数字、HEX编码、Base64编码*/", rs + "", buf.ToHex(), buf.ToBase64());
        }

        private void btnDES_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var pass = GetPass();

            var des = new DESCryptoServiceProvider();
            buf = des.Encrypt(buf, pass);

            SetResult(buf);
        }

        private void btnDES2_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var pass = GetPass();

            var des = new DESCryptoServiceProvider();
            buf = des.Decrypt(buf, pass);

            SetResult(buf);
        }

        private void btnAES_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var pass = GetPass();

            var aes = new AesCryptoServiceProvider();
            buf = aes.Encrypt(buf, pass);

            SetResult(buf);
        }

        private void btnAES2_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var pass = GetPass();

            var aes = new AesCryptoServiceProvider();
            buf = aes.Decrypt(buf, pass);

            SetResult(buf);
        }

        private void btnRC4_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var pass = GetPass();
            buf = buf.RC4(pass);

            SetResult(buf);
        }

        private void btnRC42_Click(Object sender, EventArgs e)
        {
            var buf = GetSource();
            var pass = GetPass();
            buf = buf.RC4(pass);

            SetResult(buf);
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

            var dt = v.ToDateTime();
            if (dt.Year > 1 && dt.Year < 3000)
            {
                sb.AppendLine("Unix秒：" + dt.ToInt());
                sb.AppendLine("Unix毫秒：" + dt.ToLong());
            }

            var now = DateTime.Now;
            var n = v.ToLong();
            if (n >= Int32.MaxValue)
            {
                dt = n.ToDateTime();
                if (dt.Year > 1000 && dt.Year < 3000)
                    sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss.fff} (Unix秒)\r\n", dt);

                //sb.AppendFormat("过去：{0:yyyy-MM-dd HH:mm:ss.fff}\r\n", now.AddMilliseconds(-n));
                //sb.AppendFormat("未来：{0:yyyy-MM-dd HH:mm:ss.fff}\r\n", now.AddMilliseconds(n));
            }
            else if (n > 0)
            {
                dt = v.ToInt().ToDateTime();
                if (dt.Year > 1000 && dt.Year < 3000)
                    sb.AppendFormat("时间：{0:yyyy-MM-dd HH:mm:ss} (Unix秒)\r\n", dt);

                //sb.AppendFormat("过去：{0:yyyy-MM-dd HH:mm:ss}\r\n", now.AddSeconds(-n));
                //sb.AppendFormat("未来：{0:yyyy-MM-dd HH:mm:ss}\r\n", now.AddSeconds(n));
            }

            // 有可能是过去时间或者未来时间戳
            if (n > 0)
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
        #endregion

        #region 机器信息
        private void BtnComputerInfo_Click(Object sender, EventArgs e)
        {
            var sb = Pool.StringBuilder.Get();

            var macs = GetMacs().ToList();
            if (macs.Count > 0) sb.AppendFormat("MAC:\t{0}\r\n", macs.Join(",", x => x.ToHex("-")));

            var processor = GetInfo("Win32_Processor", "Name");
            /*if (!processor.IsNullOrEmpty())*/
            sb.AppendFormat("Processor:\t{0}\t(Win32_Processor.Name)\r\n", processor);

            var cpuID = GetInfo("Win32_Processor", "ProcessorId");
            /*if (!cpuID.IsNullOrEmpty())*/
            sb.AppendFormat("ProcessorId:\t{0}\t(Win32_Processor.ProcessorId)\r\n", cpuID);

            var uuid = GetInfo("Win32_ComputerSystemProduct", "UUID");
            /*if (!uuid.IsNullOrEmpty())*/
            sb.AppendFormat("UUID:\t{0}\t(Win32_ComputerSystemProduct.UUID)\r\n", uuid);

            var id = GetInfo("Win32_ComputerSystemProduct", "IdentifyingNumber");
            /*if (!id.IsNullOrEmpty())*/
            sb.AppendFormat("IdentifyingNumber:\t{0}\t(Win32_ComputerSystemProduct.IdentifyingNumber)\r\n", id);

            var bios = GetInfo("Win32_BIOS", "SerialNumber");
            /*if (!bios.IsNullOrEmpty())*/
            sb.AppendFormat("BIOS:\t{0}\t(Win32_BIOS.SerialNumber)\r\n", bios);

            var baseBoard = GetInfo("Win32_BaseBoard", "SerialNumber");
            /*if (!baseBoard.IsNullOrEmpty())*/
            sb.AppendFormat("BaseBoard:\t{0}\t(Win32_BaseBoard.SerialNumber)\r\n", baseBoard);

            var serialNumber = GetInfo("Win32_DiskDrive", "SerialNumber");
            /*if (!serialNumber.IsNullOrEmpty())*/
            sb.AppendFormat("DiskSerial:\t{0}\t(Win32_DiskDrive.SerialNumber)\r\n", serialNumber);

            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
            if (reg != null)
            {
                var guid = reg.GetValue("MachineGuid") + "";
                /*if (!guid.IsNullOrEmpty())*/
                sb.AppendFormat("MachineGuid:\t{0}\t(SOFTWARE\\Microsoft\\Cryptography)\r\n", guid);
            }

#if !NET4 && !__CORE__
            sb.AppendLine();
            var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
            foreach (var pi in ci.GetType().GetProperties())
            {
                //if (sb.Length > 0) sb.AppendLine();
                sb.AppendFormat("{0}:\t{1:n0}\r\n", pi.Name, ci.GetValue(pi));
            }
#endif

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
            //var sb = new StringBuilder(bbs.Count * 15);
            //foreach (var s in bbs)
            //{
            //    if (sb.Length > 0) sb.Append(",");
            //    sb.Append(s);
            //}
            //return sb.ToString().Trim();
        }
        #endregion
    }
}