#!/usr/bin/env bash
# 生成 XCoderMac Apple Silicon 应用与 DMG。本脚本仅写入 XCoderMac/ 内部。
set -euo pipefail
ROOT="$(cd "$(dirname "$0")" && pwd)"
DIST="$ROOT/dist"
PUBLISH="$ROOT/obj/publish/osx-arm64"
APP="$DIST/XCoderMac.app"
CONTENTS="$APP/Contents"
STAGE="$DIST/dmg-stage"
VERSION="0.1.4"

if [[ "$(uname -m)" != "arm64" ]]; then
    echo "错误：请在 Apple Silicon Mac 上执行。" >&2
    exit 1
fi

mkdir -p "$DIST"
echo "[1/5] 发布独立运行的 arm64 应用..."
dotnet publish "$ROOT/XCoderMac.csproj" -c Release -r osx-arm64 \
    --self-contained true -p:UseAppHost=true -o "$PUBLISH"

echo "[2/5] 组装 .app..."
rm -rf "$APP" "$STAGE"
mkdir -p "$CONTENTS/MacOS" "$CONTENTS/Resources"
cp -R "$PUBLISH"/. "$CONTENTS/MacOS/"
chmod +x "$CONTENTS/MacOS/XCoderMac"
# 保留之前确认的 Dock/Finder 应用图标，资源路径统一位于 Docs/Assets。
test -s "$ROOT/Docs/Assets/XCoderMac.icns"
cp "$ROOT/Docs/Assets/XCoderMac.icns" "$CONTENTS/Resources/XCoderMac.icns"

# 不将开发机器的私有连接配置、日志或 Redis 凭据打入可分发应用包。
rm -rf "$CONTENTS/MacOS/Config" "$CONTENTS/MacOS/Log" "$CONTENTS/MacOS/Data"
cat > "$CONTENTS/Info.plist" <<'PLIST'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
    "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key><string>XCoder Mac</string>
    <key>CFBundleDisplayName</key><string>XCoder Mac</string>
    <key>CFBundleIdentifier</key><string>com.newlifex.xcodermac</string>
    <key>CFBundleExecutable</key><string>XCoderMac</string>
    <key>CFBundleIconFile</key><string>XCoderMac.icns</string>
    <key>CFBundleIconName</key><string>XCoderMac</string>
    <key>CFBundlePackageType</key><string>APPL</string>
    <key>CFBundleShortVersionString</key><string>0.1.4</string>
    <key>CFBundleVersion</key><string>5</string>
    <key>LSMinimumSystemVersion</key><string>13.0</string>
    <key>NSHighResolutionCapable</key><true/>
</dict>
</plist>
PLIST

echo "[3/5] 本机 ad-hoc 签名（非 Apple 开发者公证签名）..."
codesign --force --deep --sign - "$APP"
codesign --verify --deep --strict "$APP"

echo "[4/5] 制作 DMG..."
mkdir -p "$STAGE"
cp -R "$APP" "$STAGE/XCoderMac.app"
ln -s /Applications "$STAGE/Applications"
rm -f "$DIST/XCoderMac-preview-arm64.dmg"
hdiutil create -volname "XCoderMac" -srcfolder "$STAGE" \
    -format UDZO -ov "$DIST/XCoderMac-preview-arm64.dmg" >/dev/null
rm -rf "$STAGE"

echo "[5/5] 验证包内容..."
test -x "$APP/Contents/MacOS/XCoderMac"
test -s "$APP/Contents/Resources/XCoderMac.icns"
test -f "$DIST/XCoderMac-preview-arm64.dmg"
echo "APP: $APP"
echo "DMG: $DIST/XCoderMac-preview-arm64.dmg"
echo "NOTICE: 本预览包仅 ad-hoc 签名，没有 Apple Developer ID 公证；跨设备分发需正式签名与 notarization。"
