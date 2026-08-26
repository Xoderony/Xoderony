using System;
using System.Runtime.CompilerServices;

namespace Xoderony.ObjectPool;

/// <summary>对象归还作用域：Dispose 时把对象归还池并置空引用，防止重复归还。</summary>
/// <remarks>ref struct 只能存在于栈上，编译器禁止存入字段、容器或被捕获；同一作用域内显式复制仍会复制，复制后分别 Dispose 会导致重复归还。</remarks>
public ref struct PooledObjectScope<T>(IPool<T> pool, T buffer) : IDisposable where T : class {
    private readonly IPool<T> _pool = pool;
    private T? _buffer = buffer;

    /// <summary>当前持有的对象；归还后为 null。</summary>
    public readonly T? Buffer {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _buffer;
    }

    /// <summary>把对象归还池并置空引用；重复调用无副作用。</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() {
        if (_buffer is null) {
            return;
        }
        _pool.Return(_buffer);
        _buffer = null;
    }
}
