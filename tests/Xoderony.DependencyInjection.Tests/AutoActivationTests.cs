using System;
using System.Collections.Generic;
using Xunit;

namespace Xoderony.DependencyInjection.Tests;

public sealed class AutoActivationTests {
    [Fact]
    public void TypesAreCreatedAndInjectedBeforeCallbacksAndReuseTheirDependencies() {
        var events = new List<string>();
        var builder = new ContainerBuilder();
        builder.AddBuildCallback(resolver => {
            Assert.Equal(new[] { "dependency", "owner", "injected" }, events);
            Assert.Same(resolver.Resolve<Dependency>(), resolver.Resolve<Owner>().Dependency);
            events.Add("callback1");
        });
        var registration = builder.Register<Owner>();
        Assert.Same(registration, registration.AutoActivate().AutoActivate());
        builder.Register<Dependency>().AutoActivate();
        builder.RegisterInstance(events);
        builder.RegisterFactory<string>(_ => throw new InvalidOperationException("Must remain lazy."));
        builder.AddBuildCallback(_ => events.Add("callback2"));

        using var container = builder.Build();

        var owner = container.Resolve<Owner>();
        Assert.Same(owner, container.Resolve<Owner>());
        Assert.Equal(new[] { "dependency", "owner", "injected", "callback1", "callback2" }, events);
        container.Dispose();
        Assert.Equal(new[] { "dependency", "owner", "injected", "callback1", "callback2", "dispose owner", "dispose dependency" }, events);
    }

    [Fact]
    public void ActivationUsesEachRegistrationEvenWithAmbiguousAliasesAndKeys() {
        var events = new List<string>();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(events);
        builder.Register<Dependency>().AutoActivate().As<IDisposable>().WithKey(7);
        builder.Register<Other>().As<IDisposable>().WithKey(7).AutoActivate();

        using var container = builder.Build();

        Assert.Equal(new[] { "dependency", "other" }, events);
        Assert.False(container.IsRegistered<Dependency>(7));
        Assert.False(container.IsRegistered<IDisposable>());
        Assert.Throws<InvalidOperationException>(() => container.Resolve<IDisposable>(7));
        var items = container.Resolve<IDisposable[]>(7);
        Assert.IsType<Dependency>(items[0]);
        Assert.IsType<Other>(items[1]);
        Assert.Same(items[0], container.Resolve<IReadOnlyList<IDisposable>>(7)[0]);
        Assert.Equal(2, events.Count);
    }

    [Fact]
    public void FactoriesActivateOnceWithoutInjectingAndExistingInstancesRemainExternal() {
        var calls = 0;
        var factoryItem = new Item();
        var existing = new Item();
        IResolver? supplied = null;
        var builder = new ContainerBuilder();
        builder.RegisterFactory<Item>(resolver => { supplied = resolver; calls++; return factoryItem; })
            .As<IDisposable>().WithKey(1).AutoActivate();
        builder.RegisterInstance(existing).AutoActivate();

        using (var container = builder.Build()) {
            Assert.Equal(1, calls);
            Assert.Same(container, supplied);
            Assert.Same(factoryItem, container.Resolve<IDisposable>(1));
            Assert.Same(existing, container.Resolve<Item>());
            Assert.Equal(0, factoryItem.InjectionCount);
            Assert.Equal(0, existing.InjectionCount);
        }
        Assert.Equal(1, factoryItem.DisposeCount);
        Assert.Equal(0, existing.DisposeCount);
    }

    [Fact]
    public void ChildrenActivateOnlyLocalRegistrationsAndKeepParentOwnership() {
        var parentCalls = 0;
        var childCalls = 0;
        var parentItem = new Item();
        var childItem = new Item();
        var builder = new ContainerBuilder();
        builder.RegisterFactory<Item>(_ => { parentCalls++; return parentItem; }).AutoActivate();
        using var parent = builder.Build();
        using (var child = parent.CreateChild(local => {
            local.RegisterFactory<Item>(resolver => {
                childCalls++;
                Assert.Same(parentItem, resolver.Resolve<Item>());
                return childItem;
            }).WithKey(1).AutoActivate();
        })) {
            Assert.Equal(1, parentCalls);
            Assert.Equal(1, childCalls);
            Assert.Same(parentItem, child.Resolve<Item>());
            Assert.Same(childItem, child.Resolve<Item>(1));
        }
        using (var sibling = parent.CreateChild()) {
            Assert.Equal(1, parentCalls);
            Assert.Same(parentItem, sibling.Resolve<Item>());
        }
        Assert.Equal(1, childItem.DisposeCount);
        Assert.Equal(0, parentItem.DisposeCount);
        parent.Dispose();
        Assert.Equal(1, parentItem.DisposeCount);
    }

    [Fact]
    public void RepeatedBuildsHaveIndependentActivationAndBuilderChangesAreNotInherited() {
        var calls = 0;
        var builder = new ContainerBuilder();
        var registration = builder.RegisterFactory<Item>(_ => { calls++; return new Item(); });
        using var original = builder.Build();
        registration.AutoActivate();
        using var child = original.CreateChild();
        Assert.Equal(0, calls);
        using var first = builder.Build();
        using var second = builder.Build();
        Assert.Equal(2, calls);
        Assert.NotSame(first.Resolve<Item>(), second.Resolve<Item>());
        Assert.Same(original.Resolve<Item>(), child.Resolve<Item>());
        Assert.Equal(3, calls);
    }

    [Fact]
    public void DuplicateDeclarationsFailBeforeAnyActivationOrCallback() {
        var calls = 0;
        var builder = new ContainerBuilder();
        builder.RegisterFactory<Item>(_ => { calls++; return new Item(); }).AutoActivate();
        builder.Register<Item>();
        builder.AddBuildCallback(_ => calls++);

        Assert.Throws<ArgumentException>(() => builder.Build());
        Assert.Equal(0, calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void BuildFailuresPropagateWithoutCleanupAndStopRemainingWork(bool failInCallback) {
        var failure = new ApplicationException("build failed");
        var item = new Item();
        var events = new List<string>();
        var builder = new ContainerBuilder();
        builder.RegisterFactory<Item>(_ => item).AutoActivate();
        builder.RegisterFactory<string>(_ => {
            events.Add("activate");
            if (!failInCallback) {
                throw failure;
            }
            return "ready";
        }).AutoActivate();
        builder.RegisterFactory<object>(_ => { events.Add("last activation"); return new object(); }).AutoActivate();
        builder.AddBuildCallback(_ => { events.Add("callback"); throw failure; });
        builder.AddBuildCallback(_ => events.Add("last callback"));

        Assert.Same(failure, Assert.Throws<ApplicationException>(() => builder.Build()));
        Assert.Equal(failInCallback ? new[] { "activate", "last activation", "callback" } : new[] { "activate" }, events);
        Assert.Equal(0, item.DisposeCount);
        item.Dispose();
    }

    [Fact]
    public void FailedChildActivationDoesNotDisposeOrInvalidateParent() {
        var item = new Item();
        var builder = new ContainerBuilder();
        builder.RegisterFactory<Item>(_ => item).AutoActivate();
        using var parent = builder.Build();
        var failure = new ApplicationException("child failed");

        Assert.Same(failure, Assert.Throws<ApplicationException>(() => parent.CreateChild(local =>
            local.RegisterFactory<object>(resolver => {
                Assert.Same(item, resolver.Resolve<Item>());
                throw failure;
            }).AutoActivate())));
        Assert.Equal(0, item.DisposeCount);
        Assert.Same(item, parent.Resolve<Item>());
    }

    public sealed class Dependency : IDisposable {
        private readonly List<string> _events;
        public Dependency(List<string> events) { _events = events; events.Add("dependency"); }
        public void Dispose() => _events.Add("dispose dependency");
    }

    public sealed class Owner : IDisposable {
        private readonly Dependency _dependency;
        private readonly List<string> _events;
        public Dependency Dependency => _dependency;
        public Owner(Dependency dependency, List<string> events) {
            _dependency = dependency;
            _events = events;
            events.Add("owner");
        }
        [Inject]
        private void Initialize(Dependency dependency) {
            Assert.Same(_dependency, dependency);
            _events.Add("injected");
        }
        public void Dispose() => _events.Add("dispose owner");
    }

    public sealed class Other : IDisposable {
        public Other(List<string> events) => events.Add("other");
        public void Dispose() { }
    }

    public sealed class Item : IDisposable {
        private int _injectionCount;
        private int _disposeCount;
        public int InjectionCount => _injectionCount;
        public int DisposeCount => _disposeCount;
        [Inject]
        private void Initialize() => _injectionCount++;
        public void Dispose() => _disposeCount++;
    }
}
