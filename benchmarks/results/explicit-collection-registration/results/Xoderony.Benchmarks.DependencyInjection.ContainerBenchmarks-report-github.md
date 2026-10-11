```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-UWQVVG : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  WarmupCount=3  

```
| Method              | Categories           | Mean         | Error         | StdDev      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------- |--------------------- |-------------:|--------------:|------------:|------:|--------:|-------:|----------:|------------:|
| XoderonyBuild       | BuildAndFirstResolve | 8,322.339 ns | 2,060.7168 ns | 914.9710 ns |  1.01 |    0.14 | 0.9969 |    9824 B |        1.00 |
|                     |                      |              |               |             |       |         |        |           |             |
| XoderonyGenericType | CachedGenericType    |     8.098 ns |     0.3960 ns |   0.1758 ns |  1.00 |    0.03 |      - |         - |          NA |
|                     |                      |              |               |             |       |         |        |           |             |
| XoderonyCached      | CachedResolve        |     7.735 ns |     0.5705 ns |   0.2984 ns |  1.00 |    0.05 |      - |         - |          NA |
