# Windows「数据工具」能力在 XCoder Mac 中的对应实现

> 当前状态：2026-10-19 / XCoder 0.3.3。**本页已清除 0.3.0–0.3.1 被撤销的独立导航方案。** 各工具的当前完成程度以 [项目现状与任务总清单](项目现状与任务总清单.md) 为准。

## 现行入口（无额外侧栏）

Windows 版在菜单「数据工具」中聚合功能；Mac 版仍使用原来的四个工具分类：

- 「开发与数据 → 数据建模」：在数据库连接和代码生成设置**下方**、表与字段结构**上方**，找到「模型与数据库工具」。
- 「开发与数据 → 数据同步」：仍在原数据同步页面。
- 「设备与运维 → Redis 管理」：仍在原 Redis 页面。

**不要再添加**「工作台 → 数据工具」侧栏按钮、首页「进入数据工具」按钮或第五个工具分类；用户已要求撤销该方案。

## 模型操作实现

| 功能 | Mac 文件/方法 | 已做的验证 | 未完成的内容 |
|---|---|---|---|
| XML 导入 | `MacDataModelingWindow` → `DAL.Import` | 9 表 XML 离线导入 | 不同版本/复杂 XML 兼容测试 |
| XML 导出 | `MacDataToolsService` → `DAL.Export` | 9 表往返，表/列/索引数量保持 | 多数据库模型及编码测试 |
| 模型管理 | `MacModelManagerWindow` | 新增表、字段、索引及 UI 显示 | 全属性/索引组合与复杂编辑测试 |
| 离线 C# 生成 | 原 `EntityBuilder.BuildTables` | XML → 临时目录 C# 文件 | 多模板、文件覆盖和所有数据库差异 |
| 建表 SQL | `MacSqlPreviewWindow` / `GetSchemaSQL` | 隔离临时 SQLite 生成单/多表 SQL，保存窗口 UI | MySQL/SQL Server/PostgreSQL 方言及真实执行 |
| 架构浏览 | `MacSqlWorkbenchWindow` | 架构/SQL 窗口展示 | 真实数据库元数据读取 |
| SQL 查询/执行 | `MacSqlWorkbenchWindow` | UI、TAB 14px、查询结果固定深色 | 真实 SQL、权限/超时、数据量和安全验证 |
| 应用模型到库 | `SetTables(Migration.Full,...)` | 二次确认 UI 已实现 | **必须测试库演练、恢复/回滚和风控，不是生产完成状态** |

Windows 对照实现：`XCoder/FrmMDI.cs`、`XCoder/Windows/FrmMain.cs`、`FrmModel.cs`、`FrmSchema.cs`、`FrmQuery.cs`。Mac 使用 XCode 的公共引擎，WinForms 窗口不直接复用。

## 安全边界与交互规范

- 模型管理**只编辑内存副本**；点击「应用模型」才写回当前数据建模页面，不自动写数据库。
- SQL 预览是只读窗口，不执行语句；建表 SQL 方言需要有效连接类型。
- SQL 查询器分只读入口与二次确认的写入入口，但**依靠正则检查 SQL 文字不能构成真正的只读安全沙箱**，真实环境必须用数据库只读账号/权限隔离。
- 数据库架构变更必须先备份，并在独立测试库实测；当前没有生产库 DDL 验证结果。
- SQL 预览、SQL 查询结果使用 `Styles/LightControls.axaml` 中的 `TextBox.dark-console`，确保普通/悬停/焦点/内部焦点都为黑底浅字；网络通信日志同用此样式，RPC 日志有已验证的独立样式。
- 工具栏按钮不限制固定 36px 行高；最小高度 44px、文字完整，窄窗口可换行。

## 对应源码和专项测试

- `Services/MacDataToolsService.cs`：模型 XML、SQL 生成、校验。
- `Views/MacDataModelingWindow.axaml(.cs)`：当前六个数据模型工具操作。
- `Views/MacModelManagerWindow.cs`：表/字段/索引草稿编辑。
- `Views/MacSqlPreviewWindow.cs`：只读 SQL 预览/保存。
- `Views/MacSqlWorkbenchWindow.cs`：架构/查询器。
- `MacToolCatalog.cs`：原四分类、26 个工具入口。

```bash
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-data-tools
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-modeling-layout
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-sql-ui
```

以上测试为隔离文件/临时 SQLite 的 **DDL 生成**及界面校验；**不会执行真实库写入，也不能证明真实迁移已通过**。截图可在 [截图目录](软件界面/截图目录.md) 查看。

下一步实际任务、各工具分级和优先顺序请统一更新 [项目现状与任务总清单](项目现状与任务总清单.md)，不在此重复记录不再使用的旧导航方案。
