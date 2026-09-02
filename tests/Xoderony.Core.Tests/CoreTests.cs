using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xoderony.Extensions;
using Xoderony.ObjectPool;
using Xoderony.ObjectPool.Generic;
using Xunit;

namespace Xoderony.Tests;

public class CoreTests {

    [Fact]
    public void Clamp_LimitsRange() {
        Assert.Equal(0, (-3).Clamp(0, 10));
        Assert.Equal(10, 99.Clamp(0, 10));
        Assert.Equal(4, 4.Clamp(0, 10));
    }

    [Fact]
    public void IsNullOrEmpty_CoversNullAndEmptyArrays() {
        int[]? missing = null;
        Assert.True(missing.IsNullOrEmpty);
        Assert.True(System.Array.Empty<int>().IsNullOrEmpty);
        Assert.False(new[] { 1 }.IsNullOrEmpty);
    }

    [Fact]
    public void List_AsSpan_ViewsUnderlyingStorage() {
        var list = new List<int> { 1, 2, 3 };
        var span = list.AsSpan();
        Assert.Equal(3, span.Length);
        span[1] = 20;
        Assert.Equal(20, list[1]);
    }

    [Fact]
    public void List_SetCount_Truncates() {
        var list = new List<int> { 1, 2, 3 };
        list.SetCount(2);
        Assert.Equal(2, list.Count);
        Assert.Equal(new[] { 1, 2 }, list);
    }

    [Fact]
    public void List_AddSpan_AppendsWritableSlots() {
        var list = new List<int> { 1 };
        var span = list.AddSpan(2);
        span[0] = 2;
        span[1] = 3;
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void List_InsertSpan_ShiftsExistingItems() {
        var list = new List<int> { 1, 4 };
        var span = list.InsertSpan(1, 2);
        span[0] = 2;
        span[1] = 3;
        Assert.Equal(new[] { 1, 2, 3, 4 }, list);
    }

    [Fact]
    public void BitArray_AsBytes_ViewsBackingStorage() {
        var bits = new BitArray(8);
        bits[0] = true;
        var bytes = bits.AsBytes();
        Assert.Equal(1, bytes.Length);
        Assert.Equal(1, bytes[0]);
    }

    [Fact]
    public void Dictionary_GetValueRefOrNullRef_ReturnsExistingOrNullRef() {
        var dictionary = new Dictionary<int, string> { [1] = "a" };
        ref var found = ref dictionary.GetValueRefOrNullRef(1);
        Assert.False(Unsafe.IsNullRef(ref found));
        found = "b";
        Assert.Equal("b", dictionary[1]);

        ref var missing = ref dictionary.GetValueRefOrNullRef(2);
        Assert.True(Unsafe.IsNullRef(ref missing));
    }

    [Fact]
    public void Dictionary_GetValueRefOrAddDefault_AddsDefaultAndReturnsExisting() {
        var dictionary = new Dictionary<int, string>();
        ref var added = ref dictionary.GetValueRefOrAddDefault(1, out var addedExists);
        Assert.False(addedExists);
        added = "a";

        ref var existing = ref dictionary.GetValueRefOrAddDefault(1, out var existingExists);
        Assert.True(existingExists);
        Assert.Equal("a", existing);
        existing = "b";
        Assert.Equal("b", dictionary[1]);
    }

    [Fact]
    public void Span_AsBytesAndCast_ReinterpretStorage() {
        Span<int> values = [1, 2];
        var bytes = values.AsBytes();
        Assert.Equal(8, bytes.Length);
        var roundTrip = bytes.Cast<int>();
        roundTrip[1] = 3;
        Assert.Equal(3, values[1]);
    }

    [Fact]
    public void Span_ReadWrite_RoundTripsValue() {
        Span<byte> buffer = stackalloc byte[4];
        buffer.Write(0x01020304);
        Assert.Equal(0x01020304, ((ReadOnlySpan<byte>)buffer).Read<int>());
    }

    [Fact]
    public void ListPool_ReusesInstanceAfterDispose() {
        List<int> first;
        List<int> rented;
        using (ListPool<int>.Shared.Rent(out first)) {
            rented = first;
            first.Add(1);
        }
        Assert.Null(first);

        List<int> second;
        using (ListPool<int>.Shared.Rent(out second)) {
            Assert.Same(rented, second);
            Assert.Empty(second);
        }
    }

    [Fact]
    public void CollectionPool_HonorsCapacity() {
        var pool = new CollectionPool<List<object>, object>(capacity: 1);
        var a = new List<object>();
        var b = new List<object>();

        pool.Return(a);
        pool.Return(b);
        Assert.Same(a, pool.Rent());
    }

    [Fact]
    public void CustomPool_RentScope_ReturnsToSamePool() {
        var pool = new ListPool<int>(capacity: 2);
        List<int> first;
        List<int> rented;
        using (pool.Rent(out first)) {
            rented = first;
            first.Add(1);
        }
        Assert.Null(first);

        List<int> second;
        using (pool.Rent(out second)) {
            Assert.Same(rented, second);
            Assert.Empty(second);
        }
    }
}
