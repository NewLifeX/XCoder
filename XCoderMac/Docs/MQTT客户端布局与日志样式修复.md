# XCoder Mac MQTT 客户端布局与日志样式修复（2026-10-20）

## 一、用户反馈

- MQTT 页面的连接、订阅、发布和日志分组之间、组内控件之间间距凌乱，左侧配置密集、右侧没有统一标题/节奏。
- MQTT 日志区初始黑底，但鼠标悬停或点击后会变白。
- 连接/订阅按钮悬停时文字颜色没有按预期变化，且部分按钮文字有裁切。
- 下方多行文本框未统一圆角。

## 二、原因

- 旧版 `XCoderAv/Views/MqttWindow.axaml` 直接使用 `Grid/StackPanel` 的默认间距；部分控件在上游写死 Margin/Height/Width，而 Mac 统一外层卡片还有额外 Padding，造成分组宽度和行距不协调。
- 旧上游日志 `txtLog` 把黑色 Background 作为局部属性固定，却没有覆盖共享 TextBox 的 `:pointerover`、`:focus`、`:focus-within` 样式与内部模板背景；交互时会被浅色主题覆盖。
- 旧 Button.ActionButton 的小高度、上下 Padding 叠加，视觉上压缩按钮文字。

## 三、Mac 端实现

全部仅修改 `XCoderMac/`，不编辑上游 XCoderAv，也不复制业务 ViewModel。

1. 新增 `Services/MacMqttLayout.cs`；`Views/MacLegacyToolWindow.axaml.cs` 仅在打开“MQTT 客户端”时适配原窗口内已有控件，保留上游 Server/ClientId/UserName/Password/Topic/QoS/Retain/Publish/Subscribe/Connect/Log 全部绑定和命令。
2. 主区域网格改为 `310,16,*`，左侧连接/订阅分组 `Spacing=16`，右侧发布/日志 `RowSpacing=16`。卡片统一 Padding=15，不再叠加上游额外组 Margin。
3. 左侧内层标题及表单 `Spacing=12`、行距 9、列距 8；输入框统一 36 高，文本居中；QoS 下拉统一宽 76、高 36。
4. 右侧发布区增加明确的“发布配置”标题，主题/消息/按钮分行，内行距 12；发布消息输入区最小高度 100。
5. 连接/订阅按钮宽度 108、高 40、水平垂直居中。宿主文件 `Views/MacLegacyToolWindow.axaml` 新增 scoped `Button.mqtt-action` 样式：平时浅蓝底深蓝字，悬停蓝底白字（压下继续深蓝底白字），避免影响其他按钮。
6. MQTT 日志框直接添加已有 `dark-console` 类、清除原上游局部 Foreground/Background，沿用全局正常/悬停/聚焦/内部焦点和模板 Border 样式，固定黑底浅色文字。
7. 发布消息与日志两个多行 TextBox 都明确使用 `CornerRadius=10`，并设置裁剪，防止圆角处的内容溢出。

## 四、回归及界限

- 编译：`dotnet build XCoderMac/XCoderMac.csproj -c Debug --no-restore -p:BuildProjectReferences=false`，成功 0 错误。当前上游独立构建仍受既有 XCoderAv 错误阻挡，沿用项目临时 Mac-only 构建规则。
- 新增 `Services/MacMqttUiSmoke.cs` 及 `--smoke-mqtt-ui`：
  - 实际打开宿主 MQTT 窗口，在宽度 1140 和 900 下检查两列/四分组/组间与行间距。
  - 检查连接、订阅按钮至少 100 宽、38 高；模拟 pointerover，实测按钮前景与背景颜色发生变化且悬停文字变白。
  - 检查日志黑底浅字在 normal、pointerover、focus、focus-within 和模板内边框状态持续成立。
  - 检查发布消息与日志两个输入框 CornerRadius 至少为 8，日志 ClipToBounds 为 true。
- **测试全部通过**，见截图 `Docs/软件界面/10-MQTT客户端-布局与深色日志.png`。
- 使用 `BuildProjectReferences=false ./XCoderMac/package-macos.sh` 重新打包 `XCoderMac/dist/XCoder.app` 与 `XCoderMac/dist/XCoder-arm64.dmg` 成功；保留 0.3.3 版本与临时 ad-hoc 签名，尚未 Apple 正式公证。
- 仅覆盖 UI、样式及数据绑定保留，不连接真实 MQTT Broker、不发布/订阅、不检查鉴权、QoS 与断线重连。MQTT 正式联调仍列在 F10 的未完成任务中。
- 完成本次改动同步更新专题文档、项目总清单、文档索引、截图目录及 README。未经用户要求不提交、不推送 Git。

## 五、2026-10-20 第二轮：三列上方布局、加高日志、清空按钮移位

### 用户反馈与设计结果

用户希望：
1. 发布配置缩小，把订阅配置放在发布配置右边。
2. 日志中的“清空”按钮不要放在原标题栏位置占空间。
3. 把日志高度调高。

旧版 Mac 布局是“左连接+订阅、右发布+日志”，订阅区挤在左下角，右侧日志被发布卡片挤占高度。

本轮只更改 `XCoderMac/Services/MacMqttLayout.cs`，由 `MacLegacyToolWindow` 在 MQTT 宿主装载时适配，保留原始 XCoderAv MQTT 四张卡片、所有属性绑定和 Connect/Subscribe/Publish/ClearLog Command，没有重复新建 VM 或修改上游。

- 根 Grid 改为 **上方：连接配置｜发布配置｜订阅配置；下方：横跨全宽的日志**。布局按五列 `295,14,*,14,232`、三行 `Auto,14,*`。日志纵向占用上部卡片以外的所有可用高度。
- 宽度不足约 960 逻辑点时，自适应紧凑列宽 `258,12,*,12,204`；避免窄窗发布主题框过度挤压；仍保留顶部三列。
- 发布消息输入区缩减为 82 高（内容可滚动/编辑，输入仍支持多行），发布按钮/主题/QoS/保留字段仍在原卡内；发布区域与连接、订阅并排。
- 订阅卡片改三行：主题｜QoS｜订阅按钮，在右侧 232 / 204 宽卡片内避免把主题输入框压窄。保留原订阅按钮命令及 QoS/主题绑定。
- 日志原 `ClearLogCommand` 按钮从标题栏移到**日志黑底内容框右下角悬浮**，约 64×28，占用日志内容右下边角；日志输入框增加底部 Padding 以避免遮挡文字。标题栏只保留小标题。
- 日志 TextBox 继续复用全局 `dark-console`；鼠标 hover/focus/focus-within、模板内层边框全部保持黑底浅字、圆角。
- 无 MQTT Broker 连接、没有发布/订阅/清空实际消息、没有数据外发；只修改 UI 逻辑父子关系、位置及控件尺寸。

### 验证证据

- Mac-only Debug 构建成功：0 error（已有 49 warnings 未在本轮处理）。
- `--smoke-mqtt-ui`：真实显示 1140/900 两种窗口宽度，分别检查连接/发布/订阅左中右顺序、四卡和日志全宽布局、发布输入缩减、下方日志高度超过顶部发布卡片、悬浮清空在日志右下且命令存在、连接/订阅/发布命令仍绑定；按钮 Hover 及日志 normal/hover/focus/focus-within 颜色专项均通过。
- `--smoke-host-render`：17/17 旧工具宿主真实渲染回归通过。
- 截图：`Docs/软件界面/10-MQTT客户端-布局与深色日志.png`（1140 布局），`Docs/软件界面/10-MQTT客户端-窄窗三列布局.png`（900 布局）。
- 真实 MQTT 连接、身份认证、QoS、收发和掉线重连，继续列为待验收；该专项 UI 测试不能代替业务验收。

发布结果：`BuildProjectReferences=false ./XCoderMac/package-macos.sh` 重新构建并生成 `XCoderMac/dist/XCoder.app`、`XCoderMac/dist/XCoder-arm64.dmg` 成功，版本仍为 0.3.3，使用本机 ad-hoc 签名（未经 Apple Developer ID 公证）。

再次强调：所有代码修改必须同轮更新项目总清单与专题文档，不擅自提交、推送 Git。
