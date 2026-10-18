using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using NewLife.Log;

namespace ALib
{
    /// <summary>
    /// 嵌入 ZIP 资源释放器。
    /// 原生库多为未压缩的 PE 文件，ZIP 压缩率通常可达 3~8 倍，
    /// 因此把成组文件（或单个大 DLL）打包为 zip 嵌入，可显著减小程序集体积；
    /// 运行首次使用时再解压还原到磁盘。
    /// 仅依赖 .NET 内置的 <see cref="System.IO.Compression"/>，无需第三方库。
    /// </summary>
    public static class ZipResourceExtractor
    {
        /// <summary>是否已释放过，避免重复解压</summary>
        private static readonly HashSet<String> _done = new HashSet<String>(StringComparer.OrdinalIgnoreCase);

        /// <summary>释放过程锁</summary>
        private static readonly Object _lock = new Object();

        /// <summary>
        /// 把嵌入的 zip 资源解压到指定目录（保留 zip 内的相对结构）。
        /// </summary>
        /// <param name="resourceName">嵌入资源完整名，如 ALib.ZLG_CAN.kerneldlls.zip</param>
        /// <param name="destDir">目标目录</param>
        /// <param name="checkFile">
        /// 完整性校验用的相对文件名（如 dll_cfg.ini）。
        /// 该文件存在即认为目录已释放完整，直接跳过解压；
        /// 传 null 则每次都按缓存标记判断。
        /// </param>
        /// <returns>是否成功</returns>
        public static Boolean ExtractToDirectory(String resourceName, String destDir, String checkFile = null)
        {
            if (String.IsNullOrEmpty(resourceName) || String.IsNullOrEmpty(destDir)) return false;

            lock (_lock)
            {
                if (_done.Contains(resourceName)) return true;

                // 已释放且关键文件在，直接复用，避免每次启动都解压
                if (!String.IsNullOrEmpty(checkFile) && File.Exists(Path.Combine(destDir, checkFile)))
                {
                    _done.Add(resourceName);
                    return true;
                }

                try
                {
                    var asm = Assembly.GetExecutingAssembly();
                    using var stream = asm.GetManifestResourceStream(resourceName);
                    if (stream == null)
                    {
                        XTrace.WriteLine($"找不到嵌入资源: {resourceName}");
                        return false;
                    }

                    using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

                    Directory.CreateDirectory(destDir);

                    var count = 0;
                    foreach (var entry in zip.Entries)
                    {
                        // 目录项（以 / 结尾）本身不写文件，但其下的文件会隐式建目录
                        if (String.IsNullOrEmpty(entry.Name)) continue;

                        var rel = entry.FullName.Replace('/', Path.DirectorySeparatorChar);

                        // 防目录穿越：只允许解压到 destDir 之内
                        var dest = Path.GetFullPath(Path.Combine(destDir, rel));
                        var rootFull = Path.GetFullPath(destDir);
                        if (!dest.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
                        {
                            XTrace.WriteLine($"跳过越界条目: {entry.FullName}");
                            continue;
                        }

                        var dir = Path.GetDirectoryName(dest);
                        if (!String.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                        // 大小一致则跳过写入：驱动已被加载时无法覆盖，跳过也能保证可用
                        var fi = new FileInfo(dest);
                        if (fi.Exists && fi.Length == entry.Length) continue;

                        WriteEntry(entry, dest);
                        count++;
                    }

                    var ok = String.IsNullOrEmpty(checkFile) || File.Exists(Path.Combine(destDir, checkFile));
                    if (ok)
                    {
                        _done.Add(resourceName);
                        XTrace.WriteLine($"ZIP 资源已释放: {resourceName} -> {destDir}（解压 {count} 个文件）");
                    }
                    else
                    {
                        XTrace.WriteLine($"ZIP 释放后缺少校验文件 {checkFile}，目录可能不完整: {destDir}");
                    }
                    return ok;
                }
                catch (Exception ex)
                {
                    XTrace.WriteLine($"释放 ZIP 资源失败 {resourceName}: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 把嵌入 zip 中的某个条目解压为单个文件（zip 内可只放这一个文件）。
        /// </summary>
        /// <param name="resourceName">嵌入资源完整名，如 ALib.ZLG_CAN.zlgcan.zip</param>
        /// <param name="destPath">目标文件路径</param>
        /// <returns>是否成功</returns>
        public static Boolean ExtractFile(String resourceName, String destPath)
        {
            if (String.IsNullOrEmpty(resourceName) || String.IsNullOrEmpty(destPath)) return false;

            lock (_lock)
            {
                var key = resourceName + "|" + destPath;
                if (_done.Contains(key)) return true;

                // 已存在且非空，直接复用
                var fi = new FileInfo(destPath);
                if (fi.Exists && fi.Length > 0)
                {
                    _done.Add(key);
                    return true;
                }

                try
                {
                    var asm = Assembly.GetExecutingAssembly();
                    using var stream = asm.GetManifestResourceStream(resourceName);
                    if (stream == null)
                    {
                        XTrace.WriteLine($"找不到嵌入资源: {resourceName}");
                        return false;
                    }

                    using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

                    // 单文件模式：取第一个非目录条目
                    ZipArchiveEntry target = null;
                    foreach (var e in zip.Entries)
                    {
                        if (!String.IsNullOrEmpty(e.Name)) { target = e; break; }
                    }
                    if (target == null)
                    {
                        XTrace.WriteLine($"ZIP 中无可解压文件: {resourceName}");
                        return false;
                    }

                    var dir = Path.GetDirectoryName(destPath);
                    if (!String.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                    WriteEntry(target, destPath);

                    _done.Add(key);
                    return true;
                }
                catch (Exception ex)
                {
                    XTrace.WriteLine($"释放 ZIP 文件失败 {resourceName}: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>查找以指定后缀结尾的嵌入 zip 资源名，找不到返回 null</summary>
        /// <param name="suffix">资源名后缀，如 ".ZLG_CAN.zlgcan.zip"</param>
        /// <returns>完整资源名</returns>
        public static String FindResource(String suffix)
        {
            if (String.IsNullOrEmpty(suffix)) return null;

            try
            {
                var asm = Assembly.GetExecutingAssembly();
                foreach (var name in asm.GetManifestResourceNames())
                {
                    if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) return name;
                }
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"枚举嵌入资源失败: {ex.Message}");
            }

            return null;
        }

        /// <summary>把 zip 条目写入磁盘（先写临时文件再原子替换）</summary>
        /// <param name="entry">zip 条目</param>
        /// <param name="destPath">目标路径</param>
        private static void WriteEntry(ZipArchiveEntry entry, String destPath)
        {
            var tmp = destPath + ".tmp";

            using (var src = entry.Open())
            using (var fs = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                src.CopyTo(fs);
                fs.Flush();
            }

            try
            {
                File.Move(tmp, destPath, true);
            }
            catch
            {
                // 目标被占用（如驱动已加载）时保留旧文件；旧文件不存在才视为失败
                try { File.Delete(tmp); } catch { }
                if (!File.Exists(destPath)) throw;
            }
        }
    }
}
