using System;

namespace Xoderony.Collections;

public ref struct SpanIntSet {

    // 空槽哨兵；不可存入。用 -1 以便能存 0（含 null 的 EqualityComparer hash）。
    public const int Empty = -1;

    private Span<int> _slots;

    private int _count = 0;

    private int _capacity;

    // buffer 长度不必为 2 的幂：实际使用不超过其长度的最大 2 的幂前缀，多出的槽位忽略。
    // Capacity 为可用长度的一半（负载因子 0.5）。需要精确容量、避免截断浪费时调用 GetBufferLengthForCapacity。
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

    public readonly Span<int> Slots => _slots;

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

    public AddStatus Add(int item) {
        if (item == Empty) {
            throw new ArgumentException("Item must not be Empty (-1).", nameof(item));
        }
        if (IsFull) {
            return AddStatus.Full;
        }
        var slots = _slots;
        var maxProbeCount = _count + 1;
        var mask = slots.Length - 1;
        var index = GetStartIndexForItem(item, mask);
        for (var probeCount = 0; probeCount < maxProbeCount; probeCount++) {
            ref var slot = ref slots[index];
            if (slot == Empty) {
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
