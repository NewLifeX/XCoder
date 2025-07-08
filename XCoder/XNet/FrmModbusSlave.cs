﻿using System;
using System.ComponentModel;
using NewLife;
using NewLife.Buffers;
using NewLife.Data;
using NewLife.IoT.Protocols;
using NewLife.Log;
using NewLife.Net;
using NewLife.Net.Handlers;
using NewLife.Security;
using NewLife.Threading;
using XCoder;
using XCoder.Common;
using XCoder.XNet;

namespace XNet
{
    [Category("工业电子")]
    [DisplayName("ModbusSlave")]
    public partial class FrmModbusSlave : Form, IXForm
    {
        private ControlConfig _config;
        private ILog _log;
        private NetServer _Server;
        private List<RegisterUnit> _regs;
        private List<CoilUnit> _coils;
        private Boolean _coil;

        #region 窗体
        public FrmModbusSlave()
        {
            InitializeComponent();

            // 动态调节宽度高度，兼容高DPI
            this.FixDpi();
        }

        private void FrmMain_Load(Object sender, EventArgs e)
        {
            _log = new TextControlLog { Control = txtReceive };

            if (cbMode.SelectedIndex < 0) cbMode.SelectedIndex = 0;

            txtReceive.SetDefaultStyle(12);

            // 加载保存的颜色
            UIConfig.Apply(txtReceive);

            _config = new ControlConfig { Control = this, FileName = "ModbusSlave.json" };
            _config.Ignores.Add(nameof(txtReceive));
            _config.Load();
            LoadConfig();
        }
        #endregion

        #region 加载/保存 配置
        private void LoadConfig()
        {
            var cfg = NetConfig.Current;
            mi显示应用日志.Checked = cfg.ShowLog;
            mi显示网络日志.Checked = cfg.ShowSocketLog;
            mi显示接收字符串.Checked = cfg.ShowReceiveString;
            mi显示发送数据.Checked = cfg.ShowSend;
            mi显示接收数据.Checked = cfg.ShowReceive;
            mi日志着色.Checked = cfg.ColorLog;
        }

        private void SaveConfig()
        {
            var cfg = NetConfig.Current;
            cfg.ShowLog = mi显示应用日志.Checked;
            cfg.ShowSocketLog = mi显示网络日志.Checked;
            cfg.ShowReceiveString = mi显示接收字符串.Checked;
            cfg.ShowSend = mi显示发送数据.Checked;
            cfg.ShowReceive = mi显示接收数据.Checked;
            cfg.ColorLog = mi日志着色.Checked;

            cfg.Save();
        }
        #endregion

        #region 收发数据
        private void btnConnect_Click(Object sender, EventArgs e)
        {
            _config.Save();
            SaveConfig();

            _coil = cbStorage.SelectedItem + "" == "线圈";
            var btn = sender as Button;
            if (btn.Text == "开始")
            {
                var svr = new NetServer((Int32)numPort.Value)
                {
                    Log = _log,
                    //SessionLog = _log,
                    //SocketLog = _log,
                    //LogReceive = true,
                    //LogSend = true,
                };

                var set = NetConfig.Current;
                if (set.ShowLog) svr.SessionLog = _log;
                if (set.ShowSocketLog) svr.SocketLog = _log;
                if (set.ShowSend) svr.LogSend = true;
                if (set.ShowReceive) svr.LogReceive = true;

                // 加入定长编码器，处理Tcp粘包
                svr.Add(new LengthFieldCodec { Offset = 4, Size = -2 });
                svr.Received += OnReceived;
                svr.Start();

                _Server = svr;
                _regs = null;
                _coils = null;

                ShowData();

                pnlSetting.Enabled = false;
                btn.Text = "停止";
            }
            else
            {
                _Server.TryDispose();
                _Server = null;
                _timer.TryDispose();

                pnlSetting.Enabled = true;
                btn.Text = "开始";
            }
        }

        private void ShowData()
        {
            var addr = (Int32)numAddress.Value;
            var count = (Int32)numCount.Value;
            var mode = cbMode.SelectedItem + "";

            if (_coil)
            {
                var list = new List<CoilUnit>(count);
                switch (mode)
                {
                    case "0x0000":
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new CoilUnit { Address = addr + i, Value = 0 });
                        }
                        break;
                    case "0x7777":
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new CoilUnit { Address = addr + i, Value = (Byte)(i % 2) });
                        }
                        break;
                    case "0xFFFF":
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new CoilUnit { Address = addr + i, Value = 1 });
                        }
                        break;
                    case "递增":
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new CoilUnit { Address = addr + i, Value = (Byte)(i % 2) });
                        }
                        break;
                    case "静态随机":
                    case "动态随机":
                    default:
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new CoilUnit { Address = addr + i, Value = (Byte)Rand.Next(2) });
                        }
                        break;
                }
                _coils = list;

                dgv.DataSource = _coils;
            }
            else
            {
                var list = new List<RegisterUnit>(count);
                switch (mode)
                {
                    case "0x0000":
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new RegisterUnit { Address = addr + i, Value = 0 });
                        }
                        break;
                    case "0x7777":
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new RegisterUnit { Address = addr + i, Value = 0x7777 });
                        }
                        break;
                    case "0xFFFF":
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new RegisterUnit { Address = addr + i, Value = 0xFFFF });
                        }
                        break;
                    case "递增":
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new RegisterUnit { Address = addr + i, Value = (UInt16)i });
                        }
                        break;
                    case "静态随机":
                    case "动态随机":
                    default:
                        for (var i = 0; i < count; i++)
                        {
                            list.Add(new RegisterUnit { Address = addr + i, Value = (UInt16)Rand.Next(UInt16.MaxValue) });
                        }
                        break;
                }

                _regs = list;

                dgv.DataSource = _regs;
            }

            if (mode == "动态随机") _timer = new TimerX(DoRefreshData, null, 1_000, 1_000);
        }

        private TimerX _timer;
        private void DoRefreshData(Object state)
        {
            if (_regs != null)
            {
                for (var i = 0; i < _regs.Count; i++)
                {
                    var val = _regs[i].Value;
                    if (val == 0)
                        val = (UInt16)Rand.Next(UInt16.MaxValue);
                    else
                    {
                        var x = (Rand.Next(75) - 30) / 100.0;
                        val = (UInt16)(val * (1 + x));
                    }

                    _regs[i].Value = val;
                }
            }
            if (_coils != null)
            {
                for (var i = 0; i < _coils.Count; i++)
                {
                    var val = _coils[i].Value;
                    if (val == 0)
                        val = (Byte)Rand.Next(UInt16.MaxValue);
                    else
                    {
                        var x = (Rand.Next(75) - 30) / 100.0;
                        val = (Byte)(val * (1 + x));
                    }

                    _coils[i].Value = (Byte)(val & 0x01);
                }
            }

            Invoke(() => { dgv.Refresh(); });
        }

        private void OnReceived(Object sender, ReceivedEventArgs e)
        {
            var session = sender as NetSession;
            if (session == null) return;

            var pk = e.Packet as Packet;
            if (pk == null) pk = new Packet(e.Packet.ReadBytes());
            var spanReader = pk.AsSpan();
            var msg = ModbusIpMessage.Read(spanReader);
            if (msg == null) return;

            session.Log?.Info("<= {0}", msg);

            var addrMsg = msg.GetAddress();
            var rs = msg.CreateReply();
            switch (msg.Code)
            {
                case FunctionCodes.ReadCoil:
                case FunctionCodes.ReadDiscrete:
                    if (_coils != null)
                    {
                        // 连续地址，其实地址有可能不是8的倍数
                        var regCount = msg.Payload.ReadBytes(2, 2).ToUInt16(0, false);
                        var addr = addrMsg - _coils[0].Address;
                        if (addr >= 0 && addr + regCount <= _coils.Count)
                        {
                            // 取出该段存储单元
                            var cs = _coils.Skip(addr).Take(regCount).ToList();
                            var count = (Int32)Math.Ceiling(regCount / 8.0);
                            // 遍历存储单元，把数据聚合成为字节数组返回
                            var bits = new Byte[1 + count];
                            bits[0] = (Byte)count;
                            for (var i = 0; i < count; i++)
                            {
                                var b = 0;
                                // 每个字节最大可存储8位数据，最后一个字节可能不足8位
                                var max = regCount - i * 8;
                                if (max > 8) max = 8;
                                for (var j = 0; j < max; j++)
                                {
                                    if (cs[i * 8 + j].Value > 0)
                                        b |= 1 << j;
                                }
                                bits[1 + i] = (Byte)b;
                            }

                            rs.Payload =new Packet( bits);
                        }
                    }
                    break;
                case FunctionCodes.ReadRegister:
                case FunctionCodes.ReadInput:
                    if (_regs != null)
                    {
                        // 连续地址
                        var regCount = msg.Payload.ReadBytes(2, 2).ToUInt16(0, false);
                        var addr = addrMsg - _regs[0].Address;
                        if (addr >= 0 && addr + regCount <= _regs.Count)
                        {
                            var buf = _regs.Skip(addr).Take(regCount).SelectMany(e => e.GetData()).ToArray();
                            rs.Payload = new Packet(new Byte[] { (Byte)buf.Length }.Concat(buf).ToArray());
                           // rs.Payload.Append(new Packet(buf));
                        }
                    }
                    break;
                case FunctionCodes.WriteCoil:
                    break;
                case FunctionCodes.WriteRegister:
                    if (_regs != null)
                    {
                        // 连续地址
                        var regCount = 0;
                        for (var i = 0; i < 256 && i + 1 < msg.Payload.Total; i += 2)
                        {
                            var value = msg.Payload.ReadBytes(2 + i, 2).ToUInt16(0, false);
                            var addr = addrMsg - _regs[0].Address;
                            if (addr >= 0 && addr < _regs.Count)
                            {
                                var ru = _regs[addr];
                                ru.Value = value;
                                regCount++;
                            }
                        }
                        Invoke(() => { dgv.Refresh(); });
                        {
                            var addr = addrMsg - _regs[0].Address;
                            rs.Payload = new Packet(_regs.Skip(addr).Take(regCount).SelectMany(e => e.GetData()).ToArray());
                        }
                    }
                    break;
                case FunctionCodes.WriteCoils:
                    break;
                case FunctionCodes.WriteRegisters:
                    break;
                default:
                    break;
            }

            session.Log?.Info("=> {0}", rs);

            session.Send(rs.ToPacket());
        }

        private Int32 _pColor = 0;
        private void timer1_Tick(Object sender, EventArgs e)
        {
            var cfg = NetConfig.Current;
            if (cfg.ColorLog) txtReceive.ColourDefault(_pColor);
            _pColor = txtReceive.TextLength;
        }
        #endregion

        #region 右键菜单
        private void mi清空_Click(Object sender, EventArgs e) => txtReceive.Clear();

        private void Menu_Click(Object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;
            mi.Checked = !mi.Checked;
        }
        #endregion

        private void dataGridView1_CellValueChanged(Object sender, DataGridViewCellEventArgs e) => dgv.Refresh();

        private void btnAdd_Click(Object sender, EventArgs e)
        {
            var unit = new RegisterUnit();
            if (_regs.Count > 0) unit.Address = _regs[^1].Address + 1;
            _regs.Add(unit);

            dgv.DataSource = null;
            dgv.DataSource = _regs;
            dgv.Refresh();
        }
    }
}