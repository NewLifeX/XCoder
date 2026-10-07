using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alib.WinForm
{
    public class FormAction
    {
        #region IP/MAC <=> String 转换

        public static IPAddress StringToIPAddress(String str)
        {
            try
            {
                var ip = IPAddress.Parse(str);
                return ip;
            }
            catch
            {
                return new IPAddress(new Byte[] { 0x00, 0x00, 0x00, 0x00 });
            }
        }

        public static string IpToString(byte[] ip, string def)
        {
            if (ip == null) return def;
            if (ip.Length < 4) return def;

            var xx = new IPAddress(ip).ToString();
            return xx;
        }

        public static string MacToString(byte[] mac)
        {
            if (mac == null) return "";
            if (mac.Length < 6) return "";

            return mac.ToHex(":");
        }

        public static Byte[] StringToMac(String str)
        {
            var mac = new Byte[6];
            if (str.IsNullOrWhiteSpace()) return mac;

            str = str.Replace(",", "").Replace(":", "").Replace(" ", "");
            var mac2 = str.ToHex();

            var len = mac2.Length > 6 ? 6 : mac2.Length;
            Array.Copy(mac2, mac, len);

            return mac;
        }

        #endregion

        #region TextBox 通用输入动作过滤

        /// <summary>端口输入框 KeyPress 事件，限制输入仅为数字和小数点</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void IPInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b') return;

            if ((e.KeyChar < '0') || (e.KeyChar > '9'))
            {
                if (e.KeyChar != '.')
                {
                    e.Handled = true;
                    return;
                }
            }

            var box = sender as TextBox;
            var str = box.Text;

            var nums = str.Split('.');
            if (e.KeyChar == '.')
            {
                if (nums.Count() > 3)
                {
                    e.Handled = true;
                    return;
                }
            }

            var strnum = nums[nums.Count() - 1] + e.KeyChar;
            var num = strnum.ToInt();
            if (num > 255) e.Handled = true;
        }

        /// <summary>端口输入过滤</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void PortInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b') return;

            if ((e.KeyChar < '0') || (e.KeyChar > '9'))
            {
                e.Handled = true;
                return;
            }

            var box = sender as TextBox;
            var str = box.Text + e.KeyChar;

            var port = str.ToInt();
            if (port > 65535) e.Handled = true;
        }

        /// <summary>数字输入过滤</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void NumInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b') return;

            if ((e.KeyChar < '0') || (e.KeyChar > '9'))
            {
                e.Handled = true;
                return;
            }
        }

        #endregion
    }
}
