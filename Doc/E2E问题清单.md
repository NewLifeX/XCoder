# 码神工具（XCoder WinForm）E2E 问题清单

> 维护说明：本文档记录针对 `XCoder/` 目录 WinForm 版码神工具（`CrazyCoder.exe`）的 E2E 巡检发现的问题。修一个划一个，直到清零。
> 巡检技术栈：xUnit + FlaUI（外部 UI 自动化，真实点击菜单驱动），详见 `Tests/CrazyCoder.E2E/README.md`。

## 基线巡检结果（2026-09-02 首轮）

- 自动发现工具菜单 **27 个**，全部真实点击打开成功（MDI 子窗体正常加载）
- 打开后内容异常：**P2-01**（SqlGuidCaster 插件扫描噪音）—— **已修复并回归验证** ✅
- 窗体标题冲突 **2 对**：P3-01 / P3-02 —— **已修复并回归验证** ✅

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
- **根因分析**：`GetAllSubclasses` → `DefaultReflect.GetAllSubclasses` 遍历**所有已加载程序集**并对每个调用 `AssemblyX.FindPlugins`；后者 `GetTypes()` 遇 `ReflectionTypeLoadException` 时**无条件** `XTrace.WriteException`。依赖链中某程序集（疑似 XCode netstandard 资产）引入了 `System.Data.SqlClient 4.6.1.3`，其内部类型 `SqlGuidCaster` 的 `[StructLayout]` 混排对象字段在 .NET 8 下 `GetTypes()` 必抛 `TypeLoadException` → 日志噪音。扫描本身继续、工具功能不受影响（插件列表仍成功返回）。
- **严重度**：低（仅日志噪音，功能正常），但用户看到异常堆栈容易误判为故障。
- **修复方案（已实施，XCoder 侧自愈）**：新增 `XCoder/ReflectHelper.cs` 静默扫描（容忍 `ReflectionTypeLoadException`、取 `ex.Types` 继续、不打日志），替换 3 处 `GetAllSubclasses`：`FrmMDI`（IXForm 菜单扫描）、`XNet/FrmMain.cs`（NetServer）、`Yun/FrmMap.cs`（IMap）。结果集与原来完全一致。
- **长期根治（可选，未做）**：NewLife.Core `AssemblyX.FindPlugins` 对类型加载失败降级日志；或清理 `System.Data.SqlClient 4.6.1.3` 依赖链
- **状态**：✅ 已修复（2026-09-02）。E2E 回归：27 工具全部通过、0 内容异常警告

### P3-01 对称加密 与 加密解密 窗口标题重名「加密解密」

- **位置**：`XCoder/Tools/FrmSymmetric.cs`（菜单「对称加密」）与 `XCoder/Tools/FrmSecurity.cs`（菜单「加密解密」）
- **现象**：两个不同工具窗体的 `Text` 都是「加密解密」，MDI 中无法区分、自动打开「上次工具」时也会开错
- **建议**：`FrmSymmetric` 标题改为「对称加密」或「对称加密工具」
- **状态**：✅ 已修复（2026-09-02）。`FrmSymmetric.Designer.cs` 的 `Text` 改为「对称加密」；E2E 回归确认子窗体标题为「对称加密」

### P3-02 IP设置 与 网络调试工具 窗口标题重名「网络调试」

- **位置**：`XCoder/XNet/FrmIp.designer.cs`（菜单「IP设置」）与 `XCoder/XNet/FrmMain.cs`（菜单「网络调试工具」）
- **现象**：两窗体 `Text` 都是「网络调试」
- **建议**：`FrmIp` 标题改为「IP设置」
- **状态**：✅ 已修复（2026-09-02）。`FrmIp.designer.cs` 的 `Text` 改为「IP设置」；E2E 回归确认子窗体标题为「IP设置」

---

## 观察记录（正常行为，不修）

| 项 | 说明 |
|----|------|
| 帮助/关于 | 打开系统浏览器跳转官网（菜单已从巡检排除） |
| 帮助/检查更新 | 弹出「没有可用更新」对话框（正常，巡检自动关闭） |
| 窗口菜单（层叠/平铺/排列） | 布局命令非工具（已排除） |
| 依赖网络/硬件的工具（Modbus/串口/MQTT/SSH/Redis/地图等） | 冒烟仅验证窗体可加载，未做真实连接 |

## 待办

- [x] 纯本地工具功能断言（2026-09-02，`LocalToolTests` 7 项全部通过）：MD5_32 / MD5_16 / Base64 编解码往返 / AES 对称加解密往返 / 正则匹配 / 正则替换 / GPS HEX 坐标转浮点
- [x] 冒烟巡检 0 警告 0 异常回归（2026-09-02）
- [ ] 可选长期项：NewLife.Core 插件扫描日志降噪 / 清理 System.Data.SqlClient 依赖链（已由 XCoder 侧静默扫描自愈，暂无需处理）
