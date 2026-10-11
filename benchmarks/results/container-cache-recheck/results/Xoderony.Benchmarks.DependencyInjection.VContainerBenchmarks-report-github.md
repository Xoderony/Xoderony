```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-EWNVSB : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=10  IterationTime=500ms  WarmupCount=5  

```
| Method                | Categories        | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------- |------------------ |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| XoderonyGenericType   | CachedGenericType | 29.288 ns | 1.7997 ns | 1.1904 ns |  1.00 |    0.06 |      - |         - |          NA |
| VContainerGenericType | CachedGenericType | 34.908 ns | 1.7631 ns | 1.0492 ns |  1.19 |    0.06 | 0.0034 |      32 B |          NA |
|                       |                   |           |           |           |       |         |        |           |             |
| XoderonyCached        | CachedResolve     |  8.038 ns | 0.4530 ns | 0.2996 ns |  1.00 |    0.05 |      - |         - |          NA |
| VContainerCached      | CachedResolve     |  6.640 ns | 0.5467 ns | 0.3616 ns |  0.83 |    0.05 |      - |         - |          NA |
