using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLife;

namespace XCoder.XNet
{
    /// <summary>寄存器单元</summary>
    internal class RegisterUnit
    {
        /// <summary>寄存器地址</summary>
        [ReadOnly(true)]
        public Int32 Address { get; set; }

        /// <summary>寄存器数值。用户视角的数值，Modbus是大端字节序</summary>
        public UInt16 Value { get; set; }

        public String ValueHex => Value.GetBytes(false).ToHex();

        /// <summary>获取该寄存器单元的字节数据。Modbus是大端字节序</summary>
        /// <returns></returns>
        public Byte[] GetData() => Value.GetBytes(false);
    }
}