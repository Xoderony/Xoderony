using System;
using System.Collections.Generic;
using Xunit;

namespace Xoderony.DependencyInjection.Tests;

public sealed class RegistrationContractTests {
    [Fact]
    public void ExplicitAliasesReplaceDefaultAndDuplicateAliasDoesNotDuplicateCollection() {
        var builder = new ContainerBuilder();
        builder.Register<Item>().As<IDisposable>().As<IDisposable>();
        using var container = builder.Build();
        Assert.False(container.IsRegistered<Item>());
        Assert.Single(container.Resolve<IDisposable[]>());
        Assert.Same(container.Resolve<IDisposable>(), container.Resolve<IDisposable[]>()[0]);
    }

    [Fact]
    public void FactoryIsLazySharedAcrossAliasesAndOwnedByRegisteringContainer() {
        var builder = new ContainerBuilder();
        var calls = 0;
        IResolver? supplied = null;
        builder.RegisterFactory<Item>(resolver => { supplied = resolver; calls++; return new Item(); }).AsSelf().As<IDisposable>();
        using var root = builder.Build();
        Assert.Equal(0, calls);
        Item item;
        using (var child = root.CreateChild()) {
            item = child.Resolve<Item>();
            Assert.Same(root, supplied);
            Assert.Same(item, child.Resolve<IDisposable>());
            Assert.Same(item, root.Resolve<Item>());
            Assert.Same(child, child.Resolve<IResolver>());
        }
        Assert.Equal(0, item.DisposeCount);
        root.Dispose();
        Assert.Equal(1, calls);
        Assert.Equal(1, item.DisposeCount);
    }

    [Fact]
    public void FactoryFailureIsNotWrappedAndIsCached() {
        var builder = new ContainerBuilder();
        var failure = new ApplicationException("factory failed");
        var calls = 0;
        builder.RegisterFactory<Item>(_ => { calls++; throw failure; });
        using var container = builder.Build();
        Assert.Same(failure, Assert.Throws<ApplicationException>(() => container.Resolve<Item>()));
        Assert.Same(failure, Assert.Throws<ApplicationException>(() => container.TryResolve<Item>(out _)));
        Assert.Equal(1, calls);
    }

    [Fact]
    public void ExistingAndFactoryObjectsDoNotRequireValidInjectionDeclarations() {
        var existing = new InitTarget { Value = "existing" };
        var builder = new ContainerBuilder();
        builder.RegisterInstance(existing);
        builder.RegisterFactory<InitTarget>(_ => new InitTarget { Value = "factory" }).WithKey(1);
        using var container = builder.Build();
        Assert.Same(existing, container.Resolve<InitTarget>());
        Assert.Equal("factory", container.Resolve<InitTarget>(1).Value);
        Assert.Throws<ArgumentException>(() => container.Inject(existing));
    }

    [Fact]
    public void InitMemberValidationIsDeferredUntilTypeCreation() {
        var builder = new ContainerBuilder();
        builder.Register<InitTarget>();
        using var container = builder.Build();
        Assert.Throws<ArgumentException>(() => container.Resolve<InitTarget>());
    }

    [Fact]
    public void ExternalInjectionDoesNotAnalyzeConstructors() {
        var builder = new ContainerBuilder();
        builder.RegisterInstance("value");
        using var container = builder.Build();
        var target = new Ambiguous();
        container.Inject(target);
        Assert.Equal("value", target.Value);
    }

    [Fact]
    public void ClosedGenericAliasesDisposeOnceAndChildRegistrationIsIndependent() {
        var builder = new ContainerBuilder();
        builder.Register(typeof(GenericItem<int>)).As(typeof(IGenericItem<int>)).AsSelf();
        using var root = builder.Build();
        var inherited = root.Resolve<GenericItem<int>>();
        GenericItem<int> local;
        using (var child = root.CreateChild(b => b.Register(typeof(GenericItem<int>)).As(typeof(IGenericItem<int>)).AsSelf().WithKey(1))) {
            local = child.Resolve<GenericItem<int>>(1);
            Assert.NotSame(inherited, local);
            Assert.Same(local, child.Resolve<IGenericItem<int>>(1));
            Assert.False(child.TryResolve<GenericItem<string>>(out _, 1));
        }
        Assert.Equal(1, local.DisposeCount);
        Assert.Equal(0, inherited.DisposeCount);
        root.Dispose();
        Assert.Equal(1, inherited.DisposeCount);
    }

    [Fact]
    public void InvalidMappingsAreRejectedAtRegistration() {
        var builder = new ContainerBuilder();
        Assert.Throws<ArgumentException>(() => builder.Register(typeof(IDisposable)));
        Assert.Throws<ArgumentException>(() => builder.Register<Item>().As<string>());
        Assert.Throws<ArgumentException>(() => builder.Register(typeof(Reversed<,>)).As(typeof(IPair<,>)));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GenericAliasCachePreservesCollectionsKeysAndDisposal(bool aliasFirst) {
        var builder = new ContainerBuilder();
        builder.Register(typeof(GenericItem<int>)).As(typeof(IGenericItem<int>)).AsSelf();
        builder.Register(typeof(GenericItem<int>)).As(typeof(IGenericItem<int>)).AsSelf().WithKey(1);
        using var container = builder.Build();
        GenericItem<int> item;
        if (aliasFirst) {
            item = (GenericItem<int>)container.Resolve<IGenericItem<int>>();
        } else {
            item = container.Resolve<GenericItem<int>>();
        }

        Assert.Same(item, container.Resolve<IGenericItem<int>>());
        Assert.Same(item, container.Resolve<GenericItem<int>>());
        Assert.Same(item, Assert.Single(container.Resolve<IGenericItem<int>[]>()));
        Assert.Same(item, Assert.Single(container.Resolve<GenericItem<int>[]>()));
        var keyed = container.Resolve<IGenericItem<int>>(1);
        Assert.NotSame(item, keyed);
        Assert.Same(keyed, container.Resolve<GenericItem<int>>(1));
        Assert.Same(keyed, Assert.Single(container.Resolve<IGenericItem<int>[]>(1)));
        Assert.False(container.TryResolve<IGenericItem<string>>(out _));

        container.Dispose();
        Assert.Equal(1, item.DisposeCount);
        Assert.Equal(1, ((GenericItem<int>)keyed).DisposeCount);
    }

    [Fact]
    public void WarmedGenericAliasesAreInheritedWithoutChangingOwnership() {
        var builder = new ContainerBuilder();
        builder.Register(typeof(GenericItem<int>)).As(typeof(IGenericItem<int>)).AsSelf();
        using var root = builder.Build();
        var inherited = root.Resolve<IGenericItem<int>>();
        using var child = root.CreateChild();
        var local = child.Resolve<IGenericItem<int>>();

        Assert.Same(inherited, local);
        Assert.Same(local, child.Resolve<GenericItem<int>>());
        Assert.Same(local, Assert.Single(child.Resolve<IGenericItem<int>[]>()));
        Assert.Same(inherited, root.Resolve<GenericItem<int>>());
    }

    public sealed class Item : IDisposable {
        public int DisposeCount { get; private set; }
        public void Dispose() => DisposeCount++;
    }

    public sealed class InitTarget {
        [Inject]
        public required string Value { get; init; }
    }

    public sealed class Ambiguous {
        [Inject]
        public string? Value { get; set; }
        public Ambiguous() { }
        public Ambiguous(int ignored) { }
    }

    public interface IGenericItem<T>;
    public sealed class GenericItem<T> : IGenericItem<T>, IDisposable {
        public int DisposeCount { get; private set; }
        public void Dispose() => DisposeCount++;
    }
    public interface IPair<T1, T2>;
    public sealed class Reversed<T1, T2> : IPair<T2, T1>;
}
