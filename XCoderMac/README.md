# Newlife.XCoder — XCoder macOS 应用

> 官方源码：<https://github.com/NewLifeX/XCoder>
> 工程目录：`/Users/yanjinlong/Code/newlife/XCoder/XCoderMac`
> 项目约束：保留 `master`，不修改上游 `XCoderAv`；仅新增/维护本目录代码，根目录 `码神工具.slnx` 已加入项目。


- **主界面自适应（2026-10-18）**：左侧导航栏缩为 216px，整行选中高亮；工具卡片根据宽度自动排布 4/3/2/1 列，解决右下方大块留白。具体见 `Docs/主界面布局优化说明.md`，可运行 `--smoke-home-layout` 检验与更新首页截图。

## 最新上游兼容与单窗口多标签（2026-10-18）

- 已将项目添加到新版根目录 `码神工具.slnx`；`XCoderMac.csproj` 的 Avalonia.Desktop、Avalonia.Themes.Fluent、Avalonia.Fonts.Inter 已与上游 `XCoderAv` 对齐为 **12.1.1**。
- **恢复单窗口多标签**：左侧栏与原主页保留，工具在主窗口右侧以紧凑的 174px 标签显示；同一工具不重复开窗，顶部左右箭头浏览，标签右侧 × / `⌘W` 关闭，首页与工作区可自由返回。
- 关闭工具时触发 Mac-only 清理，涉及确认操作的弹窗以主窗口作为父窗口。
- Avalonia 12 使用了 `x:CompileBindings="False"` 保留现有反射绑定写法，后续可分阶段迁移为强类型编译绑定。
- 调试验证：`dotnet run --project XCoderMac/XCoderMac.csproj -c Debug -- --smoke-tabs`，逐个测试 26 个工具的标签打开/切换/关闭及多标签导航（不执行业务写入）。
- **Git 已恢复 master**。已结束 rebase；本轮 XCoder 新功能需完成测试与确认后再单独提交，不主动推送。

## 技术设计

- **.NET 10 + Avalonia 12.1.1**，Apple Silicon ARM64；通过 `ProjectReference` 引用原始 `../XCoderAv/XCoderAv.csproj`（上游 .NET 8）。
- **主界面**：`MacShellWindow` 提供分类、搜索、快捷入口、26 个工具卡片以及 `⌘F` 工具搜索。
- **独立 Mac 界面（8 项）**：正则表达式、数据建模、RPC API 调试、网络工具、Redis 管理、语音助手、USB 设备、声卡信息。
- **通用 Mac 宿主（18 项）**：`MacLegacyToolWindow` 把上游已实现的控件、ViewModel、事件绑定重用在统一的 Mac 外观容器中，**不复制整套上游 XAML**。
- 所有工具从 `MacToolCatalog` 统一启动。对同一个工具再次点击只会激活已打开的主窗体标签，不会新开窗口。
- 语音助手使用 macOS 系统命令 `say`，支持朗读、停止、AIFF 导出。
- USB、声卡使用 `system_profiler` 读取原生设备记录（**只读**；不控制硬件）。
- Redis 节点信息保存到 `~/Library/Application Support/Newlife.XCoder/Config/redis-nodes.json`；不保存 Redis 密码。旧版数据首次运行按需迁移。
- 数据库连接信息统一保存到 `~/Library/Application Support/Newlife.XCoder/Config/connections.json`，支持界面管理与手动编辑并重新加载；密码默认保存到 macOS 钥匙串。参见 `Docs/Newlife.XCoder用户配置与权限规范.md`。

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
open XCoderMac/dist/XCoder.app
open XCoderMac/dist
```

产物：
- `XCoderMac/dist/XCoder.app` — Apple Silicon self-contained Mac 应用。
- `XCoderMac/dist/XCoder-arm64.dmg` — DMG 预览安装包。

当前脚本执行 **ad-hoc codesign**，不是 Apple Developer ID 正式签名，也没有 notarization。适合本机试用；若对其他 Mac 分发，还需正式签名、公证与安装验证。实际重新打包不会修改上游源代码。

## 遗留事项（正式生产使用前）

1. NuGet 有高危依赖告警：`SSH.NET 2025.1.0`，`Tmds.DBus.Protocol 0.20.0`。应确定受影响路径，升级安全版本，完成 SSH/设备通信回归。
2. 通过**测试数据库、测试 Redis、测试 MQTT Broker、测试 PLC/串口设备**做端到端功能验证（避免直接对生产设备写入）。
3. IP 变更、梅尔频谱/NAudio、手机备份和可能涉及系统权限的设备操作仍需要专项 macOS 实现。
4. 检查上游原业务函数的跨线程 UI 更新、关闭窗口后的资源释放以及真实数据备份行为。
5. 完成 Apple Developer ID 签名、公证及跨 Mac 安装测试后才能作为正式外部分发版。

- **数据建模页面自适应（2026-10-18）**：上方为“数据库连接 + 代码生成设置”并排，下方为“数据表 + 字段结构”并排；小窗口两排可分别改为上下布局，整个页面允许纵向滚动。附实机截图与测试说明见 `Docs/数据建模页面调整说明.md`，可用 `--smoke-modeling-layout` 回归验证。

## 正式产品名称与用户数据目录

对外产品名称 Newlife.XCoder，APP 名称 XCoder。所有配置存储在 ~/Library/Application Support/Newlife.XCoder/，日志存储在 ~/Library/Logs/Newlife.XCoder/；密码默认使用 macOS 钥匙串。详见 Docs/Newlife.XCoder用户配置与权限规范.md。

## 0.2.2 全局亮色主题与控件状态

解决 Mac 系统深色模式下浅色窗口与默认暗色控件混用的问题，统一下拉框、列表、输入框、按钮与标签页的常态/悬停/选中配色。三个按钮类别对应主操作、次操作、危险操作；预览仍始终使用浅色主题。样式位置：`Styles/LightControls.axaml`，说明见 `Docs/全局浅色主题与控件交互规范.md`。

## 0.2.1 连接配置向导

连接管理支持常见数据库分项输入（服务器 IP/主机名、端口、数据库名称、用户名），高级连接字符串与这些输入框**双向同步**。SQLite/DuckDB 使用文件路径输入框。输入的自定义 SSL、超时等参数会保留。测试和截图见 `Docs/软件界面/01-数据库连接配置-连接串双向同步.png`。

## 0.2.5 数据同步界面

数据同步从旧版同一行混排改为“源数据库”和“目标数据库”两个独立分组，下拉框等宽且可自适应，窄窗口自动上下排列。修复错误显示布尔 `True` 的按钮，改成明确的连接/断开操作。复用上游 `DataSyncViewModel` 而不修改源库、目标库或同步业务。详见 `Docs/数据同步界面调整说明.md`。

## 0.2.6 数据同步连接按钮状态

数据同步中源库/目标库分别根据对应的连接状态切换“连接”与“断开”按钮：成功连接后禁止重复连接，断开后恢复连接；目标库还要求源库先成功连接。无需修改上游 ViewModel，相关回归测试纳入 `--smoke-sync-layout`。

## 0.2.7 数据同步连接首项默认选择

数据同步窗口打开时，源数据库列表如有连接，默认显示第一项。成功连接源库后，目标数据库列表生成完毕也默认选第一项；有有效手动选择时不会覆盖，列表为空时自动清空失效选择。该行为只针对 Mac 数据同步页面，不修改上游 ViewModel；对应测试包含在 `--smoke-sync-layout` 中。
