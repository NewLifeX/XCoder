using System;

namespace XCoderAv.Models;

/// <summary>菜单项模型</summary>
public class MenuModel
{
    /// <summary>图标字体编码</summary>
    public String IconFont { get; set; } = "";

    /// <summary>标题</summary>
    public String Title { get; set; } = "";

    /// <summary>背景颜色</summary>
    public String BackColor { get; set; } = "#888";

    /// <summary>关联视图类型</summary>
    public Type? ViewType { get; set; }
}