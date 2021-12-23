using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XCode.DataAccessLayer;
using XCoder;

namespace CrazyCoder.Data
{
    [Category("数据工具")]
    [DisplayName("跨库数据同步")]
    public partial class FrmSync : Form, IXForm
    {
        public FrmSync()
        {
            InitializeComponent();
        }

        private void FrmSync_Load(Object sender, EventArgs e)
        {
            var conns = DAL.ConnStrs.Keys.ToArray();
            cbConn.DataSource = conns;
        }

        private void btnConnection_Click(Object sender, EventArgs e)
        {

        }

        private void btnSync_Click(Object sender, EventArgs e)
        {

        }
    }
}
