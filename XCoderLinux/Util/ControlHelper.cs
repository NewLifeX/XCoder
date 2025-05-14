using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using NewLife;

namespace XCoder.Util
{
    public static class ControlHelper
    {
        /// <summary>
        /// 获取当前选择项
        /// </summary>
        /// <param name="comboBox"></param>
        /// <returns></returns>
        public static Object GetActiveObject(this ComboBox comboBox)
        {
            if (comboBox.Active == -1) return null;
            if (!(comboBox.Model is ListStore listStore)) return null;

            var i = 0;

            if (listStore.Cast<Object>().FirstOrDefault(item => i++ == comboBox.Active) is Object[] arr && arr.Length > 0)
            {
                return arr[0];
            }

            return null;
        }

        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="values"></param>
        public static void AppendValues<T>(this ComboBox comboBox, List<T> values)
        {
            if (!(comboBox.Model is ListStore listStore))
            {
                return;
            }

            foreach (var value in values)
            {
                listStore.AppendValues(value);
            }
        }

        /// <summary>
        /// 添加选项
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="value"></param>
        public static void SetActive<T>(this ComboBox comboBox, T value)
        {
            if (!(comboBox.Model is ListStore listStore))
            {
                return;
            }

            var i = 0;
            foreach (var item in listStore)
            {
                if (value.Equals(((Object[])item)[0].ToInt()))
                {
                    comboBox.Active = i;
                    return;
                }

                i++;
            }
        }


    }

    public static class MessageBox
    {
        public static void Show(String text)
        {
            if (Window.ListToplevels().Length < 1) return;

            var md = new MessageDialog(Window.ListToplevels()[0],
                DialogFlags.DestroyWithParent, MessageType.Warning,
                ButtonsType.Close, text);
            md.Run();
            md.Dispose();
        }
    }
}
