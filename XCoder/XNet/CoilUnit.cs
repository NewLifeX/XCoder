using System.ComponentModel;

namespace XCoder.XNet
{
    /// <summary>线圈单元</summary>
    internal class CoilUnit
    {
        /// <summary>寄存器地址</summary>
        [ReadOnly(true)]
        public Int32 Address { get; set; }

        /// <summary>寄存器数值。用户视角的数值，Modbus是大端字节序</summary>
        public Byte Value { get; set; }
    }
}