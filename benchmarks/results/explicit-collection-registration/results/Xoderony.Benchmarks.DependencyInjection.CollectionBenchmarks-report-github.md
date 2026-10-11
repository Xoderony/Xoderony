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
| CachedReadOnlyCollection       |    80.37 ns |   7.288 ns |   3.812 ns | 0.0033 |      32 B |
| CachedGenericCollection        |    72.96 ns |  10.454 ns |   5.468 ns | 0.0033 |      32 B |
| ArrayCopy                      |    93.41 ns |   7.982 ns |   4.175 ns | 0.0041 |      40 B |
| BuildAndFirstGenericCollection | 3,781.99 ns | 414.435 ns | 216.758 ns | 0.6666 |    6377 B |
