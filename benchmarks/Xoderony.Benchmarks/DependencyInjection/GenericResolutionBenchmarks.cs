using System;
using BenchmarkDotNet.Attributes;
using Xoderony.DependencyInjection;

namespace Xoderony.Benchmarks.DependencyInjection;

[MemoryDiagnoser]
public class GenericResolutionBenchmarks {
    private const int Batch = 1024;
    private Container _container = null!;
    private readonly Type[] _types = new Type[Batch];

    [GlobalSetup]
    public void Setup() {
        var builder = new ContainerBuilder();
        foreach (var type in new[] { typeof(int), typeof(long), typeof(float), typeof(double) }) {
            builder.Register(typeof(Value<>).MakeGenericType(type)).As(typeof(IValue<>).MakeGenericType(type)).AsSelf().WithKey(1);
        }
        _container = builder.Build();
        Type[] types = [typeof(IValue<int>), typeof(IValue<long>), typeof(IValue<float>), typeof(IValue<double>)];
        var random = new Random(42);
        for (var i = 0; i < Batch; i++) {
            _types[i] = types[random.Next(types.Length)];
        }
        foreach (var type in types) {
            _container.Resolve(type, 1);
        }
    }

    [GlobalCleanup]
    public void Cleanup() => _container.Dispose();

    [Benchmark(OperationsPerInvoke = Batch)]
    public object CachedKeyedAliases() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _container.Resolve(_types[i], 1);
        }
        return result;
    }

    // 包含注册、构建、首次展开与释放，运行时元数据已预热。
    [Benchmark]
    public object BuildAndFirstGeneric() {
        using var container = Build(1);
        return container.Resolve<IValue<int>>(1);
    }

    [Benchmark]
    public object BuildAndBothAliases() {
        using var container = Build(1);
        container.Resolve<IValue<int>>(1);
        return container.Resolve<Value<int>>(1);
    }

    [Benchmark]
    public object BuildMultipleAndResolveConcrete() {
        using var container = Build(4);
        return container.Resolve<Value<int>>(1);
    }

    // 父层已预热；只测新增子层及其请求缓存，实例仍归父层。
    [Benchmark]
    public object InheritedScopeFirstGeneric() {
        using var child = _container.CreateChild();
        return child.Resolve<IValue<int>>(1);
    }

    private static Container Build(int count) {
        var builder = new ContainerBuilder();
        builder.Register<Value<int>>().As<IValue<int>>().AsSelf().WithKey(1);
        if (count == 4) {
            builder.Register(typeof(SecondValue<int>)).As<IValue<int>>().WithKey(1);
            builder.Register(typeof(ThirdValue<int>)).As<IValue<int>>().WithKey(1);
            builder.Register(typeof(FourthValue<int>)).As<IValue<int>>().WithKey(1);
            builder.Register(typeof(CollectionValue<int>)).As<IValue<int>>().WithKey(1);
        }
        return builder.Build();
    }

    public interface IValue<T>;
    public sealed class Value<T> : IValue<T>;
    public sealed class SecondValue<T> : IValue<T>;
    public sealed class ThirdValue<T> : IValue<T>;
    public sealed class FourthValue<T> : IValue<T>;
    public sealed class CollectionValue<T> : IValue<T>;
}
