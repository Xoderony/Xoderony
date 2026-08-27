using System.Collections.Generic;

namespace Xoderony.ObjectPool.Generic;

/// <summary>基于 <see cref="ICollection{TElement}"/> 的集合对象池；归还时清空后再缓存。</summary>
/// <typeparam name="TCollection">被池化的集合类型，需实现 <see cref="ICollection{TElement}"/> 且可无参构造。</typeparam>
/// <typeparam name="TElement">集合元素类型。</typeparam>
public class CollectionPool<TCollection, TElement>(int capacity = 16) : IPool<TCollection> where TCollection : class, ICollection<TElement>, new() {

    private readonly Stack<TCollection> _pool = new(capacity);

    public static readonly CollectionPool<TCollection, TElement> Shared = new();

    public int Capacity => capacity;

    /// <summary>租借一个空集合；池空时新建。</summary>
    public TCollection Rent() {
        return _pool.TryPop(out var result) ? result : new();
    }

    /// <summary>清空并归还集合；池满或值为 null 时直接丢弃。</summary>
    /// <param name="collection">待归还的集合，归还后调用方不得继续使用。</param>
    public void Return(TCollection collection) {
        if (collection is null || _pool.Count >= capacity) {
            return;
        }
        collection.Clear();
        _pool.Push(collection);
    }

    public void Clear() {
        _pool.Clear();
    }
}
