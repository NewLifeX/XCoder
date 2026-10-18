using System.Runtime.InteropServices;
using System.Text;

namespace XCoderMac.Services;

/// <summary>调用系统钥匙串，不经 shell/argv 暴露密码，密钥只用于数据库连接。</summary>
public static class MacKeychainSecrets
{
    private const string SecurityFramework = "/System/Library/Frameworks/Security.framework/Security";
    private const string CoreFoundationFramework = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private const int NotFound = -25300;
    public const string Service = "Newlife.XCoder.Database";

    [DllImport(SecurityFramework, EntryPoint = "SecKeychainAddGenericPassword")]
    private static extern int Add(IntPtr keychain, uint serviceLen, byte[] serviceName,
        uint accountLen, byte[] accountName, uint passwordLen, byte[] passwordData, out IntPtr itemRef);

    [DllImport(SecurityFramework, EntryPoint = "SecKeychainFindGenericPassword")]
    private static extern int Find(IntPtr keychainOrArray, uint serviceLen, byte[] serviceName,
        uint accountLen, byte[] accountName, out uint passwordLen, out IntPtr passwordData, out IntPtr itemRef);

    [DllImport(SecurityFramework, EntryPoint = "SecKeychainItemModifyAttributesAndData")]
    private static extern int Modify(IntPtr itemRef, IntPtr attrList, uint passwordLen, byte[] passwordData);

    [DllImport(SecurityFramework, EntryPoint = "SecKeychainItemDelete")]
    private static extern int Delete(IntPtr itemRef);

    [DllImport(SecurityFramework, EntryPoint = "SecKeychainItemFreeContent")]
    private static extern int FreeContent(IntPtr attrList, IntPtr passwordData);

    [DllImport(CoreFoundationFramework, EntryPoint = "CFRelease")]
    private static extern void Release(IntPtr handle);

    private static (byte[] Service, byte[] Account) Keys(string name) =>
        (Encoding.UTF8.GetBytes(Service), Encoding.UTF8.GetBytes(name));

    public static string? Read(string name)
    {
        var (service, account) = Keys(name);
        var status = Find(IntPtr.Zero, (uint)service.Length, service,
            (uint)account.Length, account, out var size, out var data, out var item);
        if (status == NotFound) return null;
        if (status != 0) throw new InvalidOperationException($"macOS 钥匙串读取失败 (OSStatus={status})");
        try
        {
            if (size == 0) return string.Empty;
            var buffer = new byte[checked((int)size)];
            Marshal.Copy(data, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }
        finally
        {
            if (data != IntPtr.Zero) FreeContent(IntPtr.Zero, data);
            if (item != IntPtr.Zero) Release(item);
        }
    }

    public static void Write(string name, string password)
    {
        var (service, account) = Keys(name);
        var bytes = Encoding.UTF8.GetBytes(password);
        var found = Find(IntPtr.Zero, (uint)service.Length, service,
            (uint)account.Length, account, out _, out var oldData, out var item);
        if (oldData != IntPtr.Zero) FreeContent(IntPtr.Zero, oldData);
        try
        {
            int status;
            if (found == NotFound)
            {
                status = Add(IntPtr.Zero, (uint)service.Length, service,
                    (uint)account.Length, account, (uint)bytes.Length, bytes, out var created);
                if (created != IntPtr.Zero) Release(created);
            }
            else if (found == 0)
                status = Modify(item, IntPtr.Zero, (uint)bytes.Length, bytes);
            else
                throw new InvalidOperationException($"macOS 钥匙串查询失败 (OSStatus={found})");

            if (status != 0) throw new InvalidOperationException($"macOS 钥匙串保存失败 (OSStatus={status})");
        }
        finally
        {
            if (item != IntPtr.Zero) Release(item);
            Array.Clear(bytes);
        }
    }

    public static void Remove(string name)
    {
        var (service, account) = Keys(name);
        var found = Find(IntPtr.Zero, (uint)service.Length, service,
            (uint)account.Length, account, out _, out var oldData, out var item);
        if (oldData != IntPtr.Zero) FreeContent(IntPtr.Zero, oldData);
        try
        {
            if (found == NotFound) return;
            if (found != 0) throw new InvalidOperationException($"macOS 钥匙串查询失败 (OSStatus={found})");
            var status = Delete(item);
            if (status != 0) throw new InvalidOperationException($"macOS 钥匙串删除失败 (OSStatus={status})");
        }
        finally
        {
            if (item != IntPtr.Zero) Release(item);
        }
    }
}
