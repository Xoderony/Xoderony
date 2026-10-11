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
| CachedKeyedAliases                    |    14.97 ns |   0.652 ns |   0.289 ns |      - |         - |
| BuildAndFirstGeneric                  | 1,478.02 ns | 113.601 ns |  50.440 ns | 0.3165 |    3024 B |
| BuildAndBothAliases                   | 1,780.29 ns | 224.504 ns | 117.420 ns | 0.3344 |    3160 B |
| BuildFourContributorsAndResolveSingle | 2,975.34 ns | 294.317 ns | 153.934 ns | 0.5824 |    5522 B |
| InheritedScopeFirstGeneric            |   512.71 ns |  20.751 ns |   7.400 ns | 0.1757 |    1656 B |
