```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-UWQVVG : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  WarmupCount=3  

```
| Method                     | Mean        | Error      | StdDev     | Gen0   | Allocated |
|--------------------------- |------------:|-----------:|-----------:|-------:|----------:|
| CachedKeyedAliases         |    40.49 ns |   2.756 ns |   1.442 ns |      - |         - |
| BuildAndFirstGeneric       | 1,390.27 ns | 114.847 ns |  60.067 ns | 0.2591 |    2464 B |
| BuildAndBothAliases        | 1,415.84 ns | 152.307 ns |  79.659 ns | 0.2578 |    2464 B |
| BuildFourAndResolveLast    | 2,678.18 ns | 386.539 ns | 202.167 ns | 0.4067 |    3856 B |
| InheritedScopeFirstGeneric |   185.52 ns |  17.601 ns |   9.206 ns | 0.0739 |     696 B |
