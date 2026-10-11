using System;
using Xunit;

namespace Xoderony.DependencyInjection.Tests;

public sealed class InvalidInputTests {
    [Fact]
    public void NullFactoryCannotFallBackToTypeConstruction() {
        var builder = new ContainerBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.RegisterFactory<Service>(null!));
        using var container = builder.Build();
        Assert.False(container.IsRegistered<Service>());
        Assert.Throws<ArgumentNullException>(() => builder.AddBuildCallback(null!));
    }

    [Fact]
    public void NullFactoryResultIsAFailureAndIsCached() {
        var calls = 0;
        var builder = new ContainerBuilder();
        builder.RegisterFactory<Service>(_ => { calls++; return null!; });
        using var container = builder.Build();
        Assert.Throws<InvalidOperationException>(() => container.Resolve<Service>());
        Assert.Throws<InvalidOperationException>(() => container.TryResolve<Service>(out _));
        Assert.Throws<InvalidOperationException>(() => ((IServiceProvider)container).GetService(typeof(Service)));
        Assert.Equal(1, calls);
    }

    [Fact]
    public void UnsupportedServiceTypesBehaveAsMissing() {
        using var container = new ContainerBuilder().Build();
        Type[] types = [typeof(System.Collections.Generic.List<>), typeof(int).MakeByRefType(), typeof(int).MakePointerType(), typeof(Span<int>), typeof(void)];
        foreach (var type in types) {
            Assert.Throws<InvalidOperationException>(() => container.Resolve(type));
            Assert.Null(((IServiceProvider)container).GetService(type));
        }
    }

    private sealed class Service;
}
