```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-JESZGN : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  LaunchCount=2  
WarmupCount=3  

```
| Method             | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| Integer            | 12.038 ns | 0.3401 ns | 0.3340 ns |  1.00 |    0.04 |         - |          NA |
| Enum               | 12.463 ns | 0.5124 ns | 0.4793 ns |  1.04 |    0.05 |         - |          NA |
| RuntimeTypeAndEnum |  6.505 ns | 0.0791 ns | 0.0701 ns |  0.54 |    0.02 |         - |          NA |
