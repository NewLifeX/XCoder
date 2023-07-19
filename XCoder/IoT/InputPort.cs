using NewLife.IoT.Protocols;

namespace WinModbus;

public partial class InputPort : UserControl
{
    public String Name { get; set; }

    public Int32 Address { get; set; }

    public Modbus Modbus { get; set; }

    public Byte Host { get; set; }

    public InputPort()
    {
        InitializeComponent();
    }

    private void Relay_Load(Object sender, EventArgs e)
    {
        label1.Text = Name;
    }

    public void SetStatus(Boolean value)
    {

    }

    private void btnOpen_Click(Object sender, EventArgs e)
    {
        Modbus.WriteCoil(Host, (UInt16)Address, 0xFF00);
    }

    private void btnClose_Click(Object sender, EventArgs e)
    {
        Modbus.WriteCoil(Host, (UInt16)Address, 0x0000);
    }
}