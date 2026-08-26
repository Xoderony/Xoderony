using System.Collections.Generic;
using Xoderony.Extensions;
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
    public void IsNullOrEmpty_CoversNullAndEmpty() {
        string? missing = null;
        Assert.True(missing.IsNullOrEmpty());
        Assert.True("".IsNullOrEmpty());
        Assert.False("x".IsNullOrEmpty());
        ICollection<int>? missingItems = null;
        Assert.True(missingItems.IsNullOrEmpty());
        Assert.True(System.Array.Empty<int>().IsNullOrEmpty());
    }

    [Fact]
    public void ListPool_ReusesInstanceAfterDispose() {
        List<int> first;
        using (ListPool<int>.Rent(out first)) {
            first.Add(1);
        }

        List<int> second;
        using (ListPool<int>.Rent(out second)) {
            Assert.Same(first, second);
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
}
