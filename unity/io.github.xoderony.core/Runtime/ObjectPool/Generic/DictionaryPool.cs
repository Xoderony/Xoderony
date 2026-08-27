using System.Collections.Generic;

namespace Xoderony.ObjectPool.Generic;

/// <summary>Dictionary 池：归还时自动清空元素。</summary>
public class DictionaryPool<TKey, TValue>(int capacity = 16) : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>(capacity) where TKey : notnull;
