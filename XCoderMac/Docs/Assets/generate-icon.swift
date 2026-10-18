import AppKit

// Editable vector app icon source; build resources exclusively within XCoderMac/Assets.
func c(_ r: CGFloat, _ g: CGFloat, _ b: CGFloat, _ a: CGFloat = 1) -> NSColor {
    NSColor(calibratedRed: r, green: g, blue: b, alpha: a)
}
let size = 1024
guard let bitmap = NSBitmapImageRep(bitmapDataPlanes: nil, pixelsWide: size, pixelsHigh: size,
    bitsPerSample: 8, samplesPerPixel: 4, hasAlpha: true, isPlanar: false,
    colorSpaceName: .deviceRGB, bytesPerRow: 0, bitsPerPixel: 0),
    let ctx = NSGraphicsContext(bitmapImageRep: bitmap) else { fatalError("Bitmap allocation failed") }
NSGraphicsContext.saveGraphicsState()
NSGraphicsContext.current = ctx
ctx.imageInterpolation = .high
ctx.shouldAntialias = true
NSColor.clear.setFill()
NSRect(x: 0, y: 0, width: size, height: size).fill()

let tile = NSBezierPath(roundedRect: NSRect(x: 83, y: 96, width: 858, height: 858),
                        xRadius: 190, yRadius: 190)
let shadow = NSShadow()
shadow.shadowOffset = NSSize(width: 0, height: -19)
shadow.shadowBlurRadius = 32
shadow.shadowColor = c(0.04, 0.16, 0.34, 0.27)
NSGraphicsContext.saveGraphicsState()
shadow.set()
NSGradient(starting: c(0.07, 0.25, 0.59), ending: c(0.14, 0.51, 0.89))!
    .draw(in: tile, angle: 45)
NSGraphicsContext.restoreGraphicsState()

NSGraphicsContext.saveGraphicsState()
tile.addClip()
let glow = NSBezierPath(ovalIn: NSRect(x: 470, y: 450, width: 650, height: 650))
NSGradient(starting: c(0.5, 0.87, 1, 0.18), ending: c(0.15, 0.4, 0.85, 0))!
    .draw(in: glow, angle: 110)
NSGraphicsContext.restoreGraphicsState()

let monogram = NSBezierPath()
monogram.lineWidth = 112
monogram.lineCapStyle = .round
monogram.lineJoinStyle = .round
monogram.move(to: NSPoint(x: 361, y: 703))
monogram.line(to: NSPoint(x: 663, y: 349))
monogram.move(to: NSPoint(x: 361, y: 349))
monogram.line(to: NSPoint(x: 663, y: 703))
let xShadow = NSShadow()
xShadow.shadowOffset = NSSize(width: 0, height: -9)
xShadow.shadowBlurRadius = 11
xShadow.shadowColor = c(0.02, 0.1, 0.31, 0.26)
NSGraphicsContext.saveGraphicsState()
xShadow.set()
NSColor.white.setStroke()
monogram.stroke()
NSGraphicsContext.restoreGraphicsState()

let spark = NSBezierPath()
for p in [NSPoint(x: 742, y: 748), NSPoint(x: 755, y: 717),
          NSPoint(x: 788, y: 703), NSPoint(x: 755, y: 690),
          NSPoint(x: 742, y: 658), NSPoint(x: 729, y: 690),
          NSPoint(x: 698, y: 703), NSPoint(x: 729, y: 717)].enumerated() {
    if p.offset == 0 { spark.move(to: p.element) }
    else { spark.line(to: p.element) }
}
spark.close()
c(0.63, 0.99, 1).setFill()
spark.fill()
ctx.flushGraphics()
NSGraphicsContext.restoreGraphicsState()

let filename = CommandLine.arguments.count > 1 ? CommandLine.arguments[1] : "XCoderMac/Docs/Assets/XCoderMac-1024.png"
guard let png = bitmap.representation(using: .png, properties: [:]) else { fatalError("Cannot encode PNG") }
try png.write(to: URL(fileURLWithPath: filename))
print("Generated app icon:", filename)
