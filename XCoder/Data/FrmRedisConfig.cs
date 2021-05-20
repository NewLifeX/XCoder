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

namespace XCoder.Data
{
    public partial class FrmRedisConfig : Form
    {
        public RedisConfig Config { get; set; }

        public FrmRedisConfig()
        {
            InitializeComponent();
        }

        private void FrmRedisConfig_Load(Object sender, EventArgs e)
        {
            var cfg = Config;
            Text = cfg == null ? "添加Redis节点" : "编辑Redis节点";

            if (cfg == null)
                cfg = Config = new RedisConfig();
            else
            {
                txtName.Text = cfg.Name;
                txtServer.Text = cfg.Server;
                txtPort.Text = cfg.Port + "";
                txtUsername.Text = cfg.Username;
                txtPassword.Text = cfg.Password;
            }
        }

        private void btnOK_Click(Object sender, EventArgs e)
        {
            var cfg = Config;
            cfg.Name = txtName.Text?.Trim();
            cfg.Server = txtServer.Text?.Trim();
            cfg.Port = txtPort.Text.ToInt();
            cfg.Username = txtUsername.Text?.Trim();
            cfg.Password = txtPassword.Text?.Trim();

            if (cfg.Name.IsNullOrEmpty()) cfg.Name = cfg.Server;

            Close();
        }
    }
}