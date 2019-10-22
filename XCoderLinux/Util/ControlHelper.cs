using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;

namespace XCoder.Util
{
    public static class ControlHelper
    {
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

        public static void AppendValues(this ComboBox comboBox, params Object[] values)
        {
            
            //if (!(comboBox.Model is ListStore listStore)) comboBox.Model = listStore = new ListStore(values);

            //listStore.AppendValues(values);
        }
    }
}
