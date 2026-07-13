using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NewLife;

namespace XCoderAv.ViewModels;

/// <summary>IP 设置工具 ViewModel</summary>
public partial class IpConfigViewModel : ObservableObject
{
    #region 属性
    /// <summary>网络适配器列表</summary>
    [ObservableProperty]
    private String[] _adapters = [];

    /// <summary>选中的适配器名称</summary>
    [ObservableProperty]
    private String _selectedAdapter = "";

    /// <summary>适配器描述</summary>
    [ObservableProperty]
    private String _adapterDescription = "";

    /// <summary>IP 地址</summary>
    [ObservableProperty]
    private String _ipAddress = "";

    /// <summary>子网掩码</summary>
    [ObservableProperty]
    private String _subnetMask = "";

    /// <summary>网关</summary>
    [ObservableProperty]
    private String _gateway = "";

    /// <summary>DNS</summary>
    [ObservableProperty]
    private String _dns = "";

    /// <summary>辅助 IP（多行）</summary>
    [ObservableProperty]
    private String _secondaryIps = "";

    /// <summary>日志输出</summary>
    [ObservableProperty]
    private String _log = "";
    #endregion

    #region 构造
    /// <summary>实例化 IP 设置工具 ViewModel</summary>
    public IpConfigViewModel()
    {
        LoadAdapters();
    }
    #endregion

    #region 方法
    private void LoadAdapters()
    {
        var ns = NetworkInterface.GetAllNetworkInterfaces();
        Adapters = ns.Select(e => e.Name).ToArray();
        if (Adapters.Length > 0) SelectedAdapter = Adapters[0];
    }

    partial void OnSelectedAdapterChanged(String value)
    {
        if (value.IsNullOrEmpty()) return;

        var ns = NetworkInterface.GetAllNetworkInterfaces();
        var ni = ns.FirstOrDefault(e => e.Name == value);
        if (ni == null) return;

        AdapterDescription = ni.Description;

        var ps = ni.GetIPProperties();
        var ips = ps.UnicastAddresses.Where(e => e.Address.IsIPv4()).ToArray();
        if (ips.Length == 0) return;

        IpAddress = ips[0].Address + "";
        SubnetMask = ips[0].IPv4Mask + "";
        Gateway = ps.GatewayAddresses.Where(e => e.Address.IsIPv4()).Join(",", e => e.Address);
        Dns = ps.DnsAddresses.Where(e => e.IsIPv4()).Join();

        var ips2 = ips.Skip(1).OrderBy(e => e.Address.GetAddressBytes().ToLong()).Select(e => e.Address).ToArray();
        SecondaryIps = ips2.Join("\r\n");
    }

    /// <summary>设置 IP</summary>
    [RelayCommand]
    private void Apply()
    {
        var ip = IpAddress?.Trim();
        var mark = SubnetMask?.Trim();
        var gateway = Gateway?.Trim();
        if (ip.IsNullOrEmpty() || mark.IsNullOrEmpty())
        {
            Log += "IP 地址和子网掩码不能为空\r\n";
            return;
        }

        var name = SelectedAdapter;
        if (name.IsNullOrEmpty()) return;

        AppendLog($"设置主 IP: {ip} {mark} {gateway}");
        var args = $"interface ip add address name=\"{name}\" {ip} {mark} {gateway}";
        var rs = "netsh".Run(args, 5_000, s => AppendLog(s));
        AppendLog($"结果: {rs}");

        var dns = Dns?.Split(',');
        if (dns != null && dns.Length > 0 && !dns[0].IsNullOrEmpty())
        {
            args = $"interface ip set dns name=\"{name}\" source=static addr={dns[0]} register=primary";
            rs = "netsh".Run(args, 5_000, s => AppendLog(s));
            if (dns.Length > 1)
            {
                args = $"interface ip add dnsservers name=\"{name}\" addr={dns[1]} index=2";
                rs = "netsh".Run(args, 5_000, s => AppendLog(s));
            }
        }
        else
        {
            args = $"interface ip set dns name=\"{name}\" source=dhcp";
            rs = "netsh".Run(args, 5_000, s => AppendLog(s));
        }

        var ips = SecondaryIps.Split("\r", "\n", "\t", ",", " ").ToList();
        for (var i = ips.Count - 1; i >= 0; i--)
        {
            var addr = ips[i];
            var p = addr.LastIndexOf('-');
            if (p > 0)
            {
                var p2 = addr.LastIndexOf('.');
                if (p2 > 0)
                {
                    ips.RemoveAt(i);
                    var prefix = addr.Substring(0, p2 + 1);
                    var start = addr.Substring(p2 + 1, p - p2 - 1).ToInt();
                    var end = addr.Substring(p + 1).ToInt();
                    for (var k = start; k <= end; k++)
                    {
                        ips.Add($"{prefix}{k}");
                    }
                }
            }
        }

        var addrs = ips.Select(e => IPAddress.Parse(e)).OrderBy(e => e.GetAddressBytes().ToLong()).ToArray();
        foreach (var item in addrs)
        {
            args = $"interface ip add address name=\"{name}\" {item} {mark} {gateway}";
            rs = "netsh".Run(args, 5_000, s => AppendLog(s));
        }

        AppendLog("IP 设置完成");
    }

    /// <summary>恢复 DHCP</summary>
    [RelayCommand]
    private void Restore()
    {
        var name = SelectedAdapter;
        if (name.IsNullOrEmpty()) return;

        AppendLog("恢复 DHCP...");
        var args = $"interface ip set address name=\"{name}\" source=dhcp";
        var rs = "netsh".Run(args, 5_000, s => AppendLog(s));

        args = $"interface ip set dns name=\"{name}\" source=dhcp";
        rs = "netsh".Run(args, 5_000, s => AppendLog(s));

        rs = "ipconfig".Run("/renew", 10_000, s => AppendLog(s));
        AppendLog("DHCP 恢复完成");
    }

    private void AppendLog(String msg)
    {
        Log += $"{msg}\r\n";
    }
    #endregion
}
