using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NewLife;
using NewLife.Log;
using XCode.Code;
using XCode.DataAccessLayer;

namespace XCode;

public class CustomBuilder : EntityBuilder
{
    /// <summary>自定义业务类，仅生成一次</summary>
    public Boolean CustomBusiness { get; set; }

    /// <summary>保存</summary>
    /// <param name="ext"></param>
    /// <param name="overwrite"></param>
    /// <param name="chineseFileName"></param>
    public override String Save(String ext = null, Boolean overwrite = true, Boolean chineseFileName = true)
    {
        if (CustomBusiness)
        {
            ext = ".CusBiz.cs";
            //overwrite = false;
        }
        var p = Option.Output;
        if (ext == ".CusBiz.cs")
        {
            p = p.CombinePath(p, "CustomBiz");
        }
        if (ext.IsNullOrEmpty())
            ext = ".cs";
        else if (!ext.Contains("."))
            ext += ".cs";

        if (chineseFileName && !Table.DisplayName.IsNullOrEmpty())
            p = p.CombinePath(Table.DisplayName + ext);
        else
            p = p.CombinePath(ClassName + ext);

        p = p.GetBasePath();

        if (!File.Exists(p) || overwrite) File.WriteAllText(p.EnsureDirectory(true), ToString(), Encoding.UTF8);

        return p;
    }

    protected override void BuildItems()
    {
        if (CustomBusiness)
            BuildCustomBizDescription();
        else
            base.BuildItems();
    }

    /// <summary>自定义业务逻辑文件说明信息</summary>
    protected virtual void BuildCustomBizDescription()
    {
        WriteLine($"//生成时间:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        WriteLine("//自定义业务逻辑写在本文件中");
        WriteLine("//本文件由代码生成器初次生成后不再重新生成");
        WriteLine("#region 自定义业务逻辑生成");
        WriteLine();
        WriteLine("#endregion");
    }

    #region 静态快速

    /// <summary>为Xml模型文件生成实体类</summary>
    /// <param name="tables">模型文件</param>
    /// <param name="option">生成可选项</param>
    public static Int32 BuildTables(IList<IDataTable> tables, CustomBuilderOption option, ILog log = null)
    {
        if (tables == null || tables.Count == 0) return 0;

        if (option == null)
            option = new CustomBuilderOption();
        else
            option = option.Clone() as CustomBuilderOption;
        //option.Partial = true;

        var output = option.Output;
        if (output.IsNullOrEmpty()) output = ".";
        log?.Info("生成实体类 {0}", output.GetBasePath());

        var count = 0;
        foreach (var item in tables)
        {
            // 跳过排除项
            if (option.Excludes.Contains(item.Name)) continue;
            if (option.Excludes.Contains(item.TableName)) continue;

            var builder = new CustomBuilder
            {
                AllTables = tables,
                Option = option.Clone(),
                Log = log
            };

            var eoption = builder.EntityOption;
            if (eoption.ModelNameForToModel.IsNullOrEmpty())
            {
                if (!eoption.ModelNameForCopy.IsNullOrEmpty() && !eoption.ModelNameForCopy.StartsWith("I"))
                    eoption.ModelNameForToModel = eoption.ModelNameForCopy;
                else
                    eoption.ModelNameForToModel = item.Name;
            }

            builder.Load(item);

            builder.Execute();
            builder.Save(null, true, option.ChineseFileName);

            builder.Clear();
            builder.Business = true;
            builder.Execute();
            builder.Save(null, option.OverwriteBizFile, option.ChineseFileName);
            if (option.CreateCustomBizFile)
            {
                //新增自定义业务文件,仅生成一次
                builder.Clear();
                builder.CustomBusiness = true;
                builder.Execute();
                builder.Save(null, false, option.ChineseFileName);
                count++;
            }
        }

        return count;
    }

    #endregion 静态快速
}
