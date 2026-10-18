using ALib.UsbHub;
using NewLife;
using NewLife.Data;
using NewLife.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALib.ComPort
{
    /// <summary>GRBL机器状态枚举</summary>
    public enum GrblMachineState
    {
        /// <summary>空闲状态</summary>
        Idle,
        /// <summary>运行状态</summary>
        Run,
        /// <summary>暂停状态，急停按键或者！触发，使用~清除</summary>
        Hold,
        /// <summary>点动状态</summary>
        Jog,
        /// <summary>报警状态，通过$X解锁能到Idle，建议再$H命令归位一下</summary>
        Alarm,
        /// <summary>门状态,防护门没关</summary>
        Door,
        /// <summary>检查状态</summary>
        Check,
        /// <summary>回零状态</summary>
        Home,
        /// <summary>睡眠状态</summary>
        Sleep
    }

    /// <summary>GRBL状态报告数据</summary>
    public class GrblStatus
    {
        /// <summary>机器状态</summary>
        public GrblMachineState MachineState { get; set; } = GrblMachineState.Idle;

        /// <summary>子状态</summary>
        public int SubState { get; set; } = -1;

        /// <summary>MPos 机器位置 (X,Y,Z)</summary>
        public double[] MachinePosition { get; set; } = new double[3];

        /// <summary>WPos 工作位置 (X,Y,Z)</summary>
        public double[] WorkPosition { get; set; } = new double[3];

        /// <summary>WCO 工作坐标偏移 (X,Y,Z)</summary>
        public double[] WorkCoordinateOffset { get; set; } = new double[3];

        /// <summary>缓冲区状态 (可用块数, 可用字节数)</summary>
        public int[] BufferState { get; set; } = new int[2];

        /// <summary>当前行号</summary>
        public int LineNumber { get; set; } = -1;

        /// <summary>进给速度</summary>
        public double FeedRate { get; set; } = 0;

        /// <summary>主轴转速</summary>
        public double SpindleSpeed { get; set; } = 0;

        /// <summary>输入引脚状态</summary>
        public string PinState { get; set; } = "";

        /// <summary>进给倍率</summary>
        public int FeedOverride { get; set; } = 100;

        /// <summary>快速移动倍率</summary>
        public int RapidOverride { get; set; } = 100;

        /// <summary>主轴倍率</summary>
        public int SpindleOverride { get; set; } = 100;

        /// <summary>主轴状态</summary>
        public bool SpindleCW { get; set; } = false;

        /// <summary>主轴逆时针状态</summary>
        public bool SpindleCCW { get; set; } = false;

        /// <summary>冷却液状态</summary>
        public bool FloodCoolant { get; set; } = false;

        /// <summary>雾状冷却液状态</summary>
        public bool MistCoolant { get; set; } = false;

        /// <summary>返回状态信息的字符串表示</summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"机器状态: {MachineState}");
            if (SubState >= 0)
            {
                sb.AppendLine($"子状态: {SubState}");
            }

            sb.AppendLine($"机器位置: X={MachinePosition[0]:F3}, Y={MachinePosition[1]:F3}, Z={MachinePosition[2]:F3}");
            sb.AppendLine($"工作位置: X={WorkPosition[0]:F3}, Y={WorkPosition[1]:F3}, Z={WorkPosition[2]:F3}");
            sb.AppendLine($"工作坐标偏移: X={WorkCoordinateOffset[0]:F3}, Y={WorkCoordinateOffset[1]:F3}, Z={WorkCoordinateOffset[2]:F3}");

            if (BufferState[0] >= 0 || BufferState[1] >= 0)
            {
                sb.AppendLine($"缓冲区状态: 可用块数={BufferState[0]}, 可用字节数={BufferState[1]}");
            }

            if (LineNumber >= 0)
            {
                sb.AppendLine($"当前行号: {LineNumber}");
            }

            sb.AppendLine($"进给速度: {FeedRate:F1}");
            sb.AppendLine($"主轴转速: {SpindleSpeed:F0} RPM");

            if (!string.IsNullOrEmpty(PinState))
            {
                sb.AppendLine($"输入引脚状态: {PinState}");
            }

            sb.AppendLine($"进给倍率: {FeedOverride}%");
            sb.AppendLine($"快速移动倍率: {RapidOverride}%");
            sb.AppendLine($"主轴倍率: {SpindleOverride}%");

            sb.AppendLine($"主轴状态: {(SpindleCW ? "顺时针" : "停止")}{(SpindleCCW ? " + 逆时针" : "")}");
            sb.AppendLine($"冷却液: {(FloodCoolant ? "液冷开" : "液冷关")}, {(MistCoolant ? "雾冷开" : "雾冷关")}");

            return sb.ToString();
        }
    }

    public class Grbl_Com : DisposeBase
    {
        public SerialTransport ComPort { get; private set; } = new SerialTransport();

        public GrblStatus CurrStatus;

        public CircularQueue<byte> RxQueue { get; private set; } = new CircularQueue<byte>(256);

        public Grbl_Com(String comName)
        {
            ComPort.PortName = comName;
            ComPort.BaudRate = 115200;
            ComPort.Timeout = 1;
            ComPort.ByteTimeout = 10;
        }

        public Grbl_Com(SerialTransport com) 
        {
            ComPort = com;

            ComPort.BaudRate = 115200;
            ComPort.Timeout = 1;
            ComPort.ByteTimeout = 10;
        }

        public static Grbl_Com CreateByUsb(UsbDevice dev, int subidx)
        {
            var com = ComHelper.CreateSerialTransport(dev, subidx);
            if (com == null) return null;
            return new Grbl_Com(com);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Close();
        }

        private void ComPort_Received(object sender, NewLife.Net.ReceivedEventArgs e)
        {
            RxQueue.WriteArray(e.GetBytes());
        }

        private async Task<bool> WaitWith(string str, int timout = 100)
        {
            var tout = DateTime.Now.AddMilliseconds(timout);
            while (true)
            {
                if (DateTime.Now > tout) return false;
                await Task.Delay(20);

                if (RxQueue.Count() < str.Length) continue;
                var bs = RxQueue.ToArray();
                if (bs.ToStr().IndexOf(str) != -1) return true;
            }
        }

        public bool Open()
        {
            ComPort.Received += ComPort_Received;
            return ComPort.Open();
        }

        public bool Close()
        {
            ComPort.Received -= ComPort_Received;
            return ComPort.Close();
        }

        /// <summary>发送一行Gcode命令</summary>
        /// <param name="str"></param>
        /// <param name="hasRN">有回车</param>
        /// <param name="tout">超时，ms</param>
        /// <returns></returns>
        public async Task<bool> SendGcodeLine(String str, bool hasRN = true, int tout = 200)
        {
            // 去除 \r\n   \r   \n  等几种形式
            str = str.Replace("\n", "");
            str = str.Replace("\r", "");

            WriteLog(str);

            // 加上 \n ，确保格式正确
            if (hasRN) str += '\n';

            RxQueue.Clear();
            var bs = Encoding.ASCII.GetBytes(str);
            lock (ComPort) ComPort.Send(bs);
            if (await WaitWith("ok", tout)) return true;
            return false;
        }

        /// <summary>设置参数</summary>
        /// <param name="idx"></param>
        /// <param name="param"></param>
        /// <param name="tout">超时，ms</param>
        /// <returns></returns>
        public async Task<bool> SendParam(int idx, int param, int tout = 200)
        {
            var cmd = $"${idx}={param}\n";
            WriteLog(cmd);
            var bs = Encoding.ASCII.GetBytes(cmd);
            // var res = await ComPort.SendAsync((ArrayPacket)bs);

            RxQueue.Clear();
            lock (ComPort) ComPort.Send(bs);
            if (await WaitWith("ok", tout)) return true;
            return false;
        }

        /// <summary>解锁</summary>
        /// <returns></returns>
        public async Task<bool> UnLock() => await SendGcodeLine("$X");

        /// <summary>解除Hold</summary>
        /// <returns></returns>
        public async Task<bool> Resume() => await SendGcodeLine("~");

        /// <summary>丢弃所有已发送未执行的gcode，并进入Alarm状态</summary>
        public void Reset()
        {
            var bs = new byte[] { 0x18 };
            lock (ComPort) ComPort.Send(bs);
        }

        /// <summary>硬重启，GBRL-HAL可能支持，可能会断开USB连接</summary>
        public void HardwareReset()
        {
            var bs = new byte[] { 0x1B, 0x14 };
            lock (ComPort) ComPort.Send(bs);
        }

        /// <summary>单轴回零</summary>
        /// <param name="xyz">轴编号，xyzabc</param>
        /// <param name="tSec">超时时间</param>
        /// <returns></returns>
        public async Task<bool> GoHome(char xyz, int tSec = 30)
        {
            var tout = DateTime.Now.AddSeconds(tSec);
            // 轴名称过滤
            var chs = "xXyYzZaAbBcC";
            if (chs.IndexOf(xyz) == -1) return false;

            // 启动
            var cmd = ("$H" + xyz).ToUpper();
            if (!await SendGcodeLine(cmd, true, tSec * 1000)) return false;

            // 以获取状态成功作为结束
            if (await GetStatusReport() != null) return true;
            return false;
        }

        /// <summary>多轴回零</summary>
        /// <param name="xyz">回零的坐标轴,字符串 XYZABC 进行组合，默认X</param>
        /// <param name="tSec">每轴超时时间，秒</param>
        /// <returns></returns>
        public async Task<bool> GoHome(String xyz = "X", int tSec = 30)
        {
            if (xyz.IsNullOrEmpty()) return true;

            foreach (var ch in xyz)
            {
                if (!await GoHome(ch, tSec)) return false;
            }

            return true;
        }

        /// <summary>获取GRBL状态报告 (?命令)</summary>
        /// <returns>状态报告对象，失败返回null</returns>
        public async Task<GrblStatus> GetStatusReport()
        {
            // 发送?命令（不需要换行符）
            var queryCmd = "?";
            var bs = Encoding.ASCII.GetBytes(queryCmd);

            RxQueue.Clear();
            lock (ComPort) ComPort.Send(bs);
            if (await WaitWith(">", 400))
            {
                var resStr = RxQueue.ToArray().ToStr();

                int hidx = resStr.IndexOf('<');
                if (hidx < 0) return null;
                int tidx = resStr.IndexOf('>');
                if (tidx < 0) return null;

                resStr = resStr.Substring(hidx, tidx - hidx + 1);
                WriteLog("  " + resStr);
                return ParseStatusReport(resStr);
            }

            return null;
        }

        #region ？命令解码
        /// <summary>解析状态报告字符串</summary>
        /// <param name="statusStr">状态报告字符串</param>
        /// <returns>状态报告对象</returns>
        private GrblStatus ParseStatusReport(string statusStr)
        {
            if (CurrStatus == null) CurrStatus = new GrblStatus();
            var status = CurrStatus;

            try
            {
                // 移除<>符号
                string content = statusStr.Trim('<', '>');

                // 按|分割各个字段
                string[] fields = content.Split('|');

                foreach (string field in fields)
                {
                    if (string.IsNullOrEmpty(field)) continue;

                    // 机器状态字段（第一个字段，可能包含子状态）
                    if (field.Contains("Idle") || field.Contains("Run") || field.Contains("Hold") ||
                        field.Contains("Jog") || field.Contains("Alarm") || field.Contains("Door") ||
                        field.Contains("Check") || field.Contains("Home") || field.Contains("Sleep"))
                    {
                        ParseMachineState(status, field);
                    }
                    // 位置字段
                    else if (field.StartsWith("MPos:"))
                    {
                        ParsePosition(status.MachinePosition, field.Substring(5));
                    }
                    else if (field.StartsWith("WPos:"))
                    {
                        ParsePosition(status.WorkPosition, field.Substring(5));
                    }
                    else if (field.StartsWith("WCO:"))
                    {
                        ParsePosition(status.WorkCoordinateOffset, field.Substring(4));
                    }
                    // 缓冲区字段
                    else if (field.StartsWith("Bf:"))
                    {
                        ParseBufferState(status, field.Substring(3));
                    }
                    // 行号字段
                    else if (field.StartsWith("Ln:"))
                    {
                        if (int.TryParse(field.Substring(3), out int lineNum))
                        {
                            status.LineNumber = lineNum;
                        }
                    }
                    // 进给和主轴速度字段
                    else if (field.StartsWith("F:"))
                    {
                        if (double.TryParse(field.Substring(2), out double feedRate))
                        {
                            status.FeedRate = feedRate;
                        }
                    }
                    else if (field.StartsWith("FS:"))
                    {
                        string[] fsValues = field.Substring(3).Split(',');
                        if (fsValues.Length >= 2)
                        {
                            if (double.TryParse(fsValues[0], out double feedRate)) status.FeedRate = feedRate;
                            if (double.TryParse(fsValues[1], out double spindleSpeed)) status.SpindleSpeed = spindleSpeed;
                        }
                    }
                    // 引脚状态字段
                    else if (field.StartsWith("Pn:"))
                    {
                        status.PinState = field.Substring(3);
                    }
                    // 倍率字段
                    else if (field.StartsWith("Ov:"))
                    {
                        string[] ovValues = field.Substring(3).Split(',');
                        if (ovValues.Length >= 3)
                        {
                            if (int.TryParse(ovValues[0], out int feedOv)) status.FeedOverride = feedOv;
                            if (int.TryParse(ovValues[1], out int rapidOv)) status.RapidOverride = rapidOv;
                            if (int.TryParse(ovValues[2], out int spindleOv)) status.SpindleOverride = spindleOv;
                        }
                    }
                    // 附件状态字段
                    else if (field.StartsWith("A:"))
                    {
                        ParseAccessoryState(status, field.Substring(2));
                    }
                }
            }
            catch (Exception ex)
            {
                // 解析出错，返回部分解析的结果
                XTrace.WriteLine($"解析GRBL状态报告出错: {ex.Message}");
            }

            return status;
        }

        /// <summary>解析机器状态</summary>
        private void ParseMachineState(GrblStatus status, string stateStr)
        {
            // 检查是否包含子状态（格式：State:SubState）
            if (stateStr.Contains(":"))
            {
                string[] parts = stateStr.Split(':');
                status.MachineState = ParseMachineStateEnum(parts[0]);
                if (parts.Length > 1 && int.TryParse(parts[1], out int subState))
                {
                    status.SubState = subState;
                }
            }
            else
            {
                status.MachineState = ParseMachineStateEnum(stateStr);
            }
        }

        /// <summary>将状态字符串转换为枚举</summary>
        private GrblMachineState ParseMachineStateEnum(string stateStr)
        {
            return stateStr switch
            {
                "Idle" => GrblMachineState.Idle,
                "Run" => GrblMachineState.Run,
                "Hold" => GrblMachineState.Hold,
                "Jog" => GrblMachineState.Jog,
                "Alarm" => GrblMachineState.Alarm,
                "Door" => GrblMachineState.Door,
                "Check" => GrblMachineState.Check,
                "Home" => GrblMachineState.Home,
                "Sleep" => GrblMachineState.Sleep,
                _ => GrblMachineState.Idle
            };
        }

        /// <summary>解析位置信息</summary>
        private void ParsePosition(double[] position, string posStr)
        {
            string[] coords = posStr.Split(',');
            for (int i = 0; i < Math.Min(coords.Length, 3); i++)
            {
                if (double.TryParse(coords[i], out double value))
                {
                    position[i] = value;
                }
            }
        }

        /// <summary>解析缓冲区状态</summary>
        private void ParseBufferState(GrblStatus status, string bfStr)
        {
            string[] bfValues = bfStr.Split(',');
            if (bfValues.Length >= 2)
            {
                if (int.TryParse(bfValues[0], out int val1)) status.BufferState[0] = val1;
                if (int.TryParse(bfValues[1], out int val2)) status.BufferState[1] = val2;
            }
        }

        /// <summary>解析附件状态</summary>
        private void ParseAccessoryState(GrblStatus status, string accStr)
        {
            status.SpindleCW = accStr.Contains("S");
            status.SpindleCCW = accStr.Contains("C");
            status.FloodCoolant = accStr.Contains("F");
            status.MistCoolant = accStr.Contains("M");
        }
        #endregion

        #region G 指令发送

        /// <summary>快速移动过去</summary>
        /// <param name="p"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public async Task<bool> G0_Point(PlaneCoordinate p, double speed = 0)
        {
            string gcode = "";
            if (speed < 1) gcode = $"G0 X{p.X:F4} Y{p.Y:F4}";
            gcode = $"G0 X{p.X:F4} Y{p.Y:F4} F{speed}";

            return await SendGcodeLine(gcode);
        }

        /// <summary>切削过去</summary>
        /// <param name="p"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public async Task<bool> G1_Point(PlaneCoordinate p, double speed = 5000)
        {
            string gcode = $"G1 X{p.X:F4} Y{p.Y:F4} F{speed}";
            return await SendGcodeLine(gcode);
        }

        /// <summary>延迟</summary>
        /// <param name="s">秒</param>
        /// <returns></returns>
        public async Task<bool> G4_Delay(double s)
        {
            // 遵循RS274/NGC标准，P单位是秒
            string gcode = $"G4 P{s:F3}";
            return await SendGcodeLine(gcode);
        }

        #endregion

        #region GCODE + 完成

        /// <summary>执行到完成退出</summary>
        /// <param name="p">目标</param>
        /// <param name="speed">速度</param>
        /// <param name="tsec">等待时间</param>
        /// <returns></returns>
        public async Task<bool> WaitPoint_G1(PlaneCoordinate p, double speed = 800, int tsec = 10)
        {
            var tout = DateTime.Now.AddSeconds(tsec);

            // 确保机器空闲
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (DateTime.Now > tout) return false;
                if (i > 0) await Task.Delay(300);

                // 获取当前坐标
                var info2 = await GetStatusReport();
                if (info2 == null) continue;
                if (info2.MachineState != GrblMachineState.Idle) continue;

                break;
            }

            // 坐标判断
            var info = CurrStatus;
            if ((Math.Abs(p.X - info.MachinePosition[0]) > 1) ||
                (Math.Abs(p.Y - info.MachinePosition[1]) > 1))
            {
                // 运动
                await G1_Point(p);
            }

            // 等待运行到目的地
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (DateTime.Now > tout) return false;
                if (i > 0) await Task.Delay(300);

                // 获取当前坐标
                var info2 = await GetStatusReport();
                if (info2 == null) continue;
                if (info2.MachineState != GrblMachineState.Idle) continue;

                // // 坐标判断
                // if ((Math.Abs(p.X - info2.MachinePosition[0]) > 1) ||
                //     (Math.Abs(p.Y - info2.MachinePosition[1]) > 1))
                // {
                //     continue;
                // }

                break;
            }

            return true;
        }

        #endregion

        #region 日志

        /// <summary>日志前缀</summary>
        public String LogPre = "  ";

        public ILog Log { get; set; }

        void WriteLog(String format, params Object[] args) => Log?.Write(LogLevel.Info, format, args);

        #endregion
    }
}
