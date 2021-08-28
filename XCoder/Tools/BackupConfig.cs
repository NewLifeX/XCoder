using System;
using NewLife.Xml;

namespace XCoder.Tools
{
    /// <summary>Backup配置</summary>
    [XmlConfigFile("Config\\Backup.config")]
    public class BackupConfig : XmlConfig<BackupConfig>
    {
        #region 属性
        public String DestDir { get; set; }

        public Boolean AllowDelete { get; set; }

        public String SrcDir1 { get; set; }
        public String SrcDir2 { get; set; }
        public String SrcDir3 { get; set; }
        public String SrcDir4 { get; set; }
        public String SrcDir5 { get; set; }
        #endregion

        #region 方法
        #endregion
    }
}