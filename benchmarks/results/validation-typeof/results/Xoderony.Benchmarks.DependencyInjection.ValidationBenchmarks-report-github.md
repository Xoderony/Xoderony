```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-JESZGN : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  LaunchCount=2  
WarmupCount=3  

```
| Method                | Mean      | Error     | StdDev    | Ratio | RatioSD | Code Size | Allocated | Alloc Ratio |
|---------------------- |----------:|----------:|----------:|------:|--------:|----------:|----------:|------------:|
| Assertions            |  7.898 ns | 0.6232 ns | 0.6121 ns |  1.01 |    0.11 |   3,230 B |         - |          NA |
| NullCheck             |  7.503 ns | 0.1418 ns | 0.1393 ns |  0.96 |    0.07 |   3,251 B |         - |          NA |
| FullTypeChecks        | 15.740 ns | 0.5425 ns | 0.5075 ns |  2.00 |    0.16 |   4,017 B |         - |          NA |
| GenericAssertions     |  6.690 ns | 0.1778 ns | 0.1664 ns |  0.85 |    0.07 |   3,378 B |         - |          NA |
| GenericNullCheck      |  6.570 ns | 0.0914 ns | 0.0811 ns |  0.84 |    0.06 |   3,378 B |         - |          NA |
| GenericFullTypeChecks | 12.127 ns | 0.3280 ns | 0.3221 ns |  1.54 |    0.12 |   3,954 B |         - |          NA |
