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
using System.Windows.Forms;

namespace ALib.CAN
{
    /// <summary>
    /// J1939 报文查看器。
    /// 左侧选择 CAN 口接收扩展帧，结合已加载的 DBC 把每个 CAN ID 的最新一帧解析为
    /// 信号物理值（含枚举文本、SPN/FMI），未在 DBC 中定义的 PGN 单独归类，
    /// 便于发现 DBC 覆盖不到的实际报文。
    /// </summary>
    public partial class J1939Monitor : Form
    {
        public J1939Monitor()
        {
            InitializeComponent();
        }

        #region 属性

        /// <summary>当前加载的 DBC 数据库，未加载时为 null（此时只显示原始帧）</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DbcDatabase Database { get; private set; }

        /// <summary>日志</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILog Log { get; set; } = Logger.Null;

        /// <summary>各 CAN ID 的最新一帧，key 为 29bit ID</summary>
        private readonly Dictionary<UInt32, J1939Frame> _frames = new Dictionary<UInt32, J1939Frame>();

        /// <summary>刷新用的分帧对象，避免每帧都创建</summary>
        private readonly Object _lock = new Object();

        /// <summary>DBC 未覆盖的报文集</summary>
        private readonly HashSet<UInt32> _unknown = new HashSet<UInt32>();

        /// <summary>上一次刷新时间，用于估算刷新率</summary>
        private DateTime _lastTick = DateTime.Now;

        private String _lastDir;

        #endregion

        #region 初始化

        private void J1939Monitor_Load(Object sender, EventArgs e)
        {
            lvFrame.EnableDoubleBuffer();
            lvSignal.EnableDoubleBuffer();

            _lastDir = AppDomain.CurrentDomain.BaseDirectory;

            canSelect.Log = Log;
            canSelect.PortOpened += (s, e2) => WriteLog($"CAN 已打开：{canSelect.Port?.Name}");
            canSelect.PortClosed += (s, e2) => WriteLog("CAN 已关闭");
            canSelect.Received += CanSelect_Received;

            // 程序目录下已有 test.dbc 则自动加载，省去手工选文件
            var def = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.dbc");
            if (File.Exists(def)) LoadDbc(def, false);

            // 拖入 dbc 即加载
            lvFrame.AllowDrop = true;
            lvFrame.DragEnter += (s, e2) =>
            {
                if (e2.Data.GetDataPresent(DataFormats.FileDrop)) e2.Effect = DragDropEffects.Copy;
            };
            lvFrame.DragDrop += (s, e2) =>
            {
                if (!e2.Data.GetDataPresent(DataFormats.FileDrop)) return;
                if (e2.Data.GetData(DataFormats.FileDrop) is String[] files && files.Length > 0) LoadDbc(files[0]);
            };

            MinimumSize = new Size(900, 600);
        }

        #endregion

        #region DBC 加载

        private void btOpenDbc_Click(Object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "打开 DBC 文件",
                Filter = "DBC 文件 (*.dbc)|*.dbc|所有文件 (*.*)|*.*",
                InitialDirectory = _lastDir,
            };

            if (dlg.ShowDialog(this) == DialogResult.OK) LoadDbc(dlg.FileName);
        }

        /// <summary>加载 DBC 并重建索引</summary>
        public Boolean LoadDbc(String path, Boolean showError = true)
        {
            try
            {
                var db = DbcDatabase.LoadFile(path);

                Database = db;
                _lastDir = Path.GetDirectoryName(path);
                lblDbc.Text = $"{Path.GetFileName(path)}  报文 {db.Messages.Count}  SPN {db.SpnCount}";

                // DBC 换了，之前判定为"未知"的要重新判定
                _unknown.Clear();

                WriteLog($"已加载 DBC：{db}");
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

        #region 接收

        private void CanSelect_Received(Object sender, CanMsg msg)
        {
            if (msg == null || !msg.ExternFlag) return;   // J1939 只走扩展帧

            var id = (UInt32)msg.ID;
            var now = DateTime.Now;

            J1939Frame frame;
            lock (_lock)
            {
                if (!_frames.TryGetValue(id, out frame))
                {
                    frame = new J1939Frame(id);
                    _frames[id] = frame;
                }

                var prev = frame.LastTime;
                frame.Update(msg, now);
                if (prev != DateTime.MinValue) frame.Interval = now - prev;
            }
        }

        private void timerRefresh_Tick(Object sender, EventArgs e)
        {
            if (cbPause.Checked) return;

            var elapsed = DateTime.Now - _lastTick;
            _lastTick = DateTime.Now;

            List<J1939Frame> list;
            lock (_lock) list = _frames.Values.ToList();

            // 默认只显示近 3 秒内有更新的帧，避免历史帧占满列表
            var frames = cbHex.Checked
                ? list.Where(f => f.LastTime >= _lastTick.AddSeconds(-3)).ToList()
                : list;

            if (cbUnknownOnly.Checked) frames = frames.Where(f => _unknown.Contains(f.Id)).ToList();

            frames = frames.OrderBy(f => f.Pgn).ThenBy(f => f.Id).ToList();

            lvFrame.BeginUpdate();
            try
            {
                UpdateFrameRows(frames);
                lblRate.Text = $"{list.Count} 个 ID  显示 {frames.Count}  刷新 {elapsed.TotalMilliseconds:F0}ms";
            }
            finally { lvFrame.EndUpdate(); }
        }

        /// <summary>就地刷新列表，保留滚动条与选中行（Tag 为帧对象，引用不变即内容不变）</summary>
        private void UpdateFrameRows(List<J1939Frame> frames)
        {
            var cnt = lvFrame.Items.Count;

            for (int i = 0; i < frames.Count; i++)
            {
                var f = frames[i];
                if (i < cnt)
                {
                    var it = lvFrame.Items[i];
                    if (ReferenceEquals(it.Tag, f) && !f.Dirty) continue;

                    FillFrameRow(it, f, f.Dirty);
                    f.Dirty = false;
                }
                else
                {
                    var it = new ListViewItem();
                    FillFrameRow(it, f, true);
                    f.Dirty = false;
                    lvFrame.Items.Add(it);
                }
            }

            while (lvFrame.Items.Count > frames.Count)
                lvFrame.Items.RemoveAt(lvFrame.Items.Count - 1);
        }

        private void FillFrameRow(ListViewItem it, J1939Frame f, Boolean full)
        {
            var desc = Database == null ? null : Lookup(f);

            // 信号值摘要：取前两个非零信号，避免一行塞下所有内容
            var summary = "";
            if (desc != null)
            {
                var parts = desc.Signals
                    .Where(s => s.Signal.Factor != 0)
                    .Take(3)
                    .Select(s => $"{s.Signal.Name}={s.Display}")
                    .ToList();
                summary = String.Join("  ", parts);
            }

            SetSub(it, 0, $"{f.Time:HH:mm:ss.fff}");
            SetSub(it, 1, $"0x{f.Id:X8}");
            SetSub(it, 2, $"0x{f.Pgn:X5}");
            SetSub(it, 3, f.Priority.ToString());
            SetSub(it, 4, f.Pf.ToString("X2"));
            SetSub(it, 5, f.Ps.ToString("X2"));
            SetSub(it, 6, $"{f.SourceAddress:X2}");
            SetSub(it, 7, f.PduFormat + (f.SourceAddress == 0 ? "" : ""));
            SetSub(it, 8, f.DataHex);
            SetSub(it, 9, desc == null ? "" : desc.Message.Name);
            SetSub(it, 10, summary);
            SetSub(it, 11, f.Interval.TotalMilliseconds.ToString("F0"));

            it.Tag = f;
            it.ForeColor = desc == null ? Color.DimGray : lvFrame.ForeColor;
        }

        private void SetSub(ListViewItem it, Int32 index, String text)
        {
            while (it.SubItems.Count <= index) it.SubItems.Add("");
            if (it.SubItems[index].Text != text) it.SubItems[index].Text = text;
        }

        #endregion

        #region DBC 解析

        /// <summary>把一帧解析为报文定义 + 各信号值；DBC 未定义时返回 null</summary>
        private FrameDescription Lookup(J1939Frame f)
        {
            var db = Database;
            if (db == null) return null;

            // 先用 ID 精确匹配（DBC 里 ID 含优先级与源地址时最准）
            var msg = db.FindMessageById(f.Id);

            // 退化为按 PGN 匹配（DBC 中 ID 的源地址常被写成固定值，实际帧 SA 会变）
            if (msg == null) msg = db.FindMessagesByPgn(f.Pgn).FirstOrDefault();

            if (msg == null)
            {
                _unknown.Add(f.Id);
                return null;
            }

            _unknown.Remove(f.Id);

            var desc = new FrameDescription { Message = msg };
            var data = f.Data;

            foreach (var s in msg.Signals)
            {
                // 多路复用信号的从属路号需要与复用器当前值比对，此处仅标注不做裁剪
                var raw = s.GetRawValue(data);
                var value = s.RawToPhysical(raw);

                desc.Signals.Add(new SignalValue
                {
                    Signal = s,
                    Raw = raw,
                    Value = value,
                    Display = FormatValue(s, raw, value),
                });
            }

            // 故障码：把 DTC 与 DBC 定义关联，便于直接看出是哪条报文哪个信号
            if (IsDmMessage(f.Pgn))
            {
                var dm = Msg_DM1.ParseData(data, f.Pgn, f.SourceAddress, f.Priority);
                if (dm != null) desc.DtcMappings = dm.MapToDbc(db);
            }

            return desc;
        }

        static Boolean IsDmMessage(UInt32 pgn)
            => pgn == J1939Pgn.DM1 || pgn == J1939Pgn.DM2 || pgn == J1939Pgn.DM3 || pgn == J1939Pgn.DM11;

        /// <summary>信号显示文本：有枚举取枚举，否则按缩放格式化</summary>
        static String FormatValue(DbcSignal s, Int64 raw, Double value)
        {
            var text = s.GetValueDescription(raw);
            if (text != null) return text;

            var v = Math.Abs(value) >= 1000 ? value.ToString("F0") : value.ToString("F3").TrimEnd('0').TrimEnd('.');
            return String.IsNullOrEmpty(s.Unit) ? v : v + s.Unit;
        }

        #endregion

        #region 选中帧的信号明细

        private void lvFrame_ItemSelectionChanged(Object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected) return;

            var f = e.Item.Tag as J1939Frame;
            if (f == null) return;

            ShowFrameDetail(f);
        }

        private void ShowFrameDetail(J1939Frame f)
        {
            lvSignal.BeginUpdate();
            try
            {
                lvSignal.Items.Clear();

                var desc = Lookup(f);
                if (desc == null)
                {
                    lblFrame.Text = $"0x{f.Id:X8}  PGN=0x{f.Pgn:X5}  数据 {f.DataHex}    DBC 中无此报文定义（PGN {f.Pgn} / 十进制 {f.Pgn}）";

                    // 未定义报文也按 J1939 规则列一遍基本信息，方便人工判读
                    FillColumns("项目", 140, "内容", 700);
                    AddSignalRow("扩展 ID", $"0x{f.Id:X8}");
                    AddSignalRow("优先级", f.Priority.ToString());
                    AddSignalRow("EDP/DP", $"{(f.Edp ? 1 : 0)}/{(f.Dp ? 1 : 0)}");
                    AddSignalRow("PF", $"0x{f.Pf:X2}");
                    AddSignalRow("PS", $"0x{f.Ps:X2}");
                    AddSignalRow("源地址 SA", $"0x{f.SourceAddress:X2}");
                    AddSignalRow("PGN", $"{f.Pgn} (0x{f.Pgn:X5})");
                    AddSignalRow("PDU 格式", f.PduFormat);
                    AddSignalRow("数据", f.DataHex);
                    return;
                }

                var m = desc.Message;
                var known = desc.Signals.Count(s => !IsPlaceholder(s));

                lblFrame.Text = $"{m.Name}  PGN={m.PgnText}(0x{m.Pgn:X5})  ID=0x{f.Id:X8}  优先级={f.Priority}  " +
                                $"SA=0x{f.SourceAddress:X2}  DLC={m.Size}  信号 {known}/{m.Signals.Count}  " +
                                $"周期={f.Interval.TotalMilliseconds:F0}ms" +
                                (String.IsNullOrEmpty(m.Comment) ? "" : Environment.NewLine + "注释：" + m.Comment);

                FillColumns("信号", 250, "原始值", 90, "物理值", 110, "显示", 180, "位", 80, "单位", 60, "SPN", 80, "FMI", 60);

                foreach (var sv in desc.Signals)
                {
                    var s = sv.Signal;
                    var it = new ListViewItem(s.Name);
                    it.SubItems.Add(sv.Raw.ToString());
                    it.SubItems.Add(sv.Value.ToString("0.####"));
                    it.SubItems.Add(sv.Display);
                    it.SubItems.Add($"{s.StartBit}|{s.Length}");
                    it.SubItems.Add(s.Unit);
                    it.SubItems.Add(s.Spn?.ToString() ?? "");
                    it.SubItems.Add(s.FmiText ?? "");
                    it.Tag = s;
                    lvSignal.Items.Add(it);
                }

                // DM1/DM2 之类诊断报文，额外给出故障码定位结果
                if (desc.DtcMappings.Count > 0)
                {
                    lblDtc.Text = $"故障码 {desc.DtcMappings.Count} 条：" +
                                  String.Join(" | ", desc.DtcMappings.Select(d => d.ToString()));
                }
                else lblDtc.Text = "";
            }
            finally { lvSignal.EndUpdate(); }
        }

        /// <summary>信号在所有帧上恒为 0 且无枚举定义，视为未使用（仅统计用）</summary>
        static Boolean IsPlaceholder(SignalValue sv) => sv.Raw == 0 && sv.Signal.ValueDescriptions.Count == 0;

        private void FillColumns(params Object[] nameWidthPairs)
        {
            lvSignal.Columns.Clear();
            for (int i = 0; i + 1 < nameWidthPairs.Length; i += 2)
                lvSignal.Columns.Add((String)nameWidthPairs[i], (Int32)nameWidthPairs[i + 1]);
        }

        private void AddSignalRow(String name, String value)
        {
            var it = new ListViewItem(name ?? "");
            it.SubItems.Add(value ?? "");
            lvSignal.Items.Add(it);
        }

        #endregion

        #region 过滤与清空

        private void cbFilter_CheckedChanged(Object sender, EventArgs e) => timerRefresh_Tick(null, null);

        private void btClear_Click(Object sender, EventArgs e)
        {
            lock (_lock)
            {
                _frames.Clear();
                _unknown.Clear();
            }

            lvFrame.Items.Clear();
            lvSignal.Items.Clear();
            lblFrame.Text = "";
            lblDtc.Text = "";
            WriteLog("已清空缓存");
        }

        private void btPause_Click(Object sender, EventArgs e)
        {
            cbPause.Checked = !cbPause.Checked;
            btPause.Text = cbPause.Checked ? "继续" : "暂停";
        }

        #endregion

        #region 日志

        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);

        #endregion

        #region 内部类型

        /// <summary>一帧 J1939 报文的解码视图</summary>
        public class J1939Frame
        {
            /// <summary>29bit 扩展 ID</summary>
            public UInt32 Id;

            /// <summary>最近一次收到的时刻</summary>
            public DateTime LastTime = DateTime.MinValue;

            /// <summary>与上一帧的间隔</summary>
            public TimeSpan Interval = TimeSpan.Zero;

            /// <summary>负载数据</summary>
            public Byte[] Data = new Byte[8];

            /// <summary>内容是否已变化，用于避免无谓重绘</summary>
            public Boolean Dirty = true;

            public J1939Frame(UInt32 id) { Id = id; }

            /// <summary>优先级 bit28..26</summary>
            public Int32 Priority => (Int32)((Id >> 26) & 0x07);

            /// <summary>扩展数据页 EDP bit25</summary>
            public Boolean Edp => (Id & (1u << 25)) != 0;

            /// <summary>数据页 DP bit24</summary>
            public Boolean Dp => (Id & (1u << 24)) != 0;

            /// <summary>PDU 格式 PF</summary>
            public Byte Pf => (Byte)((Id >> 16) & 0xFF);

            /// <summary>PDU 特定 PS</summary>
            public Byte Ps => (Byte)((Id >> 8) & 0xFF);

            /// <summary>源地址 SA</summary>
            public Byte SourceAddress => (Byte)(Id & 0xFF);

            /// <summary>是否 PDU1（点对点）</summary>
            public Boolean IsPDU1 => Pf < 240;

            /// <summary>PDU 格式描述</summary>
            public String PduFormat => IsPDU1 ? "PDU1 点对点" : "PDU2 广播";

            /// <summary>PGN 数值</summary>
            public UInt32 Pgn
            {
                get
                {
                    UInt32 pgn = (UInt32)Pf << 8;
                    if (!IsPDU1) pgn |= Ps;
                    if (Dp) pgn |= 0x10000;
                    if (Edp) pgn |= 0x20000;
                    return pgn;
                }
            }

            /// <summary>负载十六进制文本</summary>
            public String DataHex => Data == null ? "" : String.Join(" ", Data.Select(b => b.ToString("X2")));

            /// <summary>用收到的帧更新内容</summary>
            public void Update(CanMsg msg, DateTime time)
            {
                LastTime = time;
                Data = msg.Data ?? new Byte[8];
                Dirty = true;
            }

            public DateTime Time => LastTime;

            public override String ToString() => $"0x{Id:X8} PGN=0x{Pgn:X5} {DataHex}";
        }

        /// <summary>单条信号的解析结果</summary>
        public class SignalValue
        {
            /// <summary>信号定义</summary>
            public DbcSignal Signal;

            /// <summary>原始值</summary>
            public Int64 Raw;

            /// <summary>物理值</summary>
            public Double Value;

            /// <summary>显示文本（枚举优先）</summary>
            public String Display;
        }

        /// <summary>一帧报文结合 DBC 后的完整解读</summary>
        public class FrameDescription
        {
            /// <summary>匹配到的报文定义</summary>
            public DbcMessage Message;

            /// <summary>各信号解析结果</summary>
            public List<SignalValue> Signals = new List<SignalValue>();

            /// <summary>诊断报文的故障码定位结果</summary>
            public List<DtcMapping> DtcMappings = new List<DtcMapping>();
        }

        #endregion
    }
}
