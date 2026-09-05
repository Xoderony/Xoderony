using System;
#if NET10_0_OR_GREATER
using System.Runtime.CompilerServices;
#else
using System.Runtime.InteropServices;
#endif

namespace Xoderony.Collections;

/// <summary>基于连续缓冲区的固定容量 int 到值开放寻址表，可使用 stackalloc 分配存储。</summary>
/// <remarks><typeparamref name="TValue"/> 必须为非托管类型；引用类型应使用 <c>Dictionary</c> 与 <c>CollectionsMarshal</c>。</remarks>
/// <typeparam name="TValue">非托管值类型。</typeparam>
public ref struct SpanIntMap<TValue> where TValue : unmanaged {

    private Span<Entry> _entries;

    private int _count = 0;

    private int _capacity;

    /// <summary>使用调用方提供的缓冲区创建映射。</summary>
    /// <remarks>仅使用不超过缓冲区长度的最大二次幂前缀，并以 0.5 负载因子确定容量；其余槽位不会使用。</remarks>
    /// <param name="buffer">用于存储条目的缓冲区，长度至少为 2。</param>
    public SpanIntMap(Span<Entry> buffer) {
        if (buffer.Length < 2) {
            throw new ArgumentException("Buffer length must be at least 2.", nameof(buffer));
        }
#if NET10_0_OR_GREATER
        var usableLength = 1 << int.Log2(buffer.Length);
#else
        var usableLength = 2;
        while (usableLength <= (buffer.Length >> 1)) {
            usableLength <<= 1;
        }
#endif
        _entries = buffer[..usableLength];
        _capacity = usableLength >> 1;
        _entries.Clear();
    }

    public readonly int Count => _count;

    public readonly int Capacity => _capacity;

    public readonly int EntryCount => _entries.Length;

    public readonly int RemainingCapacity => _capacity - _count;

    public readonly bool IsFull => _count >= _capacity;

    /// <summary>计算容纳指定元素数量所需的最小二次幂缓冲区长度。</summary>
    /// <param name="capacity">所需元素容量。</param>
    /// <returns>应分配的条目数量。</returns>
    public static int GetBufferLengthForCapacity(int capacity) {
        if (capacity <= 0) {
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        }
        if (capacity > 5120) {
            throw new ArgumentException("Capacity must be less than or equal to 5120.", nameof(capacity));
        }
#if NET10_0_OR_GREATER
        return 1 << (int.Log2((capacity * 2) - 1) + 1);
#else
        var bufferLength = 2;
        while (bufferLength < (capacity * 2)) {
            bufferLength <<= 1;
        }
        return bufferLength;
#endif
    }

    public AddStatus Add(int key, TValue value) {
        var maxProbeCount = _count + 1;
        var mask = _entries.Length - 1;
        var index = GetStartIndexForKey(key, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var entry = ref _entries[index];
            if (entry.State == EntryState.Unused) {
                if (IsFull) {
                    return AddStatus.Full;
                }
                entry.Key = key;
                entry.Value = value;
                entry.State = EntryState.Used;
                _count++;
                return AddStatus.Added;
            }
            if (entry.Key == key) {
                return AddStatus.Duplicate;
            }
            index = (index + 1) & mask;
        }
        throw new InvalidOperationException("Probe count exceeded expected search range.");
    }

    public SetStatus Set(int key, TValue value) {
        var maxProbeCount = _count + 1;
        var mask = _entries.Length - 1;
        var index = GetStartIndexForKey(key, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var entry = ref _entries[index];
            if (entry.State == EntryState.Unused) {
                if (IsFull) {
                    return SetStatus.Full;
                }
                entry.Key = key;
                entry.Value = value;
                entry.State = EntryState.Used;
                _count++;
                return SetStatus.Added;
            }
            if (entry.Key == key) {
                entry.Value = value;
                return SetStatus.Updated;
            }
            index = (index + 1) & mask;
        }
        throw new InvalidOperationException("Probe count exceeded expected search range.");
    }

    public readonly bool ContainsKey(int key) {
        var maxProbeCount = _count + 1;
        var mask = _entries.Length - 1;
        var index = GetStartIndexForKey(key, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var entry = ref _entries[index];
            if (entry.State == EntryState.Unused) {
                return false;
            }
            if (entry.Key == key) {
                return true;
            }
            index = (index + 1) & mask;
        }
        throw new InvalidOperationException("Probe count exceeded expected search range.");
    }

    public readonly bool TryGetValue(int key, out TValue value) {
        var maxProbeCount = _count + 1;
        var mask = _entries.Length - 1;
        var index = GetStartIndexForKey(key, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var entry = ref _entries[index];
            if (entry.State == EntryState.Unused) {
                value = default;
                return false;
            }
            if (entry.Key == key) {
                value = entry.Value;
                return true;
            }
            index = (index + 1) & mask;
        }
        throw new InvalidOperationException("Probe count exceeded expected search range.");
    }

    /// <summary>获取现有值的引用，或在容量允许时添加默认值并返回其引用。</summary>
    /// <param name="key">要查找或添加的键。</param>
    /// <param name="status">指示键已存在、已添加，或映射已满。</param>
    /// <returns>状态为 <see cref="GetOrAddStatus.Found"/> 或 <see cref="GetOrAddStatus.Added"/> 时返回有效的可写引用；状态为 <see cref="GetOrAddStatus.Full"/> 时返回不得解引用的 null ref。</returns>
    public ref TValue GetValueRefOrAddDefaultOrNullRef(int key, out GetOrAddStatus status) {
        var maxProbeCount = _count + 1;
        var mask = _entries.Length - 1;
        var index = GetStartIndexForKey(key, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var entry = ref _entries[index];
            if (entry.State == EntryState.Unused) {
                if (IsFull) {
                    status = GetOrAddStatus.Full;
#if NET10_0_OR_GREATER
                    return ref Unsafe.NullRef<TValue>();
#else
                    return ref MemoryMarshal.GetReference(Span<TValue>.Empty);
#endif
                }
                entry.Key = key;
                entry.Value = default;
                entry.State = EntryState.Used;
                _count++;
                status = GetOrAddStatus.Added;
                return ref entry.Value;
            }
            if (entry.Key == key) {
                status = GetOrAddStatus.Found;
                return ref entry.Value;
            }
            index = (index + 1) & mask;
        }
        throw new InvalidOperationException("Probe count exceeded expected search range.");
    }

    public void Clear() {
        _count = 0;
        _entries.Clear();
    }

    public readonly Enumerator GetEnumerator() {
        return new Enumerator(_entries);
    }

    private static int GetStartIndexForKey(int key, int mask) {
        var hash = (uint)key;
        hash ^= hash >> 16;
        hash *= 0x7feb352d;
        hash ^= hash >> 15;
        return ((int)hash) & mask;
    }

    public struct Entry {

        public int Key;

        public TValue Value;

        internal EntryState State;

    }

    public ref struct Enumerator {

        private readonly Span<Entry> _entries;

        private int _index;

        internal Enumerator(Span<Entry> entries) {
            _entries = entries;
            _index = -1;
        }

        public readonly ref readonly Entry Current => ref _entries[_index];

        public bool MoveNext() {
            for (_index++; _index < _entries.Length; _index++) {
                if (_entries[_index].State == EntryState.Used) {
                    return true;
                }
            }
            return false;
        }
    }
}
