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
| XoderonyBuild       | BuildAndFirstResolve | 8,191.469 ns | 464.5819 ns | 242.9853 ns |  1.00 |    0.04 | 1.0326 |    9824 B |        1.00 |
|                     |                      |              |             |             |       |         |        |           |             |
| XoderonyGenericType | CachedGenericType    |     8.259 ns |   0.9334 ns |   0.4882 ns |  1.00 |    0.08 |      - |         - |          NA |
|                     |                      |              |             |             |       |         |        |           |             |
| XoderonyCached      | CachedResolve        |     7.342 ns |   0.4097 ns |   0.2143 ns |  1.00 |    0.04 |      - |         - |          NA |
