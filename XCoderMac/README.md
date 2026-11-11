# XCoder for macOS（Newlife.XCoder）

当前整理版本：**0.3.3 · 2026-10-19**。官方上游：[NewLifeX/XCoder](https://github.com/NewLifeX/XCoder)。本工程仅维护 `XCoderMac/`，通过 `../XCoderAv/XCoderAv.csproj` 复用原项目，不修改 Windows / XCoderAv 上游源码。

**先读这里：** [项目现状、已完成和未完成 26 项清单](Docs/项目现状与任务总清单.md) · [项目文档导航](Docs/文档索引.md) · [界面截图](Docs/软件界面/截图目录.md)。

## 当前可以使用什么

- **26 个工具入口、4 个分类**，主窗口多标签、左侧「已打开工具」、快速搜索与可管理的快捷入口。
- 统一亮色主题和日志/SQL 结果专用黑底；针对悬停/焦点有 UI 检查。
- 数据建模：数据库连接配置、表/字段展示、生成实体；XML 模型离线导入导出、表字段索引编辑、建表 SQL 预览/保存、架构管理和 SQL 查询器入口。
- 数据同步：源/目标库配置、连接状态、表选择 UI；尚未进行真实跨库数据一致性验证。
- RPC API 调试、加密解密异常保护、正则表达式、网络工具、Redis、USB 信息、声卡信息、语音助手等 Mac 适配界面/局部实现。

**重要：** 目前 **26/26 有 A 级界面入口，只有部分 B 级局部功能已验证**。SQL/DDL 真正执行、数据库同步、硬件写入、文件覆盖等没有完整生产验收。详见[进度分级和未完成项](Docs/项目现状与任务总清单.md)，不要直接把高风险操作用于生产库或生产设备。

### Windows “数据工具”的 Mac 入口

无需额外的「数据工具」侧栏按钮。现在：
- 「**开发与数据 → 数据建模**」：在连接设置和代码生成设置下方，找到「模型与数据库工具」的 XML/模型管理/SQL/架构操作。
- 「**开发与数据 → 数据同步**」：使用既有同步页面。
- 「**设备与运维 → Redis 管理**」。

详见 [Windows 数据工具迁移说明](Docs/Windows数据工具Mac版功能迁移与验收.md)。

## 工程结构

```text
XCoderMac/
├── Views/                 # Mac 原生窗口和适配视图
├── Services/              # 用户配置、Keychain、模型工具、快捷入口等
├── Styles/                # 统一颜色和深色控制台样式
├── Dialogs/               # 统一操作确认
├── Docs/                  # 当前进度、专题说明及实际界面截图
├── package-macos.sh       # Mac 自包含 App / DMG 制作
├── XCoderMac.csproj       # .NET 10 / Avalonia 12.1.1
└── bin/ obj/ dist/        # 本地生成产物（git 忽略）
```

## 编译、测试与发布

在仓库根目录执行：

```bash
dotnet build XCoderMac/XCoderMac.csproj -c Debug --no-restore
# 2026-10-20 上游 XCoderAv 暂有 CS0103（L 未定义）时，临时复用已有上游编译产物：
dotnet build XCoderMac/XCoderMac.csproj -c Debug --no-restore -p:BuildProjectReferences=false
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build
# 只测刚修改的模块，例如：
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-sql-ui
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-modeling-layout
# 文件编码窗口专项布局测试（不会进行扫描或文件转换）：
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-file-encoding-layout
# RPC Action 空状态 / 网络默认绑定地址 / 消息调试长地址与分组/控件间距（1050/900）：
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-debug-input-layout
# MQTT 顶部连接/发布/订阅三列、底部全宽加高日志、右下清空按钮、hover 黑底（无网络连接）：
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-mqtt-ui
# API 发现按钮不重叠、hover 变色、服务列表/日志圆角（无 UDP 广播）：
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-api-discover-ui
# 全项目分组留白及加密解密 WrapPanel 两列/按钮间距：
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-spacing-ui
# 全 26 工具初始化（仅窗口，不执行协议或文件写入）：
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-all
# 需要生成安装包时：
./XCoderMac/package-macos.sh
# 上游 XCoderAv 无法重新编译的临时打包方式（需已有上游编译产物）：
BuildProjectReferences=false ./XCoderMac/package-macos.sh
```

- Mac 安装包：`XCoderMac/dist/XCoder.app`、`XCoderMac/dist/XCoder-arm64.dmg`，仅 ARM64，自签未公证。
- 用户配置：`~/Library/Application Support/Newlife.XCoder/Config/`；日志：`~/Library/Logs/Newlife.XCoder/`。
- Git：在 `master` 上工作；**未获明确要求不提交、不推送**。每次小界面改动只跑相关模块测试，不必重复全量回归。
- **开发交接硬性规则：每次完成代码或样式修改，必须在同一次任务中更新对应专题文档和 `Docs/项目现状与任务总清单.md`，说明改动位置、原因、验证范围及未解决问题；后续开发先读文档。**
- 历史评估保留作参考，不作为现行完成状态；本 README 不再滚动堆叠互相矛盾的逐版本说明。
