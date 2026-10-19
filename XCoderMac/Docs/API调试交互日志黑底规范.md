# API 调试：交互日志固定黑底规范

日期：2026-10-19。XCoder macOS 版本 0.2.11。

## 需求

“API 调试”右侧的“交互日志”是控制台区域，必须一直使用深黑色背景、浅色可读文字；不能因鼠标悬停、点击或获得焦点而变成亮色背景。请求参数等普通编辑框仍应保持统一亮色主题。

## 实现

- 修改 `Views/MacApiDebugWindow.axaml`，交互日志使用独立的 `TextBox.rpc-console` 类，**不再与 `TextBox.editor` 共用样式**。
- 背景固定 `#0A0F16`，前景色 `#EDF5FF`，边框 `#26394D`；`pointerover`、`focus`、`focus-within` 也显式固定同一背景和文字颜色。
- 覆盖 Fluent 模板的 `Border#PART_BorderElement` 背景，避免模板状态将日志区重新填充为白色。
- 原日志 `ReceiveLog` 仍以 `Mode=OneWay` 绑定，保持只读、换行和字体设置；未更改任何 NewLife.Remoting 网络逻辑。

## 自动测试

```bash
dotnet run --project XCoderMac/XCoderMac.csproj -c Debug --no-build -- --smoke-api-console
```

测试在主界面工具标签中打开 RPC API 调试，确认：

1. 正常、鼠标悬停、焦点、内部焦点状态均为黑底浅字，且模板内层背景不变。
2. 地址和参数输入框继续使用亮色样式；日志保持只读。
3. `ReceiveLog` 的 ViewModel 数据正常进入日志窗口。
4. 生成真实界面截图 `Docs/软件界面/03-API 调试.png` 和带演示日志的 `03-API调试-黑色交互日志.png`。

测试只注入虚构日志字符串，不建立网络连接、不监听端口。

## 顶部重复状态标签移除（0.2.12）

API 调试页顶部右侧原“打开”只是 `ConnectButtonText` 的重复展示，不能点击且容易被误认为独立操作。本次删除顶部重复标签，只保留“连接设置”中的真实“打开/关闭”按钮；当前状态仍由工作台底部的统一状态栏显示。测试 `--smoke-api-console` 确认重复标签已消失、真实操作按钮仍存在。

## RPC 请求表单紧凑布局（0.2.13）

- “发送 RPC 请求”卡片把 **API Action 下拉框、手动 Action 输入框、发送请求按钮** 改为同一行，仍绑定原 `ApiActions`/`SelectedAction`/`SendCommand`。
- 请求参数 `SendText` 多行输入框置于下方，独占卡片剩余空间，至少有 **145px** 实际编辑高度，并启用垂直滚动条，避免长参数在此前旧布局中被挤成一条。
- `--smoke-api-console` 新增真实窗口布局、三个控件列与高度、发送命令、输入绑定、参数高度验证；继续测试深色日志框和模板悬停/聚焦状态。
- 截图覆盖 `Docs/软件界面/03-API 调试.png` 和 `03-API调试-黑色交互日志.png`。
