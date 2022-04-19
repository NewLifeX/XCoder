using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NewLife;
using NewLife.Reflection;
using NewLife.Serialization;

namespace XCoder.Common
{
    /// <summary>控件配置</summary>
    internal class ControlConfig
    {
        public Control Control { get; set; }

        public String FileName { get; set; }

        public IDictionary<String, Object> Items { get; set; } = new Dictionary<String, Object>();

        public void Load()
        {
            var dataPath = Setting.Current.DataPath;
            var file = dataPath.CombinePath(FileName).GetFullPath();
            if (File.Exists(file))
            {
                var dic = JsonParser.Decode(File.ReadAllText(file));
                LoadConfig(dic, Control);

                Items.Merge(dic);
            }
        }

        private void LoadConfig(IDictionary<String, Object> dic, Control control)
        {
            foreach (Control item in control.Controls)
            {
                switch (item)
                {
                    case TextBoxBase txt:
                        if (dic.TryGetValue(item.Name, out var v)) txt.Text = v + "";
                        break;
                    case RadioButton rb:
                        if (dic.TryGetValue(item.Name, out v)) rb.Checked = v.ToBoolean();
                        break;
                    case CheckBox cb:
                        if (dic.TryGetValue(item.Name, out v)) cb.Checked = v.ToBoolean();
                        break;
                    case NumericUpDown nud:
                        if (dic.TryGetValue(item.Name, out v)) nud.Value = v.ToInt();
                        break;
                    case ComboBox cbox:
                        if (dic.TryGetValue(item.Name, out v))
                        {
                            if (cbox.DropDownStyle == ComboBoxStyle.DropDownList)
                            {
                                var type = cbox.Items[0].GetType();
                                v = v.ChangeType(type);

                                if (cbox.Items.Count > 0 && type.GetTypeCode() == TypeCode.Object)
                                    cbox.SelectedValue = v;
                                else
                                    cbox.SelectedItem = v;
                            }
                            else
                                cbox.DataSource = (v + "").Split(",");
                        }
                        break;
                    default:
                        if (item.Controls.Count > 0) LoadConfig(dic, item);
                        break;
                }
            }
        }

        public void Save()
        {
            //var dic = new Dictionary<String, Object>();
            var dic = Items;
            SaveConfig(dic, Control);

            var dataPath = Setting.Current.DataPath;
            var file = dataPath.CombinePath(FileName).GetFullPath();
            file.EnsureDirectory(true);
            File.WriteAllText(file, dic.ToJson(true));
        }

        private void SaveConfig(IDictionary<String, Object> dic, Control control)
        {
            foreach (Control item in control.Controls)
            {
                switch (item)
                {
                    case TextBoxBase txt: dic[item.Name] = txt.Text; break;
                    case RadioButton rb: dic[item.Name] = rb.Checked; break;
                    case CheckBox cb: dic[item.Name] = cb.Checked; break;
                    case NumericUpDown nud: dic[item.Name] = nud.Value; break;
                    case ComboBox cbox:
                        if (cbox.DropDownStyle == ComboBoxStyle.DropDownList)
                        {
                            if (cbox.SelectedValue != null)
                                dic[item.Name] = cbox.SelectedValue;
                            else
                                dic[item.Name] = cbox.SelectedItem;
                        }
                        else
                        {
                            var list = new List<String> { cbox.Text };
                            for (var i = 0; i < cbox.Items.Count; i++)
                            {
                                var elm = cbox.Items[i];
                                if (elm != null) list.Add(elm + "");
                            }
                            dic[item.Name] = list.Where(e => !e.IsNullOrEmpty()).Distinct().Join();
                        }
                        break;
                    default:
                        if (item.Controls.Count > 0) SaveConfig(dic, item);
                        break;
                }
            }
        }
    }
}