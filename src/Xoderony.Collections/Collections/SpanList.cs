using System;
using System.Collections.Generic;

namespace Xoderony.Collections;

public ref struct SpanList<T> {

    private readonly Span<T> _buffer;

    private readonly EqualityComparerDelegate<T>? _equalityComparer;

    private int _count;

    public SpanList(Span<T> span, int count = 0, EqualityComparerDelegate<T>? equalityComparer = null) {
        if ((uint)count > (uint)span.Length) {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        _buffer = span;
        _count = count;
        _equalityComparer = equalityComparer;
    }

    public readonly ref T this[int index] {
        get {
            if ((uint)index >= (uint)_count) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return ref _buffer[index];
        }
    }

    public readonly int Count => _count;

    public readonly int Capacity => _buffer.Length;

    public readonly int RemainingCapacity => _buffer.Length - _count;

    public readonly bool IsFull => _count >= _buffer.Length;

    public readonly EqualityComparerDelegate<T>? EqualityComparer => _equalityComparer;

    public readonly Span<T> Buffer => _buffer;

    public readonly Span<T> Span => _buffer[.._count];

    public readonly Span<T>.Enumerator GetEnumerator() {
        return Span.GetEnumerator();
    }

    public readonly T[] ToArray() {
        return Span.ToArray();
    }

    public void Clear() {
        _count = 0;
        _buffer.Clear();
    }

    public bool Add(T item) {
        if (IsFull) {
            return false;
        }
        _buffer[_count] = item;
        _count++;
        return true;
    }

    public bool AddRange(ReadOnlySpan<T> items) {
        if (items.Length > RemainingCapacity) {
            return false;
        }
        var destination = _buffer.Slice(_count, items.Length);
        items.CopyTo(destination);
        _count += items.Length;
        return true;
    }

    public bool Insert(int index, T item) {
        if ((uint)index > (uint)_count || IsFull) {
            return false;
        }
        var countToMove = _count - index;
        var buffer = _buffer;
        if (countToMove > 0) {
            var source = buffer.Slice(index, countToMove);
            var destination = buffer.Slice(index + 1, countToMove);
            source.CopyTo(destination);
        }
        buffer[index] = item;
        _count++;
        return true;
    }

    public bool InsertUnordered(int index, T item) {
        if ((uint)index > (uint)_count || IsFull) {
            return false;
        }
        if (index < _count) {
            _buffer[_count] = _buffer[index];
        }
        _buffer[index] = item;
        _count++;
        return true;
    }

    public bool Remove(T item) {
        return RemoveAt(IndexOf(item));
    }

    public bool RemoveAt(int index) {
        if ((uint)index >= (uint)_count) {
            return false;
        }
        var span = Span;
        var countToMove = _count - index - 1;
        if (countToMove > 0) {
            var source = span.Slice(index + 1, countToMove);
            var destination = span.Slice(index, countToMove);
            source.CopyTo(destination);
        }
        _count--;
        _buffer[_count] = default!;
        return true;
    }

    public bool RemoveUnordered(T item) {
        return RemoveAtUnordered(IndexOf(item));
    }

    public bool RemoveAtUnordered(int index) {
        if ((uint)index >= (uint)_count) {
            return false;
        }
        var lastIndex = _count - 1;
        if (index != lastIndex) {
            _buffer[index] = _buffer[lastIndex];
        }
        _count--;
        _buffer[_count] = default!;
        return true;
    }

    public int RemoveAll(Predicate<T> match) {
        ArgumentNullException.ThrowIfNull(match);
        var span = Span;
        var firstIndex = 0;
        for (; firstIndex < span.Length; firstIndex++) {
            if (match(span[firstIndex])) {
                break;
            }
        }
        if (firstIndex == span.Length) {
            return 0;
        }
        var write = firstIndex;
        for (var read = firstIndex + 1; read < span.Length; read++) {
            if (!match(span[read])) {
                span[write++] = span[read];
            }
        }
        var removedCount = span.Length - write;
        span[write..].Clear();
        _count = write;
        return removedCount;
    }

    public readonly int IndexOf(T item) {
        var span = Span;
        if (_equalityComparer is not null) {
            for (var i = 0; i < span.Length; i++) {
                if (_equalityComparer(span[i], item)) {
                    return i;
                }
            }
        } else {
            var comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < span.Length; i++) {
                if (comparer.Equals(span[i], item)) {
                    return i;
                }
            }
        }
        return -1;
    }

    public readonly bool Contains(T item) {
        return IndexOf(item) != -1;
    }

    public static implicit operator SpanList<T>(Span<T> span) {
        return new SpanList<T>(span);
    }

    public static implicit operator Span<T>(SpanList<T> spanList) {
        return spanList._buffer[..spanList._count];
    }
}

public delegate bool EqualityComparerDelegate<T>(T a, T b);
