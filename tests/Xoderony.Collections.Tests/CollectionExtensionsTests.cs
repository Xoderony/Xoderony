using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xoderony.Extensions;
using Xunit;

namespace Xoderony.Tests;

public class CollectionExtensionsTests {

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
}
