```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-UWQVVG : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  WarmupCount=3  

```
| Method                                | Mean        | Error      | StdDev     | Gen0   | Allocated |
|-------------------------------------- |------------:|-----------:|-----------:|-------:|----------:|
| CachedKeyedAliases                    |    14.65 ns |   1.176 ns |   0.615 ns |      - |         - |
| BuildAndFirstGeneric                  | 1,552.62 ns | 146.492 ns |  76.618 ns | 0.3168 |    3024 B |
| BuildAndBothAliases                   | 1,614.04 ns | 122.076 ns |  63.848 ns | 0.3309 |    3160 B |
| BuildFourContributorsAndResolveSingle | 3,022.31 ns | 274.640 ns | 143.642 ns | 0.5834 |    5522 B |
| InheritedScopeFirstGeneric            |   514.46 ns |  33.036 ns |  17.279 ns | 0.1753 |    1656 B |
