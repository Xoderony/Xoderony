using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Xoderony.Benchmarks.DependencyInjection;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class RegistryDictionaryBenchmarks {
    private const int QueryCount = 1024;
    private Dictionary<(Type ServiceType, object? Key), List<Lazy<object>>> _dictionary = null!;
    private FrozenDictionary<(Type ServiceType, object? Key), List<Lazy<object>>> _frozen = null!;
    private (Type ServiceType, object? Key)[] _queries = null!;

    [Params(8, 64, 512)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup() {
        Type[] types = [typeof(byte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(string)];
        _dictionary = [];
        var keys = new (Type ServiceType, object? Key)[Count];
        for (var i = 0; i < Count; i++) {
            var serviceType = typeof(Service<,,>).MakeGenericType(types[i % 8], types[i / 8 % 8], types[i / 64 % 8]);
            object? key = i % 3 == 0 ? null : i % 3 == 1 ? "key-" + i : (object)i;
            keys[i] = (serviceType, key);
            _dictionary.Add(keys[i], [new Lazy<object>(new object())]);
        }
        _frozen = _dictionary.ToFrozenDictionary();
        _queries = new (Type ServiceType, object? Key)[QueryCount];
        var random = new Random(42);
        for (var i = 0; i < QueryCount; i++) {
            var key = keys[random.Next(keys.Length)];
            // 精确类型未命中也是开放泛型解析的常见前置操作。
            _queries[i] = i % 4 == 0 ? (key.ServiceType, "missing") : key;
        }
        if (DictionaryLookup() != 768 || FrozenLookup() != 768) {
            throw new InvalidOperationException("Lookup results differ from expected hit count.");
        }
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = QueryCount), BenchmarkCategory("Lookup")]
    public int DictionaryLookup() {
        var sum = 0;
        foreach (var key in _queries) {
            if (_dictionary.TryGetValue(key, out var values)) {
                sum += values.Count;
            }
        }
        return sum;
    }

    [Benchmark(OperationsPerInvoke = QueryCount), BenchmarkCategory("Lookup")]
    public int FrozenLookup() {
        var sum = 0;
        foreach (var key in _queries) {
            if (_frozen.TryGetValue(key, out var values)) {
                sum += values.Count;
            }
        }
        return sum;
    }

    // 构建期已有可变字典；测量转换为冻结字典新增的时间和分配。
    [Benchmark, BenchmarkCategory("Freeze")]
    public FrozenDictionary<(Type ServiceType, object? Key), List<Lazy<object>>> Freeze() => _dictionary.ToFrozenDictionary();

    private sealed class Service<T1, T2, T3>;
}
