using NewLife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.FormComponent
{
    public static class ComboBoxHelper
    {
        private static bool needUp(this ComboBox cb, String[] strs)
        {
            var oldds = cb.DataSource as String[];
            bool up = false;
            if (oldds == null) return true;
            if (strs.Length != oldds.Length) return true;
            for (int i = 0; i < strs.Length; i++)
            {
                if (!oldds.Contains(strs[i])) return true;
            }

            return false;
        }

        /// <summary>更新数据源</summary>
        /// <param name="cb"></param>
        /// <param name="strs">需要先排序再传入</param>
        public static void UpdateDataSource(this ComboBox cb, String[] strs)
        {
            if (!needUp(cb, strs)) return;

            // 更新列表 + 恢复选择
            var curr = cb.SelectedItem as string;
            cb.DataSource = strs;

            if (curr.IsNullOrEmpty()) return;
            if (!strs.Contains(curr)) return;
            cb.SelectedItem = curr;
        }

        /// <summary>更新数据源</summary>
        /// <param name="cb"></param>
        /// <param name="strs">需要先排序再传入</param>
        public static void UpdateDataSource(this ComboBox[] cbs, String[] strs)
        {
            foreach (var item in cbs)
            {
                UpdateDataSource(item, strs.Clone() as string[]);
            }
        }

        /// <summary>获取串口名称</summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        public static String GetComName(this ComboBox cb)
        {
            var str = cb.SelectedItem as string;
            if (str == null) return null;

            // "COM22"
            if (str.StartsWith("COM")) return str;

            // "USB-SERIAL CH340 (COM22)"
            var head = str.IndexOf("(COM", 0);
            if (head < 0) return null;
            var tail = str.IndexOf(")", head);
            var comName = str.Substring(head + 1, tail - head - 1);

            return comName;
        }
    }
}
