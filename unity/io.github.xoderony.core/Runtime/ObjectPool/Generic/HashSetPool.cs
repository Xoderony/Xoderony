using System.Collections.Generic;

namespace Xoderony.ObjectPool.Generic;

/// <summary>HashSet 池：归还时自动清空元素。</summary>
public class HashSetPool<T>(int capacity = 16) : CollectionPool<HashSet<T>, T>(capacity);
