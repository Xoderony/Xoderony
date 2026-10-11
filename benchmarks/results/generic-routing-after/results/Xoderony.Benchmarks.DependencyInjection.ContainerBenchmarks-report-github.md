```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-UWQVVG : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  WarmupCount=3  

```
| Method              | Categories           | Mean         | Error       | StdDev      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------- |--------------------- |-------------:|------------:|------------:|------:|--------:|-------:|----------:|------------:|
| XoderonyBuild       | BuildAndFirstResolve | 8,062.193 ns | 567.5106 ns | 296.8190 ns |  1.00 |    0.05 | 0.9865 |    9848 B |        1.00 |
|                     |                      |              |             |             |       |         |        |           |             |
| XoderonyGenericType | CachedGenericType    |     9.016 ns |   0.7409 ns |   0.3875 ns |  1.00 |    0.06 |      - |         - |          NA |
|                     |                      |              |             |             |       |         |        |           |             |
| XoderonyCached      | CachedResolve        |     8.390 ns |   0.5823 ns |   0.3046 ns |  1.00 |    0.05 |      - |         - |          NA |
