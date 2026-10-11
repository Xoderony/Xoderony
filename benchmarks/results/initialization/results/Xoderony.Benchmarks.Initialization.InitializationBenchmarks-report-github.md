```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]   : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method             | Categories | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Code Size | Gen0   | Allocated | Alloc Ratio |
|------------------- |----------- |----------:|----------:|----------:|----------:|------:|--------:|----------:|-------:|----------:|------------:|
| CreateConventional | Create     | 6.1442 ns | 4.3951 ns | 0.2409 ns | 6.0367 ns |  1.00 |    0.05 |      91 B | 0.0051 |      48 B |        1.00 |
| CreatePrimary      | Create     | 6.1581 ns | 2.1957 ns | 0.1204 ns | 6.1001 ns |  1.00 |    0.04 |      91 B | 0.0051 |      48 B |        1.00 |
| CreateRequiredInit | Create     | 6.7917 ns | 1.3128 ns | 0.0720 ns | 6.8150 ns |  1.11 |    0.04 |      83 B | 0.0051 |      48 B |        1.00 |
|                    |            |           |           |           |           |       |         |           |        |           |             |
| ReadConventional   | Read       | 0.0116 ns | 0.1393 ns | 0.0076 ns | 0.0151 ns |  1.83 |    2.15 |      31 B |      - |         - |          NA |
| ReadPrimary        | Read       | 0.0125 ns | 0.1580 ns | 0.0087 ns | 0.0144 ns |  1.98 |    2.37 |      31 B |      - |         - |          NA |
| ReadRequiredInit   | Read       | 0.0178 ns | 0.0785 ns | 0.0043 ns | 0.0173 ns |  2.82 |    2.66 |      31 B |      - |         - |          NA |
