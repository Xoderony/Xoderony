using System;
using System.Diagnostics.CodeAnalysis;

namespace Xoderony.ObjectPool;

/// <summary>对象归还作用域：Dispose 时归还池，并把调用方持有的引用置空，防止重复归还与归还后误用。</summary>
/// <remarks>ref struct 只能存在于栈上；通过 ref 字段别名调用方局部变量，拷贝后仍指向同一引用。</remarks>
public ref struct PooledObjectScope<T> : IDisposable where T : class {
    private readonly IPool<T> _pool;
    private ref T _value;

    public PooledObjectScope(IPool<T> pool, [UnscopedRef] ref T value) {
        _pool = pool;
        _value = ref value;
    }

    /// <summary>把对象归还池并置空调用方引用；重复调用无副作用。</summary>
    public void Dispose() {
        if (_value is null) {
            return;
        }
        _pool.Return(_value);
        _value = null!;
    }
}
