```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-EWNVSB : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=10  IterationTime=500ms  WarmupCount=5  

```
| Method                | Categories           | Mean         | Error       | StdDev      | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|---------------------- |--------------------- |-------------:|------------:|------------:|------:|--------:|-------:|-------:|----------:|------------:|
| XoderonyBuild         | BuildAndFirstResolve | 7,612.520 ns | 335.7467 ns | 222.0757 ns |  1.00 |    0.04 | 1.0325 | 0.0161 |    9760 B |        1.00 |
| VContainerBuild       | BuildAndFirstResolve | 3,398.475 ns | 150.4417 ns |  99.5079 ns |  0.45 |    0.02 | 0.9219 | 0.0126 |    8712 B |        0.89 |
|                       |                      |              |             |             |       |         |        |        |           |             |
| XoderonyGenericType   | CachedGenericType    |   190.742 ns |  16.0990 ns |  10.6485 ns |  1.00 |    0.08 | 0.0102 |      - |      96 B |        1.00 |
| VContainerGenericType | CachedGenericType    |    32.144 ns |   2.2501 ns |   1.4883 ns |  0.17 |    0.01 | 0.0034 |      - |      32 B |        0.33 |
|                       |                      |              |             |             |       |         |        |        |           |             |
| XoderonyCached        | CachedResolve        |     7.446 ns |   0.5138 ns |   0.3399 ns |  1.00 |    0.06 |      - |      - |         - |          NA |
| VContainerCached      | CachedResolve        |     6.794 ns |   0.4263 ns |   0.2820 ns |  0.91 |    0.05 |      - |      - |         - |          NA |
