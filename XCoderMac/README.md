# XCoderMac — NewLife 码神工具 macOS 版

> 官方源码：<https://git.newlifex.com/NewLife/XCoder>
> 工程目录：`/Users/yanjinlong/Code/newlife/XCoder/XCoderMac`
> 项目约束：保留 `master`，不修改上游 `XCoderAv`；仅新增/维护本目录代码，根目录 `码神工具.sln` 只添加项目。

## 最新上游兼容与单窗口多标签（2026-10-18）

- 已将项目添加到新版根目录 `码神工具.slnx`；`XCoderMac.csproj` 的 Avalonia.Desktop、Avalonia.Themes.Fluent、Avalonia.Fonts.Inter 已与上游 `XCoderAv` 对齐为 **12.1.1**。
- **恢复单窗口多标签**：左侧栏与原主页保留，工具在主窗口右侧以紧凑的 174px 标签显示；同一工具不重复开窗，顶部左右箭头浏览，标签右侧 × / `⌘W` 关闭，首页与工作区可自由返回。
- 关闭工具时触发 Mac-only 清理，涉及确认操作的弹窗以主窗口作为父窗口。
- Avalonia 12 使用了 `x:CompileBindings="False"` 保留现有反射绑定写法，后续可分阶段迁移为强类型编译绑定。
- 调试验证：`dotnet run --project XCoderMac/XCoderMac.csproj -c Debug -- --smoke-tabs`，逐个测试 26 个工具的标签打开/切换/关闭及多标签导航（不执行业务写入）。
- **Git 当前处于未完成的 rebase**。此次只修改 Mac 源码与 `码神工具.slnx` 的项目登记，不提交、不暂存、不切换分支、不推进 rebase。

## 技术设计

- **.NET 10 + Avalonia 11.2.3**，Apple Silicon ARM64；通过 `ProjectReference` 引用原始 `../XCoderAv/XCoderAv.csproj`（上游 .NET 8）。
- **主界面**：`MacShellWindow` 提供分类、搜索、快捷入口、26 个工具卡片以及 `⌘F` 工具搜索。
- **独立 Mac 界面（8 项）**：正则表达式、数据建模、RPC API 调试、网络工具、Redis 管理、语音助手、USB 设备、声卡信息。
- **通用 Mac 宿主（18 项）**：`MacLegacyToolWindow` 把上游已实现的控件、ViewModel、事件绑定重用在统一的 Mac 外观容器中，**不复制整套上游 XAML**。
- 所有工具从 `MacToolCatalog` 统一启动。对同一个工具再次点击会激活已有窗口而不是重复打开。
- 语音助手使用 macOS 系统命令 `say`，支持朗读、停止、AIFF 导出。
- USB、声卡使用 `system_profiler` 读取原生设备记录（**只读**；不控制硬件）。
- Redis 连接信息保存到 `~/Library/Application Support/XCoderMac/RedisNodes.json`，**密码不写入磁盘**，仅在本次应用会话内使用。由于引用上游 ViewModel，初始加载逻辑仍执行上游配置读取；Mac 窗口会优先恢复 Mac 用户级配置。

## 26 个工具适配情况

| 工具 | Mac UI | 核心功能与验证状态 |
|---|---|---|
| 数据建模 | 独立 | 复用 NewLife.XCode 代码生成；Finder 输出目录；真实数据库未验收 |
| 数据同步 | 通用宿主 | 上游 ViewModel 复用；真实库同步未验收 |
| API 调试 | 独立 | NewLife.Remoting **RPC** 客户端和服务器，不是普通 HTTP API |
| API 发现 | 通用宿主 | 上游复用 |
| 正则表达式 | 独立 | 匹配/替换/分组，`⌘Return`；批量文件覆盖需要确认 |
| 加密解密 | 通用宿主 | 上游复用 |
| 文件编码 | 通用宿主 | 上游复用 |
| 网络工具 | 独立 | NewLife.Net TCP/UDP；真实收发待验收 |
| 消息调试 | 通用宿主 | 上游复用 |
| MQTT 客户端 | 通用宿主 | 上游复用；Broker 未验收 |
| Modbus TCP | 通用宿主 | 上游复用；PLC 未验收 |
| Modbus RTU | 通用宿主 | 上游复用；实体串口未验收 |
| 串口工具 | 通用宿主 | 上游复用；macOS 需检测 `/dev/cu.*`，硬件未验收 |
| SSH 工具 | 通用宿主 | 上游复用；**SSH.NET 高危版本警告待处理** |
| Redis 管理 | 独立 | 树节点浏览、搜索、只读 Key、连接配置；真实 Redis 未验收 |
| IP 设置 | 通用宿主 | 上游复用；更改 macOS 网卡配置需额外授权/适配 |
| USB 设备 | 独立 | macOS `system_profiler SPUSBDataType` 设备只读枚举 |
| 声卡选择 | 独立 | macOS `system_profiler SPAudioDataType` 信息展示；不能切换系统默认声卡 |
| I/O 控制 | 通用宿主 | 上游复用；实体设备未验收 |
| 图标水印 | 通用宿主 | 上游复用 |
| 文件夹统计 | 通用宿主 | 上游复用，保留树节点事件 |
| GPS 辅助 | 通用宿主 | 上游复用 |
| 地图接口 | 通用宿主 | 上游复用 |
| 手机备份 | 通用宿主 | 上游复用；真实手机未验收 |
| 语音助手 | 独立 | macOS `say` 原生朗读、停止、AIFF 文件导出 |
| 梅尔频谱 | 通用宿主 | 上游依赖 Windows NAudio，目前仍属**功能待移植** |

**表中“通用宿主”代表窗口有统一的 Mac 外壳并复用原功能，不代表底层业务与 Mac 外设全部兼容。**

## 本地开发与测试

```bash
cd /Users/yanjinlong/Code/newlife/XCoder
dotnet build XCoderMac/XCoderMac.csproj -c Debug
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build

# 自动化窗口检查（不连接外部业务服务）
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-all
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-host-render
```

本机检查：`--smoke-all` 26/26 通过；`--smoke-host-render` 18/18 通过，错误 0。测试仅覆盖窗口初始化与宿主显示，不覆盖所有真实业务操作。

## 打包和安装

```bash
cd /Users/yanjinlong/Code/newlife/XCoder
./XCoderMac/package-macos.sh
open XCoderMac/dist/XCoderMac.app
open XCoderMac/dist
```

产物：
- `XCoderMac/dist/XCoderMac.app` — Apple Silicon self-contained Mac 应用。
- `XCoderMac/dist/XCoderMac-preview-arm64.dmg` — DMG 预览安装包。

当前脚本执行 **ad-hoc codesign**，不是 Apple Developer ID 正式签名，也没有 notarization。适合本机试用；若对其他 Mac 分发，还需正式签名、公证与安装验证。实际重新打包不会修改上游源代码。

## 遗留事项（正式生产使用前）

1. NuGet 有高危依赖告警：`SSH.NET 2025.1.0`，`Tmds.DBus.Protocol 0.20.0`。应确定受影响路径，升级安全版本，完成 SSH/设备通信回归。
2. 通过**测试数据库、测试 Redis、测试 MQTT Broker、测试 PLC/串口设备**做端到端功能验证（避免直接对生产设备写入）。
3. IP 变更、梅尔频谱/NAudio、手机备份和可能涉及系统权限的设备操作仍需要专项 macOS 实现。
4. 检查上游原业务函数的跨线程 UI 更新、关闭窗口后的资源释放以及真实数据备份行为。
5. 完成 Apple Developer ID 签名、公证及跨 Mac 安装测试后才能作为正式外部分发版。
