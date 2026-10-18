using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALib.CAN;
using XCoder;

namespace CrazyCoder.XCan
{
    [Category("工业电子")]
    [DisplayName("CAN调试工具")]
    public class FrmCan : CanMonitor, IXForm
    {
    }
}
