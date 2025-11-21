using System;
using System.ComponentModel;
using XCode.Code;

namespace XCode;

public class CustomBuilderOption : EntityBuilderOption
{
    /// <summary>
    /// 是否创建用户自定义业务文件
    /// </summary>
    [Description("是否创建用户自定义业务文件")]
    public Boolean CreateCustomBizFile { get; set; } = false;

    /// <summary>
    /// 是否在生成.biz.cs文件时覆盖已存在的文件
    /// </summary>
    [Description("是否在生成.biz.cs文件时覆盖已存在的文件")]
    public Boolean OverwriteBizFile { get; set; } = false;
}
