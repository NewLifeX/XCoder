using NewLife;
using NewLife.Data;
using NewLife.IoT.Protocols;
using NewLife.Log;
using NewLife.Serialization;

namespace WinModbus;

public partial class FrmI6O6N : Form
{
    private Byte _host = 1;
    private Modbus _modbus;
    private ILog _log;

    public Modbus Modbus { get => _modbus; set => _modbus = value; }

    public Byte Host { get => _host; set => _host = value; }

    Int32[] _baudrates = new[] { 4800, 9600, 19200, 115200, 128000 };

    public FrmI6O6N() => InitializeComponent();

    private void Form1_Load(Object sender, EventArgs e)
    {
    }

    private void btnReadInfo_Click(Object sender, EventArgs e)
    {
        var pk = _modbus.Read(FunctionCodes.ReadRegister,_host, 0x1000, 14);
        if (pk == null || pk.Length == 0) return;

        var ms = pk.GetStream();
        var reader = new Binary { Stream = ms, IsLittleEndian = false };

        txtProduct.Text = reader.ReadFixedString(4);
        txtVersion.Text = reader.ReadFixedString(4);
        txtUUID.Text = reader.ReadBytes(8 * 2).ToHex();

        // 波特率
        if (ms.Position < ms.Length)
        {
            var n = reader.ReadUInt16();
            if (n >= 0 && n < _baudrates.Length) numBaudrate.Value = _baudrates[n];
        }
        // 站号
        if (ms.Position < ms.Length)
        {
            var n = reader.ReadUInt16();
            numHost.Value = n;
        }

        // 识别产品型号，绘制输入输出口
        var prd = txtProduct.Text;
        for (var i = 0; i < prd.Length; i++)
        {
            if (prd[i] == 'I')
            {
                var ch = prd[i + 1];
                var n = ch >= 'A' ? (ch - 'A' + 10) : (ch - '0');
                CreateInput(n);
            }
            else if (prd[i] == 'O')
            {
                var ch = prd[i + 1];
                var n = ch >= 'A' ? (ch - 'A' + 10) : (ch - '0');
                CreateOutput(n);
            }
        }

        groupBox1.Enabled = true;
        groupBox2.Enabled = true;
    }

    void CreateInput(Int32 n)
    {
        var root = inputPort1;
        root.Name = "1号";
        root.Address = 0x0100;
        root.Modbus = _modbus;
        root.Host = _host;

        for (var j = 1; j < n; j++)
        {
            var id = root.Name[..^1] + (j + 1);
            var rs = root.Parent.Controls.Find(id, false);
            if (rs == null || rs.Length == 0)
            {
                var r = new InputPort
                {
                    Name = $"{j + 1}号",
                    Address = 0x0100 + j,
                    Left = root.Left + root.Width * j,
                    Top = root.Top,

                    Modbus = _modbus,
                    Host = _host,
                };

                root.Parent.Controls.Add(r);
            }
        }
    }

    void CreateOutput(Int32 n)
    {
        var root = outputPort1;
        root.Name = "1号";
        root.Address = 0x0000;
        root.Modbus = _modbus;
        root.Host = _host;

        for (var j = 1; j < n; j++)
        {
            var id = root.Name[..^1] + (j + 1);
            var rs = root.Parent.Controls.Find(id, false);
            if (rs == null || rs.Length == 0)
            {
                var r = new OutputPort
                {
                    Name = $"{j + 1}号",
                    Address = 0x0000 + j,
                    Left = root.Left + root.Width * j,
                    Top = root.Top,

                    Modbus = _modbus,
                    Host = _host,
                };

                root.Parent.Controls.Add(r);
            }
        }
    }

    private void btnOpen1_Click(Object sender, EventArgs e)
    {
        var btn = sender as Button;
        var addr = btn.Tag.ToInt() - 1;
        var delay = (Int32)numDelay.Value;

        if (delay > 0)
            _modbus.WriteRegisters(_host, (UInt16)(0x0003 + addr * 5), new UInt16[] { 0x0004, (UInt16)(delay / 100) });
        else
            _modbus.WriteCoil(_host, (UInt16)addr, 0xFF00);
    }

    private void btnClose1_Click(Object sender, EventArgs e)
    {
        var btn = sender as Button;
        var addr = btn.Tag.ToInt() - 1;
        var delay = (Int32)numDelay.Value;

        if (delay > 0)
            _modbus.WriteRegisters(_host, (UInt16)(0x0003 + addr * 5), new UInt16[] { 0x0002, (UInt16)(delay / 100) });
        else
            _modbus.WriteCoil(_host, (UInt16)addr, 0);
    }

    private void btnOpenAll_Click(Object sender, EventArgs e)
    {
        //for (var i = 0; i < 4; i++)
        //{
        //    _modbus.WriteCoil(_host, (UInt16)i, 0xFF00);
        //}
        _modbus.WriteCoils(_host, 0, new UInt16[] { 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00, 0xFF00 });
    }

    private void btnCloseAll_Click(Object sender, EventArgs e)
    {
        //for (var i = 0; i < 4; i++)
        //{
        //    _modbus.WriteCoil(_host, (UInt16)i, 0);
        //}
        _modbus.WriteCoils(_host, 0, new UInt16[] { 0, 0, 0, 0, 0, 0, 0, 0 });
    }

    private void btnReadAll_Click(Object sender, EventArgs e)
    {
        _modbus.ReadCoil(_host, 0, 8);
    }

    private void btnReadAddr_Click(Object sender, EventArgs e)
    {
        var rs = _modbus.Read(FunctionCodes.ReadRegister,_host, 0, 1);
        if (rs == null || rs.Total < 3) return;

        var addr = rs.ReadBytes(1).ToUInt16(0, false);
        numHost.Value = addr;
    }

    private void btnWriteAddr_Click(Object sender, EventArgs e)
    {
        var addr = (UInt16)numHost.Value;

        _modbus.WriteRegisters(_host, 0, new[] { addr });
    }

    private void btnReadIn_Click(Object sender, EventArgs e)
    {
        _modbus.ReadDiscrete(_host, 0x0100, 8);
    }
}