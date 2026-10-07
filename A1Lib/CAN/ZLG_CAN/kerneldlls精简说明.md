# kerneldlls 目录精简说明

本文档说明 `kerneldlls/` 目录的作用、精简依据，以及后续扩展（更换设备型号）时的恢复方法。

---

## 一、kerneldlls 是什么

ZLG（致远）CAN 接口卡的原生 SDK 分为两层：

```
zlgcan.dll                    分发层 / 统一 API（ZCAN_OpenDevice、ZCAN_Transmit ...）
    └── kerneldlls/           内核驱动层（按设备型号分别提供）
            ├── dll_cfg.ini   设备类型码 → 驱动文件 的映射表
            ├── USBCAN.dll    某一类设备的实际收发实现
            └── ...
```

`zlgcan.dll` 自身**不含**任何设备逻辑。它启动时读取 `kerneldlls/dll_cfg.ini`，
按映射表加载对应型号的驱动，再由驱动完成与硬件的实际通信。

**关键约束**：`zlgcan.dll` 按「自身所在目录 / kerneldlls /」这一**固定相对路径**查找，
不支持通过参数指定其它位置。因此 kerneldlls 必须与 `zlgcan.dll` 落在同一个目录。

---

## 二、为什么要精简

官方发布的 kerneldlls 覆盖**全部产品线**，包含大量本项目用不到的驱动：

| 类别 | 举例 | 本项目是否需要 |
|---|---|---|
| PCI / PCIe 插卡 | `PCI9810.dll`、`pcie9120.dll` | 否 |
| 以太网 / 无线 | `CANET_TCP.dll`、`CANFDNET.dll`、`CANWIFI_TCP.dll` | 否 |
| CANFD 系列 | `USBCANFD.dll`、`xpcfd.dll`、`ZPSCANFD.dll` | 否 |
| 其它 | `ZlgCloud.dll`、`CAN_REPLAY.dll`、`CANDTU_*.dll` | 否 |
| **USB 经典 CAN** | **`USBCAN.dll`** | **是** |

精简带来的收益：

- 嵌入资源由 **135 个文件降为 3 个**，首次运行释放耗时显著降低
- 发布体积减小
- 目录结构清晰，便于维护与排查

---

## 三、当前精简结果

### 3.1 保留的文件

```
kerneldlls/
├── dll_cfg.ini     驱动映射表（已改写，见 3.2）
├── USBCAN.dll      本项目实际使用的驱动，支持 USBCAN-I / USBCAN-II
└── USBCAN.xml      USBCAN.dll 对应的属性描述文件
```

### 3.2 dll_cfg.ini 的改法

原始文件按段组织，每段内的条目是**顺序索引**（`1=`、`2=` ……），
条目的序号与段头的 `COUNT` 必须自洽，**因此不能简单删行**，
必须同时修改 `COUNT`，否则 `zlgcan.dll` 解析会出错。

段名（`[KERNELDLL_VCI_CAN]` 等）是 `zlgcan.dll` 内部约定的**固定标识，不可删除**，
没有对应设备时把 `COUNT` 置 `0` 即可。

精简后的内容：

```ini
[KERNELDLL_VCI_CAN]
COUNT=2
START=1
1=USBCAN.dll 3 32 1
2=USBCAN.dll 4 32 1

[KERNELDLL_ZLG_CAN]
COUNT=0
START=1

[KERNELDLL_CANSCOPE]
COUNT=0
START=1
```

三个段的作用：

| 段名 | 用途 | 精简前条数 |
|---|---|---|
| `KERNELDLL_VCI_CAN` | 经典 CAN 设备 | 33 |
| `KERNELDLL_ZLG_CAN` | CANFD 设备 | 49 |
| `KERNELDLL_CANSCOPE` | CANScope 分析仪 | 1 |

### 3.3 条目字段含义

```
1=USBCAN.dll 3 32 1
│  │          │ │  └─ USB 标志（1 = USB 接口设备）
│  │          │ └──── 通道数上限
│  │          └────── 设备类型码（对应 ZlgCanApi 中的 ZCAN_* 常量）
│  └───────────────── 驱动文件名
└──────────────────── 段内序号
```

保留的两条分别对应：

| 序号 | 驱动 | 设备类型码 | 常量 | 含义 |
|---|---|---|---|---|
| 1 | `USBCAN.dll` | 3 | `ZCAN_USBCAN1` | USBCAN-I，单通道 |
| 2 | `USBCAN.dll` | 4 | `ZCAN_USBCAN2` | USBCAN-II，双通道 |

> **注意**：这两条**共用同一个 `USBCAN.dll`**，是同一驱动注册的两个类型别名。
> 因此枚举时同一台物理设备会被识别两次（`ZLG-3-*` 与 `ZLG-4-*`）。
> 代码侧已在 `ZlgCanPort.EnumDevices()` 中按「硬件类型 + 序列号」去重，
> 并优先保留通道数与实际相符的类型（双通道设备归 `ZCAN_USBCAN2`）。
> 详见 `ZlgCanPort._ChannelFit()` 与 `IsBetterType()`。

---

## 四、精简是如何验证的

为避免「删出问题」，采取了先验证后替换的步骤：

1. **备份**：整份复制原始 kerneldlls（135 个文件）到源码树外的
   `ZLG_USBCAN/kerneldlls_original/`
   （放在源码树外是刻意的 —— `A1Lib.csproj` 用 `kerneldlls\**\*.*` 通配符嵌入，
   若备份留在目录内会被一并打包）

2. **边界测试**：在临时目录构造最激进的配置（仅 `USBCAN.dll` + `dll_cfg.ini`
   两条记录，不含 `CANDevCore.dll`、不含 `devices_property/`），
   实测设备仍能正常枚举 → 证明这些文件对**枚举与收发**并非必需

3. **替换并端到端验证**：精简实际目录后，从零释放再跑完整枚举流程

验证结果：

```
[before] kerneldlls=False        ← 运行前物理目录不存在（走资源释放路径）
[after ] files=3                 ← 仅释放 3 个文件
[enum  ] count=2
   ZLG-4-0-0                     ← 设备正常识别
   ZLG-4-0-1                     ← 且已按序列号去重
```

---

## 五、关于 devices_property 目录

原始 kerneldlls 下的 `devices_property/` 存放各型号的**属性描述 XML**
（波特率选项、参数范围等元数据），供 ZLG 官方上位机绘制配置界面使用。

本项目的波特率由代码中的 `ZlgCanApi.TryGetTiming()` 直接映射为寄存器值，
**不读取这些 XML**，故已全部删除。

---

## 六、后续扩展：更换设备型号

如需接入其它型号，按以下步骤操作：

1. 从 `ZLG_USBCAN/kerneldlls_original/` 取回对应驱动 `.dll`
   （以及可选的同名 `.xml`）放回 `kerneldlls/`

2. 在 `kerneldlls/dll_cfg.ini` 的相应段中**恢复或追加**条目，
   并**同步更新该段的 `COUNT`**

3. 如新增的是 USB 设备，确认其类型码已列入
   `ZlgCanPort.ScanDeviceTypes`，否则不会被枚举

常用型号对照（摘自原始 dll_cfg.ini）：

| 型号 | 驱动文件 | 类型码 |
|---|---|---|
| USBCAN-I | `USBCAN.dll` | 3 |
| USBCAN-II | `USBCAN.dll` | 4 |
| USBCAN-II E-U（单通道） | `USBCAN_E_64.dll` | 20 |
| USBCAN-II 2E-U（双通道） | `USBCAN_E_64.dll` | 21 |
| USBCAN-4E-U | `USBCAN_4E_U_X64.dll` | 31 |
| USBCAN-8E-U | `USBCAN_8E_U_x64.dll` | 34 |
| USBCANFD-200U | `USBCANFD.dll` | 41 |
| USBCANFD-100U | `USBCANFD.dll` | 42 |
| USBCANFD-MINI | `USBCANFD.dll` | 43 |
| USBCANFD-800U | `USBCANFD800U.dll` | 59 |

> CANFD 设备属于 `[KERNELDLL_ZLG_CAN]` 段，且初始化时
> `ZCAN_CHANNEL_INIT_CONFIG.can_type` 需传 `TYPE_CANFD`（当前实现固定为 `TYPE_CAN`）。

---

## 七、相关代码位置

| 功能 | 文件 |
|---|---|
| P/Invoke 声明、设备类型常量、波特率寄存器表 | `ZlgCanApi.cs` |
| 端口实现（枚举、去重、打开、收发、关闭） | `ZlgCanPort.cs` |
| kerneldlls 嵌入资源的运行时释放 | `KernelDllExtractor.cs` |
| 资源嵌入配置（csproj） | `A1Lib/A1Lib.csproj` |
