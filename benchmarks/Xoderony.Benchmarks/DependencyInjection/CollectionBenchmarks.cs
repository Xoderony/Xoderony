using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Xoderony.DependencyInjection;

namespace Xoderony.Benchmarks.DependencyInjection;

[MemoryDiagnoser]
public class CollectionBenchmarks {
    private const int Batch = 1024;
    private Container _container = null!;
    private readonly Type[] _types = new Type[Batch];

    [GlobalSetup]
    public void Setup() {
        _container = Build();
        var handlers = _container.Resolve<IReadOnlyList<IHandler>>();
        if (!ReferenceEquals(handlers[0], _container.Resolve<First>()) || !ReferenceEquals(handlers, _container.Resolve<IEnumerable<IHandler>>())) {
            throw new InvalidOperationException("Collection instances are not shared.");
        }
        Type[] types = [typeof(IReadOnlyList<IValue<int>>), typeof(IReadOnlyList<IValue<long>>), typeof(IReadOnlyList<IValue<float>>), typeof(IReadOnlyList<IValue<double>>)];
        var random = new Random(42);
        for (var i = 0; i < Batch; i++) {
            _types[i] = types[random.Next(types.Length)];
        }
        foreach (var type in types) {
            _container.Resolve(type);
        }
    }

    [GlobalCleanup]
    public void Cleanup() => _container.Dispose();

    [Benchmark(OperationsPerInvoke = Batch)]
    public object CachedReadOnlyCollection() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _container.Resolve<IReadOnlyList<IHandler>>();
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch)]
    public object CachedGenericCollection() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _container.Resolve(_types[i]);
        }
        return result;
    }

    [Benchmark]
    public IHandler[] ArrayCopy() => _container.Resolve<IHandler[]>();

    [Benchmark]
    public object BuildAndFirstGenericCollection() {
        using var container = Build();
        return container.Resolve<IReadOnlyList<IValue<int>>>();
    }

    private static Container Build() {
        var builder = new ContainerBuilder();
        builder.Register<First>().As<IHandler>().AsSelf();
        builder.Register<Second>().As<IHandler>().AsSelf();
        foreach (var type in new[] { typeof(int), typeof(long), typeof(float), typeof(double) }) {
            builder.Register(typeof(FirstValue<>).MakeGenericType(type)).As(typeof(IValue<>).MakeGenericType(type));
            builder.Register(typeof(SecondValue<>).MakeGenericType(type)).As(typeof(IValue<>).MakeGenericType(type));
        }
        return builder.Build();
    }

    public interface IHandler;
    public sealed class First : IHandler;
    public sealed class Second : IHandler;
    public interface IValue<T>;
    public sealed class FirstValue<T> : IValue<T>;
    public sealed class SecondValue<T> : IValue<T>;
}
