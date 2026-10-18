using ALib.Can_SAEJ1939;
using ALib.WinForm;
using NewLife;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ALib.CAN
{
    /// <summary>
    /// DBC 文件查看器。
    /// 加载 Vector CANdb++ 导出的 DBC 后，按「报文 / SPN / 取值表 / 节点 / 属性」五个视图呈现，
    /// 并提供全局搜索定位。所有内容只读，用于核对 DBC 定义与实际报文是否一致。
    /// </summary>
    public partial class DbcViewer : Form
    {
        public DbcViewer()
        {
            InitializeComponent();
        }

        #region 属性

        /// <summary>当前加载的 DBC 数据库</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DbcDatabase Database { get; private set; }

        /// <summary>日志</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>上一次使用的目录，用于文件对话框记忆位置</summary>
        private String _lastDir;

        #endregion

        #region 初始化

        private void DbcViewer_Load(Object sender, EventArgs e)
        {
            lvDetail.EnableDoubleBuffer();
            lvSearch.EnableDoubleBuffer();

            _lastDir = AppDomain.CurrentDomain.BaseDirectory;

            // 启动时若程序目录下已有 test.dbc，直接加载，省去手工选文件
            var def = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.dbc");
            if (File.Exists(def)) LoadFile(def, false);

            // 拖入文件即加载，便于快速核对任意 DBC
            AllowDrop = true;
            DragEnter += (s, e2) =>
            {
                if (e2.Data.GetDataPresent(DataFormats.FileDrop)) e2.Effect = DragDropEffects.Copy;
            };
            DragDrop += (s, e2) =>
            {
                if (!e2.Data.GetDataPresent(DataFormats.FileDrop)) return;
                if (e2.Data.GetData(DataFormats.FileDrop) is String[] files && files.Length > 0) LoadFile(files[0]);
            };
        }

        #endregion

        #region 加载

        private void btOpen_Click(Object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "打开 DBC 文件",
                Filter = "DBC 文件 (*.dbc)|*.dbc|所有文件 (*.*)|*.*",
                InitialDirectory = _lastDir,
            };

            if (dlg.ShowDialog(this) == DialogResult.OK) LoadFile(dlg.FileName);
        }

        /// <summary>重新加载当前文件（文件被外部工具改动后核对用）</summary>
        private void btReload_Click(Object sender, EventArgs e)
        {
            var path = Database?.FilePath;
            if (String.IsNullOrEmpty(path)) { MessageBox.Show(this, "尚未加载 DBC 文件。", Text); return; }
            LoadFile(path);
        }

        /// <summary>加载 DBC 文件并刷新界面</summary>
        /// <param name="path">文件路径</param>
        /// <param name="showError">读取失败时是否弹窗</param>
        public Boolean LoadFile(String path, Boolean showError = true)
        {
            try
            {
                var db = DbcDatabase.LoadFile(path);

                Database = db;
                _lastDir = Path.GetDirectoryName(path);

                Text = $"DBC 查看器 - {Path.GetFileName(path)}";
                lblPath.Text = path;

                FillTree();
                FillSummary();
                btReload.Enabled = true;

                // 解析告警直接给用户看，避免"少解析了内容却悄无声息"
                if (db.Warnings.Count > 0)
                {
                    lblWarn.Text = $"解析告警 {db.Warnings.Count} 条（鼠标悬停查看）";
                    tipWarn.SetToolTip(lblWarn, String.Join(Environment.NewLine, db.Warnings.Take(30)));
                }
                else
                {
                    lblWarn.Text = "";
                    tipWarn.SetToolTip(lblWarn, null);
                }

                WriteLog($"已加载 {path}：{db}");
                return true;
            }
            catch (Exception ex)
            {
                WriteLog("加载 DBC 失败: " + ex);
                if (showError) MessageBox.Show(this, "加载 DBC 失败：" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        #endregion

        #region 左侧树

        private void FillTree()
        {
            tvMain.BeginUpdate();
            try
            {
                tvMain.Nodes.Clear();

                var db = Database;
                if (db == null) return;

                var root = new TreeNode($"DBC {Path.GetFileName(db.FilePath)}") { Tag = db };

                var msgNode = new TreeNode($"报文 / 信号 ({db.Messages.Count} / {db.Messages.Sum(m => m.Signals.Count)})");
                foreach (var m in db.Messages.OrderBy(m => m.Pgn).ThenBy(m => m.Name))
                {
                    var mn = new TreeNode($"{m.Name}  PGN={m.PgnText}  ID=0x{m.Id:X8}  [{m.Signals.Count}]") { Tag = m };
                    foreach (var s in m.Signals)
                    {
                        var spn = s.Spn;
                        var spnText = spn == null ? "" : $"  SPN={spn}";
                        mn.Nodes.Add(new TreeNode($"{s.Name}{spnText}") { Tag = s });
                    }
                    msgNode.Nodes.Add(mn);
                }
                root.Nodes.Add(msgNode);

                var spnNode = new TreeNode($"SPN ({db.SpnCount})");
                foreach (var spn in db.AllSpns)
                {
                    var sigs = db.FindSignalsBySpn(spn);
                    var sn = new TreeNode($"{DbcDatabase.FormatSpn(spn)}  [{sigs.Count}]") { Tag = spn };
                    foreach (var s in sigs) sn.Nodes.Add(new TreeNode($"{s.Message?.Name}.{s.Name}") { Tag = s });
                    spnNode.Nodes.Add(sn);
                }
                root.Nodes.Add(spnNode);

                var vtNode = new TreeNode($"取值表 ({db.ValueTables.Count})");
                foreach (var vt in db.ValueTables)
                {
                    var vn = new TreeNode($"{vt.Name}  [{vt.Values.Count}]") { Tag = vt };
                    foreach (var kv in vt.Values.OrderByDescending(k => k.Key))
                        vn.Nodes.Add(new TreeNode($"{kv.Key} = {kv.Value}"));
                    vtNode.Nodes.Add(vn);
                }
                root.Nodes.Add(vtNode);

                var nodeNode = new TreeNode($"节点 ({db.Nodes.Count})");
                foreach (var n in db.Nodes) nodeNode.Nodes.Add(new TreeNode(n.Name) { Tag = n });
                root.Nodes.Add(nodeNode);

                var attrNode = new TreeNode($"属性定义 ({db.AttributeDefinitions.Count})");
                foreach (var a in db.AttributeDefinitions)
                {
                    var an = new TreeNode($"{a.Name}  [{a.ObjectType} {a.ValueType}]") { Tag = a };
                    for (int i = 0; i < a.EnumValues.Count; i++) an.Nodes.Add(new TreeNode($"{i} = {a.EnumValues[i]}"));
                    attrNode.Nodes.Add(an);
                }
                root.Nodes.Add(attrNode);

                if (db.EnvVars.Count > 0)
                {
                    var evNode = new TreeNode($"环境变量 ({db.EnvVars.Count})");
                    foreach (var ev in db.EnvVars) evNode.Nodes.Add(new TreeNode(ev.Name) { Tag = ev });
                    root.Nodes.Add(evNode);
                }

                tvMain.Nodes.Add(root);
                root.Expand();
                msgNode.Expand();
                spnNode.Expand();
            }
            finally { tvMain.EndUpdate(); }
        }

        private void tvMain_AfterSelect(Object sender, TreeViewEventArgs e)
        {
            ShowDetail(e.Node?.Tag);
        }

        #endregion

        #region 右侧详情

        /// <summary>按选中对象类型展示详情</summary>
        private void ShowDetail(Object tag)
        {
            lvDetail.BeginUpdate();
            try
            {
                lvDetail.Items.Clear();
                lvDetail.Columns.Clear();

                switch (tag)
                {
                    case null:
                        FillColumns("项目", 220, "内容", 620);
                        AddRow("提示", "请在左侧选择报文、信号、SPN、取值表或节点");
                        break;

                    case DbcDatabase db:
                        ShowDatabaseDetail(db);
                        break;

                    case DbcMessage m:
                        ShowMessageDetail(m);
                        break;

                    case DbcSignal s:
                        ShowSignalDetail(s);
                        break;

                    case long spn:
                        ShowSpnDetail(spn);
                        break;

                    case DbcValueTable vt:
                        FillColumns("原始值", 100, "描述", 740);
                        foreach (var kv in vt.Values.OrderByDescending(k => k.Key)) AddRow(kv.Key.ToString(), kv.Value);
                        break;

                    case DbcNode n:
                        FillColumns("项目", 160, "内容", 680);
                        AddRow("节点名", n.Name);
                        AddRow("注释", n.Comment);
                        foreach (var kv in n.Attributes) AddRow("属性 " + kv.Key, DescribeAttribute(kv.Key, kv.Value));
                        break;

                    case DbcAttributeDefinition a:
                        ShowAttributeDetail(a);
                        break;

                    case DbcEnvVar ev:
                        FillColumns("项目", 160, "内容", 680);
                        AddRow("名称", ev.Name);
                        AddRow("类型", ev.Type.ToString());
                        AddRow("范围", $"{ev.Minimum} ~ {ev.Maximum}");
                        AddRow("单位", ev.Unit);
                        AddRow("初始值", ev.InitialValue.ToString());
                        AddRow("ID", ev.EnvVarId.ToString());
                        AddRow("访问类型", ev.AccessType);
                        AddRow("访问节点", String.Join(",", ev.AccessNodes));
                        AddRow("注释", ev.Comment);
                        break;

                    default:
                        FillColumns("项目", 220, "内容", 620);
                        AddRow("对象", tag.ToString());
                        break;
                }
            }
            finally { lvDetail.EndUpdate(); }
        }

        private void ShowDatabaseDetail(DbcDatabase db)
        {
            FillColumns("项目", 220, "内容", 620);
            AddRow("文件", db.FilePath);
            AddRow("版本", db.Version);
            AddRow("节点", String.Join(", ", db.Nodes.Select(n => n.Name)));
            AddRow("报文数", db.Messages.Count.ToString());
            AddRow("信号数", db.Messages.Sum(m => m.Signals.Count).ToString());
            AddRow("SPN 数", db.SpnCount.ToString());
            AddRow("取值表数", db.ValueTables.Count.ToString());
            AddRow("属性定义", db.AttributeDefinitions.Count.ToString());
            AddRow("解析告警", db.Warnings.Count.ToString());
            foreach (var w in db.Warnings.Take(20)) AddRow("告警", w);
        }

        /// <summary>报文详情：全部信号表</summary>
        private void ShowMessageDetail(DbcMessage m)
        {
            FillColumns("信号", 250, "起始位|长度", 100, "字节序", 80, "系数/偏移", 130, "范围", 130, "单位", 60, "SPN", 90, "FMI", 70, "接收节点", 200);

            foreach (var s in m.Signals)
            {
                var it = new ListViewItem(s.Name);
                it.SubItems.Add($"{s.StartBit}|{s.Length}");
                it.SubItems.Add(s.ByteOrder == DbcByteOrder_e.Intel ? "Intel" : "Motorola");
                it.SubItems.Add($"({s.Factor},{s.Offset})");
                it.SubItems.Add($"[{s.Minimum}|{s.Maximum}]");
                it.SubItems.Add(s.Unit);
                it.SubItems.Add(s.Spn?.ToString() ?? "");
                it.SubItems.Add(s.FmiText ?? "");
                it.SubItems.Add(String.Join(",", s.Receivers));
                it.Tag = s;
                lvDetail.Items.Add(it);
            }

            var fmi = m.CommunicationErrorFmi;
            var head = new StringBuilder();
            head.Append($"报文 {m.Name}  PGN={m.PgnText}(0x{m.Pgn:X5})  ID=0x{m.Id:X8}  优先级={m.Priority}  DLC={m.Size}  " +
                        $"格式={(m.IsPDU1 ? "PDU1" : "PDU2")}  发送={m.Transmitter}");
            if (m.AdditionalTransmitters.Count > 0) head.Append($"  附加发送={String.Join(",", m.AdditionalTransmitters)}");
            if (fmi.Count > 0) head.Append($"  通信错误DTC FMI={String.Join("/", fmi)}");
            if (!String.IsNullOrEmpty(m.Comment)) head.Append(Environment.NewLine + "注释：" + m.Comment);

            lblDetail.Text = head.ToString();
        }

        /// <summary>信号详情：定义、缩放、枚举、SPN/FMI、属性</summary>
        private void ShowSignalDetail(DbcSignal s)
        {
            FillColumns("项目", 200, "内容", 640);

            AddRow("信号名", s.Name);
            AddRow("所属报文", $"{s.Message?.Name}  PGN={s.Message?.PgnText}(0x{s.Message?.Pgn:X5})");
            AddRow("起始位|长度", $"{s.StartBit}|{s.Length}");
            AddRow("字节序", s.ByteOrder == DbcByteOrder_e.Intel ? "Intel（小端 @1）" : "Motorola（大端 @0）");
            AddRow("取值类型", s.ValueType == DbcValueType_e.Unsigned ? "无符号 +" : "有符号 -");
            AddRow("系数/偏移", $"物理值 = 原始值 * {s.Factor} + {s.Offset}");
            AddRow("原始值范围", $"{s.RawMinimum} ~ {s.RawMaximum}");
            AddRow("物理值范围", $"{s.PhysicalMinimum} ~ {s.PhysicalMaximum}");
            AddRow("单位", s.Unit);
            AddRow("接收节点", String.Join(",", s.Receivers));
            AddRow("多路复用", String.IsNullOrEmpty(s.Multiplex) ? "" : s.Multiplex);

            var spn = s.Spn;
            if (spn != null)
            {
                AddRow("SPN", DbcDatabase.FormatSpn(spn.Value));
                var others = Database?.FindSignalsBySpn(spn.Value)
                                     .Where(x => !ReferenceEquals(x, s))
                                     .Select(x => $"{x.Message?.Name}.{x.Name}").ToList();
                if (others != null && others.Count > 0) AddRow("同 SPN 其它信号", String.Join(", ", others));
            }

            var fmi = s.FmiList;
            if (fmi.Count > 0)
            {
                AddRow("FMI", String.Join(";", fmi));
                AddRow("FMI 含义", String.Join("; ", fmi.Select(DescribeFmi)));
            }

            AddRow("注释", s.Comment);

            // 属性逐条列出，便于核对 GenMsgCycleTime / SigType / VFrameFormat 等
            foreach (var kv in s.Attributes.OrderBy(k => k.Key))
                AddRow("属性 " + kv.Key, DescribeAttribute(kv.Key, kv.Value));

            if (s.ValueDescriptions.Count > 0)
            {
                AddRow("", "");
                foreach (var kv in s.ValueDescriptions.OrderByDescending(k => k.Key))
                    AddRow("取值 " + kv.Key, kv.Value);
            }
        }

        /// <summary>SPN 详情：列出承载它的全部信号</summary>
        private void ShowSpnDetail(long spn)
        {
            FillColumns("报文", 250, "信号", 220, "起始位|长度", 100, "系数/偏移", 130, "FMI", 90, "单位", 60);

            var sigs = Database?.FindSignalsBySpn(spn) ?? new List<DbcSignal>();
            foreach (var s in sigs)
            {
                var it = new ListViewItem(s.Message?.Name);
                it.SubItems.Add(s.Name);
                it.SubItems.Add($"{s.StartBit}|{s.Length}");
                it.SubItems.Add($"({s.Factor},{s.Offset})");
                it.SubItems.Add(s.FmiText ?? "");
                it.SubItems.Add(s.Unit);
                it.Tag = s;
                lvDetail.Items.Add(it);
            }

            var msgNames = sigs.Select(x => x.Message?.Name).Where(x => !String.IsNullOrEmpty(x)).Distinct();
            lblDetail.Text = $"SPN={spn} (0x{spn:X})  承载信号 {sigs.Count} 个  分布于：{String.Join(", ", msgNames)}";
        }

        private void ShowAttributeDetail(DbcAttributeDefinition a)
        {
            FillColumns("项目", 160, "内容", 680);
            AddRow("属性名", a.Name);
            AddRow("作用对象", a.ObjectType.ToString());
            AddRow("值类型", a.ValueType.ToString());
            if (a.ValueType == DbcAttrType_e.Int || a.ValueType == DbcAttrType_e.Hex || a.ValueType == DbcAttrType_e.Float)
                AddRow("取值范围", $"{a.Minimum} ~ {a.Maximum}");
            AddRow("默认值", a.DefaultValue?.ToString());
            for (int i = 0; i < a.EnumValues.Count; i++) AddRow($"枚举 {i}", a.EnumValues[i]);
        }

        #endregion

        #region 搜索

        private void btSearch_Click(Object sender, EventArgs e) => DoSearch();

        private void txtSearch_KeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; DoSearch(); }
        }

        /// <summary>
        /// 全局搜索：支持报文名、信号名、SPN 数字、PGN 数字、注释文本。
        /// 输入纯数字时按 SPN 与 PGN 双路匹配。
        /// </summary>
        private void DoSearch()
        {
            var db = Database;
            if (db == null) { MessageBox.Show(this, "尚未加载 DBC 文件。", Text); return; }

            var key = txtSearch.Text.Trim();
            lvSearch.BeginUpdate();
            try
            {
                lvSearch.Items.Clear();
                if (key.Length == 0) { lblSearch.Text = ""; return; }

                var lower = key.ToLowerInvariant();
                var numeric = Int64.TryParse(key, out var number);

                // 1) SPN 精确匹配
                if (numeric)
                {
                    foreach (var s in db.FindSignalsBySpn(number))
                    {
                        lvSearch.Items.Add(new ListViewItem(new[]
                        {
                            "SPN", $"{s.Message?.Name}.{s.Name}",
                            $"PGN={s.Message?.PgnText}", $"位[{s.StartBit}|{s.Length}]",
                            Truncate(s.Comment)
                        }) { Tag = s });
                    }
                }

                // 2) 报文名 / PGN
                foreach (var m in db.Messages)
                {
                    var hit = m.Name.ToLowerInvariant().Contains(lower);
                    if (!hit && numeric) hit = m.Pgn == (UInt32)number;

                    if (hit)
                    {
                        lvSearch.Items.Add(new ListViewItem(new[]
                        {
                            "报文", m.Name,
                            $"PGN={m.PgnText} 0x{m.Pgn:X5}", $"ID=0x{m.Id:X8} DLC={m.Size}",
                            Truncate(m.Comment)
                        }) { Tag = m });
                    }
                }

                // 3) 信号名 / 信号注释
                foreach (var m in db.Messages)
                {
                    foreach (var s in m.Signals)
                    {
                        var hit = s.Name.ToLowerInvariant().Contains(lower);
                        if (!hit && !String.IsNullOrEmpty(s.Comment)) hit = s.Comment.ToLowerInvariant().Contains(lower);

                        if (hit)
                        {
                            lvSearch.Items.Add(new ListViewItem(new[]
                            {
                                "信号", $"{m.Name}.{s.Name}",
                                $"PGN={m.PgnText}", $"位[{s.StartBit}|{s.Length}]",
                                Truncate(s.Comment)
                            }) { Tag = s });
                        }
                    }
                }

                // 4) 取值表
                foreach (var vt in db.ValueTables)
                {
                    if (!vt.Name.ToLowerInvariant().Contains(lower)) continue;

                    lvSearch.Items.Add(new ListViewItem(new[] { "取值表", vt.Name, "", $"{vt.Values.Count} 项", Truncate(vt.Comment) }) { Tag = vt });
                }

                lblSearch.Text = $"命中 {lvSearch.Items.Count} 项";
            }
            finally { lvSearch.EndUpdate(); }
        }

        private void lvSearch_MouseDoubleClick(Object sender, MouseEventArgs e)
        {
            if (lvSearch.SelectedItems.Count == 0) return;

            var tag = lvSearch.SelectedItems[0].Tag;

            // 选中项落到左侧树上，保持两个视图的上下文一致
            SelectInTree(tag);
            ShowDetail(tag);

            WriteLog($"定位 {tag}");
        }

        /// <summary>在左侧树里选中并滚动到指定对象</summary>
        private void SelectInTree(Object tag)
        {
            if (tag == null) return;

            var node = FindNode(tvMain.Nodes, tag);
            if (node == null) return;

            tvMain.SelectedNode = node;
            node.EnsureVisible();
        }

        static TreeNode FindNode(TreeNodeCollection nodes, Object tag)
        {
            foreach (TreeNode n in nodes)
            {
                if (ReferenceEquals(n.Tag, tag) || Equals(n.Tag, tag)) return n;

                var sub = FindNode(n.Nodes, tag);
                if (sub != null) return sub;
            }
            return null;
        }

        #endregion

        #region 导出与工具

        private void FillSummary()
        {
            var db = Database;
            lblDetail.Text = db == null ? "" : $"{db}    文件：{db.FilePath}";
        }

        private void FillColumns(params Object[] nameWidthPairs)
        {
            lvDetail.Columns.Clear();
            for (int i = 0; i + 1 < nameWidthPairs.Length; i += 2)
                lvDetail.Columns.Add((String)nameWidthPairs[i], (Int32)nameWidthPairs[i + 1]);
        }

        private void AddRow(String name, String value)
        {
            var it = new ListViewItem(name ?? "");
            it.SubItems.Add(value ?? "");
            lvDetail.Items.Add(it);
        }

        static String Truncate(String s, Int32 max = 120)
        {
            if (String.IsNullOrEmpty(s)) return "";
            s = s.Replace("\r", " ").Replace("\n", " ");
            return s.Length <= max ? s : s.Substring(0, max) + "...";
        }

        /// <summary>把属性值转为可读文本：枚举属性显示候选名而非索引</summary>
        private String DescribeAttribute(String name, Object value)
        {
            if (value == null) return "";

            var def = Database?.AttributeDefinitions.FirstOrDefault(d => d.Name == name);
            if (def != null && def.ValueType == DbcAttrType_e.Enum && value is Int32 idx && idx >= 0 && idx < def.EnumValues.Count)
                return $"{def.EnumValues[idx]} ({idx})";

            if (value is Double d) return d.ToString("0.####");
            return value.ToString();
        }

        /// <summary>导出报文清单为 CSV</summary>
        private void btExport_Click(Object sender, EventArgs e)
        {
            var db = Database;
            if (db == null) { MessageBox.Show(this, "尚未加载 DBC 文件。", Text); return; }

            using var dlg = new SaveFileDialog
            {
                Title = "导出报文清单",
                Filter = "CSV 文件 (*.csv)|*.csv",
                FileName = Path.GetFileNameWithoutExtension(db.FilePath) + "_messages.csv",
                InitialDirectory = _lastDir,
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("报文,PGN,ID,优先级,格式,DLC,发送节点,信号,起始位,长度,字节序,系数,偏移,最小值,最大值,单位,SPN,FMI,接收节点");

                foreach (var m in db.Messages.OrderBy(x => x.Pgn).ThenBy(x => x.Name))
                {
                    foreach (var s in m.Signals)
                    {
                        sb.AppendLine(String.Join(",",
                            Csv(m.Name), m.PgnText, $"0x{m.Id:X8}", m.Priority, m.IsPDU1 ? "PDU1" : "PDU2", m.Size, Csv(m.Transmitter),
                            Csv(s.Name), s.StartBit, s.Length, s.ByteOrder == DbcByteOrder_e.Intel ? "Intel" : "Motorola",
                            s.Factor, s.Offset, s.Minimum, s.Maximum, Csv(s.Unit),
                            s.Spn?.ToString() ?? "", Csv(s.FmiText), Csv(String.Join(" ", s.Receivers))));
                    }
                }

                File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));
                WriteLog($"已导出 {dlg.FileName}");
                MessageBox.Show(this, $"已导出：{dlg.FileName}", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WriteLog("导出失败: " + ex);
                MessageBox.Show(this, "导出失败：" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        static String Csv(String s)
        {
            if (String.IsNullOrEmpty(s)) return "";
            if (s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) < 0) return s;
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        /// <summary>FMI 枚举转中文含义（J1939-73 定义）</summary>
        public static String DescribeFmi(Int32 fmi) => (Fmi_e)fmi switch
        {
            Fmi_e.AboveNormalMostSevere => "高于正常范围(最严重)",
            Fmi_e.BelowNormalMostSevere => "低于正常范围(最严重)",
            Fmi_e.DataErratic => "数据间断/不稳定",
            Fmi_e.VoltageAboveNormal => "电压高于正常范围",
            Fmi_e.VoltageBelowNormal => "电压低于正常范围",
            Fmi_e.CurrentBelowNormal => "电流低于正常范围",
            Fmi_e.CurrentAboveNormal => "电流高于正常范围",
            Fmi_e.MechanicalNoResponse => "机械系统无响应",
            Fmi_e.AbnormalFrequency => "异常频率",
            Fmi_e.AbnormalUpdateRate => "异常更新速率",
            Fmi_e.AbnormalRateOfChange => "异常变化率",
            Fmi_e.RootCauseUnknown => "根本原因未知",
            Fmi_e.BadComponent => "部件故障",
            Fmi_e.OutOfCalibration => "超出标定范围",
            Fmi_e.SpecialInstructions => "特殊说明",
            Fmi_e.AboveNormalLeastSevere => "高于正常范围(最轻微)",
            Fmi_e.BelowNormalLeastSevere => "低于正常范围(最轻微)",
            Fmi_e.DataValidButBelowNormal => "数据有效但偏低",
            Fmi_e.DataValidButAboveNormal => "数据有效但偏高",
            Fmi_e.ReceivedNetworkDataError => "接收网络数据错误",
            Fmi_e.DataDriftedHigh => "数据漂移过高",
            Fmi_e.DataDriftedLow => "数据漂移过低",
            Fmi_e.Inconsistent => "与其他数据不一致",
            _ => $"FMI {fmi}",
        };

        #endregion

        #region 日志

        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);

        #endregion
    }
}
