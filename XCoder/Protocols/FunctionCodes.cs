using System;

namespace NewLife.IoT.Protocols
{
    /// <summary>Modbus功能码</summary>
    public enum FunctionCodes : Byte
    {
        /// <summary>读单个线圈</summary>
        ReadCoil = 1,

        /// <summary>读离散量输入状态</summary>
        ReadDiscrete = 2,

        /// <summary>读保持寄存器</summary>
        ReadRegister = 3,

        /// <summary>读输入寄存器</summary>
        ReadInput = 4,

        /// <summary>写单个线圈</summary>
        WriteCoil = 5,

        /// <summary>写单个保持寄存器</summary>
        WriteRegister = 6,

        /// <summary>写多个线圈</summary>
        WriteCoils = 15,

        /// <summary>写多个保持寄存器</summary>
        WriteRegisters = 16,

        //ReadWriteMultipleRegisters = 23,
    }
}