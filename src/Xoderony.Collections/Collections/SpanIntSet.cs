using System;

namespace Xoderony.Collections;

public ref struct SpanIntSet {

    /// <summary>表示空槽的哨兵值；不能作为集合元素存储。</summary>
    public const int Empty = -1;

    private Span<int> _slots;

    private int _count = 0;

    private int _capacity;

    /// <summary>使用调用方提供的缓冲区创建集合。</summary>
    /// <remarks>仅使用不超过缓冲区长度的最大二次幂前缀，并以 0.5 负载因子确定容量；其余槽位不会使用。</remarks>
    /// <param name="buffer">用于存储元素的缓冲区，长度至少为 2。</param>
    public SpanIntSet(Span<int> buffer) {
        if (buffer.Length < 2) {
            throw new ArgumentException("Buffer length must be at least 2.", nameof(buffer));
        }
        var usableLength = 1 << int.Log2(buffer.Length);
        _slots = buffer[..usableLength];
        _capacity = usableLength >> 1;
        _slots.Fill(Empty);
    }

    public readonly int Count => _count;

    public readonly int Capacity => _capacity;

    public readonly int RemainingCapacity => _capacity - _count;

    public readonly int SlotCount => _slots.Length;

    public readonly bool IsFull => _count >= _capacity;

    public readonly ReadOnlySpan<int> Slots => _slots;

    /// <summary>计算容纳指定元素数量所需的最小二次幂缓冲区长度。</summary>
    /// <param name="capacity">所需元素容量。</param>
    /// <returns>应分配的槽位数量。</returns>
    public static int GetBufferLengthForCapacity(int capacity) {
        if (capacity <= 0) {
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        }
        if (capacity > 5120) {
            throw new ArgumentException("Capacity must be less than or equal to 5120.", nameof(capacity));
        }
        return 1 << (int.Log2((capacity * 2) - 1) + 1);
    }

    public AddStatus Add(int item) {
        if (item == Empty) {
            throw new ArgumentException("Item must not be Empty (-1).", nameof(item));
        }
        var slots = _slots;
        var maxProbeCount = _count + 1;
        var mask = slots.Length - 1;
        var index = GetStartIndexForItem(item, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var slot = ref slots[index];
            if (slot == Empty) {
                if (IsFull) {
                    return AddStatus.Full;
                }
                slot = item;
                _count++;
                return AddStatus.Added;
            }
            if (slot == item) {
                return AddStatus.Duplicate;
            }
            index = (index + 1) & mask;
        }
        throw new InvalidOperationException("Probe count exceeded expected search range.");
    }

    public readonly bool Contains(int item) {
        var slots = _slots;
        var maxProbeCount = _count + 1;
        var mask = slots.Length - 1;
        var index = GetStartIndexForItem(item, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            var slot = slots[index];
            if (slot == Empty) {
                return false;
            }
            if (slot == item) {
                return true;
            }
            index = (index + 1) & mask;
        }
        throw new InvalidOperationException("Probe count exceeded expected search range.");
    }

    public void Clear() {
        _count = 0;
        _slots.Fill(Empty);
    }

    public readonly Enumerator GetEnumerator() {
        return new Enumerator(_slots);
    }

    private static int GetStartIndexForItem(int item, int mask) {
        var hash = (uint)item;
        hash ^= hash >> 16;
        hash *= 0x7feb352d;
        hash ^= hash >> 15;
        return ((int)hash) & mask;
    }

    public ref struct Enumerator {

        private readonly Span<int> _slots;

        private int _index;

        internal Enumerator(Span<int> slots) {
            _slots = slots;
            _index = -1;
        }

        public readonly int Current => _slots[_index];

        public bool MoveNext() {
            for (_index++; _index < _slots.Length; _index++) {
                if (_slots[_index] != Empty) {
                    return true;
                }
            }
            return false;
        }
    }
}
