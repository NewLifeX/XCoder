using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewLife;

namespace XCoder.Tools
{
    [Category("地理信息")]
    [DisplayName("GPS辅助")]
    public partial class FrmGPS : Form, IXForm
    {
        public FrmGPS()
        {
            InitializeComponent();
        }

        private void btn_16_latlong_Click(Object sender, EventArgs e)
        {
            var temp = txt_16_latlong.Text.Trim();
            var _lat = temp.Substring(0, 8);
            var _long = temp.Substring(8);

            SetText(_lat, _long);
        }

        private void btn_latlong_Click(Object sender, EventArgs e)
        {
            var _lat = txt_16_lat.Text.Trim();
            var _long = txt_16_long.Text.Trim();

            SetText(_lat, _long);
        }

        private void SetText(String _lat, String _long)
        {
            var v_lat = BitConverter.ToSingle(_lat.ToHex(), 0);
            var v_long = BitConverter.ToSingle(_long.ToHex(), 0);

            txt_lat.Text = v_lat + "";
            txt_long.Text = v_long + "";
            txt_latlong.Text = $"{v_lat},{v_long}";
        }
    }
}
