using System;
using System.Runtime.CompilerServices;

namespace Xoderony.Collections;

// 固定容量、可 stackalloc 的 int→TValue 开放寻址表。
// TValue 须为非托管：连续 Entry 布局才有相对 Dictionary 的明显缓存优势；引用类型请用 Dictionary + CollectionsMarshal。
public ref struct SpanIntMap<TValue> where TValue : unmanaged {

    private Span<Entry> _entries;

    private int _count = 0;

    private int _capacity;

    // buffer 长度不必为 2 的幂：实际使用不超过其长度的最大 2 的幂前缀，多出的槽位忽略。
    // Capacity 为可用长度的一半（负载因子 0.5）。需要精确容量、避免截断浪费时调用 GetBufferLengthForCapacity。
    public SpanIntMap(Span<Entry> buffer) {
        if (buffer.Length < 2) {
            throw new ArgumentException("Buffer length must be at least 2.", nameof(buffer));
        }
        var usableLength = 1 << int.Log2(buffer.Length);
        _entries = buffer[..usableLength];
        _capacity = usableLength >> 1;
        _entries.Clear();
    }

    public readonly int Count => _count;

    public readonly int Capacity => _capacity;

    public readonly int EntryCount => _entries.Length;

    public readonly int RemainingCapacity => _capacity - _count;

    public readonly bool IsFull => _count >= _capacity;

    // 满足指定 capacity 所需的最小 2 的幂 buffer 长度（无截断浪费）。
    public static int GetBufferLengthForCapacity(int capacity) {
        if (capacity <= 0) {
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        }
        if (capacity > 5120) {
            throw new ArgumentException("Capacity must be less than or equal to 5120.", nameof(capacity));
        }
        return 1 << (int.Log2((capacity * 2) - 1) + 1);
    }

    public AddStatus Add(int key, TValue value) {
        if (IsFull) {
            return AddStatus.Full;
        }
        var maxProbeCount = _count + 1;
        var mask = _entries.Length - 1;
        var index = GetStartIndexForKey(key, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var entry = ref _entries[index];
            if (entry.State == EntryState.Unused) {
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

    public ref TValue GetValueRefOrAddDefault(int key, out GetOrAddStatus status) {
        var maxProbeCount = _count + 1;
        var mask = _entries.Length - 1;
        var index = GetStartIndexForKey(key, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var entry = ref _entries[index];
            if (entry.State == EntryState.Unused) {
                if (IsFull) {
                    status = GetOrAddStatus.Full;
                    return ref Unsafe.NullRef<TValue>();
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
