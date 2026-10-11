```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-UWQVVG : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  WarmupCount=3  

```
| Method                          | Mean        | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|-------------------------------- |------------:|----------:|----------:|-------:|-------:|----------:|
| CachedKeyedAliases              |    14.67 ns |  1.604 ns |  0.712 ns |      - |      - |         - |
| BuildAndFirstGeneric            | 1,664.51 ns | 58.087 ns | 30.381 ns | 0.4207 |      - |    3993 B |
| BuildAndBothAliases             | 1,669.08 ns | 82.140 ns | 42.961 ns | 0.4197 |      - |    3993 B |
| BuildMultipleAndResolveConcrete | 2,175.67 ns | 96.218 ns | 50.324 ns | 0.6156 | 0.0071 |    5834 B |
| InheritedScopeFirstGeneric      | 1,027.42 ns | 44.266 ns | 23.152 ns | 0.3576 |      - |    3376 B |
