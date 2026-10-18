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
        public String ComName => _Port.PortName;

        /// <summary>波特率</summary>
        public Int32 Baudrate => _Port.Baudrate;

        #endregion

        #region 数据

        /// <summary>最多32路输入</summary>
        private bool[] _Input;
        /// <summary>最多32路输出</summary>
        private bool[] _Output;

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

        /// <summary>一次写所有</summary>
        /// <param name="oup"></param>
        public async Task SetDo(byte[] oup)
        {
            if ((oup == null) || (oup.Length < 1)) return;

            var regCnt = (DOCnt + 15) / 16;
            var reg = new ushort[regCnt];

            var ouplen = (oup.Length + 1) / 2;


            // return await _Port.WriteRegisters(DevAddr, 0, oup);

        }



    }

    public class BSM_0816RB : BSM_XXXXRB
    {
        public BSM_0816RB() : base(8, 16) { }
    }
}
