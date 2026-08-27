using System.Collections.Generic;
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
