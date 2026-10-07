using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALib.FormComponent
{
    public class RemoteButton : Button
    {
        public int RectMargin = 6;
        public int RectHeight = 3;
        
        protected override void OnPaint(PaintEventArgs pevent)
        {
            // 先按按钮默认外观绘制（基类 Button 的状态绘制）
            base.OnPaint(pevent);

            // 再在默认外观之上叠加自定义内容
            int x = RectMargin;
            int y = RectMargin;
            int width = this.ClientSize.Width - RectMargin * 2;
            int height = RectHeight;

            Rectangle rect = new Rectangle(x, y, width, height);
            pevent.Graphics.FillRectangle(Brushes.Red, rect);
        }
    }
}


