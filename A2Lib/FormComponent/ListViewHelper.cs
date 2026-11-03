using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.FormComponent
{
    public static class ListViewHelper
    {
        /// <summary>获取 ListView 当前可见的行区间（与是否有数据无关）</summary>
        /// <param name="lv"></param>
        /// <returns>(可见行首行行号，行数)，无可见行时返回 (0, 0)</returns>
        public static (int, int) Lines(this ListView lv)
        {
            if (lv == null || !lv.IsHandleCreated || lv.ClientSize.Height <= 0) return (0, 0);

            // 行高与列头高度：有数据时用 GetItemRect 实测；无数据时按字体估算（GetItemRect 会抛异常）
            int rowHeight = 0;
            int headerHeight = 0;
            if (lv.Items.Count > 0)
            {
                var r0 = lv.GetItemRect(0);
                rowHeight = r0.Height;
                headerHeight = r0.Top;
            }
            if (rowHeight <= 0)
            {
                rowHeight = (lv.Font != null ? lv.Font.Height : 12) + 6;
                if (lv.SmallImageList != null && lv.SmallImageList.ImageSize.Height + 4 > rowHeight)
                    rowHeight = lv.SmallImageList.ImageSize.Height + 4;
            }
            if (rowHeight <= 0) return (0, 0);

            // 可见区域：客户区去掉列头（若显示）
            if (headerHeight <= 0 && lv.View == View.Details && lv.HeaderStyle != ColumnHeaderStyle.None)
                headerHeight = rowHeight;   // 无数据时的粗略估算

            int areaHeight = lv.ClientSize.Height - headerHeight;
            if (areaHeight <= 0) return (0, 0);

            // 可见行数（至少 1 行，向上取整，避免末行只露一半被漏算）
            int count = Math.Max(1, (areaHeight + rowHeight - 1) / rowHeight);

            // 首个可见行行号：有数据时取 TopItem，否则为 0
            int first = lv.Items.Count > 0 && lv.TopItem != null ? lv.TopItem.Index : 0;

            return (first, count);
        }

        /// <summary>判断 ListView 是否已滚动到底部（可见区末行即末行）</summary>
        /// <param name="lv"></param>
        /// <returns></returns>
        public static bool IsAtBottom(this ListView lv)
        {
            if (lv == null || lv.Items.Count == 0) return true;

            // 控件尚未布局完成时无法测量，视为在底部，避免丢失跟随
            var (start, count) = lv.Lines();
            if (count <= 0) return true;

            return start + count >= lv.Items.Count;
        }

        /// <summary>让 ListView 滚动到底部，显示最后一行</summary>
        /// <param name="lv"></param>
        public static void ScrollToBottom(this ListView lv)
        {
            if (lv == null || lv.Items.Count == 0) return;

            lv.EnsureVisible(lv.Items.Count - 1);
        }

        /// <summary>判断指定行是否已在可见区内</summary>
        /// <param name="lv"></param>
        /// <param name="index">行号</param>
        /// <returns></returns>
        public static bool IsVisible(this ListView lv, int index)
        {
            if (lv == null || index < 0 || index >= lv.Items.Count) return false;

            // 可见区间：IsAtBottom 已经把“控件未布局好”等情况兜住，此处仅做行号比较
            var (start, count) = lv.Lines();
            if (count <= 0) return false;

            return index >= start && index < start + count;
        }

        /// <summary>
        /// 让选中的行跟随其对应的数据（Tag 标识的对象）移动，保持选中。
        /// 用于表格行位置会随数据刷新而变化的场景，例如新数据行插入到顶部时选中的旧行整体下移。
        /// 行已滚出可见区时不再滚动滚动条，直接取消选中（丢弃跟随目标）。
        /// </summary>
        /// <param name="lv">ListView</param>
        /// <param name="tag">选中行对应的标识对象（与 ListViewItem.Tag 比较，引用相同即认为同一行）</param>
        /// <returns>该行仍在可见区内并保持选中返回 true，否则返回 false</returns>
        public static bool FollowSelection(this ListView lv, Object tag)
        {
            if (lv == null || tag == null) return false;

            for (int i = 0; i < lv.Items.Count; i++)
            {
                var it = lv.Items[i];
                if (!ReferenceEquals(it.Tag, tag)) continue;

                // 数据已跑出屏幕，取消选中，不再滚动滚动条把它拉回来
                if (!lv.IsVisible(i))
                {
                    it.Selected = false;
                    return false;
                }

                if (!it.Selected) it.Selected = true;

                return true;
            }

            // 找不到对应行说明该数据已不在表格中
            return false;
        }
    }
}
