using ALib.WinForm;
using NewLife;
using NewLife.Collections;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ALib.CAN
{
    public partial class CanMonitor : Form
    {
        public CanMonitor()
        {
            InitializeComponent();
        }

        #region 初始化界面部分

        private void CanMonitor_Load(object sender, EventArgs e)
        {
            // 表格频繁刷新，开启双缓冲减少闪烁
            lvMessage.EnableDoubleBuffer();

            // 记录各列初始宽度与列表初始宽度，作为拖拽窗口时等比缩放的基准
            _colBaseWidths = new int[lvMessage.Columns.Count];
            for (int i = 0; i < lvMessage.Columns.Count; i++)
                _colBaseWidths[i] = lvMessage.Columns[i].Width;
            _listBaseWidth = lvMessage.ClientSize.Width;

            // 拖拽窗口时按列表宽度等比缩放列宽（行高不变）
            this.Resize += CanMonitor_Resize;
            CanMonitor_Resize(null, null);

            // 限制最小尺寸，避免窗口过小导致列宽被压得不可用
            this.MinimumSize = new Size(640, 480);

            // 日志转发到 CanSelect，便于在外部统一观察
            canSelect.Log = Log;

            cbSeCanId_DropDown(null, null);
        }

        /// <summary>各列初始宽度（设计基准），用于窗口缩放时按比例换算</summary>
        private int[] _colBaseWidths;
        /// <summary>列表初始可用宽度（设计基准）</summary>
        private int _listBaseWidth;

        /// <summary>
        /// 拖拽窗口时按列表宽度等比缩放各列，最后一列填满剩余空间避免水平滚动条；
        /// 仅调整列宽，不改变行高。
        /// </summary>
        private void CanMonitor_Resize(object sender, EventArgs e)
        {
            if (_colBaseWidths == null || _listBaseWidth <= 0) return;
            if (lvMessage.Columns.Count != _colBaseWidths.Length) return;

            int avail = lvMessage.ClientSize.Width;
            double scale = (double)avail / _listBaseWidth;
            if (scale <= 0) return;

            int sum = 0;
            for (int i = 0; i < lvMessage.Columns.Count - 1; i++)
            {
                int w = (int)(_colBaseWidths[i] * scale);
                if (w < 10) w = 10;
                lvMessage.Columns[i].Width = w;
                sum += w;
            }

            // 最后一列填满剩余空间，避免出现水平滚动条
            int last = lvMessage.Columns.Count - 1;
            int lastW = avail - sum;
            if (lastW < 10) lastW = 10;
            lvMessage.Columns[last].Width = lastW;
        }

        /// <summary>
        /// 拦截空格键：在按键消息分发到任何子控件之前处理，
        /// 避免焦点在按钮上时被 Button 当成“空格=Click”而误触发
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Space)
            {
                // 有下拉框展开时不拦截，交给下拉框自己处理
                if (!cbSeCanId.DroppedDown && !canSelect.DroppedDown)
                {
                    btPause_Click(null, null);
                    return true;   // 已处理，不再分发，按钮不会收到空格
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region CAN 口

        /// <summary>当前 CAN 口，随 <see cref="CanSelect"/> 打开/关闭变化</summary>
        public ICanPort Port => canSelect.Port;

        /// <summary>供外部注入已打开的 CAN 口（注入后由外部负责关闭）</summary>
        public void AttachPort(ICanPort port) => canSelect.Port = port;

        /// <summary>CAN 口打开成功</summary>
        private void canSelect_PortOpened(object sender, EventArgs e)
        {
            WriteLog($"使用 CAN 设备: {Port?.Name}");
        }

        #endregion

        #region 暂停接收

        private void btPause_Click(object sender, EventArgs e)
        {
            var stop = cbStop.Checked;
            stop = !stop;
            cbStop.Checked = stop;

            if (stop)
            {
                btPause.Text = "继续接收";
                btPause.BackColor = Color.Gray;
            }
            else
            {
                btPause.Text = "暂停接收";
                btPause.BackColor = this.BackColor;
            }
        }

        #endregion

        #region 接收与刷新

        private int showNumber = 256;
        public Int32 ShowNumber
        {
            get { return showNumber; }
            set
            {
                if (value < 64) showNumber = 64;
                else if (value > 1024 * 1024) showNumber = 1024 * 1024;
                else showNumber = value;
            }
        }

        private void canSelect_Received(object sender, CanMsg e)
        {
            if (cbStop.Checked) return;
            AddMsg(e);
        }

        private void cbSeCanId_DropDown(object sender, EventArgs e)
        {
            var old = cbSeCanId.DataSource as string[];
            var oldIdx = (int)cbSeCanId.SelectedIndex;

            var ids = AllCanId();
            // ids.Add(-2);
            // ids.Add(-1);

            var idsStr = ids.Select(x => x.ToString("X8")).ToList();
            idsStr.Add("仅最新");
            idsStr.Add("全部");

            cbSeCanId.DataSource = idsStr;
            cbSeCanId.Tag = ids;

            if (old != null)
            {
                string oId = old[oldIdx];
                int nId = idsStr.IndexOf(oId);
                if (nId >= 0) cbSeCanId.SelectedIndex = nId;
            }
        }

        /// <summary>用户当前选中的行所对应的消息，刷新时用它把选中行跟着数据一起挪动</summary>
        private CanMsgInfo _followMsg = null;

        private void lvMessage_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!e.IsSelected) return;

            // 记录选中行对应的消息对象（避免在刷新中赋值，刷新时 SelectedItems 已被程序改动）
            _followMsg = e.Item.Tag as CanMsgInfo;
        }

        /// <summary>右键菜单：清空表格显示并清除缓存的消息</summary>
        private void tsmiClearMsg_Click(object sender, EventArgs e)
        {
            lvMessage.BeginUpdate();
            try
            {
                lvMessage.Items.Clear();
                lvMessage.SelectedItems.Clear();
                _followMsg = null;
            }
            finally { lvMessage.EndUpdate(); }

            ClearMsgs();
            cbSeCanId_DropDown(null, null);
        }

        /// <summary>
        /// 让选中行跟着数据一起挪动。
        /// 已在底部（自动跟随最新数据）时不跟随，避免和底部跟随打架；
        /// 数据仍可见时保持选中；一旦滚出可见区（或已被挤出表格）就取消选中，不再滚动过去。
        /// </summary>
        /// <param name="atBottom">刷新前是否处于底部</param>
        private void FollowSelection(bool atBottom)
        {
            // if (atBottom) return;
            if (_followMsg == null) return;

            // 数据跑出屏幕或已不在表格中：丢弃选中，避免跟随一个看不到的目标
            if (!lvMessage.FollowSelection(_followMsg))
            {
                lvMessage.SelectedItems.Clear();
                _followMsg = null;
            }
        }

        private void lvtime1_Tick(object sender, EventArgs e)
        {
            lvMessage.BeginUpdate();
            try
            {
                var str = cbSeCanId.SelectedItem as String;
                if (str == "全部")
                {
                    // 刷新前先判断是否已在底部，避免打断用户向上翻看
                    var atBottom = lvMessage.IsAtBottom();

                    var allmsgs = AllMsgs.PeekHeadArray(showNumber);
                    UpdateLatest(allmsgs);

                    // 表格显示最底部
                    if (atBottom) lvMessage.ScrollToBottom();

                    FollowSelection(atBottom);
                }
                else if (str == "仅最新")
                {
                    // 刷新前先判断是否已在底部，避免打断用户向上翻看
                    // var atBottom = lvMessage.IsAtBottom();

                    var msgs = LastMsgs().ToArray();
                    UpdateLatest(msgs);
                    // FollowSelection(atBottom);
                }
                else
                {
                    var ids = cbSeCanId.Tag as List<int>;
                    var id = ids == null ? -1 : ids[cbSeCanId.SelectedIndex];

                    // 刷新前先判断是否已在底部，避免打断用户向上翻看
                    var atBottom = lvMessage.IsAtBottom();

                    var allmsgs = GetMsgByCanId(id, showNumber);
                    UpdateLatest(allmsgs);

                    // 表格显示最底部
                    if (atBottom) lvMessage.ScrollToBottom();

                    FollowSelection(atBottom);
                }
            }
            finally { lvMessage.EndUpdate(); }
        }

        #endregion

        #region ListView 相关

        /// <summary>按最新帧刷新表格（就地更新，避免增删行引起的闪烁）</summary>
        /// <param name="msgs"></param>
        private void UpdateLatest(CanMsgInfo[] msgs)
        {
            var lvCnt = lvMessage.Items.Count;

            for (int i = 0; i < msgs.Length; i++)
            {
                var mi = msgs[i];
                if (i < lvCnt)
                {
                    // 内容未变则跳过，避免无谓重绘
                    if (ReferenceEquals(lvMessage.Items[i].Tag, mi)) continue;

                    FillRow(lvMessage.Items[i], mi);
                }
                else
                {
                    lvMessage.Items.Add(lvMsgsView(mi));
                }
            }

            // 多余的行一次删除（从尾部删）
            while (lvMessage.Items.Count > msgs.Length)
                lvMessage.Items.RemoveAt(lvMessage.Items.Count - 1);
        }

        /// <summary>就地填充一行，保留 ListViewItem 对象</summary>
        /// <param name="it"></param>
        /// <param name="mi"></param>
        private void FillRow(ListViewItem it, CanMsgInfo mi)
        {
            var msg = mi.Msg;

            var id = msg.ID.GetBytes(false).ReadBytes(0, 4).ToHex();
            if (msg.RemoteFlag) id += " R";
            if (msg.ExternFlag) id += " E";

            var data = msg.Data.ToHex();
            var time = msg.TimeStamp.ToString("hh\\:mm\\:ss\\.fff");
            var interval = mi.Interval.TotalMilliseconds.ToString("F1") + "ms";

            SetSubItem(it, 0, id);
            SetSubItem(it, 1, data);
            SetSubItem(it, 2, time);
            SetSubItem(it, 3, interval);

            it.Tag = mi;
        }

        private void SetSubItem(ListViewItem it, int index, string text)
        {
            while (it.SubItems.Count <= index) it.SubItems.Add("");
            if (it.SubItems[index].Text != text) it.SubItems[index].Text = text;
        }

        /// <summary>消息转表格行</summary>
        /// <param name="mi">消息</param>
        /// <returns></returns>
        private ListViewItem lvMsgsView(CanMsgInfo mi)
        {
            var it = new ListViewItem();
            FillRow(it, mi);
            return it;
        }

        #endregion

        #region 消息部分

        public class CanMsgInfo
        {
            /// <summary>消息本体</summary>
            public CanMsg Msg;

            /// <summary>和上一条同CAN ID消息的间隔</summary>
            public TimeSpan Interval = TimeSpan.Zero;

            public CanMsgInfo(CanMsg msg) { Msg = msg; }
        }

        #region 消息管理

        /// <summary>各 CANID 的最新帧</summary>
        public Dictionary<Int32, CircularQueue<CanMsgInfo>> Msgs = new Dictionary<Int32, CircularQueue<CanMsgInfo>>(1024 * 2);

        /// <summary>全部消息，只按照时间排序</summary>
        public CircularQueue<CanMsgInfo> AllMsgs = new CircularQueue<CanMsgInfo>(1024 * 100);

        public List<int> AllCanId() { lock (Msgs) { return Msgs.Keys.ToList(); } }

        public CanMsgInfo[] GetMsgByCanId(Int32 id, int count)
        {
            if (id < 0)
            {
                lock (AllMsgs) { return AllMsgs.PeekHeadArray(count); }
            }
            else
            {
                if (!Msgs.ContainsKey(id)) return null;

                lock (Msgs)
                {
                    var queue = Msgs[id];
                    return queue.PeekHeadArray(count);
                }
            }
        }

        public List<CanMsgInfo> LastMsgs()
        {
            lock (Msgs)
            {
                var list = new List<CanMsgInfo>();
                foreach (var item in Msgs.Values) list.Add(item.PeekHead());
                return list;
            }
        }

        /// <summary>添加新消息</summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool AddMsg(CanMsg msg)
        {
            lock (Msgs)
            {
                if (!Msgs.ContainsKey(msg.ID))
                {
                    var cir = new CircularQueue<CanMsgInfo>(1000);
                    Msgs.Add(msg.ID, cir);
                }

                // 入队检查
                var queue = Msgs[msg.ID];
                if (queue.Remain() < 1) queue.Seek(1);

                // 创建入队对象
                var cin = new CanMsgInfo(msg);
                var last = queue.PeekHead();
                if (last != null) { cin.Interval = msg.TimeStamp - last.Msg.TimeStamp; }

                // 入队
                queue.Enqueue(cin);

                // 全部消息入队
                if (AllMsgs.Remain() < 1) AllMsgs.Seek(1);
                AllMsgs.Enqueue(cin);
            }

            return true;
        }

        /// <summary>清空消息</summary>
        public void ClearMsgs()
        {
            lock (Msgs)
            {
                foreach (var item in Msgs) { item.Value.Clear(); }
                Msgs.Clear();
                AllMsgs.Clear();
            }
        }

        #endregion
        #endregion

        #region 日志

        public ILog Log { get; set; } = Logger.Null;

        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);

        #endregion
    }
}
