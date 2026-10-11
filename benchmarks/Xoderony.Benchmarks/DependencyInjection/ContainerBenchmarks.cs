using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using DryIoc;
using Jab;
using Microsoft.Extensions.DependencyInjection;
using Stashbox;
using XContainer = Xoderony.DependencyInjection.Container;
using XBuilder = Xoderony.DependencyInjection.ContainerBuilder;

namespace Xoderony.Benchmarks.DependencyInjection;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class ContainerBenchmarks {
    private const int Batch = 1024;
    private XContainer _xoderony = null!;
    private XContainer _factory = null!;
    private DryIoc.Container _dry = null!;
    private StashboxContainer _stash = null!;
    private ServiceProvider _microsoft = null!;
    private JabServices _jab = null!;
    private readonly Type[] _genericTypes = new Type[Batch];

    [GlobalSetup]
    public void Setup() {
        _xoderony = BuildXoderony(false);
        _factory = BuildXoderony(true);
        _dry = BuildDryIoc();
        _stash = BuildStashbox();
        _microsoft = BuildMicrosoft();
        _jab = new JabServices();
        Verify(_xoderony.Resolve<Root>(), _xoderony.Resolve<Root>());
        Verify(_factory.Resolve<Root>(), _factory.Resolve<Root>());
        Verify(_dry.Resolve<Root>(), _dry.Resolve<Root>());
        Verify(_stash.Resolve<Root>(), _stash.Resolve<Root>());
        Verify(_microsoft.GetRequiredService<Root>(), _microsoft.GetRequiredService<Root>());
        Verify(_jab.GetService<Root>(), _jab.GetService<Root>());
        Type[] types = [typeof(Box<int>), typeof(Box<long>), typeof(Box<float>), typeof(Box<double>)];
        var random = new Random(42);
        for (var i = 0; i < Batch; i++) {
            _genericTypes[i] = types[random.Next(types.Length)];
        }
        foreach (var type in types) {
            VerifyGeneric(type, _xoderony.Resolve(type), _xoderony.Resolve(type));
            VerifyGeneric(type, _dry.Resolve(type), _dry.Resolve(type));
            VerifyGeneric(type, _stash.Resolve(type), _stash.Resolve(type));
            VerifyGeneric(type, _microsoft.GetRequiredService(type), _microsoft.GetRequiredService(type));
            VerifyGeneric(type, ((IServiceProvider)_jab).GetService(type)!, ((IServiceProvider)_jab).GetService(type)!);
        }
    }

    private static void VerifyGeneric(Type type, object first, object second) {
        if (first.GetType() != type || !ReferenceEquals(first, second)) {
            throw new InvalidOperationException("Generic singleton resolution failed.");
        }
    }

    [GlobalCleanup]
    public void Cleanup() {
        _xoderony.Dispose();
        _factory.Dispose();
        _dry.Dispose();
        _stash.Dispose();
        _microsoft.Dispose();
        _jab.Dispose();
    }

    private static void Verify(Root first, Root second) {
        if (!ReferenceEquals(first, second) || !ReferenceEquals(first.Left.A, first.Right.A) || !ReferenceEquals(first.Left.B, first.Right.B)) {
            throw new InvalidOperationException("Singleton graph is not shared.");
        }
    }

    internal static XContainer BuildXoderony(bool factory) {
        var builder = new XBuilder();
        if (factory) {
            builder.RegisterFactory(static _ => new LeafA());
            builder.RegisterFactory(static _ => new LeafB());
            builder.RegisterFactory(static _ => new LeafC());
            builder.RegisterFactory(static _ => new LeafD());
            builder.RegisterFactory(static r => new Left(r.Resolve<LeafA>(), r.Resolve<LeafB>(), r.Resolve<LeafC>()));
            builder.RegisterFactory(static r => new Right(r.Resolve<LeafA>(), r.Resolve<LeafB>(), r.Resolve<LeafD>()));
            builder.RegisterFactory(static r => new Root(r.Resolve<Left>(), r.Resolve<Right>()));
        } else {
            builder.Register<LeafA>();
            builder.Register<LeafB>();
            builder.Register<LeafC>();
            builder.Register<LeafD>();
            builder.Register<Left>();
            builder.Register<Right>();
            builder.Register<Root>();
        }
        builder.Register<Box<int>>();
        builder.Register<Box<long>>();
        builder.Register<Box<float>>();
        builder.Register<Box<double>>();
        return builder.Build();
    }

    private static DryIoc.Container BuildDryIoc() {
        var container = new DryIoc.Container();
        container.Register<LeafA>(Reuse.Singleton);
        container.Register<LeafB>(Reuse.Singleton);
        container.Register<LeafC>(Reuse.Singleton);
        container.Register<LeafD>(Reuse.Singleton);
        container.Register<Left>(Reuse.Singleton);
        container.Register<Right>(Reuse.Singleton);
        container.Register<Root>(Reuse.Singleton);
        container.Register(typeof(Box<>), typeof(Box<>), Reuse.Singleton);
        return container;
    }

    private static StashboxContainer BuildStashbox() {
        var container = new StashboxContainer();
        container.RegisterSingleton<LeafA>();
        container.RegisterSingleton<LeafB>();
        container.RegisterSingleton<LeafC>();
        container.RegisterSingleton<LeafD>();
        container.RegisterSingleton<Left>();
        container.RegisterSingleton<Right>();
        container.RegisterSingleton<Root>();
        container.RegisterSingleton(typeof(Box<>), typeof(Box<>));
        return container;
    }

    private static ServiceProvider BuildMicrosoft() {
        var services = new ServiceCollection();
        services.AddSingleton<LeafA>();
        services.AddSingleton<LeafB>();
        services.AddSingleton<LeafC>();
        services.AddSingleton<LeafD>();
        services.AddSingleton<Left>();
        services.AddSingleton<Right>();
        services.AddSingleton<Root>();
        services.AddSingleton(typeof(Box<>), typeof(Box<>));
        return services.BuildServiceProvider();
    }

    // 包含注册、构建、首次解析和容器释放；进程及运行时元数据已预热。
    [Benchmark(Baseline = true), BenchmarkCategory("BuildAndFirstResolve")]
    public Root XoderonyBuild() {
        using var container = BuildXoderony(false);
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

    // 包含注册、构建、首次解析和容器释放；进程及运行时元数据已预热。
    [Benchmark, BenchmarkCategory("BuildAndFirstResolve")]
    public Root XoderonyFactoryBuild() {
        using var container = BuildXoderony(true);
        return container.Resolve<Root>();
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedResolve")]
    public Root XoderonyFactoryCached() {
        Root result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _factory.Resolve<Root>();
        }
        return result;
    }

    // 包含注册、构建、首次解析和容器释放；进程及运行时元数据已预热。
    [Benchmark, BenchmarkCategory("BuildAndFirstResolve")]
    public Root DryIocBuild() {
        using var container = BuildDryIoc();
        return container.Resolve<Root>();
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedResolve")]
    public Root DryIocCached() {
        Root result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _dry.Resolve<Root>();
        }
        return result;
    }

    // 包含注册、构建、首次解析和容器释放；进程及运行时元数据已预热。
    [Benchmark, BenchmarkCategory("BuildAndFirstResolve")]
    public Root StashboxBuild() {
        using var container = BuildStashbox();
        return container.Resolve<Root>();
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedResolve")]
    public Root StashboxCached() {
        Root result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _stash.Resolve<Root>();
        }
        return result;
    }

    // 包含注册、构建、首次解析和容器释放；进程及运行时元数据已预热。
    [Benchmark, BenchmarkCategory("BuildAndFirstResolve")]
    public Root MicrosoftBuild() {
        using var container = BuildMicrosoft();
        return container.GetRequiredService<Root>();
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedResolve")]
    public Root MicrosoftCached() {
        Root result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _microsoft.GetRequiredService<Root>();
        }
        return result;
    }

    // 包含注册、构建、首次解析和容器释放；进程及运行时元数据已预热。
    [Benchmark, BenchmarkCategory("BuildAndFirstResolve")]
    public Root JabBuild() {
        using var container = new JabServices();
        return container.GetService<Root>();
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedResolve")]
    public Root JabCached() {
        Root result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _jab.GetService<Root>();
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
    public object DryIocGenericType() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _dry.Resolve(_genericTypes[i]);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedGenericType")]
    public object StashboxGenericType() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _stash.Resolve(_genericTypes[i]);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedGenericType")]
    public object MicrosoftGenericType() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _microsoft.GetRequiredService(_genericTypes[i]);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("CachedGenericType")]
    public object JabGenericType() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = ((IServiceProvider)_jab).GetService(_genericTypes[i])!;
        }
        return result;
    }
}

public sealed class LeafA;
public sealed class LeafB;
public sealed class LeafC;
public sealed class LeafD;
public sealed class Left(LeafA a, LeafB b, LeafC c) {
    public LeafA A { get; } = a;
    public LeafB B { get; } = b;
    public LeafC C { get; } = c;
}
public sealed class Right(LeafA a, LeafB b, LeafD d) {
    public LeafA A { get; } = a;
    public LeafB B { get; } = b;
    public LeafD D { get; } = d;
}
public sealed class Root(Left left, Right right) {
    public Left Left { get; } = left;
    public Right Right { get; } = right;
}
public sealed class Box<T>;

[ServiceProvider(RootServices = new[] { typeof(Box<int>), typeof(Box<long>), typeof(Box<float>), typeof(Box<double>) })]
[Singleton(typeof(LeafA))]
[Singleton(typeof(LeafB))]
[Singleton(typeof(LeafC))]
[Singleton(typeof(LeafD))]
[Singleton(typeof(Left))]
[Singleton(typeof(Right))]
[Singleton(typeof(Root))]
[Singleton(typeof(Box<>), typeof(Box<>))]
internal partial class JabServices;
