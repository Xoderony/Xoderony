```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-JESZGN : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  LaunchCount=2  
WarmupCount=3  

```
| Method         | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| Assertions     |  7.228 ns | 0.2999 ns | 0.2945 ns |  1.00 |    0.05 |         - |          NA |
| NullCheck      |  7.359 ns | 0.4246 ns | 0.3972 ns |  1.02 |    0.07 |         - |          NA |
| FullTypeChecks | 15.043 ns | 0.4116 ns | 0.3850 ns |  2.08 |    0.09 |         - |          NA |
