using System.ComponentModel;

namespace CrazyCoder.Data.Models
{
    /// <summary>
    /// 表模型
    /// </summary>
    internal class TableModel
    {
        [DisplayName("名称")]
        public String Name { get; set; }

        [DisplayName("昵称")]
        public String DisplayName { get; set; }

        /// <summary>启用同步</summary>
        [DisplayName("同步")]
        public Boolean EnableSync { get; set; }

        [DisplayName("源表行数")]
        public Int32 Total { get; set; } = -1;

        [DisplayName("已同步")]
        public Int32 Finish { get; set; } = -1;

        [DisplayName("目标行数")]
        public Int32 Total2 { get; set; } = -1;

        [DisplayName("备注")]
        public String Description{ get; set; }
    }
}