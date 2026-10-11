using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Xoderony.Benchmarks.DependencyInjection;

[MemoryDiagnoser]
public class DictionaryKeyBenchmarks {
    private const int EntryCount = 64;
    private const int QueryCount = 1024;
    private const int Key = 7;
    private const string StringKey = "service";
    private readonly object _boxedKey = Key;
    private readonly Dictionary<Type, object> _types = new(EntryCount);
    private readonly Dictionary<int, object> _ids = new(EntryCount);
    private readonly Dictionary<(Type, int), object> _integers = new(EntryCount);
    private readonly Dictionary<(Type, string), object> _strings = new(EntryCount);
    private readonly Dictionary<(Type, ServiceKey), object> _enums = new(EntryCount);
    private readonly Dictionary<(Type, object?), object> _nulls = new(EntryCount);
    private readonly Dictionary<(Type, object?), object> _objects = new(EntryCount);
    private Type[] _queries = null!;
    private int[] _idQueries = null!;

    [GlobalSetup]
    public void Setup() {
        Type[] arguments = [typeof(byte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(string)];
        var types = new Type[EntryCount];
        for (var i = 0; i < types.Length; i++) {
            var type = typeof(Service<,>).MakeGenericType(arguments[i % 8], arguments[i / 8]);
            var value = new object();
            types[i] = type;
            _types.Add(type, value);
            _ids.Add(i, value);
            _integers.Add((type, Key), value);
            _strings.Add((type, StringKey), value);
            _enums.Add((type, ServiceKey.Default), value);
            _nulls.Add((type, null), value);
            _objects.Add((type, _boxedKey), value);
        }
        _queries = new Type[QueryCount];
        _idQueries = new int[QueryCount];
        var random = new Random(42);
        for (var i = 0; i < _queries.Length; i++) {
            var index = random.Next(types.Length);
            _queries[i] = types[index];
            _idQueries[i] = index;
        }
        if (IntOnly() != QueryCount || TypeOnly() != QueryCount || TypeAndInt() != QueryCount || TypeAndString() != QueryCount || TypeAndEnum() != QueryCount || TypeAndNull() != QueryCount || TypeAndObject() != QueryCount) {
            throw new InvalidOperationException("Lookup results differ from expected hit count.");
        }
        for (var i = 0; i < types.Length; i++) {
            var type = types[i];
            if (!ReferenceEquals(_types[type], _ids[i]) ||
                !ReferenceEquals(_types[type], _integers[(type, Key)]) ||
                !ReferenceEquals(_types[type], _strings[(type, StringKey)]) ||
                !ReferenceEquals(_types[type], _enums[(type, ServiceKey.Default)]) ||
                !ReferenceEquals(_types[type], _nulls[(type, null)]) ||
                !ReferenceEquals(_types[type], _objects[(type, _boxedKey)])) {
                throw new InvalidOperationException("Lookup values differ between key representations.");
            }
        }
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = QueryCount)]
    public int TypeOnly() {
        var hits = 0;
        foreach (var type in _queries) {
            if (_types.TryGetValue(type, out var value) && value is not null) {
                hits++;
            }
        }
        return hits;
    }

    [Benchmark(OperationsPerInvoke = QueryCount)]
    public int IntOnly() {
        var hits = 0;
        foreach (var id in _idQueries) {
            if (_ids.TryGetValue(id, out var value) && value is not null) {
                hits++;
            }
        }
        return hits;
    }

    [Benchmark(OperationsPerInvoke = QueryCount)]
    public int TypeAndInt() {
        var hits = 0;
        foreach (var type in _queries) {
            if (_integers.TryGetValue((type, Key), out var value) && value is not null) {
                hits++;
            }
        }
        return hits;
    }

    [Benchmark(OperationsPerInvoke = QueryCount)]
    public int TypeAndString() {
        var hits = 0;
        foreach (var type in _queries) {
            if (_strings.TryGetValue((type, StringKey), out var value) && value is not null) {
                hits++;
            }
        }
        return hits;
    }

    [Benchmark(OperationsPerInvoke = QueryCount)]
    public int TypeAndEnum() {
        var hits = 0;
        foreach (var type in _queries) {
            if (_enums.TryGetValue((type, ServiceKey.Default), out var value) && value is not null) {
                hits++;
            }
        }
        return hits;
    }

    [Benchmark(OperationsPerInvoke = QueryCount)]
    public int TypeAndNull() {
        var hits = 0;
        foreach (var type in _queries) {
            if (_nulls.TryGetValue((type, null), out var value) && value is not null) {
                hits++;
            }
        }
        return hits;
    }

    [Benchmark(OperationsPerInvoke = QueryCount)]
    public int TypeAndObject() {
        var hits = 0;
        foreach (var type in _queries) {
            // 复用已装箱键，避免把每次装箱的成本混入字典查询。
            if (_objects.TryGetValue((type, _boxedKey), out var value) && value is not null) {
                hits++;
            }
        }
        return hits;
    }

    private enum ServiceKey {
        Default = Key
    }

    private sealed class Service<T1, T2>;
}
