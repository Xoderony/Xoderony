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
| XoderonyBuild       | BuildAndFirstResolve | 8,022.764 ns | 1,162.3440 ns | 607.9284 ns |  1.01 |    0.10 | 1.0174 |    9760 B |        1.00 |
|                     |                      |              |               |             |       |         |        |           |             |
| XoderonyGenericType | CachedGenericType    |    29.429 ns |     2.7139 ns |   1.4194 ns |  1.00 |    0.06 |      - |         - |          NA |
|                     |                      |              |               |             |       |         |        |           |             |
| XoderonyCached      | CachedResolve        |     7.598 ns |     0.3943 ns |   0.2062 ns |  1.00 |    0.04 |      - |         - |          NA |
