# CrazyCoder.E2E — 码神工具（WinForm）菜单 E2E 测试

针对 `XCoder/` 目录 WinForm 版码神工具（20 年历史、输出 `Bin\CrazyCoder\CrazyCoder.exe`）的**外部 UI 自动化** E2E 测试。

## 技术方案

- **驱动**：xUnit + FlaUI（UIA3），启动真实 `CrazyCoder.exe`，用真实鼠标点击菜单操作
- **主窗体**：`FrmMDI` 菜单动态生成（反射 `IXForm` 按 `[Category]` 分组），因此测试从**真实 UI 枚举菜单**，不做硬编码清单
- **覆盖**：27 个工具逐个点击打开 → 等待子窗体加载 → 截图 → 扫描内容中的异常堆栈 → 生成巡检报告

## 关键工程点（踩坑记录）

1. **窗口需 Win32 激活**：FlaUI 找到主窗口后必须 `ShowWindow(SW_RESTORE)+SetForegroundWindow`，否则 UIA 子树为空（窗口会被置于 -32000 离屏坐标）
2. **FlaUI 5 API**：查找条件用函数式 `cf => cf.ByControlType(...)`；`As*` 扩展在 `FlaUI.Core.AutomationElements`；读文本用 `Patterns.Value.Pattern.Value` 或 `Patterns.Text.Pattern.DocumentRange.GetText(n)`（RichTextBox 是 Document 需走 TextPattern）
3. **隔离配置**：每次启动前改写 `Bin\CrazyCoder\Config\XCoder.config`（语音关闭、LastTool 清空、更新检查置当天），保证可复现、不弹更新框
4. **共享会话**：`[CollectionDefinition("CrazyCoder")]` + `ICollectionFixture<CrazyCoderSession>`，全测试串行共用一个 app 实例；进程崩溃自动重启
5. **不要给本测试项目加 NewLife.* 依赖**（String.IsNullOrEmpty 等用 BCL 静态方法）

## 文件结构

| 文件 | 说明 |
|------|------|
| `CrazyCoderSession.cs` | app 生命周期 + 菜单发现/点击/子窗体检测/截图/文本扫描 |
| `SmokeTourTests.cs` | 全菜单冒烟巡检（单测试，输出报告与截图） |
| `ProbeTests.cs` | 结构探测（调试用） |
| `Win32.cs` | 窗口激活/关闭/PrintWindow 截图/枚举 |
| `AssemblyInfo.cs` | 关闭 xUnit 并行 |

## 运行

```powershell
# 构建主项目后
cd Tests\CrazyCoder.E2E
dotnet test --filter FullyQualifiedName~SmokeTourTests
```

输出：`bin\Debug\net10.0-windows\out\巡检报告.md` + `out\screenshots\*.png`

巡检结果跟踪：`Doc/E2E问题清单.md`
