```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-JESZGN : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  LaunchCount=2  
WarmupCount=3  

```
| Method        | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|-------------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| TypeOnly      |  3.890 ns | 0.0904 ns | 0.0845 ns |  1.00 |    0.03 |         - |          NA |
| IntOnly       |  1.839 ns | 0.0426 ns | 0.0418 ns |  0.47 |    0.01 |         - |          NA |
| TypeAndInt    |  5.772 ns | 0.1530 ns | 0.1503 ns |  1.48 |    0.05 |         - |          NA |
| TypeAndString | 11.167 ns | 0.2259 ns | 0.2219 ns |  2.87 |    0.08 |         - |          NA |
| TypeAndEnum   |  5.658 ns | 0.1291 ns | 0.1268 ns |  1.46 |    0.04 |         - |          NA |
| TypeAndNull   |  5.800 ns | 0.1996 ns | 0.1961 ns |  1.49 |    0.06 |         - |          NA |
| TypeAndObject |  8.667 ns | 0.1235 ns | 0.1213 ns |  2.23 |    0.06 |         - |          NA |
