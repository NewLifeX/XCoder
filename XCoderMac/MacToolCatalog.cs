using System;
using Avalonia.Controls;

namespace XCoderMac;

/// <summary>Mac 导航工具目录；仅引用现有 XCoderAv 工具窗口类型，不改变其实现。</summary>
public sealed record MacTool(string Title, string Description, string Category, string Icon, Type WindowType);

public sealed record MacCategory(string Name, string Icon);

public static class MacToolCatalog
{
    public static readonly MacCategory[] Categories =
    [
        new("开发与数据", "⌘"),
        new("网络与协议", "◇"),
        new("设备与运维", "⚙"),
        new("效率工具", "✦"),
    ];

    public static readonly MacTool[] Tools =
    [
        new("数据建模", "数据库模型设计与实体代码生成", "开发与数据", "▦", typeof(XCoderMac.Views.MacDataModelingWindow)),
        new("数据同步", "跨数据库数据迁移与同步", "开发与数据", "⇄", typeof(XCoderAv.Views.DataSyncWindow)),
        new("API 调试", "NewLife.Remoting RPC 连接与调用", "开发与数据", "⌘", typeof(XCoderMac.Views.MacApiDebugWindow)),
        new("API 发现", "服务接口扫描与发现", "开发与数据", "◎", typeof(XCoderAv.Views.ApiDiscoverWindow)),
        new("正则表达式", "匹配、替换及捕获组分析", "开发与数据", ".*", typeof(XCoderMac.Views.MacRegexWindow)),
        new("加密解密", "摘要、编码和安全工具", "开发与数据", "⌁", typeof(XCoderAv.Views.SecurityWindow)),
        new("文件编码", "文件字符集查看与转换", "开发与数据", "¶", typeof(XCoderAv.Views.FileEncodingWindow)),

        new("网络工具", "TCP / UDP 通信调试", "网络与协议", "⌁", typeof(XCoderMac.Views.MacNetworkWindow)),
        new("消息调试", "消息协议收发及分析", "网络与协议", "◇", typeof(XCoderAv.Views.MessageDebugWindow)),
        new("MQTT 客户端", "MQTT Broker 连接及消息订阅", "网络与协议", "↗", typeof(XCoderAv.Views.MqttWindow)),
        new("Modbus TCP", "工业设备 TCP 协议调试", "网络与协议", "⇄", typeof(XCoderAv.Views.ModbusTcpWindow)),
        new("Modbus RTU", "工业串口 Modbus RTU 通信", "网络与协议", "⇋", typeof(XCoderAv.Views.ModbusRtuWindow)),
        new("串口工具", "串口设备通信与数据监控", "网络与协议", "↔", typeof(XCoderAv.Views.SerialPortWindow)),
        new("SSH 工具", "远程服务器终端连接", "网络与协议", ">_", typeof(XCoderAv.Views.SshWindow)),

        new("Redis 管理", "缓存连接、键值浏览及维护", "设备与运维", "◫", typeof(XCoderMac.Views.MacRedisWindow)),
        new("IP 设置", "网卡状态和网络地址配置", "设备与运维", "⊞", typeof(XCoderAv.Views.IpConfigWindow)),
        new("USB 设备", "查看本机 USB 接入设备", "设备与运维", "⎙", typeof(XCoderMac.Views.MacUsbWindow)),
        new("声卡选择", "音频设备查询与选择", "设备与运维", "♫", typeof(XCoderMac.Views.MacAudioWindow)),
        new("I/O 控制", "设备输入输出控制及调试", "设备与运维", "⚡", typeof(XCoderAv.Views.IoControlWindow)),

        new("图标水印", "图像处理、图标和水印工具", "效率工具", "◇", typeof(XCoderAv.Views.IconToolWindow)),
        new("文件夹统计", "磁盘目录大小及文件统计", "效率工具", "▤", typeof(XCoderAv.Views.FolderStatWindow)),
        new("GPS 辅助", "定位数据查看与辅助测试", "效率工具", "◎", typeof(XCoderAv.Views.GpsWindow)),
        new("地图接口", "地理编码及地图服务接口", "效率工具", "◈", typeof(XCoderAv.Views.MapWindow)),
        new("手机备份", "设备资料整理与备份", "效率工具", "▣", typeof(XCoderAv.Views.BackupWindow)),
        new("语音助手", "语音合成与辅助功能", "效率工具", "♪", typeof(XCoderMac.Views.MacSpeechWindow)),
        new("梅尔频谱", "音频频谱与数据分析", "效率工具", "≋", typeof(XCoderAv.Views.MelSpectrumWindow)),
    ];
}