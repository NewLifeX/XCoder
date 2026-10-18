# Newlife.XCoder 用户配置与 macOS 权限规范

版本：0.2.0；2026-10-18

## 对外命名

正式产品名称：**Newlife.XCoder**。macOS APP 及 Finder 名称：**XCoder**。
分发产物：XCoder.app、XCoder-arm64.dmg；Bundle ID：com.newlifex.xcoder。
内部工程及 .NET 程序集继续使用 XCoderMac，不改变现有 .slnx 项目引用。

## 永久配置与日志

路径：~/Library/Application Support/Newlife.XCoder/

    Config/
        connections.json      用户数据库连接，可直接编辑
        XCode.config          NewLife.XCode 自身运行参数
        Core.config           NewLife.Core 自身运行参数
        redis-nodes.json      Redis 节点设置（不保存密码）
    Backups/                  配置备份
    Data/                     组件运行数据

日志路径：~/Library/Logs/Newlife.XCoder/

上述目录位于 APP 包外，重装或升级 XCoder.app 不会覆盖。首次启动只创建不存在的文件和目录，不覆盖已有配置。
开发者可通过 Finder 快捷键 Command+Shift+G 输入配置目录路径。
文件权限 600，用户专属目录权限 700。

## connections.json 示例

    {
      "version": 1,
      "connections": [
        {
          "name": "LocalSqlite",
          "provider": "SQLite",
          "connectionString": "Data Source=sample.db",
          "passwordMode": "none"
        },
        {
          "name": "ExampleMySql",
          "provider": "MySql",
          "connectionString": "Server=127.0.0.1;Database=Demo;User Id=tester",
          "passwordMode": "keychain"
        }
      ]
    }

连接名称在整个列表中唯一。密码模式包括 keychain（推荐）、none 和 plaintext。
默认不允许在 connectionString 内嵌入 Password/Pwd/Pass。钥匙串服务标识：Newlife.XCoder.Database。
plaintext 模式允许独立的 password 字段，但密码会进入配置及备份文件，请仅在理解风险的情况下选择。

## 配置管理

在「数据建模 → 数据库连接 → 连接配置」可新增、编辑、删除、测试和保存连接；
常用 MySQL/SQL Server/PostgreSQL 默认直接填写服务器 IP/主机名、端口、数据库名称、用户名；
SQLite/DuckDB 输入本地文件路径。高级连接字符串按需展开。
在高级区域输入连接字符串后，以上文本框会自动解析、实时回填；反过来修改文本框也会自动更新连接字符串。
附加的 SSL、超时等扩展参数会保留。连接字符串临时语法错误时不清空文本框，待修复后重新解析；
对于未提供结构化向导的数据库类型，仍可使用完整连接字符串。
也可以点击「在 Finder 中打开配置文件」后手动编辑 JSON，再点击「重新加载文件」。
两种方式使用同一个文件，注册到 XCode.DAL.ConnStrs，数据建模与数据同步共用连接名称。
编辑操作使用原子替换及写入前备份；无效的 JSON 不会破坏内存中已加载的配置。
请在修改连接时先断开相关工具里的现有数据库会话，重新加载配置后再连接。测试连接只打开与关闭连接，不执行 SQL。

## APP 与安全边界

正式用户不应修改 XCoder.app/Contents/MacOS 里的文件。打包会清理内部 Config/Log/Data，
启动时 NewLife.PathHelper 基础目录被重定向至用户支持目录，日志目录单独设置为 ~/Library/Logs/Newlife.XCoder/。
外部配置和日志不应影响已签名 APP 的代码签名。

权限按需申请：首次启动和访问自身 Application Support/Logs 无需申请完全磁盘访问；
用户选择桌面、文稿、下载等受保护文件时应使用系统文件选择器，按 macOS 提示授权；
麦克风、摄像头等功能在使用时单独授权；对网络、USB、串口或系统配置的访问依具体能力与系统政策处理，
不得首次运行时请求所有权限。macOS 钥匙串可能因首次访问或签名变更提示用户确认。

## 验证边界

已进行：JSON 保存、手动重载、权限、格式保护、UI 初始化、主界面 26 个工具标签、
用户目录重定向、APP 实际启动及启动后严格签名校验。
未做生产端到端验收：真实数据库实例、钥匙串密码读取/写入的用户授权行为、
正式 Developer ID 签名和 notarization、沙盒/企业权限策略。
