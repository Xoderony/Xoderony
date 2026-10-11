```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-UWQVVG : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=8  IterationTime=300ms  WarmupCount=3  

```
| Method                         | Mean         | Error       | StdDev      | Gen0   | Gen1   | Allocated |
|------------------------------- |-------------:|------------:|------------:|-------:|-------:|----------:|
| CachedReadOnlyCollection       |    10.253 ns |   0.3591 ns |   0.1878 ns |      - |      - |         - |
| CachedGenericCollection        |     6.927 ns |   0.1333 ns |   0.0697 ns |      - |      - |         - |
| ArrayCopy                      |    56.804 ns |   1.3902 ns |   0.4958 ns | 0.0041 |      - |      40 B |
| BuildAndFirstGenericCollection | 7,307.628 ns | 606.8256 ns | 317.3815 ns | 1.5549 | 0.0278 |   14718 B |
