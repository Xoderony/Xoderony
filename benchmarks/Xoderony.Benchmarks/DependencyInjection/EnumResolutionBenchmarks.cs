using BenchmarkDotNet.Attributes;
using Xoderony.DependencyInjection;

namespace Xoderony.Benchmarks.DependencyInjection;

[MemoryDiagnoser]
public class EnumResolutionBenchmarks {
    private const int Batch = 1024;
    private Container _container = null!;
    private IResolver _resolver = null!;

    [GlobalSetup]
    public void Setup() {
        var builder = new ContainerBuilder();
        builder.Register<Service>().WithKey(ServiceKey.Named);
        _container = builder.Build();
        _resolver = _container;
        _resolver.Resolve<Service, ServiceKey>(ServiceKey.Named);
    }

    [GlobalCleanup]
    public void Cleanup() => _container.Dispose();

    [Benchmark(Baseline = true, OperationsPerInvoke = Batch)]
    public object Integer() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _resolver.Resolve<Service>(7);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch)]
    public object Enum() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _resolver.Resolve<Service, ServiceKey>(ServiceKey.Named);
        }
        return result;
    }

    [Benchmark(OperationsPerInvoke = Batch)]
    public object RuntimeTypeAndEnum() {
        object result = null!;
        for (var i = 0; i < Batch; i++) {
            result = _resolver.Resolve(typeof(Service), ServiceKey.Named);
        }
        return result;
    }

    private enum ServiceKey { Named = 7 }
    private sealed class Service;
}
