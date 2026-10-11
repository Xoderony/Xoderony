```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-UWQVVG : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  WarmupCount=3  

```
| Method                     | Mean        | Error      | StdDev    | Gen0   | Allocated |
|--------------------------- |------------:|-----------:|----------:|-------:|----------:|
| CachedKeyedAliases         |    15.26 ns |   0.929 ns |  0.486 ns |      - |         - |
| BuildAndFirstGeneric       | 1,384.73 ns | 131.041 ns | 68.537 ns | 0.3090 |    2936 B |
| BuildAndBothAliases        | 1,467.53 ns |  74.239 ns | 38.829 ns | 0.3325 |    3136 B |
| BuildFourAndResolveLast    | 2,598.99 ns | 141.043 ns | 73.768 ns | 0.4987 |    4761 B |
| InheritedScopeFirstGeneric |   276.83 ns |  23.771 ns | 12.433 ns | 0.1235 |    1168 B |
