using System;
using System.Collections;
#if NET10_0_OR_GREATER
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endif

namespace Xoderony.Extensions;

public static class CollectionExtensions {

#if NET10_0_OR_GREATER
    extension<T>(T[]? array) {

        public bool IsNullOrEmpty => (array is null) || (array.Length == 0);

    }

    extension<T>(T? collection) where T : ICollection {

        public bool IsNullOrEmpty => (collection is null) || (collection.Count == 0);

    }
#else
    public static bool IsNullOrEmpty<T>(this T[]? array) {
        return (array is null) || (array.Length == 0);
    }

    public static bool IsNullOrEmpty<T>(this T? collection) where T : ICollection {
        return (collection is null) || (collection.Count == 0);
    }
#endif

#if NET10_0_OR_GREATER
    extension<T>(List<T>? list) {

        public Span<T> AsSpan() {
            return CollectionsMarshal.AsSpan(list);
        }
    }

    extension<T>(List<T> list) {

        public void SetCount(int count) {
            CollectionsMarshal.SetCount(list, count);
        }

        public Span<T> AddSpan(int count) {
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            var index = list.Count;
            list.SetCount(index + count);
            return list.AsSpan().Slice(index, count);
        }

        public Span<T> InsertSpan(int index, int count) {
            var oldCount = list.Count;
            if ((uint)index > (uint)oldCount) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            list.SetCount(oldCount + count);
            var span = list.AsSpan();
            var countToMove = oldCount - index;
            if (countToMove > 0) {
                var source = span.Slice(index, countToMove);
                var destination = span.Slice(index + count, countToMove);
                source.CopyTo(destination);
            }
            return span.Slice(index, count);
        }
    }

    extension(BitArray? array) {

        public Span<byte> AsBytes() {
            return CollectionsMarshal.AsBytes(array);
        }
    }

    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull {

        public ref TValue GetValueRefOrNullRef(TKey key) {
            return ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        }

        public ref TValue? GetValueRefOrAddDefault(TKey key, out bool exists) {
            return ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out exists);
        }
    }

    extension<TKey, TValue, TAlternateKey>(Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> dictionary) where TKey : notnull where TAlternateKey : notnull, allows ref struct {

        public ref TValue GetValueRefOrNullRef(TAlternateKey key) {
            return ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        }

        public ref TValue? GetValueRefOrAddDefault(TAlternateKey key, out bool exists) {
            return ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out exists);
        }
    }
#endif
}
