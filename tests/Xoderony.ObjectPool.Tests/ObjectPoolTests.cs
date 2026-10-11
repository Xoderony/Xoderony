using System;
using System.Collections.Generic;
using Xoderony.ObjectPool;
using Xoderony.ObjectPool.Generic;
using Xunit;

namespace Xoderony.Tests;

public class ObjectPoolTests {

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void NullReturnsCannotSilentlySucceed(int capacity) {
        Assert.Throws<ArgumentNullException>(() => new CollectionPool<List<int>, int>(capacity).Return(null!));
        Assert.Throws<ArgumentNullException>(() => new StackPool<int>(capacity).Return(null!));
        Assert.Throws<ArgumentNullException>(() => new QueuePool<int>(capacity).Return(null!));
    }

    [Fact]
    public void ListPool_ReusesInstanceAfterDispose() {
        List<int> first;
        List<int> rented;
        using (ListPool<int>.Shared.Rent(out first)) {
            rented = first;
            first.Add(1);
        }

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

        List<int> second;
        using (pool.Rent(out second)) {
            Assert.Same(rented, second);
            Assert.Empty(second);
        }
    }
}
