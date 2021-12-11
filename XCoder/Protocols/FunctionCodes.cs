using System;
using System.ComponentModel;

namespace NewLife.IoT.Protocols
{
    /// <summary>Modbus功能码</summary>
    public enum FunctionCodes : Byte
    {
        /// <summary>读单个线圈</summary>
        [Description("01读单个线圈")]
        ReadCoil = 1,

        /// <summary>读离散量输入状态</summary>
        [Description("02读离散量输入")]
        ReadDiscrete = 2,

        /// <summary>读保持寄存器</summary>
        [Description("03读保持寄存器")]
        ReadRegister = 3,

        /// <summary>读输入寄存器</summary>
        [Description("04读输入寄存器")]
        ReadInput = 4,

        /// <summary>写单个线圈</summary>
        [Description("05写单个线圈")]
        WriteCoil = 5,

        /// <summary>写单个保持寄存器</summary>
        [Description("06写保持寄存器")]
        WriteRegister = 6,

        /// <summary>写多个线圈</summary>
        [Description("15写多个线圈")]
        WriteCoils = 15,

        /// <summary>写多个保持寄存器</summary>
        [Description("16写多个保持寄存器")]
        WriteRegisters = 16,

        //ReadWriteMultipleRegisters = 23,
    }
}