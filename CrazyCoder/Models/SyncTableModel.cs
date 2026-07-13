using System.ComponentModel;

namespace CrazyCoder.Models
{
    /// <summary>同步表模型</summary>
    public class SyncTableModel
    {
        /// <summary>表名</summary>
        [DisplayName("名称")]
        public String Name { get; set; } = "";

        /// <summary>显示名</summary>
        [DisplayName("昵称")]
        public String DisplayName { get; set; } = "";

        /// <summary>启用同步</summary>
        [DisplayName("同步")]
        public Boolean EnableSync { get; set; } = true;

        /// <summary>源表行数</summary>
        [DisplayName("源行数")]
        public Int32 SourceCount { get; set; } = -1;

        /// <summary>目标表行数</summary>
        [DisplayName("目标行数")]
        public Int32 TargetCount { get; set; } = -1;

        /// <summary>已同步行数</summary>
        [DisplayName("已同步")]
        public Int32 SyncCount { get; set; }

        /// <summary>备注</summary>
        [DisplayName("备注")]
        public String Description { get; set; } = "";
    }
}
