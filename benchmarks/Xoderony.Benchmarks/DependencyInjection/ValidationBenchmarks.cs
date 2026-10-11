using System;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using Xoderony.DependencyInjection;

namespace Xoderony.Benchmarks.DependencyInjection;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 4)]
public class ValidationBenchmarks {
    private const int Batch = 1024;
    private Container _container = null!;
    private Type[] _queries = null!;

    [GlobalSetup]
    public void Setup() {
        Type[] arguments = [typeof(byte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(string)];
        var types = new Type[64];
        var builder = new ContainerBuilder();
        for (var i = 0; i < types.Length; i++) {
            types[i] = typeof(Service<,>).MakeGenericType(arguments[i % 8], arguments[i / 8]);
            builder.Register(types[i]);
        }
        _container = builder.Build();
        if (!ReferenceEquals(ResolveWithAssertions<Service<int, int>>(), ResolveWithFullChecks<Service<int, int>>()) ||
            !ReferenceEquals(ResolveWithAssertions<Service<int, int>>(), ResolveWithNullCheck<Service<int, int>>())) {
            throw new InvalidOperationException("Generic validation paths returned different instances.");
        }
        foreach (var type in types) {
            var instance = ResolveWithAssertions(type);
            if (!ReferenceEquals(instance, ResolveWithNullCheck(type)) || !ReferenceEquals(instance, ResolveWithFullChecks(type))) {
                throw new InvalidOperationException("Validation paths returned different instances.");
            }
        }
        _queries = new Type[Batch];
        var random = new Random(42);
        for (var i = 0; i < Batch; i++) {
            _queries[i] = types[random.Next(types.Length)];
        }
    }

    [GlobalCleanup]
    public void Cleanup() => _container.Dispose();

    [Benchmark(Baseline = true, OperationsPerInvoke = Batch)]
    public object Assertions() {
        object result = null!;
        foreach (var type in _queries) {
            result = ResolveWithAssertions(type);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch)]
    public object NullCheck() {
        object result = null!;
        foreach (var type in _queries) {
            result = ResolveWithNullCheck(type);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch)]
    public object FullTypeChecks() {
        object result = null!;
        foreach (var type in _queries) {
            result = ResolveWithFullChecks(type);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch)]
    public object GenericAssertions() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = ResolveWithAssertions<Service<int, int>>();
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch)]
    public object GenericNullCheck() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = ResolveWithNullCheck<Service<int, int>>();
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch)]
    public object GenericFullTypeChecks() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = ResolveWithFullChecks<Service<int, int>>();
        }
        return result;
    }

    private object ResolveWithAssertions<T>() where T : class => ResolveWithAssertions(typeof(T));

    private object ResolveWithNullCheck<T>() where T : class => ResolveWithNullCheck(typeof(T));

    private object ResolveWithFullChecks<T>() where T : class => ResolveWithFullChecks(typeof(T));

    private object ResolveWithAssertions(Type serviceType) {
        Debug.Assert(serviceType is not null);
        Debug.Assert(!serviceType.ContainsGenericParameters);
        Debug.Assert(!serviceType.IsByRef && !serviceType.IsPointer && !serviceType.IsByRefLike && serviceType != typeof(void));
        return _container.Resolve(serviceType);
    }

    private object ResolveWithNullCheck(Type serviceType) {
        ArgumentNullException.ThrowIfNull(serviceType);
        return _container.Resolve(serviceType);
    }

    private object ResolveWithFullChecks(Type serviceType) {
        ArgumentNullException.ThrowIfNull(serviceType);
        if (serviceType.ContainsGenericParameters) {
            throw new ArgumentException("Resolution requires a closed service type.", nameof(serviceType));
        }
        if (serviceType.IsByRef || serviceType.IsPointer || serviceType.IsByRefLike || serviceType == typeof(void)) {
            throw new ArgumentException("Unsupported service type.", nameof(serviceType));
        }
        return _container.Resolve(serviceType);
    }

    private sealed class Service<T1, T2>;
}
