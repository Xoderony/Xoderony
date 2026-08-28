using System;

namespace Xoderony.ObjectPool;

/// <summary>对象归还作用域：Dispose 时把持有的对象归还池并清空内部引用。</summary>
/// <remarks>不得复制作用域；副本分别 Dispose 会重复归还同一对象。</remarks>
public ref struct PooledObjectScope<T> : IDisposable where T : class {
    private readonly IPool<T> _pool;
    private T? _value;

    public PooledObjectScope(IPool<T> pool, T value) {
        _pool = pool;
        _value = value;
    }

    /// <summary>把对象归还池并清空内部引用；重复调用无副作用。</summary>
    public void Dispose() {
        var value = _value;
        if (value is null) {
            return;
        }
        _pool.Return(value);
        _value = null;
    }
}
