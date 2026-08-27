using System.Diagnostics.CodeAnalysis;

namespace Xoderony.ObjectPool;

public static class PoolExtensions {

    /// <summary>租借对象并返回自动归还作用域；Dispose 时会清空 <paramref name="value"/>。</summary>
    public static PooledObjectScope<T> Rent<T>(this IPool<T> pool, [UnscopedRef] out T value) where T : class {
        value = pool.Rent();
        return new PooledObjectScope<T>(pool, ref value);
    }
}
