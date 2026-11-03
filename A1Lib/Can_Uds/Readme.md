# Can_Uds — CAN 上 UDS 诊断协议（ISO 14229 / ISO 15765-2）

在 `ALib.CAN.ICanPort` 之上实现 UDS 诊断协议栈，分三层：

```
UdsClient / UdsServer        应用层：UDS 服务（ISO 14229-1）
        ↓
UdsRequest / UdsResponse     报文封装、NRC、会话上下文
        ↓
IsoTpChannel                 传输层：ISO-TP（ISO 15765-2）多帧拆包/流控
        ↓
ICanPort (ZLG / CX / PCAN)   数据链路层：CAN 帧收发
```

## 文件

| 文件 | 内容 |
|------|------|
| `IsoTp.cs` | ISO-TP 传输层：寻址模式、PCI 帧类型、SF/FF/CF/FC 收发、多帧重组、流控与定时参数 |
| `UdsDefinition.cs` | 服务 SID、NRC、会话、复位、DTC 状态、例程、安全等级、DID、常量 |
| `UdsMessage.cs` | `UdsRequest` / `UdsResponse` 封装、`UdsSession` 会话上下文、`UdsException` |
| `UdsClient.cs` | Tester 侧客户端：同步请求响应、常用服务方法、S3 保活、安全解锁 |
| `UdsServer.cs` | ECU 侧服务端：注册服务处理器并自动回响应，用于仿真与联调 |

## 快速上手（客户端）

```csharp
using ALib.CAN;
using ALib.Can_Uds;

var port = ...;                    // 已打开的 ICanPort
port.Open();

// 在端口上创建 UDS 客户端（每端口单例，自动复用 ISO-TP 通道）
var uds = port.GetOrAdd(UdsClient.KeyItems, () => UdsClient.CreateByEcu(port, 0));
uds.Log = XTrace.Log;

// 读 VIN（DID 0xF190）
uds.DiagnosticSessionControl(UdsSession_e.Extended);
var vin = uds.ReadDataByIdentifier(0xF190);
Console.WriteLine(vin.ToStr());

// 进入编程会话 + 安全解锁（需提供种子→密钥算法）
uds.DiagnosticSessionControl(UdsSession_e.Programming);
uds.SecurityAccess(UdsSecurityLevel_e.RequestSeedLevel1, seed => KeyAlgorithm(seed));

// 读取 DTC
var dtcs = uds.ReadDtcByStatusMask(0xFF);
foreach (var d in dtcs) Console.WriteLine(d);

// 用完释放
uds.Close();
```

## 常用服务方法（UdsClient）

| 服务 | 方法 |
|------|------|
| 0x10 会话控制 | `DiagnosticSessionControl(UdsSession_e)` |
| 0x11 复位 | `ECUReset(UdsReset_e)` |
| 0x14 清除 DTC | `ClearDiagnosticInformation()` |
| 0x19 读 DTC | `ReadDtcByStatusMask()` / `ReadDtcCount()` / `ReadDTCInformation()` |
| 0x22 读数据 | `ReadDataByIdentifier(did)` |
| 0x23 读内存 | `ReadMemoryByAddress(addr, len)` |
| 0x27 安全访问 | `SecurityAccess(level, keyAlgo)` / `SecurityAccessRequestSeed` / `SecurityAccessSendKey` |
| 0x28 通信控制 | `CommunicationControl(ctrl, type)` |
| 0x2E 写数据 | `WriteDataByIdentifier(did, data)` |
| 0x2F IO 控制 | `InputOutputControlByIdentifier(did, ctrl, data)` |
| 0x31 例程控制 | `StartRoutine/StopRoutine/RequestRoutineResults` |
| 0x34/35/36/37 刷写 | `RequestDownload` / `Download` / `TransferData` / `RequestTransferExit` |
| 0x3D 写内存 | `WriteMemoryByAddress(addr, data)` |
| 0x3E 保活 | `TesterPresent()` |

- `Request()` 返回 `UdsResponse`，超时返回 `null`，否定响应通过 `IsNegative` / `Nrc` 判断。
- `RequestThrow()` 在否定响应时抛出 `UdsException`（含 NRC 与中文描述）。
- 自动处理 `NRC 0x78`（响应挂起）：延长到 `P2*` 继续等待。
- `StartKeepAlive()` 在非默认会话时自动周期发送 `3E 80` 保活。

## 服务端（仿真 ECU）

```csharp
var srv = port.GetOrAdd(UdsServer.KeyItems, () => new UdsServer(port, 0x7E0, 0x7E8));
srv.Register(new UdsSessionControlHandler());
srv.Register(new UdsTesterPresentHandler());
srv.Register(new UdsEcuResetHandler { OnReset = r => Console.WriteLine($"复位 {r}") });

// 自定义服务：返回 null 表示抑制响应；抛 UdsException 回否定响应
srv.Register(UdsService_e.ReadDataByIdentifier, (server, req, session) =>
{
    var did = (UInt16)((req[1] << 8) | req[2]);
    if (did != 0xF190) throw new UdsException(UdsService_e.ReadDataByIdentifier, UdsNrc_e.RequestOutOfRange);
    var body = new List<byte> { 0xF1, 0x90 };   // 响应体自动补 SID+0x40
    body.AddRange(Encoding.ASCII.GetBytes("VIN1234567890ABCD"));
    return body.ToArray();
});
```

## ISO-TP 说明

```csharp
var tp = new IsoTpChannel(port, 0x7E0, 0x7E8) { Log = XTrace.Log };
tp.Addressing = IsoTpAddressing_e.Normal;    // 或 Extended 扩展寻址
tp.Timing.N_Bs = 1000;                        // 等待流控超时
tp.Timing.N_Cr = 1000;                        // 连续帧接收超时
tp.MessageReceived += (s, m) => Console.WriteLine(m);
tp.Send(new byte[] { 0x22, 0xF1, 0x90 });     // 自动单帧/多帧
```

- 支持普通寻址（Normal）与扩展寻址（Extended）；混合寻址未实现。
- 发送侧：自动发 FF → 等 FC → 按 BS/STmin 发 CF，支持多轮流控窗口与 `FS=Wait`。
- 接收侧：自动重组 FF/CF 并回送 FC，序号不连续/超时丢包。
- 经典 CAN 多帧上限 4095 字节。

## 线程模型（重要）

- `IsoTpChannel.Process` 由 CAN 接收线程回调，负责收包与状态机推进。
- `UdsClient` 的所有诊断方法、`IsoTpChannel.Send` 都会**阻塞调用线程**直到完成/超时。
- **禁止在接收线程或 `MessageReceived` 事件内调用诊断方法或发送**，否则死锁。
