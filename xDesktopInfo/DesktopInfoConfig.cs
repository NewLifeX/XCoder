using NewLife.Configuration;
using NewLife.Xml;
using System.Collections.Generic;
using System.ComponentModel;

namespace xDesktopInfo
{
    /// <summary>
    /// 桌面信息显示配置。
    /// </summary>
    [Description("桌面信息配置")]
    [XmlConfigFile("Config/DesktopInfo.config", 15000)]
    public class DesktopInfoConfig : Config<DesktopInfoConfig>
    {
        /// <summary>显示所在屏幕编号（0=主屏，1=第二屏，以此类推）</summary>
        [Description("显示所在屏幕编号(0=主屏,1=第二屏…)")]
        public int ScreenIndex { get; set; } = 0;

        /// <summary>定时刷新间隔(秒)。1/5/10</summary>
        [Description("定时刷新间隔(秒)。1/5/10")]
        public int RefreshInterval { get; set; } = 10;

        /// <summary>勾选显示的网卡 Id 列表</summary>
        [Description("勾选显示的网卡Id列表")]
        public List<string> SelectedNetCards { get; set; } = new List<string>();
    }
}
