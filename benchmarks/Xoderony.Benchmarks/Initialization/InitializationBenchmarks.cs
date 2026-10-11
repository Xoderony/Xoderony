using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Xoderony.Benchmarks.Initialization;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 3)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class InitializationBenchmarks {
    private readonly List<object> _fields = [new object()];
    private readonly List<object> _properties = [new object(), new object()];
    private readonly List<object> _methods = [new object(), new object(), new object()];
    private Conventional _conventional = null!;
    private Primary _primary = null!;
    private RequiredInit _requiredInit = null!;

    [GlobalSetup]
    public void Setup() {
        _conventional = CreateConventional();
        _primary = CreatePrimary();
        _requiredInit = CreateRequiredInit();
        if (_conventional.Read() != 13 || _primary.Read() != 13 || _requiredInit.Read() != 13) {
            throw new InvalidOperationException("Initialization produced incorrect state.");
        }
    }

    // 返回对象使创建结果逃逸，避免分配被消除；列表由所有写法复用。
    [Benchmark(Baseline = true), BenchmarkCategory("Create")]
    public Conventional CreateConventional() => new(_fields, _properties, _methods, 7);

    [Benchmark, BenchmarkCategory("Create")]
    public Primary CreatePrimary() => new(_fields, _properties, _methods, 7);

    [Benchmark, BenchmarkCategory("Create")]
    public RequiredInit CreateRequiredInit() => new() { Fields = _fields, Properties = _properties, Methods = _methods, MaxParameterCount = 7 };

    // 使用已存在对象，隔离初始化与读取成本。
    [Benchmark(Baseline = true), BenchmarkCategory("Read")]
    public int ReadConventional() => _conventional.Read();

    [Benchmark, BenchmarkCategory("Read")]
    public int ReadPrimary() => _primary.Read();

    [Benchmark, BenchmarkCategory("Read")]
    public int ReadRequiredInit() => _requiredInit.Read();

    public sealed class Conventional {
        private readonly List<object> _fields;
        private readonly List<object> _properties;
        private readonly List<object> _methods;
        private readonly int _maxParameterCount;

        public Conventional(List<object> fields, List<object> properties, List<object> methods, int maxParameterCount) {
            _fields = fields;
            _properties = properties;
            _methods = methods;
            _maxParameterCount = maxParameterCount;
        }

        public int Read() => _fields.Count + _properties.Count + _methods.Count + _maxParameterCount;
    }

    public sealed class Primary(List<object> fields, List<object> properties, List<object> methods, int maxParameterCount) {
        private readonly List<object> _fields = fields;
        private readonly List<object> _properties = properties;
        private readonly List<object> _methods = methods;
        private readonly int _maxParameterCount = maxParameterCount;

        public int Read() => _fields.Count + _properties.Count + _methods.Count + _maxParameterCount;
    }

    public sealed class RequiredInit {
        public required List<object> Fields { get; init; }
        public required List<object> Properties { get; init; }
        public required List<object> Methods { get; init; }
        public required int MaxParameterCount { get; init; }

        public int Read() => Fields.Count + Properties.Count + Methods.Count + MaxParameterCount;
    }
}
