using System;
using System.Collections.Generic;
using Xunit;

namespace Xoderony.DependencyInjection.Tests;

public sealed class EnumKeyTests {
    [Fact]
    public void EnumAndIntegerEntrypointsShareInstancesAndCollections() {
        var builder = new ContainerBuilder();
        builder.Register<Service>();
        builder.Register<Service>().WithKey(Keys.Named);
        using var container = builder.Build();
        IResolver resolver = container;
        Assert.Same(resolver.Resolve<Service>(), resolver.Resolve<Service, Keys>(Keys.Default));
        Assert.Same(resolver.Resolve<Service>(), resolver.Resolve<Service>(0));
        var named = resolver.Resolve<Service, Keys>(Keys.Named);
        Assert.NotSame(resolver.Resolve<Service>(), named);
        Assert.Same(named, resolver.Resolve<Service>(7));
        Assert.Same(named, resolver.Resolve(typeof(Service), OtherKeys.Named));
        Assert.True(resolver.TryResolve<Service, OtherKeys>(out var found, OtherKeys.Named));
        Assert.Same(named, found);
        Assert.True(resolver.IsRegistered<Service, Keys>(Keys.Named));
        Assert.Same(named, Assert.Single(resolver.Resolve<IReadOnlyList<Service>, Keys>(Keys.Named)));
        Assert.False(resolver.TryResolve<Service, Keys>(out _, (Keys)99));
        Assert.False(resolver.IsRegistered<Service, Keys>((Keys)99));
        Assert.Throws<InvalidOperationException>(() => resolver.Resolve<Service, Keys>((Keys)99));
    }

    [Fact]
    public void EqualNumbersConflictWithinLayerAndOverrideAcrossLayers() {
        var duplicates = new ContainerBuilder();
        duplicates.Register<Service>().WithKey(Keys.Named);
        duplicates.Register<Service>().WithKey(OtherKeys.Named);
        Assert.Throws<ArgumentException>(() => duplicates.Build());
        var defaults = new ContainerBuilder();
        defaults.Register<Service>();
        defaults.Register<Service>().WithKey(Keys.Default);
        Assert.Throws<ArgumentException>(() => defaults.Build());
        var builder = new ContainerBuilder();
        builder.Register<Service>().WithKey(Keys.Named);
        using var parent = builder.Build();
        using var child = parent.CreateChild(b => b.Register<Service>().WithKey(OtherKeys.Named));
        Assert.NotSame(parent.Resolve<Service>(7), child.Resolve<Service>(7));
        Assert.Same(child.Resolve<Service>(7), Assert.Single(child.Resolve<Service[], Keys>(Keys.Named)));
    }

    [Fact]
    public void EnumAttributesSelectConstructorAndMemberDependencies() {
        var builder = new ContainerBuilder();
        builder.Register<Service>().WithKey(Keys.Named);
        builder.Register<Consumer>();
        using var container = builder.Build();
        var consumer = container.Resolve<Consumer>();
        var service = container.Resolve<Service>(7);
        Assert.Same(service, consumer.Constructor);
        Assert.Same(service, consumer.Field);
        Assert.Same(service, consumer.Property);
        Assert.Same(service, consumer.Method);
    }

    [Fact]
    public void UnsupportedUnderlyingTypesAreRejectedAtEveryEnumBoundary() {
        AssertRejected((ByteKey)0);
        AssertRejected((SByteKey)0);
        AssertRejected((ShortKey)0);
        AssertRejected((UShortKey)0);
        AssertRejected((UIntKey)0);
        AssertRejected((LongKey)0);
        AssertRejected((ULongKey)0);
        Assert.Throws<ArgumentException>(() => new InjectKeyAttribute("named"));
    }

    [Fact]
    public void FullSignedIntegerRangeIsPreserved() {
        var builder = new ContainerBuilder();
        builder.Register<Service>().WithKey((Keys)int.MinValue);
        builder.Register<Service>().WithKey((Keys)int.MaxValue);
        using var container = builder.Build();
        Assert.Same(container.Resolve<Service>(int.MinValue), container.Resolve<Service, Keys>((Keys)int.MinValue));
        Assert.Same(container.Resolve<Service>(int.MaxValue), container.Resolve<Service, Keys>((Keys)int.MaxValue));
        Assert.Equal(int.MinValue, new InjectKeyAttribute((Keys)int.MinValue).Key);
    }

    private static void AssertRejected<TKey>(TKey key) where TKey : struct, Enum {
        var builder = new ContainerBuilder();
        Assert.Throws<ArgumentException>(() => builder.Register<Service>().WithKey(key));
        using var container = new ContainerBuilder().Build();
        Assert.Throws<ArgumentException>(() => container.Resolve<Service, TKey>(key));
        Assert.Throws<ArgumentException>(() => container.Resolve(typeof(Service), key));
        Assert.Throws<ArgumentException>(() => container.TryResolve<Service, TKey>(out _, key));
        Assert.Throws<ArgumentException>(() => container.IsRegistered<Service, TKey>(key));
        Assert.Throws<ArgumentException>(() => new InjectKeyAttribute(key));
    }

    private enum Keys { Default, Named = 7 }
    private enum OtherKeys { Named = 7 }
    private enum ByteKey : byte { }
    private enum SByteKey : sbyte { }
    private enum ShortKey : short { }
    private enum UShortKey : ushort { }
    private enum UIntKey : uint { }
    private enum LongKey : long { }
    private enum ULongKey : ulong { }
    private sealed class Service;

    private sealed class Consumer([InjectKey(Keys.Named)] Service service) {
        public Service Constructor { get; } = service;
        [Inject, InjectKey(OtherKeys.Named)] public Service? Field = null;
        [Inject, InjectKey(Keys.Named)] public Service? Property { get; set; }
        public Service? Method { get; private set; }

        [Inject]
        private void SetService([InjectKey(Keys.Named)] Service value) {
            Method = value;
        }
    }
}
