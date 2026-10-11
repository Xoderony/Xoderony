```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-EWNVSB : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=10  IterationTime=500ms  WarmupCount=5  

```
| Method                | Categories        | Mean       | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------- |------------------ |-----------:|----------:|----------:|------:|--------:|----------:|------------:|
| XoderonyGenericType   | CachedGenericType | 24.4049 ns | 0.6852 ns | 0.4532 ns |  1.00 |    0.03 |         - |          NA |
| DryIocGenericType     | CachedGenericType |  9.2693 ns | 0.3212 ns | 0.2124 ns |  0.38 |    0.01 |         - |          NA |
| StashboxGenericType   | CachedGenericType | 11.1890 ns | 0.4672 ns | 0.3090 ns |  0.46 |    0.01 |         - |          NA |
| MicrosoftGenericType  | CachedGenericType |  4.7022 ns | 0.3615 ns | 0.2391 ns |  0.19 |    0.01 |         - |          NA |
| JabGenericType        | CachedGenericType |  2.1505 ns | 0.1253 ns | 0.0829 ns |  0.09 |    0.00 |         - |          NA |
|                       |                   |            |           |           |       |         |           |             |
| XoderonyCached        | CachedResolve     |  7.8813 ns | 0.3554 ns | 0.2351 ns |  1.00 |    0.04 |         - |          NA |
| XoderonyFactoryCached | CachedResolve     |  7.5764 ns | 0.4890 ns | 0.3234 ns |  0.96 |    0.05 |         - |          NA |
| DryIocCached          | CachedResolve     |  6.3827 ns | 0.4031 ns | 0.2666 ns |  0.81 |    0.04 |         - |          NA |
| StashboxCached        | CachedResolve     |  9.0882 ns | 0.2445 ns | 0.1617 ns |  1.15 |    0.04 |         - |          NA |
| MicrosoftCached       | CachedResolve     |  4.3682 ns | 0.3288 ns | 0.2175 ns |  0.55 |    0.03 |         - |          NA |
| JabCached             | CachedResolve     |  0.4801 ns | 0.0220 ns | 0.0146 ns |  0.06 |    0.00 |         - |          NA |
