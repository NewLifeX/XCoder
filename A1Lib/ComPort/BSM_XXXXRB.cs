using NewLife.Serial.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALib.ComPort
{
    /*
    VOKM昱控科技
    Modbus-RTU  IO控制器。
    BSM-[1][2]RB-[3]AD-[4]DA
    1: DI数字输入数量
    2: DO数字输出数量
    3: AI模拟输入数量
    4: AO模拟输出数量
    RB:纯串口(E:网口+串口)

    本类只考虑 DI  DO
    */

    /// <summary></summary>    
    public class BSM_XXXXRB
    {
        #region 串口

        /// <summary>端口</summary>
        public String ComName
        {
            get => _Port.PortName;
            set => _Port.PortName = value;
        }

        /// <summary>波特率</summary>
        public Int32 Baudrate
        {
            get => _Port.Baudrate;
            set => _Port.Baudrate = value;
        }

        /// <summary>打开</summary>
        public void Open() => _Port.Open();

        #endregion

        #region 数据

        /// <summary>输入路数</summary>
        public Int32 DICnt;

        /// <summary>输出路数</summary>
        public Int32 DOCnt;

        #endregion

        #region 通讯参数

        /// <summary>设备地址</summary>
        public byte DevAddr;

        // 读输入  DevAddr + Cmd02 + RegAddr00 + DICnt
        // 写输出  DevAddr + cmd0f + RegAddr00 + DOCnt

        private ModbusRtu _Port = new ModbusRtu();

        #endregion

        /// <summary></summary>
        /// <param name="inpCnt">输入口数量</param>
        /// <param name="OupCnt">输出口数量</param>
        public BSM_XXXXRB(int inpCnt, int OupCnt)
        {
            DICnt = inpCnt;
            DOCnt = OupCnt;
        }

        /// <summary>设置串口参数</summary>
        /// <param name="com"></param>
        /// <param name="baudrate"></param>
        public void SetCom(string com, int baudrate = 9600)
        {
            _Port.PortName = com;
            _Port.Baudrate = baudrate;
        }

        #region 输出

        /// <summary>一次写所有，从0开始</summary>
        /// <param name="oup">按照bit使用</param>
        public int WriteAllDO(byte[] oup)
        {
            if ((oup == null) || (oup.Length < 1)) return -1;
            // 长度限制
            int len = oup.Length * 8  > DOCnt ? DOCnt : oup.Length;
            var coils = new ushort[len];
            // 数值
            for (int i = 0; i < coils.Length; i++)
            {
                int byteIdx = i / 8;
                int bitIdx = i % 8;
                int mask = 1 << bitIdx;
                var coil = (oup[byteIdx] & mask) == 0 ? 0 : 0xffff;
                coils[i] = (ushort)coil;
            }
            // 写
            return _Port.WriteCoils(DevAddr, 0, coils);
        }

        public int WriteCoil(ushort addr, ushort coil) => _Port.WriteCoil(DevAddr, addr, coil);

        public int WriteCoils(ushort addr, ushort[] coils) => _Port.WriteCoils(DevAddr, addr, coils);

        #endregion
    }

    public class BSM_0816RB : BSM_XXXXRB
    {
        public BSM_0816RB() : base(8, 16) { }
    }
}
