using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using NewLife.Net;
using NewLife.Reflection;
using NewLife.Threading;
using NewLife.Windows;
using XCoder;
using XCoder.XNet;
#if !NET4
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace XNet
{
    [DisplayName("IP设置")]
    public partial class FrmIp : Form, IXForm
    {
        #region 窗体
        public FrmIp()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();

            Icon = IcoHelper.GetIcon("IP");
        }

        private void FrmMain_Load(Object sender, EventArgs e)
        {
            var ns = NetworkInterface.GetAllNetworkInterfaces();
            cbAdapter.Tag = ns;
            cbAdapter.DataSource = ns.Select(e => e.Name).ToArray();
        }
        #endregion

        private void cbMode_SelectedIndexChanged(Object sender, EventArgs e)
        {
            var name = cbAdapter.SelectedItem + "";
            if (name.IsNullOrEmpty()) return;

            var ns = cbAdapter.Tag as NetworkInterface[];
            var ni = ns.FirstOrDefault(e => e.Name == name);
            if (ni == null) return;

            gbInfo.Tag = ni;
            gbInfo.Text = ni.Description;

            var ps = ni.GetIPProperties();
            var ips = ps.UnicastAddresses.Where(e => e.Address.IsIPv4()).ToArray();
            if (ips.Length == 0) return;

            txtIp.Text = ips[0].Address + "";
            txtSubMark.Text = ips[0].IPv4Mask + "";
            txtGateway.Text = ps.GatewayAddresses.Where(e => e.Address.IsIPv4()).Join(",", e => e.Address);
            txtDns.Text = ps.DnsAddresses.Where(e => e.IsIPv4()).Join();

            var ips2 = ips.Skip(1).OrderBy(e => e.Address.GetAddressBytes().ToLong()).Select(e => e.Address).ToArray();
            txtIp2.Text = ips2.Join("\r\n");
        }

        private void btnSet_Click(Object sender, EventArgs e)
        {

        }

        private void btnRestore_Click(Object sender, EventArgs e)
        {
            var ni = gbInfo.Tag as NetworkInterface;
            if (ni == null) return;

            var args = $"interface ip set address name=\"{ni.Name}\" source=dhcp";
            "netsh".Run(args);
        }
    }
}