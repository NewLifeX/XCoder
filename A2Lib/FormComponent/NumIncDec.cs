using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALib.FormComponent
{
    /// <summary>
    /// 数值增减微调控件。
    /// 提供 <c>&lt;&lt; &lt; &gt; &gt;&gt;</c> 四个按钮，分别按 -10X、-1X、+1X、+10X 的倍数步进调整数值，
    /// 也可在中间文本框内直接键入或粘贴数字（仅允许合法数字内容，含可选负号与单个小数点）。
    /// 通过 <see cref="ValueChanged"/> 事件向外通知数值发生修改，修改来源不同触发时机也不同：
    /// 按键修改立即触发；键入延迟 2 秒触发；粘贴延迟 200ms 触发；
    /// 通过 <see cref="Value"/> 代码赋值不触发。
    /// </summary>
    public partial class NumIncDec : UserControl
    {
        /// <summary> 修改基准值 X。四个按键分别按 -10X、-1X、+1X、+10X 修改当前值。</summary>
        [Browsable(true)]
        [Description("修改基准值 X，按键按 X 的倍数调整数值")]
        public float X { get; set; } = 1f;

        /// <summary> 数值被修改后触发。按键修改立即触发；键入延迟 3 秒触发；粘贴延迟 200ms 触发；代码赋值不触发。</summary>
        [Browsable(true)]
        [Description("数值修改后触发")]
        public event EventHandler ValueChanged;

        /// <summary> 键入时触发数值修改事件的延迟(ms)。</summary>
        private const int TYPE_DELAY_MS = 2000;

        /// <summary> 粘贴时触发数值修改事件的延迟(ms)。</summary>
        private const int PASTE_DELAY_MS = 200;

        private const int WM_PASTE = 0x0302;

        private readonly System.Windows.Forms.Timer _delayTimer;

        /// <summary> 当前变更是否由按键(按钮)引起。</summary>
        private bool _isButtonChange;

        /// <summary> 当前变更是否由粘贴引起。</summary>
        private bool _isPaste;

        /// <summary> 当前变更是否由代码通过 <see cref="Value"/> setter 赋值引起。</summary>
        private bool _isCodeSet;

        public NumIncDec()
        {
            InitializeComponent();

            _delayTimer = new System.Windows.Forms.Timer();
            _delayTimer.Tick += (s, e) =>
            {
                _delayTimer.Stop();
                RaiseValueChanged();
            };

            tbNumber.TextChanged += TbNumber_TextChanged;
            tbNumber.KeyPress += TbNumber_KeyPress;

            btDec10.Click += (s, e) => ChangeValue(-10f);
            btDec1.Click += (s, e) => ChangeValue(-1f);
            btInc1.Click += (s, e) => ChangeValue(1f);
            btInc10.Click += (s, e) => ChangeValue(10f);
        }

        /// <summary> 拦截粘贴，仅允许粘贴合法数字内容。</summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PASTE && m.HWnd == tbNumber.Handle)
            {
                if (!PasteAllowed())
                {
                    return; // 拦截非法粘贴
                }
                _isPaste = true; // 合法的粘贴，交由 TextChanged 按粘贴延迟处理
            }
            base.WndProc(ref m);
        }

        private bool PasteAllowed()
        {
            string clip = Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty;
            string merged = tbNumber.Text.Substring(0, tbNumber.SelectionStart)
                          + clip
                          + tbNumber.Text.Substring(tbNumber.SelectionStart + tbNumber.SelectionLength);
            return IsValidNumberText(merged);
        }

        /// <summary> 校验文本是否为合法数字（可选负号开头、单个小数点）。</summary>
        private bool IsValidNumberText(string text)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return true;

            bool hasDot = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '-')
                {
                    if (i != 0)
                        return false;
                }
                else if (c == '.')
                {
                    if (hasDot || i == 0)
                        return false;
                    hasDot = true;
                }
                else if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary> 当前数值。</summary>
        public float Value
        {
            get
            {
                if (float.TryParse(tbNumber.Text.Trim(), out float v))
                    return v;
                return 0f;
            }
            set
            {
                // 代码赋值：设置标志并取消可能挂起的延迟触发，不抛 ValueChanged 事件
                _delayTimer.Stop();
                _isCodeSet = true;
                tbNumber.Text = value.ToString();
                _isCodeSet = false;
            }
        }

        private void ChangeValue(float multiplier)
        {
            float step = X * multiplier;
            _isPaste = false;        // 按钮修改不属于粘贴
            _isButtonChange = true;  // 标记为按钮变更，TextChanged 中跳过延迟处理
            SetValue(Value + step);
            _isButtonChange = false;

            // 按键修改立即触发，并取消可能挂起的延迟触发
            _delayTimer.Stop();
            RaiseValueChanged();
        }

        private void SetValue(float v)
        {
            tbNumber.Text = v.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary> 文本变化：区分按钮/粘贴/键入三种来源并据此触发数值修改事件。</summary>
        private void TbNumber_TextChanged(object sender, EventArgs e)
        {
            if (_isCodeSet)
            {
                // 代码赋值来源（Value setter），不触发数值修改事件
                _isCodeSet = false;
                return;
            }

            if (_isButtonChange)
            {
                // 按钮修改路径已在 ChangeValue 中立即触发，这里直接跳过
                _isButtonChange = false;
                return;
            }

            if (_isPaste)
            {
                // 粘贴来源
                _isPaste = false;
                ScheduleValueChanged(PASTE_DELAY_MS);
                return;
            }

            // 键入来源（按键输入，含退格/删除等编辑）
            ScheduleValueChanged(TYPE_DELAY_MS);
        }

        /// <summary> 安排延迟触发（每次重新计时，实现 debounce）。</summary>
        private void ScheduleValueChanged(int ms)
        {
            _delayTimer.Stop();
            _delayTimer.Interval = ms;
            _delayTimer.Start();
        }

        /// <summary> 触发数值修改事件。</summary>
        private void RaiseValueChanged()
        {
            var handler = ValueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary> 只允许输入数字（含小数点和负号）。</summary>
        private void TbNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 只要发生了键入（含退格/删除），就不再按粘贴处理
            _isPaste = false;

            if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Delete)
                return;

            TextBox tb = sender as TextBox;
            if (tb == null)
                return;

            string text = tb.Text;
            int selStart = tb.SelectionStart;
            string newText = text.Remove(selStart, tb.SelectionLength).Insert(selStart, e.KeyChar.ToString());

            if (e.KeyChar == '.')
            {
                // 只允许一个小数点，且不能作为首字符
                if (text.Contains('.') || selStart == 0)
                {
                    e.Handled = true;
                    return;
                }
                return;
            }

            if (e.KeyChar == '-')
            {
                // 只允许在首位输入负号
                if (selStart != 0 || text.Contains('-') || newText == "-")
                {
                    e.Handled = true;
                }
                return;
            }

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            // 负号后面不允许再出现负号
            if (newText.IndexOf('-') > 0)
            {
                e.Handled = true;
            }
        }
    }
}
