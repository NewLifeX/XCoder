# XCoder Mac API 发现页面按钮与大框圆角修复（2026-10-20）

## 问题（用户截图）

- “清空日志”按钮文字被裁剪，悬停后背景/文字变化不符合统一浅色主题。
- “发现的服务”区域和下方日志区域大框仍显示直角，未沿用 Mac 风格圆角。

## 根因

1. 上游 `XCoderAv/Views/ApiDiscoverWindow.axaml` 的“清空服务”和“清空日志”同时放在 Grid 第 3 列；“清空日志”还设置右 Margin=90、Width=80，导致两个按钮重叠并压缩文字可视空间。
2. “发现的服务”实际是 `ListBox` 而非 `TextBox`，调整全局文本框 CornerRadius 对它不生效。
3. 两个大框的默认 Fluent 模板内部边框可能保持方形；单给原控件设置圆角不足以修复用户看到的外部轮廓。

## 本次修改（只在 XCoderMac 中）

- 新增 `Services/MacApiDiscoverLayout.cs`；由 `Views/MacLegacyToolWindow.axaml.cs` 仅在“API 发现”工具加载时调用，不改上游 XCoderAv。
- 顶部按钮改为独立五列（端口标签、端口值、扫描、清空服务、清空日志），`ColumnSpacing=10`。三个按钮高度 38，扫描列 94，其余列各 112；去掉上游冲突的固定宽度及右边距，保留原来的 `ScanCommand`、`ClearServicesCommand`、`ClearLogCommand`。
- `Views/MacLegacyToolWindow.axaml` 增加仅限 `api-discover-action` 的正常、pointerover、pressed 样式：浅蓝底深字 → 蓝底白字，避免全局 Hover 覆盖造成文字不明显。
- 服务 `ListBox` 和日志 `TextBox` 分别包在圆角 `Border` 内：`CornerRadius=11`、`ClipToBounds=true`、浅底、淡蓝边框。原控件内部边框取消，只由外层 Border 画圆角；日志控件自身同时设置 CornerRadius=10 和 ClipToBounds，保留原来的绑定、只读/编辑属性与内容。
- 不执行 API 扫描，不改变 UDP 协议行为、网络权限或服务识别逻辑。

## 专项验证

- 编译：`dotnet build XCoderMac/XCoderMac.csproj -c Debug --no-restore -p:BuildProjectReferences=false`，成功，0 错误。上游 XCoderAv 的原有两处 CS0103 问题仍未修改。
- 新增 `Services/MacApiDiscoverUiSmoke.cs` 和参数 `--smoke-api-discover-ui`。
- GUI 实际初始化后，在 1060/850 两种宽度检查按钮不重叠、宽度及命令，模拟 pointerover 验证前景文字和背景都变化；确认 ListBox 和日志 TextBox 的外层 Border 四角圆角及裁剪，验证日志 normal/pointerover/focus/focus-within 不丢失圆角与浅色背景。**全部通过**。
- 实际渲染截图：`Docs/软件界面/04-API发现-按钮与圆角修复.png`，已人工复核按钮排列、圆角形状。
- 额外回归 `--smoke-mqtt-ui`：通过；已确认通用宿主新增的 API 发现按钮样式不影响 MQTT 按钮 Hover、分组和日志黑底。
- 使用已有上游编译产物执行 `BuildProjectReferences=false ./XCoderMac/package-macos.sh` 重新打包成功，生成 `XCoderMac/dist/XCoder.app` 和 `XCoderMac/dist/XCoder-arm64.dmg`；ad-hoc 签名、尚未正式 Apple 公证。
- 仅验证视觉、交互样式与绑定，尚未执行本机真实网络广播/服务发现，不把 UI 测试认定为协议验收。

## 交接与规则

- 本轮须同步 `README.md`、`Docs/项目现状与任务总清单.md`、`Docs/文档索引.md` 及截图目录；以后每次代码修改同轮记录原因、文件、专项测试和遗留问题。
- 未经要求不提交或推送 Git；禁止修改上游文件。
