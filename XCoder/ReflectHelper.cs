using System.Reflection;

namespace XCoder;

/// <summary>反射辅助：静默扫描所有已加载程序集中的类型，避免 AssemblyX 对加载失败程序集产生日志噪音</summary>
static class ReflectHelper
{
    /// <summary>在所有已加载程序集中查找指定基类或接口的实现类型（静默容忍类型加载失败）</summary>
    /// <param name="baseType">基类或接口</param>
    /// <returns>实现类型集合</returns>
    /// <remarks>
    /// 替代 <see cref="NewLife.Reflection.Reflect.GetAllSubclasses"/>：后者在扫描到含坏类型的程序集
    /// （如旧版 System.Data.SqlClient 的 SqlGuidCaster 在 .NET 8 下无法加载）时会向 XTrace 打印异常堆栈，
    /// 污染网络调试/地图接口等工具的日志区。本方法同样返回可加载部分类型，但不产生日志噪音。
    /// </remarks>
    public static IEnumerable<Type> GetSubclasses(Type baseType)
    {
        if (baseType == null) throw new ArgumentNullException(nameof(baseType));

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 某些程序集含 .NET 8 下无法加载的类型，取可加载部分继续
                types = ex.Types;
            }
            catch { continue; }
            if (types == null || types.Length <= 0) continue;

            foreach (var type in types)
            {
                if (type == null) continue;
                if (type.IsInterface || type.IsAbstract || type.IsGenericType || type == baseType) continue;
                if (!baseType.IsAssignableFrom(type)) continue;

                yield return type;
            }
        }
    }
}
