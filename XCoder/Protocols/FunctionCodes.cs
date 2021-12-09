using System;

namespace NewLife.IoT.Protocols
{
    public enum FunctionCodes : Byte
    {
        ReadCoil = 1,

        ReadDiscrete = 2,

        ReadHolding = 3,

        ReadInput = 4,

        WriteCoil = 5,

        WriteHolding = 6,

        //WriteMultipleCoils = 15,

        //WriteMultipleRegister = 16,

        //ReadWriteMultipleRegisters = 23,
    }
}