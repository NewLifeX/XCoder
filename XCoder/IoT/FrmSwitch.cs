using NewLife;
using NewLife.Data;
using NewLife.IoT.Protocols;
using NewLife.Log;

namespace WinModbus;

public partial class FrmSwitch : Form
{
    private Byte _host = 1;
    private Modbus _modbus;
    private ILog _log;

    public Modbus Modbus { get => _modbus; set => _modbus = value; }

    public Byte Host { get => _host; set => _host = value; }

    public FrmSwitch() => InitializeComponent();

    private void Form1_Load(Object sender, EventArgs e)
    {
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
        var rs = _modbus.Read(FunctionCodes.ReadRegister, _host, 0, 1);
        if (rs == null || rs.Total < 3) return;

        var addr = rs.ReadBytes(1).ToUInt16(0, false);
        numAddr.Value = addr;
    }

    private void btnWriteAddr_Click(Object sender, EventArgs e)
    {
        var addr = (UInt16)numAddr.Value;

        _modbus.WriteRegisters(_host, 0, new[] { addr });
    }

    private void btnReadIn_Click(Object sender, EventArgs e)
    {
        _modbus.ReadDiscrete(_host, 0, 8);
    }
}