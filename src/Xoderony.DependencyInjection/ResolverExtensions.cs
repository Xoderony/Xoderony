using System;
using System.Diagnostics.CodeAnalysis;
using Xoderony.DependencyInjection.Internal;

namespace Xoderony.DependencyInjection;

/// <summary>枚举键仅按数值区分，必须以 int 为底层类型；0 表示默认键。</summary>
public static class ResolverExtensions {
    public static T Resolve<T, TKey>(this IResolver resolver, TKey key) where T : class where TKey : struct, Enum {
        return resolver.Resolve<T>(EnumKey<TKey>.ToInt32(key));
    }

    public static object Resolve<TKey>(this IResolver resolver, Type serviceType, TKey key) where TKey : struct, Enum {
        return resolver.Resolve(serviceType, EnumKey<TKey>.ToInt32(key));
    }

    /// <summary>仅未注册时返回 false；键类型不支持、多个候选或创建失败仍抛出异常。</summary>
    public static bool TryResolve<T, TKey>(this IResolver resolver, [NotNullWhen(true)] out T? instance, TKey key) where T : class where TKey : struct, Enum {
        return resolver.TryResolve(out instance, EnumKey<TKey>.ToInt32(key));
    }

    /// <summary>只检查解析入口，不创建对象；多个候选时仍为 true，不保证解析成功。</summary>
    public static bool IsRegistered<T, TKey>(this IResolver resolver, TKey key) where T : class where TKey : struct, Enum {
        return resolver.IsRegistered<T>(EnumKey<TKey>.ToInt32(key));
    }
}
