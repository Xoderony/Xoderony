using System;
using Xoderony.Extensions;
using Xunit;

namespace Xoderony.Tests;

public class NumberExtensionsTests {

    [Theory]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    public void MoveTowardsRejectsInvalidDelta(double delta) {
        Assert.Throws<ArgumentOutOfRangeException>(() => 0.0.MoveTowards(10.0, delta));
    }

    [Fact]
    public void MoveTowardsClampsTheStep() {
        Assert.Equal(2.0, 0.0.MoveTowards(10.0, 2.0));
        Assert.Equal(10.0, 9.0.MoveTowards(10.0, 2.0));
    }

    [Fact]
    public void Clamp_LimitsRange() {
        Assert.Equal(0, (-3).Clamp(0, 10));
        Assert.Equal(10, 99.Clamp(0, 10));
        Assert.Equal(4, 4.Clamp(0, 10));
    }
}
