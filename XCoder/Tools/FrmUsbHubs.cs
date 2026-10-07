using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALib.UsbHub;
using XCoder;

namespace CrazyCoder.Tools
{
    [Category("工具")]
    [DisplayName("USB HUB 查看器")]
    public class FrmUsbHubs : UsbHubSelect, IXForm
    {
        // 隐藏 确认(bt_Comf) / 取消(bt_Canel) 按钮，本窗体仅作 HUB 信息查看器使用
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Controls.Find("bt_Comf", true).FirstOrDefault()?.Hide();
            Controls.Find("bt_Canel", true).FirstOrDefault()?.Hide();
        }
    }
}
