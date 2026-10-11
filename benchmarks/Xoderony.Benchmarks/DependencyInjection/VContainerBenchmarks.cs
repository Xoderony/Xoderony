#if VCONTAINER_BENCHMARK
using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using VContainer;
using XContainer = Xoderony.DependencyInjection.Container;

namespace Xoderony.Benchmarks.DependencyInjection;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class VContainerBenchmarks {
    private const int Batch = 1024;
    private XContainer _xoderony = null!;
    private IObjectResolver _vcontainer = null!;
    private readonly Type[] _genericTypes = new Type[Batch];

    [GlobalSetup]
    public void Setup() {
        _xoderony = ContainerBenchmarks.BuildXoderony(false);
        _vcontainer = BuildVContainer();
        var root = _vcontainer.Resolve<Root>();
        if (!ReferenceEquals(root, _vcontainer.Resolve<Root>()) || !ReferenceEquals(root.Left.A, root.Right.A) || !ReferenceEquals(root.Left.B, root.Right.B)) {
            throw new InvalidOperationException("Singleton graph is not shared.");
        }
        _xoderony.Resolve<Root>();
        Type[] types = [typeof(Box<int>), typeof(Box<long>), typeof(Box<float>), typeof(Box<double>)];
        var random = new Random(42);
        for (var i = 0; i < Batch; i++) {
            _genericTypes[i] = types[random.Next(types.Length)];
        }
        foreach (var type in types) {
            var instance = _vcontainer.Resolve(type);
            if (instance.GetType() != type || !ReferenceEquals(instance, _vcontainer.Resolve(type))) {
                throw new InvalidOperationException("Generic singleton resolution failed.");
            }
            _xoderony.Resolve(type);
        }
    }

    [GlobalCleanup]
    public void Cleanup() {
        _xoderony.Dispose();
        _vcontainer.Dispose();
    }

    private static IObjectResolver BuildVContainer() {
        var builder = new ContainerBuilder();
        builder.Register<LeafA>(Lifetime.Singleton);
        builder.Register<LeafB>(Lifetime.Singleton);
        builder.Register<LeafC>(Lifetime.Singleton);
        builder.Register<LeafD>(Lifetime.Singleton);
        builder.Register<Left>(Lifetime.Singleton);
        builder.Register<Right>(Lifetime.Singleton);
        builder.Register<Root>(Lifetime.Singleton);
        builder.Register(typeof(Box<>), Lifetime.Singleton);
        return builder.Build();
    }

    [Benchmark(Baseline = true), BenchmarkCategory("BuildAndFirstResolve")]
    public Root XoderonyBuild() {
        using var container = ContainerBenchmarks.BuildXoderony(false);
        return container.Resolve<Root>();
    }

    [Benchmark, BenchmarkCategory("BuildAndFirstResolve")]
    public Root VContainerBuild() {
        using var container = BuildVContainer();
        return container.Resolve<Root>();
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Batch), BenchmarkCategory("CachedResolve")]
    public Root XoderonyCached() {
        Root result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _xoderony.Resolve<Root>();
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedResolve")]
    public Root VContainerCached() {
        Root result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _vcontainer.Resolve<Root>();
        }
        return result;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Batch), BenchmarkCategory("CachedGenericType")]
    public object XoderonyGenericType() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _xoderony.Resolve(_genericTypes[i]);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedGenericType")]
    public object VContainerGenericType() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _vcontainer.Resolve(_genericTypes[i]);
        }
        return result;
    }
}
#endif
