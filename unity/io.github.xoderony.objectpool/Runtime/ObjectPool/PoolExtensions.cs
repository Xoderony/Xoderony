namespace Xoderony.ObjectPool;

public static class PoolExtensions {

    /// <summary>租借对象并返回自动归还作用域；作用域结束后调用方不得继续使用 <paramref name="value"/>。</summary>
    public static PooledObjectScope<T> Rent<T>(this IPool<T> pool, out T value) where T : class {
        value = pool.Rent();
        return new PooledObjectScope<T>(pool, value);
    }
}
