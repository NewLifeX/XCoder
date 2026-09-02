# 码神工具（XCoder WinForm）E2E 问题清单

> 维护说明：本文档记录针对 `XCoder/` 目录 WinForm 版码神工具（`CrazyCoder.exe`）的 E2E 巡检发现的问题。修一个划一个，直到清零。
> 巡检技术栈：xUnit + FlaUI（外部 UI 自动化，真实点击菜单驱动），详见 `Tests/CrazyCoder.E2E/README.md`。

## 基线巡检结果（2026-09-02 首轮）

- 自动发现工具菜单 **27 个**，全部真实点击打开成功（MDI 子窗体正常加载）
- 打开后内容异常 **2 个**（详见问题清单）
- 另发现窗体标题冲突 **2 对**

### 运行方式

```powershell
# 1. 先构建主项目（产出 Bin\CrazyCoder\CrazyCoder.exe）
dotnet build XCoder\CrazyCoder.csproj

# 2. 运行全菜单冒烟巡检（真实点击全部 27 个工具，输出报告+截图）
cd Tests\CrazyCoder.E2E
dotnet test --filter FullyQualifiedName~SmokeTourTests
# 报告：bin\Debug\net10.0-windows\out\巡检报告.md（含截图）
```

> 注意：E2E 会真实弹出/操作码神工具窗口；运行前请勿手动占用该程序。启动时会自动将 `Bin\CrazyCoder\Config\XCoder.config` 的语音提示关闭、清空上次工具、更新检查置当天，保证可复现。

---

## 问题清单

### P2-01 网络调试工具 / 地图接口 日志区打印插件扫描异常（噪音）

- **位置**：`XCoder/XNet/FrmMain.cs:576`（`typeof(NetServer).GetAllSubclasses()`）、`XCoder/Yun/FrmMap.cs:46`（`typeof(IMap).GetAllSubclasses()`）
- **现象**：两个工具打开后，接收/日志区打印一段红色异常堆栈：
  `ReflectionTypeLoadException: Could not load type 'SqlGuidCaster' from assembly 'System.Data.SqlClient, Version=4.6.1.3...' ... at NewLife.Reflection.AssemblyX.FindPlugins`
- **根因分析**：依赖链中某程序集（疑似 XCode 的 netstandard2.0 资产）引入了 `System.Data.SqlClient 4.6.1.3`（netstandard 版，强命名）。该程序集内部类型 `SqlGuidCaster` 的 `[StructLayout]` 混排对象字段，在 .NET 8 运行时枚举类型（`Assembly.GetTypes()`）时抛 `TypeLoadException`。`NewLife.Core` 的 `AssemblyX.FindPlugins` 捕获后写入 XTrace 日志（被工具接收区显示），扫描本身继续、工具功能不受影响（插件列表仍成功返回）。
- **严重度**：低（仅日志噪音，功能正常），但用户看到异常堆栈容易误判为故障。
- **修复方向**：
  1. 首选：NewLife.Core `AssemblyX` 扫描跳过/降级日志该类型加载失败（跨仓库，需发版）
  2. 或：清理依赖链中 `System.Data.SqlClient 4.6.1.3`（如 XCode 引用升级到 `Microsoft.Data.SqlClient`）
  3. 或在 XCoder 侧对这两处扫描前先移除/替换出错程序集（不优雅，备选）
- **状态**：⏳ 待修复

### P3-01 对称加密 与 加密解密 窗口标题重名「加密解密」

- **位置**：`XCoder/Tools/FrmSymmetric.cs`（菜单「对称加密」）与 `XCoder/Tools/FrmSecurity.cs`（菜单「加密解密」）
- **现象**：两个不同工具窗体的 `Text` 都是「加密解密」，MDI 中无法区分、自动打开「上次工具」时也会开错
- **建议**：`FrmSymmetric` 标题改为「对称加密」或「对称加密工具」
- **状态**：⏳ 待修复

### P3-02 IP设置 与 网络调试工具 窗口标题重名「网络调试」

- **位置**：`XCoder/XNet/FrmIp.cs`（菜单「IP设置」）与 `XCoder/XNet/FrmMain.cs`（菜单「网络调试工具」）
- **现象**：两窗体 `Text` 都是「网络调试」
- **建议**：`FrmIp` 标题改为「IP设置」
- **状态**：⏳ 待修复

---

## 观察记录（正常行为，不修）

| 项 | 说明 |
|----|------|
| 帮助/关于 | 打开系统浏览器跳转官网（菜单已从巡检排除） |
| 帮助/检查更新 | 弹出「没有可用更新」对话框（正常，巡检自动关闭） |
| 窗口菜单（层叠/平铺/排列） | 布局命令非工具（已排除） |
| 依赖网络/硬件的工具（Modbus/串口/MQTT/SSH/Redis/地图等） | 冒烟仅验证窗体可加载，未做真实连接 |

## 待办

- [ ] 对纯本地工具补充功能级断言：MD5/加密解密/对称加密/正则表达式/坐标转换/图标水印等
- [ ] 修复后回归：重跑冒烟巡检直至 0 警告、0 异常
