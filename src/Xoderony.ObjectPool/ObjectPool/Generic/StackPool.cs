using System.Collections.Generic;
using System.Diagnostics;

namespace Xoderony.ObjectPool.Generic;

/// <summary>Stack 池：归还时自动清空元素。</summary>
public class StackPool<T>(int capacity = 16) : IPool<Stack<T>> {
    private readonly Stack<Stack<T>> _pool = new(capacity);

    public static readonly StackPool<T> Shared = new();

    public int Capacity => capacity;

    /// <summary>租借一个空 Stack；池空时新建。</summary>
    public Stack<T> Rent() {
        return _pool.TryPop(out var result) ? result : new();
    }

    /// <summary>清空并归还 Stack；池满时直接丢弃。</summary>
    /// <param name="value">待归还的 Stack，归还后调用方不得继续使用。</param>
    public void Return(Stack<T> value) {
        Debug.Assert(value is not null);
        if (_pool.Count >= capacity) {
            return;
        }
        value.Clear();
        _pool.Push(value);
    }

    public void Clear() {
        _pool.Clear();
    }
}
