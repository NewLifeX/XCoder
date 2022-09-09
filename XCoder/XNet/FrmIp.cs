using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using NewLife;
using NewLife.Log;
using XCoder;

namespace XNet;

[Category("网络通信")]
[DisplayName("IP设置")]
public partial class FrmIp : Form, IXForm
{
    #region 窗体
    public FrmIp()
    {
        InitializeComponent();

        // 动态调节宽度高度，兼容高DPI
        this.FixDpi();

        //Icon = IcoHelper.GetIcon("IP");
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
        var ni = gbInfo.Tag as NetworkInterface;
        if (ni == null) return;

        var ip = txtIp.Text?.Trim();
        var mark = txtSubMark.Text?.Trim();
        var gateway = txtGateway.Text?.Trim();
        if (ip.IsNullOrEmpty() || mark.IsNullOrEmpty()) return;

        // 设置主IP
        var args = $"interface ip add address name=\"{ni.Name}\" {ip} {mark} {gateway}";
        var rs = "netsh".Run(args, 5_000, s => XTrace.WriteLine(s));

        // 设置DNS
        var dns = txtDns.Text.Split(",");
        if (dns.Length > 0)
        {
            args = $"interface ip set dns name=\"{ni.Name}\" source=static addr={dns[0]} register=primary";
            rs = "netsh".Run(args, 5_000, s => XTrace.WriteLine(s));
            if (dns.Length > 1)
            {
                args = $"interface ip add dnsservers name=\"{ni.Name}\" addr={dns[1]} index=2";
                rs = "netsh".Run(args, 5_000, s => XTrace.WriteLine(s));
            }
        }
        else
        {
            args = $"interface ip set dns name=\"{ni.Name}\" source=dhcp";
            rs = "netsh".Run(args, 5_000, s => XTrace.WriteLine(s));
        }

        // 解析私有IP，特殊格式如 10.0.0.30-50
        var ips = txtIp2.Text.Split("\r", "\n", "\t", ",", " ").ToList();
        // 倒序，要拆分插入末尾
        for (var i = ips.Count - 1; i >= 0; i--)
        {
            ip = ips[i];
            var p = ip.LastIndexOf('-');
            if (p > 0)
            {
                var p2 = ip.LastIndexOf('.');
                if (p2 > 0)
                {
                    // 删掉这一行，因为要拆分为多行
                    ips.RemoveAt(i);

                    // 解析前缀、开始、结束
                    var prefix = ip.Substring(0, p2 + 1);
                    var start = ip.Substring(p2 + 1, p - p2 - 1).ToInt();
                    var end = ip.Substring(p + 1).ToInt();
                    for (var k = start; k <= end; k++)
                    {
                        ips.Add($"{prefix}{k}");
                    }
                }
            }
        }

        // 设置私有IP，排序
        var addrs = ips.Select(e => IPAddress.Parse(e)).OrderBy(e => e.GetAddressBytes().ToLong()).ToArray();
        foreach (var item in addrs)
        {
            args = $"interface ip add address name=\"{ni.Name}\" {item} {mark} {gateway}";
            rs = "netsh".Run(args, 5_000, s => XTrace.WriteLine(s));
        }

        MessageBox.Show($"执行成功！返回 {rs}");
    }

    private void btnRestore_Click(Object sender, EventArgs e)
    {
        var ni = gbInfo.Tag as NetworkInterface;
        if (ni == null) return;

        var args = $"interface ip set address name=\"{ni.Name}\" source=dhcp";
        var rs = "netsh".Run(args, 5_000, s => XTrace.WriteLine(s));

        args = $"interface ip set dns name=\"{ni.Name}\" source=dhcp";
        rs = "netsh".Run(args, 5_000, s => XTrace.WriteLine(s));

        rs = "ipconfig".Run("/renew");

        MessageBox.Show($"执行成功！返回 {rs}");
    }
}