using System;
using System.Collections.Generic;
using Xunit;

namespace Xoderony.DependencyInjection.Tests;

public sealed class BuildTimeCollectionTests {
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SingleAndAllCollectionFormsShareOneOwnedInstance(bool collectionFirst) {
        var calls = 0;
        var builder = new ContainerBuilder();
        builder.RegisterFactory(_ => { calls++; return new First(); }).As<IHandler>().As<IHandler>().AsSelf().As<IDisposable>();
        using var container = builder.Build();
        Assert.True(container.IsRegistered<IHandler[]>());
        Assert.Equal(0, calls);
        First item;
        if (collectionFirst) {
            item = (First)Assert.Single(container.Resolve<IHandler[]>());
        } else {
            item = container.Resolve<First>();
        }
        var list = container.Resolve<IReadOnlyList<IHandler>>();
        Assert.Same(item, Assert.Single(list));
        Assert.Same(list, container.Resolve<IEnumerable<IHandler>>());
        Assert.Same(item, container.Resolve<IHandler>());
        Assert.Same(item, Assert.Single(container.Resolve<IDisposable[]>()));
        Assert.Same(item, Assert.Single(container.Resolve<First[]>()));
        Assert.Throws<NotSupportedException>(() => ((IList<IHandler>)list)[0] = new Second());
        var copy = container.Resolve<IHandler[]>();
        copy[0] = new Second();
        Assert.Same(item, list[0]);
        Assert.NotSame(copy, container.Resolve<IHandler[]>());
        Assert.Equal(1, calls);
        container.Dispose();
        Assert.Equal(1, item.DisposeCount);
    }

    [Fact]
    public void AmbiguityDoesNotCreateCandidatesAndCollectionPreservesOrder() {
        var calls = 0;
        var builder = new ContainerBuilder();
        builder.RegisterFactory(_ => { calls++; return new First(); }).As<IHandler>().AsSelf();
        builder.RegisterFactory(_ => { calls++; return new Second(); }).As<IHandler>().AsSelf();
        using var container = builder.Build();
        Assert.True(container.IsRegistered<IHandler>());
        Assert.True(container.IsRegistered<IReadOnlyList<IHandler>>());
        Assert.Throws<InvalidOperationException>(() => container.Resolve<IHandler>());
        Assert.Throws<InvalidOperationException>(() => container.TryResolve<IHandler>(out _));
        Assert.Equal(0, calls);
        var list = container.Resolve<IReadOnlyList<IHandler>>();
        Assert.Collection(list, first => Assert.Same(container.Resolve<First>(), first), second => Assert.Same(container.Resolve<Second>(), second));
        Assert.Equal(2, calls);
        Assert.Throws<InvalidOperationException>(() => container.Resolve<IHandler>());
    }

    [Fact]
    public void MissingCollectionsStayMissingAndGeneratedCollectionsDoNotRecurse() {
        var builder = new ContainerBuilder();
        builder.Register<First>();
        using var container = builder.Build();
        Assert.False(container.IsRegistered<IHandler[]>());
        Assert.False(container.TryResolve<IReadOnlyList<IHandler>>(out _));
        Assert.Throws<InvalidOperationException>(() => container.Resolve<IEnumerable<IHandler>>());
        Assert.False(container.IsRegistered<First[][]>());
        Assert.False(container.TryResolve<IReadOnlyList<IReadOnlyList<First>>>(out _));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void DuplicateDeclarationsAcrossSourcesFailBeforeCallbacks(int source) {
        var builder = new ContainerBuilder();
        builder.Register<First>();
        if (source == 0) {
            builder.Register<First>().As<IHandler>();
        } else if (source == 1) {
            builder.RegisterFactory(static _ => new First());
        } else {
            builder.RegisterInstance(new First());
        }
        var callback = false;
        builder.AddBuildCallback(_ => callback = true);
        Assert.Throws<ArgumentException>(() => builder.Build());
        Assert.False(callback);
    }

    [Fact]
    public void FactoryDeclarationUsesDeclaredTypeAndKeysUseEquality() {
        var builder = new ContainerBuilder();
        var calls = 0;
        builder.RegisterFactory<IHandler>(_ => { calls++; return new First(); }).WithKey(1);
        builder.RegisterFactory<IHandler>(_ => { calls++; return new Second(); }).WithKey(1);
        Assert.Throws<ArgumentException>(() => builder.Build());
        Assert.Equal(0, calls);
    }

    [Fact]
    public void DifferentKeysRemainIndependent() {
        var builder = new ContainerBuilder();
        builder.Register<First>().As<IHandler>();
        builder.Register<First>().As<IHandler>().WithKey(1);
        using var container = builder.Build();
        Assert.NotSame(container.Resolve<IHandler>(), container.Resolve<IHandler>(1));
        Assert.Same(container.Resolve<IHandler>(), Assert.Single(container.Resolve<IHandler[]>()));
        Assert.Same(container.Resolve<IHandler>(1), Assert.Single(container.Resolve<IHandler[]>(1)));
        Assert.False(container.TryResolve<IHandler[]>(out _, 2));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ChildReplacesAllGroupEntriesRegardlessOfParentWarmup(bool warmFirst) {
        var builder = new ContainerBuilder();
        builder.Register<First>().As<IHandler>().AsSelf();
        builder.Register<Second>().As<IHandler>();
        using var root = builder.Build();
        if (warmFirst) {
            root.Resolve<IHandler[]>();
        }
        using var child = root.CreateChild(b => b.Register<Second>().As<IHandler>());
        Assert.Throws<InvalidOperationException>(() => root.Resolve<IHandler>());
        Assert.Equal(2, root.Resolve<IHandler[]>().Length);
        Assert.Same(child.Resolve<IHandler>(), Assert.Single(child.Resolve<IEnumerable<IHandler>>()));
        Assert.Same(child.Resolve<IHandler>(), Assert.Single(child.Resolve<IHandler[]>()));
        Assert.Same(root.Resolve<First>(), child.Resolve<First>());
        using var grandchild = child.CreateChild();
        Assert.Same(child.Resolve<IHandler>(), grandchild.Resolve<IHandler>());
        Assert.Same(child.Resolve<IReadOnlyList<IHandler>>(), grandchild.Resolve<IReadOnlyList<IHandler>>());
    }

    [Fact]
    public void ChildCanReplaceSingleWithAmbiguousGroupAndOwnResolverCollection() {
        var builder = new ContainerBuilder();
        builder.Register<First>().As<IHandler>();
        using var root = builder.Build();
        using var child = root.CreateChild(b => {
            b.Register<First>().As<IHandler>();
            b.Register<Second>().As<IHandler>();
        });
        Assert.Throws<InvalidOperationException>(() => child.Resolve<IHandler>());
        Assert.Equal(2, child.Resolve<IHandler[]>().Length);
        Assert.Single(root.Resolve<IHandler[]>());
        Assert.Same(root, Assert.Single(root.Resolve<IResolver[]>()));
        Assert.Same(child, Assert.Single(child.Resolve<IReadOnlyList<IResolver>>()));
    }

    [Fact]
    public void ClosedGenericRegistrationsUseExactTypesAndNormalScopeRules() {
        var builder = new ContainerBuilder();
        builder.Register(typeof(Generic<int>)).As(typeof(IGeneric<int>)).AsSelf();
        using var root = builder.Build();
        using var child = root.CreateChild(b => {
            b.Register<Generic<int>>().As<IGeneric<int>>().AsSelf();
            b.Register<Generic<string>>().As<IGeneric<string>>();
        });
        Assert.NotSame(root.Resolve<IGeneric<int>>(), child.Resolve<IGeneric<int>>());
        Assert.Same(child.Resolve<Generic<int>>(), Assert.Single(child.Resolve<IGeneric<int>[]>()));
        Assert.IsType<Generic<string>>(child.Resolve<IGeneric<string>>());
        Assert.False(root.TryResolve<IGeneric<string>>(out _));
        Assert.False(child.TryResolve<IGeneric<double>>(out _));
    }

    [Fact]
    public void OpenAndPartiallyOpenTypesAreRejectedAtRegistration() {
        var builder = new ContainerBuilder();
        Assert.Throws<ArgumentException>(() => builder.Register(typeof(Generic<>)));
        Assert.Throws<ArgumentException>(() => builder.Register<Generic<int>>().As(typeof(IGeneric<>)));
        var partial = typeof(Dictionary<,>).MakeGenericType(typeof(int), typeof(Generic<>));
        Assert.Throws<ArgumentException>(() => builder.Register(partial));
    }

    [Fact]
    public void ReservedCollectionTypesAreRejectedAcrossSourcesAndAliases() {
        var builder = new ContainerBuilder();
        Assert.Throws<ArgumentException>(() => builder.Register(typeof(First[])));
        Assert.Throws<ArgumentException>(() => builder.RegisterInstance(Array.Empty<int>()).As<object>());
        Assert.Throws<ArgumentException>(() => builder.RegisterFactory<IReadOnlyList<First>>(static _ => Array.Empty<First>()));
        Assert.Throws<ArgumentException>(() => builder.RegisterFactory<IEnumerable<First>>(static _ => Array.Empty<First>()));
        Assert.Throws<ArgumentException>(() => builder.Register<List<First>>().As<IReadOnlyList<First>>());
        Assert.Throws<ArgumentException>(() => builder.Register<List<Second>>().As<IEnumerable<Second>>());
        var extensionBuilder = new ContainerBuilder();
        Assert.Throws<ArgumentException>(() => extensionBuilder.Register<List<First>>().AsImplementedInterfaces());
        var concrete = new ContainerBuilder();
        concrete.RegisterInstance(new List<int>());
        using var container = concrete.Build();
        Assert.Empty(container.Resolve<List<int>>());
        Assert.Single(container.Resolve<List<int>[]>());
    }

    [Fact]
    public void FailedCollectionKeepsCreatedElementsOwnedAndFailureCached() {
        var failure = new ApplicationException("failed");
        var calls = 0;
        var builder = new ContainerBuilder();
        builder.Register<First>().As<IHandler>().AsSelf();
        builder.RegisterFactory<Second>(_ => { calls++; throw failure; }).As<IHandler>().AsSelf();
        using var container = builder.Build();
        Assert.Same(failure, Assert.Throws<ApplicationException>(() => container.Resolve<IHandler[]>()));
        Assert.Same(failure, Assert.Throws<ApplicationException>(() => container.Resolve<IEnumerable<IHandler>>()));
        Assert.Same(failure, Assert.Throws<ApplicationException>(() => container.Resolve<Second>()));
        Assert.Equal(1, calls);
        var first = container.Resolve<First>();
        container.Dispose();
        Assert.Equal(1, first.DisposeCount);
    }

    [Fact]
    public void CollectionCycleFailsWithoutRecursingForever() {
        var builder = new ContainerBuilder();
        builder.Register<Cycle>().As<IHandler>();
        using var container = builder.Build();
        var failure = Assert.Throws<InvalidOperationException>(() => container.Resolve<IHandler[]>());
        Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => container.Resolve<IReadOnlyList<IHandler>>()));
    }

    [Fact]
    public void ExistingInstancesRemainExternalAndBuildsKeepIndependentState() {
        var existing = new First();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(existing).As<IHandler>();
        using var first = builder.Build();
        builder.Register<Second>().As<IHandler>();
        using var second = builder.Build();
        Assert.Same(existing, first.Resolve<IHandler>());
        Assert.Same(existing, Assert.Single(first.Resolve<IHandler[]>()));
        Assert.Equal(2, second.Resolve<IHandler[]>().Length);
        Assert.Throws<InvalidOperationException>(() => second.Resolve<IHandler>());
        first.Dispose();
        second.Dispose();
        Assert.Equal(0, existing.DisposeCount);
    }

    [Fact]
    public void InheritedCollectionUsesParentDependenciesAndDisposesOnlyAtParent() {
        var builder = new ContainerBuilder();
        IResolver? supplied = null;
        builder.RegisterFactory(resolver => { supplied = resolver; return new First(); }).As<IHandler>().AsSelf();
        using var root = builder.Build();
        First item;
        using (var child = root.CreateChild()) {
            item = (First)Assert.Single(child.Resolve<IHandler[]>());
            Assert.Same(root, supplied);
            Assert.Same(item, root.Resolve<First>());
        }
        Assert.Equal(0, item.DisposeCount);
        root.Dispose();
        Assert.Equal(1, item.DisposeCount);
    }

    public interface IHandler;
    public sealed class First : IHandler, IDisposable {
        public int DisposeCount { get; private set; }
        public void Dispose() => DisposeCount++;
    }
    public sealed class Second : IHandler;
    public sealed class Cycle(IReadOnlyList<IHandler> handlers) : IHandler {
        public IReadOnlyList<IHandler> Handlers { get; } = handlers;
    }
    public interface IGeneric<T>;
    public sealed class Generic<T> : IGeneric<T>;
}
