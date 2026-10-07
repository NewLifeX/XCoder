using System;
using System.IO;
using System.Reflection;
using NewLife.Log;

namespace ALib.ZLG_CAN
{
    /// <summary>
    /// 把嵌入的 ZLG kerneldlls 驱动目录释放到磁盘。
    /// zlgcan.dll 运行时固定按「自身同级目录\kerneldlls\」查找内核驱动与 dll_cfg.ini，
    /// 因此必须还原出真实的目录结构，不能只把 dll 平铺到程序目录。
    /// </summary>
    internal static class KernelDllExtractor
    {
        /// <summary>驱动目录名（zlgcan.dll 约定，不可更改）</summary>
        public const String DirName = "kerneldlls";

        /// <summary>驱动映射配置文件，用于判断目录是否已释放完整</summary>
        public const String CfgFileName = "dll_cfg.ini";

        /// <summary>
        /// 嵌入资源前缀。csproj 中 LogicalName 为
        /// <c>ALib.ZLG_CAN.kerneldlls.%(RecursiveDir)%(Filename)%(Extension)</c>，
        /// 生成结果形如：
        /// <c>ALib.ZLG_CAN.kerneldlls.dll_cfg.ini</c>（根目录文件）
        /// <c>ALib.ZLG_CAN.kerneldlls.devices_property\default\usbcan2.xml</c>（子目录文件）
        /// 即前缀之后「反斜杠」保留为路径分隔符，可据此精确还原目录层级。
        /// </summary>
        private const String ResourcePrefix = "ALib.ZLG_CAN.kerneldlls";

        /// <summary>释放结果缓存</summary>
        private static Boolean _extracted;
        private static readonly Object _lock = new Object();

        /// <summary>把 kerneldlls 下所有嵌入资源释放到 应用目录\kerneldlls\</summary>
        /// <param name="appDir">应用目录</param>
        /// <returns>是否全部就位</returns>
        public static Boolean Extract(String appDir)
        {
            lock (_lock)
            {
                if (_extracted) return true;
                if (String.IsNullOrEmpty(appDir)) return false;

                try
                {
                    var root = Path.Combine(appDir, DirName);

                    // 1）优先使用 zip 打包形式：kerneldlls 内含大量 DLL/XML，
                    //    整目录压缩后体积可减小数倍，且解压逻辑更简单。
                    var zipName = $"{ResourcePrefix}.zip";
                    if (Assembly.GetExecutingAssembly().GetManifestResourceStream(zipName) != null)
                    {
                        if (ZipResourceExtractor.ExtractToDirectory(zipName, root, CfgFileName))
                        {
                            _extracted = true;
                            return true;
                        }
                        XTrace.WriteLine("zip 形式释放失败，回退逐个资源释放");
                    }

                    // 2）回退：逐个嵌入资源释放（兼容未打包的旧配置）
                    return ExtractIndividual(appDir, root);
                }
                catch (Exception ex)
                {
                    XTrace.WriteLine("释放 ZLG kerneldlls 异常: " + ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// 逐个嵌入资源释放（kerneldlls 未打包为 zip 时的兼容路径）。
        /// </summary>
        /// <param name="appDir">应用目录</param>
        /// <param name="root">目标根目录</param>
        /// <returns>是否全部就位</returns>
        private static Boolean ExtractIndividual(String appDir, String root)
        {
            var asm = Assembly.GetExecutingAssembly();
            var names = asm.GetManifestResourceNames();

            var count = 0;
            var skipped = 0;

            foreach (var name in names)
            {
                if (!name.StartsWith(ResourcePrefix, StringComparison.OrdinalIgnoreCase)) continue;

                var rel = ToRelativePath(name);
                if (String.IsNullOrEmpty(rel)) continue;

                count++;

                var dest = Path.Combine(root, rel);

                // 已存在且大小一致则跳过写入，减少重复磁盘写入
                if (IsUpToDate(asm, name, dest))
                {
                    skipped++;
                    continue;
                }

                if (!TryExtractOne(asm, name, dest))
                {
                    XTrace.WriteLine($"释放 ZLG 驱动失败: {rel}");
                    return false;
                }
            }

            if (count == 0)
            {
                XTrace.WriteLine("未找到 ZLG kerneldlls 嵌入资源，请检查 A1Lib.csproj 的 EmbeddedResource 配置");
                return false;
            }

            var ok = File.Exists(Path.Combine(root, CfgFileName));
            if (ok) XTrace.WriteLine($"ZLG 驱动就绪: 共 {count} 个文件（复用 {skipped}，写入 {count - skipped}）于 {root}");
            else XTrace.WriteLine($"ZLG 驱动目录缺少 {CfgFileName}，zlgcan.dll 可能无法加载设备驱动");

            _extracted = ok;
            return ok;
        }

        /// <summary>目标文件是否已是最新（存在且长度与嵌入资源一致）</summary>
        /// <param name="asm">程序集</param>
        /// <param name="resourceName">资源名</param>
        /// <param name="destPath">磁盘路径</param>
        /// <returns></returns>
        private static Boolean IsUpToDate(Assembly asm, String resourceName, String destPath)
        {
            try
            {
                var fi = new FileInfo(destPath);
                if (!fi.Exists) return false;

                using var stream = asm.GetManifestResourceStream(resourceName);
                if (stream == null) return false;

                return fi.Length == stream.Length;
            }
            catch { return false; }
        }

        /// <summary>资源名 -> 相对路径（如 devices_property\default\usbcan2.xml）</summary>
        /// <param name="resourceName">嵌入资源名</param>
        /// <returns>相对路径，无法解析返回 null</returns>
        private static String ToRelativePath(String resourceName)
        {
            // 前缀之后的部分：".dll_cfg.ini" 或 ".devices_property\default\usbcan2.xml"
            var tail = resourceName.Substring(ResourcePrefix.Length);
            tail = tail.TrimStart('.');

            if (String.IsNullOrEmpty(tail)) return null;

            // 根目录文件：资源名中不含反斜杠，直接就是文件名；
            // 子目录文件：反斜杠已被 MSBuild 保留，天然就是相对路径。
            return tail;
        }

        /// <summary>释放单个资源到指定路径</summary>
        /// <param name="asm">程序集</param>
        /// <param name="resourceName">资源名</param>
        /// <param name="destPath">目标路径</param>
        /// <returns>是否成功</returns>
        private static Boolean TryExtractOne(Assembly asm, String resourceName, String destPath)
        {
            try
            {
                var dir = Path.GetDirectoryName(destPath);
                if (!String.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                using var stream = asm.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    XTrace.WriteLine($"找不到嵌入资源: {resourceName}");
                    return false;
                }

                // 先写临时文件再原子替换，避免驱动正被加载时写入失败
                var tmp = destPath + ".tmp";
                using (var fs = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.CopyTo(fs);
                    fs.Flush();
                }

                try
                {
                    File.Move(tmp, destPath, true);
                }
                catch
                {
                    // 目标文件被占用（如驱动已加载）时保留旧文件，视为成功
                    try { File.Delete(tmp); } catch { }
                    if (!File.Exists(destPath)) throw;
                }

                return true;
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"写入 {Path.GetFileName(destPath)} 失败: {ex.Message}");
                return false;
            }
        }
    }
}
