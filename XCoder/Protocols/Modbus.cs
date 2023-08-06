using System;
using System.Linq;
using NewLife.Log;
using NewLife.Serialization;

namespace NewLife.IoT.Protocols
{
    /// <summary>Modbus协议</summary>
    public abstract class Modbus
    {
        #region 属性
        /// <summary>性能追踪器</summary>
        public ITracer Tracer { get; set; }
        #endregion

        #region 核心方法
        /// <summary>初始化。传入配置字符串</summary>
        /// <param name="config"></param>
        public virtual void Init(String config) { }

        /// <summary>打开</summary>
        public virtual void Open() { }

        /// <summary>发送两字节命令，并接收返回</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="code">功能码</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public abstract Byte[] SendCommand(Byte host, FunctionCodes code, UInt16 address, Object value);
        #endregion

        #region 读取
        /// <summary>按功能码读取</summary>
        /// <param name="code"></param>
        /// <param name="host"></param>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public virtual Byte[] Read(FunctionCodes code, Byte host, UInt16 address, UInt16 count)
        {
            switch (code)
            {
                case FunctionCodes.ReadCoil: return ReadCoil(host, address, count);
                case FunctionCodes.ReadDiscrete: return ReadDiscrete(host, address, count);
                case FunctionCodes.ReadRegister: return ReadRegister(host, address, count);
                case FunctionCodes.ReadInput: return ReadInput(host, address, count);
                default:
                    break;
            }

            throw new NotSupportedException();
        }

        /// <summary>读取线圈，0x01</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="count">线圈个数</param>
        /// <returns></returns>
        public Byte[] ReadCoil(Byte host, UInt16 address, UInt16 count)
        {
            using var span = Tracer?.NewSpan("modbus:ReadCoil", $"{host} {address:X4} {count:X4}");

            var rs = SendCommand(host, FunctionCodes.ReadCoil, address, count);
            if (rs == null || rs.Length <= 0) return null;

            return rs;
        }

        /// <summary>读离散量输入，0x02</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="count">寄存器个数。每个寄存器2个字节</param>
        /// <returns></returns>
        public Byte[] ReadDiscrete(Byte host, UInt16 address, UInt16 count)
        {
            using var span = Tracer?.NewSpan("modbus:ReadDiscrete", $"{host} {address:X4} {count:X4}");

            var rs = SendCommand(host, FunctionCodes.ReadDiscrete, address, count);
            if (rs == null || rs.Length <= 0) return null;

            return rs;
        }

        /// <summary>读取保持寄存器，0x03</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="count">寄存器个数。每个寄存器2个字节</param>
        /// <returns></returns>
        public Byte[] ReadRegister(Byte host, UInt16 address, UInt16 count)
        {
            using var span = Tracer?.NewSpan("modbus:ReadRegister", $"{host} {address:X4} {count:X4}");

            var rs = SendCommand(host, FunctionCodes.ReadRegister, address, count);
            if (rs == null || rs.Length <= 0) return null;

            return rs;
        }

        /// <summary>读取输入寄存器，0x04</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="count">寄存器个数。每个寄存器2个字节</param>
        /// <returns></returns>
        public Byte[] ReadInput(Byte host, UInt16 address, UInt16 count)
        {
            using var span = Tracer?.NewSpan("modbus:ReadInput", $"{host} {address:X4} {count:X4}");

            var rs = SendCommand(host, FunctionCodes.ReadInput, address, count);
            if (rs == null || rs.Length <= 0) return null;

            return rs;
        }
        #endregion

        #region 写入
        /// <summary>按功能码写入</summary>
        /// <param name="code"></param>
        /// <param name="host"></param>
        /// <param name="address"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual Byte[] Write(FunctionCodes code, Byte host, UInt16 address, UInt16[] values)
        {
            switch (code)
            {
                case FunctionCodes.WriteCoil: return WriteCoil(host, address, values[0]);
                case FunctionCodes.WriteRegister: return WriteRegister(host, address, values[0]);
                case FunctionCodes.WriteCoils: return WriteCoils(host, address, values);
                case FunctionCodes.WriteRegisters: return WriteRegisters(host, address, values);
                default:
                    break;
            }

            throw new NotSupportedException();
        }

        /// <summary>写入单线圈，0x05</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="value">值。一般是 0xFF00/0x0000</param>
        /// <returns></returns>
        public Byte[] WriteCoil(Byte host, UInt16 address, UInt16 value)
        {
            using var span = Tracer?.NewSpan("modbus:WriteCoil", $"{host} {address:X4} {value:X4}");

            var rs = SendCommand(host, FunctionCodes.WriteCoil, address, value);
            if (rs == null || rs.Length <= 0) return null;

            // 去掉2字节地址
            return rs.ReadBytes(2, -1);
        }

        /// <summary>写入保持寄存器，0x06</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="value">数值</param>
        /// <returns></returns>
        public Byte[] WriteRegister(Byte host, UInt16 address, UInt16 value)
        {
            using var span = Tracer?.NewSpan("modbus:WriteRegister", $"{host} {address:X4} {value:X4}");

            var rs = SendCommand(host, FunctionCodes.WriteRegister, address, value);
            if (rs == null || rs.Length <= 0) return null;

            return rs;
        }

        /// <summary>写多个线圈，0x0F</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="values">值。一般是 0xFF00/0x0000</param>
        /// <returns></returns>
        public Byte[] WriteCoils(Byte host, UInt16 address, UInt16[] values)
        {
            using var span = Tracer?.NewSpan("modbus:WriteCoils", $"{host} {address:X4} {values.Join("-", e => e.ToString("X4"))}");

            var binary = new Binary { IsLittleEndian = false };
            binary.Write(address);
            binary.Write((UInt16)values.Length);

            var buf = values.SelectMany(e => e.GetBytes(false)).ToArray();
            binary.Write((UInt16)(1 + buf.Length));
            binary.Write(buf, 0, buf.Length);

            var rs = SendCommand(host, FunctionCodes.WriteCoils, address, binary.GetBytes());
            if (rs == null || rs.Length <= 0) return null;

            return rs;
        }

        /// <summary>写多个保持寄存器，0x10</summary>
        /// <param name="host">主机。一般是1</param>
        /// <param name="address">地址。例如0x0002</param>
        /// <param name="values">数值</param>
        /// <returns></returns>
        public Byte[] WriteRegisters(Byte host, UInt16 address, UInt16[] values)
        {
            using var span = Tracer?.NewSpan("modbus:WriteRegisters", $"{host} {address:X4} {values.Join("-", e => e.ToString("X4"))}");

            var binary = new Binary { IsLittleEndian = false };
            binary.Write(address);
            binary.Write((UInt16)values.Length);

            var buf = values.SelectMany(e => e.GetBytes(false)).ToArray();
            binary.Write((UInt16)(1 + buf.Length));
            binary.Write(buf, 0, buf.Length);

            var rs = SendCommand(host, FunctionCodes.WriteRegisters, address, binary.GetBytes());
            if (rs == null || rs.Length <= 0) return null;

            return rs;
        }
        #endregion

        #region 日志
        /// <summary>日志</summary>
        public ILog Log { get; set; }

        /// <summary>写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
        #endregion
    }
}