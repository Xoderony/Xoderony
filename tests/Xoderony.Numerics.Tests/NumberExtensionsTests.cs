using Xoderony.Extensions;
using Xunit;

namespace Xoderony.Tests;

public class NumberExtensionsTests {

    [Fact]
    public void Clamp_LimitsRange() {
        Assert.Equal(0, (-3).Clamp(0, 10));
        Assert.Equal(10, 99.Clamp(0, 10));
        Assert.Equal(4, 4.Clamp(0, 10));
    }
}
