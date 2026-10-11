```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-UWQVVG : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  WarmupCount=3  

```
| Method                         | Mean        | Error      | StdDev     | Gen0   | Allocated |
|------------------------------- |------------:|-----------:|-----------:|-------:|----------:|
| CachedReadOnlyCollection       |    14.64 ns |   1.405 ns |   0.735 ns |      - |         - |
| CachedGenericCollection        |    18.10 ns |   0.850 ns |   0.444 ns |      - |         - |
| ArrayCopy                      |    78.53 ns |  12.244 ns |   6.404 ns | 0.0041 |      40 B |
| BuildAndFirstGenericCollection | 4,401.59 ns | 337.321 ns | 176.426 ns | 0.7282 |    6939 B |
