using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NewLife.Log;

namespace ALib
{
    /// <summary>
    /// DLL资源管理器 - 提供通用的DLL嵌入资源释放功能
    /// </summary>
    public static class DllResourceManager
    {
        private static readonly Dictionary<string, bool> _loadedDlls = new Dictionary<string, bool>();
        private static readonly Dictionary<string, IntPtr> _loadedHandles = new Dictionary<string, IntPtr>();
        private static readonly object _lockObject = new object();

        /// <summary>
        /// 已释放DLL的绝对路径缓存：文件名(小写) -> 绝对路径。
        /// 供 DllImport 解析器使用，保证原生库能被按绝对路径正确加载。
        /// </summary>
        private static readonly Dictionary<string, string> _resolvedPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>DllImport 解析器是否已注册（只会注册一次）</summary>
        private static bool _resolverRegistered;

        /// <summary>
        /// 确保DLL可用，如果不存在则从嵌入资源中释放并显式加载
        /// </summary>
        /// <param name="resourcePrefix">资源前缀（通常是命名空间路径）</param>
        /// <param name="dllFileName">DLL文件名</param>
        /// <returns>是否成功</returns>
        public static bool EnsureDllAvailable(string resourcePrefix, string dllFileName)
        {
            lock (_lockObject)
            {
                if (_loadedDlls.ContainsKey(dllFileName) && _loadedDlls[dllFileName])
                {
                    return true;
                }

                try
                {
                    RegisterResolver();

                    // 1）优先应用目录：Release 后与主程序同目录，DllImport 默认搜索顺序即可命中
                    // 注意：单文件发布时 Assembly.Location 为空，必须用 AppContext.BaseDirectory
                    var appDir = GetAppDirectory();
                    if (!string.IsNullOrEmpty(appDir))
                    {
                        var appDllPath = Path.Combine(appDir, dllFileName);
                        if (File.Exists(appDllPath))
                        {
                            CacheResolvedPath(dllFileName, appDllPath);
                            _loadedDlls[dllFileName] = true;
                            return true;
                        }
                    }

                    // 2）系统路径（应用目录 / PATH）中已存在，直接使用
                    var existPath = FindDllInSystem(dllFileName);
                    if (!string.IsNullOrEmpty(existPath))
                    {
                        CacheResolvedPath(dllFileName, existPath);
                        _loadedDlls[dllFileName] = true;
                        return true;
                    }

                    // 3）从嵌入资源中释放（释放到应用目录，失败则回退临时目录）
                    var dllPath = ExtractDllFromResource(resourcePrefix, dllFileName);
                    CacheResolvedPath(dllFileName, dllPath);

                    // 显式加载DLL
                    return LoadDllExplicitly(dllFileName, dllPath);
                }
                catch (Exception ex)
                {
                    XTrace.WriteLine($"DLL加载失败 {dllFileName}: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 记录已解析到的DLL绝对路径，供 DllImport 解析器回查。
        /// </summary>
        private static void CacheResolvedPath(string dllFileName, string dllPath)
        {
            if (string.IsNullOrEmpty(dllPath)) return;
            _resolvedPaths[dllFileName] = dllPath;
            _resolvedPaths[Path.GetFileName(dllPath)] = dllPath;
        }

        /// <summary>
        /// 为当前程序集注册 DllImport 解析器。
        /// 单文件发布时原生DLL可能被释放到临时目录，而 DllImport 默认搜索顺序不包含该目录，
        /// 通过解析器把逻辑名映射到释放后的绝对路径，避免 DllNotFoundException。
        /// </summary>
        private static void RegisterResolver()
        {
            if (_resolverRegistered) return;
            _resolverRegistered = true;

            try
            {
                NativeLibrary.SetDllImportResolver(typeof(DllResourceManager).Assembly, ResolveNativeLibrary);
            }
            catch (InvalidOperationException)
            {
                // 同一程序集重复注册会抛此异常，忽略即可
                _resolverRegistered = true;
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"注册DllImport解析器失败: {ex.Message}");
            }
        }

        /// <summary>
        /// DllImport 解析回调：先查缓存路径，再回退到默认加载逻辑。
        /// </summary>
        private static IntPtr ResolveNativeLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (!string.IsNullOrEmpty(libraryName))
            {
                string cached;
                if (_resolvedPaths.TryGetValue(libraryName, out cached) && File.Exists(cached))
                {
                    if (NativeLibrary.TryLoad(cached, out var handle)) return handle;
                }
            }

            // 回退：按系统默认搜索顺序查找
            string fallback = libraryName;
            if (!string.IsNullOrEmpty(fallback) && NativeLibrary.TryLoad(fallback, assembly, searchPath, out var defaultHandle))
            {
                return defaultHandle;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 获取应用目录。单文件发布时 Assembly.Location 为空字符串，必须回退到 AppContext.BaseDirectory。
        /// </summary>
        private static string GetAppDirectory()
        {
            try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(location))
                {
                    var dir = Path.GetDirectoryName(location);
                    if (!string.IsNullOrEmpty(dir)) return dir;
                }
            }
            catch { }

            // 单文件发布会走到这里：BaseDirectory 指向 exe 所在目录
            return AppContext.BaseDirectory;
        }

        /// <summary>
        /// 在系统路径中查找DLL文件
        /// </summary>
        private static string FindDllInSystem(string dllFileName)
        {
            // 检查应用程序目录
            string appDir = GetAppDirectory();
            if (string.IsNullOrEmpty(appDir)) return null;

            string dllPathInAppDir = Path.Combine(appDir, dllFileName);
            if (File.Exists(dllPathInAppDir))
            {
                return dllPathInAppDir;
            }

            // 检查系统PATH中的目录
            string[] paths = Environment.GetEnvironmentVariable("PATH")?.Split(';') ?? Array.Empty<string>();
            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(path)) continue;
                string fullPath = Path.Combine(path.Trim(), dllFileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }

        /// <summary>
        /// 显式加载DLL到内存
        /// </summary>
        private static bool LoadDllExplicitly(string dllFileName, string dllPath)
        {
            try
            {
                // 如果已经加载过，先释放
                if (_loadedHandles.ContainsKey(dllFileName) && _loadedHandles[dllFileName] != IntPtr.Zero)
                {
                    NativeLibrary.Free(_loadedHandles[dllFileName]);
                    _loadedHandles.Remove(dllFileName);
                }

                // 显式加载DLL
                IntPtr handle = NativeLibrary.Load(dllPath);
                if (handle == IntPtr.Zero)
                {
                    throw new InvalidOperationException($"无法加载DLL: {dllPath}");
                }

                _loadedHandles[dllFileName] = handle;
                _loadedDlls[dllFileName] = true;
                
                XTrace.WriteLine($"DLL已显式加载: {dllPath}");
                return true;
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"显式加载DLL失败 {dllFileName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 从嵌入资源中释放DLL。
        /// 优先释放到应用目录（DllImport 默认搜索顺序第一步即可命中）；若应用目录不可写，则回退到临时目录。
        /// 支持两种嵌入形态，按「zip 优先」选择：
        ///   1）zip 压缩：{前缀}.{dll名去扩展名}.zip，内含同名 dll，体积可减小数倍（推荐）
        ///   2）裸文件：{前缀}.{dll文件名}
        /// </summary>
        private static string ExtractDllFromResource(string resourcePrefix, string dllFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // 约定：zlgcan.dll -> ALib.ZLG_CAN.zlgcan.zip
            var baseName = Path.GetFileNameWithoutExtension(dllFileName);
            var zipResourceName = $"{resourcePrefix}.{baseName}.zip";

            // 优先释放到应用目录，确保 DllImport("xxx.dll") 能被默认搜索路径命中
            // （临时目录不在原生DLL默认搜索路径中，会导致 DllImport 运行时找不到库而抛 DllNotFoundException）
            var appDir = GetAppDirectory();
            if (!string.IsNullOrEmpty(appDir))
            {
                var appDllPath = Path.Combine(appDir, dllFileName);
                if (TryExtract(assembly, zipResourceName, $"{resourcePrefix}.{dllFileName}", appDllPath))
                {
                    XTrace.WriteLine($"DLL已释放到: {appDllPath}");
                    return appDllPath;
                }
            }

            // 回退：应用目录不可写（如单文件放在 Program Files、只读盘）时释放到临时目录。
            // 释放到 Temp 后必须依赖已注册的 DllImport 解析器按绝对路径加载，否则 DllImport 找不到库。
            var tempDir = Path.Combine(Path.GetTempPath(), "ALib.Native");
            string tempDllPath = Path.Combine(tempDir, dllFileName);
            if (!TryExtract(assembly, zipResourceName, $"{resourcePrefix}.{dllFileName}", tempDllPath))
            {
                throw new InvalidOperationException($"无法释放嵌入资源: {resourcePrefix}.{dllFileName} 或 {zipResourceName}");
            }

            // 进程退出时清理临时目录内本次释放的文件
            var pathToDelete = tempDllPath;
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                try { if (File.Exists(pathToDelete)) File.Delete(pathToDelete); } catch { }
            };

            XTrace.WriteLine($"DLL已释放到: {tempDllPath}");
            return tempDllPath;
        }

        /// <summary>
        /// 释放单个 DLL：优先尝试 zip 资源，不存在或失败则回退裸文件资源。
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="zipResourceName">zip 资源名（可不存在）</param>
        /// <param name="rawResourceName">裸文件资源名（可不存在）</param>
        /// <param name="destPath">目标路径</param>
        /// <returns>是否成功</returns>
        private static bool TryExtract(Assembly assembly, string zipResourceName, string rawResourceName, string destPath)
        {
            // 目标已存在（且非本次写入）时直接复用，避免文件被占用导致写入失败
            if (File.Exists(destPath) && new FileInfo(destPath).Length > 0) return true;

            // 1）zip 形式
            if (!string.IsNullOrEmpty(zipResourceName) &&
                assembly.GetManifestResourceStream(zipResourceName) != null)
            {
                if (ZipResourceExtractor.ExtractFile(zipResourceName, destPath)) return true;
                XTrace.WriteLine($"zip 释放失败，尝试裸文件资源: {zipResourceName}");
            }

            // 2）裸文件形式
            return TryExtractTo(assembly, rawResourceName, destPath);
        }

        /// <summary>将嵌入资源写入指定路径，成功返回 true</summary>
        private static bool TryExtractTo(Assembly assembly, string resourceName, string destPath)
        {
            try
            {
                // 目标已存在（且非本次写入）时直接复用，避免文件被占用导致写入失败
                if (File.Exists(destPath)) return true;

                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    throw new InvalidOperationException($"无法找到嵌入资源: {resourceName}");
                }

                // 确保目录存在
                var dir = Path.GetDirectoryName(destPath);
                if (!String.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                // 写临时文件后原子替换，避免并发/占用
                var tmp = destPath + ".tmp";
                using (var fileStream = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.CopyTo(fileStream);
                    fileStream.Flush();
                }
                File.Move(tmp, destPath, true);

                return true;
            }
            catch (Exception ex)
            {
                XTrace.WriteLine($"释放 {Path.GetFileName(destPath)} 失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 重置DLL加载状态（主要用于测试或重新加载）
        /// </summary>
        public static void ResetLoadStatus(string dllFileName = null)
        {
            lock (_lockObject)
            {
                if (dllFileName != null)
                {
                    if (_loadedHandles.ContainsKey(dllFileName))
                    {
                        NativeLibrary.Free(_loadedHandles[dllFileName]);
                        _loadedHandles.Remove(dllFileName);
                    }
                    _loadedDlls.Remove(dllFileName);
                }
                else
                {
                    // 释放所有句柄
                    foreach (var handle in _loadedHandles.Values)
                    {
                        if (handle != IntPtr.Zero)
                        {
                            NativeLibrary.Free(handle);
                        }
                    }
                    _loadedHandles.Clear();
                    _loadedDlls.Clear();
                }
            }
        }

        /// <summary>
        /// 程序退出时清理资源（可选）
        /// </summary>
        public static void Cleanup()
        {
            ResetLoadStatus();
        }
    }
}