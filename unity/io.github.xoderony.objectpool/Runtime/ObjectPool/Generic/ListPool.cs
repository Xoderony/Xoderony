using System.Collections.Generic;

namespace Xoderony.ObjectPool.Generic;

/// <summary>List 池：归还时自动清空元素。</summary>
public class ListPool<T>(int capacity = 16) : CollectionPool<List<T>, T>(capacity);
