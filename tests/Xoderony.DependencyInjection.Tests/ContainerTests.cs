using System;
using System.Collections.Generic;
using Xunit;

namespace Xoderony.DependencyInjection.Tests;

public sealed class ContainerTests {
    [Fact]
    public void BuiltInContainerAndResolverStayLocalAcrossDefaultOverridesAndInjection() {
        using var external = new ContainerBuilder().Build();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(external).AsSelf().As<IResolver>();
        builder.RegisterInstance(external).AsSelf().As<IResolver>().WithKey(1);
        using (var root = builder.Build()) {
            AssertCurrentContainer(root);
            var target = new InjectedContainerTarget();
            root.Inject(target);
            Assert.Same(root, target.Container);
            Assert.Same(root, target.Resolver);

            using (var child = root.CreateChild(b => b.RegisterInstance(external).AsSelf().As<IResolver>())) {
                AssertCurrentContainer(child);
                Assert.Same(external, child.Resolve<Container>(1));
                Assert.Same(external, child.Resolve<IResolver>(1));
                child.Inject(target);
                Assert.Same(child, target.Container);
                Assert.Same(child, target.Resolver);
            }

            Assert.Same(root, root.Resolve<Container>());
        }
        Assert.Same(external, external.Resolve<Container>());

        static void AssertCurrentContainer(Container container) {
            Assert.Same(container, container.Resolve<Container>());
            Assert.Same(container, container.Resolve<IResolver>());
            Assert.Same(container, Assert.Single(container.Resolve<Container[]>()));
            Assert.Same(container, Assert.Single(container.Resolve<IResolver[]>()));
            var containers = container.Resolve<IReadOnlyList<Container>>();
            Assert.Same(container, Assert.Single(containers));
            Assert.Same(containers, container.Resolve<IEnumerable<Container>>());
            var resolvers = container.Resolve<IReadOnlyList<IResolver>>();
            Assert.Same(container, Assert.Single(resolvers));
            Assert.Same(resolvers, container.Resolve<IEnumerable<IResolver>>());
        }
    }

    [Fact]
    public void ContainerDependenciesUseRegistrationScopeAndKeepItsOwnership() {
        var builder = new ContainerBuilder();
        builder.Register<ContainerOwner>();
        using var root = builder.Build();
        ContainerOwner inherited;
        ContainerOwner local;
        using (var child = root.CreateChild(b => b.Register<ContainerOwner>().WithKey(1))) {
            inherited = child.Resolve<ContainerOwner>();
            local = child.Resolve<ContainerOwner>(1);
            Assert.Same(root, inherited.Container);
            Assert.Same(root, inherited.Resolver);
            Assert.Same(child, local.Container);
            Assert.Same(child, local.Resolver);
            Assert.Same(inherited, root.Resolve<ContainerOwner>());
        }

        Assert.Equal(1, local.DisposeCount);
        Assert.Equal(0, inherited.DisposeCount);
        Assert.Same(inherited, root.Resolve<ContainerOwner>());
        Assert.Same(root, root.Resolve<Container>());
        root.Dispose();
        Assert.Equal(1, inherited.DisposeCount);
    }

    [Fact]
    public void InheritedAliasesShareInstanceAndUseRegistrationScope() {
        var global = new Config("global");
        var builder = new ContainerBuilder();
        builder.RegisterInstance(global);
        builder.Register<Service>().As<IService>().AsSelf();
        using var root = builder.Build();
        using var child = root.CreateChild(b => b.RegisterInstance(new Config("child")));

        var instance = child.Resolve<IService>();

        Assert.Same(instance, root.Resolve<Service>());
        Assert.Same(global, instance.Config);
    }

    [Fact]
    public void ChildRegistrationOwnsInstanceAndUsesChildDependencies() {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(new Config("global"));
        builder.Register<Service>().As<IService>().AsSelf();
        using var root = builder.Build();
        var local = new Config("child");
        using var child = root.CreateChild(b => { b.RegisterInstance(local); b.Register<Service>().As<IService>().AsSelf(); });

        Assert.Same(child.Resolve<Service>(), child.Resolve<IService>());
        Assert.Same(local, child.Resolve<IService>().Config);
        Assert.NotSame(root.Resolve<Service>(), child.Resolve<Service>());
    }

    [Fact]
    public void CollectionsRespectOrderKeysAndWholeGroupOverrides() {
        var builder = new ContainerBuilder();
        builder.Register<FirstHandler>().As<IHandler>();
        builder.Register<SecondHandler>().As<IHandler>().As<IHandler>();
        builder.Register<FirstHandler>().As<IHandler>().As<IHandler>().WithKey(1);
        using var root = builder.Build();
        using var child = root.CreateChild(b => b.Register<FirstHandler>().As<IHandler>().As<IHandler>());

        Assert.Throws<InvalidOperationException>(() => root.Resolve<IHandler>());
        Assert.Collection(root.Resolve<IHandler[]>(), value => Assert.IsType<FirstHandler>(value), value => Assert.IsType<SecondHandler>(value));
        Assert.Single(child.Resolve<IHandler[]>());
        Assert.Same(root.Resolve<IHandler>(1), child.Resolve<IHandler>(1));
        Assert.False(root.TryResolve<IHandler[]>(out _, 2));
        Assert.Equal(2, root.Resolve<IHandler[]>().Length);
        Assert.Equal(2, root.Resolve<IReadOnlyList<IHandler>>().Count);
        Assert.Equal(2, new List<IHandler>(root.Resolve<IEnumerable<IHandler>>()).Count);
    }

    [Fact]
    public void ClosedGenericsShareInheritedAliasesButDoNotExpand() {
        var builder = new ContainerBuilder();
        builder.Register<Repository<Config>>().As<IRepository<Config>>().AsSelf();
        using var root = builder.Build();
        using var child = root.CreateChild();

        Assert.Same(root.Resolve<IRepository<Config>>(), root.Resolve<Repository<Config>>());
        Assert.Same(root.Resolve<IRepository<Config>>(), child.Resolve<IRepository<Config>>());
        Assert.False(root.TryResolve<IRepository<Service>>(out _));
    }

    [Fact]
    public void InjectionProcessesBaseMembersBeforeMethodsAndRepeatsExplicitly() {
        var builder = new ContainerBuilder();
        var config = new Config("music");
        builder.RegisterInstance(config).WithKey(1);
        using var container = builder.Build();
        var target = new InjectedTarget();

        container.Inject(target);
        container.Inject(target);

        Assert.Same(config, target.Config);
        Assert.Equal(new[] { "base:music", "derived:music", "base:music", "derived:music" }, target.Events);
    }

    [Fact]
    public void ContainerOwnsTypeAndFactoryResultsButNotExistingInstances() {
        var events = new List<string>();
        var existing = new ExternalDisposable();
        var factoryResult = new ExternalDisposable();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(events);
        builder.Register<Dependency>();
        builder.Register<Owner>().As<IDisposable>().AsSelf();
        builder.Register<AdditionalDisposable>();
        builder.RegisterInstance(existing);
        builder.RegisterFactory<ExternalDisposable>(_ => factoryResult).WithKey(1);
        var container = builder.Build();
        var owner = container.Resolve<Owner>();
        Assert.Same(owner, container.Resolve<IDisposable>());
        var additional = container.Resolve<AdditionalDisposable>();
        container.Resolve<ExternalDisposable>();
        container.Resolve<ExternalDisposable>(1);

        container.Dispose();

        Assert.Equal(new[] { "owner", "dependency" }, events);
        Assert.False(existing.Disposed);
        Assert.True(factoryResult.Disposed);
        Assert.True(additional.Disposed);
        existing.Dispose();
    }

    [Fact]
    public void BuildCallbackCanWarmServicesWithoutRepeatingOnResolveOrInject() {
        var factoryCalls = 0;
        var callbackCalls = 0;
        var builder = new ContainerBuilder();
        builder.RegisterFactory<Config>(_ => { factoryCalls++; return new Config("warm"); });
        builder.AddBuildCallback(resolver => { callbackCalls++; resolver.Resolve<Config>(); });
        using var container = builder.Build();

        Assert.Equal(1, factoryCalls);
        container.Resolve<Config>();
        container.Inject(new object());
        Assert.Equal(1, factoryCalls);
        Assert.Equal(1, callbackCalls);
    }

    [Fact]
    public void BuildDoesNotInvokeFactoriesAndFactoriesDoNotInjectTheirResults() {
        var calls = 0;
        var builder = new ContainerBuilder();
        builder.RegisterFactory<InjectedTarget>(_ => { calls++; return new InjectedTarget(); });
        using var container = builder.Build();
        Assert.Equal(0, calls);

        var target = container.Resolve<InjectedTarget>();

        Assert.Equal(1, calls);
        Assert.Empty(target.Events);
        Assert.Null(target.Config);
    }

    [Fact]
    public void TryResolveOnlySuppressesMissingTarget() {
        var builder = new ContainerBuilder();
        builder.Register<Service>();
        using var container = builder.Build();

        Assert.False(container.TryResolve<Config>(out _));
        var error = Assert.Throws<InvalidOperationException>(() => container.TryResolve<Service>(out _));
        Assert.Contains(nameof(Config), error.Message);
    }

    [Fact]
    public void CircularDependencyFailsWithoutPreventingUnrelatedResolution() {
        var builder = new ContainerBuilder();
        builder.Register<CycleA>();
        builder.Register<CycleB>();
        builder.RegisterInstance(new Config("valid"));
        using var container = builder.Build();

        var error = Assert.Throws<InvalidOperationException>(() => container.Resolve<CycleA>());

        Assert.Same(error, Assert.Throws<InvalidOperationException>(() => container.Resolve<CycleA>()));
        Assert.Equal("valid", container.Resolve<Config>().Name);
    }

    [Fact]
    public void PrivateMarkedConstructorWinsAndAmbiguityFailsOnFirstResolve() {
        var builder = new ContainerBuilder();
        var config = new Config("selected");
        builder.RegisterInstance(config);
        builder.Register<SelectedConstructor>();
        using var container = builder.Build();
        Assert.Same(config, container.Resolve<SelectedConstructor>().Config);

        var invalid = new ContainerBuilder();
        invalid.Register<AmbiguousConstructor>();
        using var invalidContainer = invalid.Build();
        Assert.Throws<ArgumentException>(() => invalidContainer.Resolve<AmbiguousConstructor>());
    }

    public sealed class InjectedContainerTarget {
        [Inject]
        public Container? Container { get; private set; }
        [Inject]
        public IResolver? Resolver { get; private set; }
    }

    public sealed class ContainerOwner(Container container, IResolver resolver) : IDisposable {
        public Container Container { get; } = container;
        public IResolver Resolver { get; } = resolver;
        public int DisposeCount { get; private set; }
        public void Dispose() => DisposeCount++;
    }

    public sealed record Config(string Name);
    public interface IService { Config Config { get; } }
    public sealed class Service(Config config) : IService { public Config Config { get; } = config; }
    public interface IHandler;
    public sealed class FirstHandler : IHandler;
    public sealed class SecondHandler : IHandler;
    public interface IRepository<T>;
    public sealed class Repository<T> : IRepository<T>;
    public sealed class StructRepository<T> : IRepository<T> where T : struct;
    public sealed class SpecialRepository : IRepository<Config>;

    public class InjectedBase {
        private readonly List<string> _events = [];
        [Inject, InjectKey(1)]
        private Config? _config = null;
        public Config? Config => _config;
        public List<string> Events => _events;

        [Inject]
        private void OnInjected() {
            _events.Add($"base:{_config?.Name}");
        }
    }

    public sealed class InjectedTarget : InjectedBase {
        [Inject, InjectKey(1)]
        private Config? LocalConfig { get; set; }

        [Inject]
        private void OnInjected() {
            Events.Add($"derived:{LocalConfig?.Name}");
        }
    }

    public sealed class ExternalDisposable : IDisposable {
        public bool Disposed { get; private set; }
        public void Dispose() { Disposed = true; }
    }

    public sealed class AdditionalDisposable : IDisposable {
        public bool Disposed { get; private set; }
        public void Dispose() { Disposed = true; }
    }

    public sealed class Dependency(List<string> events) : IDisposable {
        public void Dispose() { events.Add("dependency"); }
    }

    public sealed class Owner(Dependency dependency, List<string> events) : IDisposable {
        public Dependency Dependency { get; } = dependency;
        public void Dispose() { events.Add("owner"); }
    }

    public sealed class CycleA(CycleB dependency) { public CycleB Dependency { get; } = dependency; }
    public sealed class CycleB(CycleA dependency) { public CycleA Dependency { get; } = dependency; }

    public sealed class SelectedConstructor {
        public Config? Config { get; }
        public SelectedConstructor() { }
        [Inject]
        private SelectedConstructor(Config config) { Config = config; }
    }

    public sealed class AmbiguousConstructor {
        public AmbiguousConstructor() { }
        public AmbiguousConstructor(Config config) { }
    }
}
