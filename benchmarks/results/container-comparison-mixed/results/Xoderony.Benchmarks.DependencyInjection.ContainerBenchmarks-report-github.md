```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12400F 2.50GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  Job-EWNVSB : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3

IterationCount=10  IterationTime=500ms  WarmupCount=5  

```
| Method                | Categories           | Mean              | Error           | StdDev         | Ratio  | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|---------------------- |--------------------- |------------------:|----------------:|---------------:|-------:|--------:|-------:|-------:|----------:|------------:|
| XoderonyBuild         | BuildAndFirstResolve |     7,872.6165 ns |     432.2848 ns |    285.9296 ns |   1.00 |    0.05 | 1.0310 | 0.0156 |    9760 B |        1.00 |
| XoderonyFactoryBuild  | BuildAndFirstResolve |     1,108.9949 ns |      73.3719 ns |     48.5310 ns |   0.14 |    0.01 | 0.5154 | 0.0046 |    4864 B |        0.50 |
| DryIocBuild           | BuildAndFirstResolve |     2,140.3806 ns |     158.2655 ns |    104.6828 ns |   0.27 |    0.02 | 0.4768 |      - |    4520 B |        0.46 |
| StashboxBuild         | BuildAndFirstResolve | 1,175,244.3207 ns | 100,153.7331 ns | 66,245.4933 ns | 149.46 |    9.56 | 2.7174 |      - |   42712 B |        4.38 |
| MicrosoftBuild        | BuildAndFirstResolve |     4,585.6048 ns |     388.0897 ns |    230.9459 ns |   0.58 |    0.03 | 1.0443 | 0.0177 |    9888 B |        1.01 |
| JabBuild              | BuildAndFirstResolve |       149.9745 ns |      17.9368 ns |     11.8641 ns |   0.02 |    0.00 | 0.0347 |      - |     328 B |        0.03 |
|                       |                      |                   |                 |                |        |         |        |        |           |             |
| XoderonyGenericType   | CachedGenericType    |       168.1434 ns |       4.4822 ns |      2.9647 ns |   1.00 |    0.02 | 0.0100 |      - |      96 B |        1.00 |
| DryIocGenericType     | CachedGenericType    |         7.8791 ns |       0.4275 ns |      0.2828 ns |   0.05 |    0.00 |      - |      - |         - |        0.00 |
| StashboxGenericType   | CachedGenericType    |        11.0124 ns |       0.6074 ns |      0.4018 ns |   0.07 |    0.00 |      - |      - |         - |        0.00 |
| MicrosoftGenericType  | CachedGenericType    |         4.6738 ns |       0.1847 ns |      0.1222 ns |   0.03 |    0.00 |      - |      - |         - |        0.00 |
| JabGenericType        | CachedGenericType    |         2.3543 ns |       0.2151 ns |      0.1423 ns |   0.01 |    0.00 |      - |      - |         - |        0.00 |
|                       |                      |                   |                 |                |        |         |        |        |           |             |
| XoderonyCached        | CachedResolve        |         7.7612 ns |       0.5438 ns |      0.3597 ns |   1.00 |    0.06 |      - |      - |         - |          NA |
| XoderonyFactoryCached | CachedResolve        |         7.9669 ns |       0.6914 ns |      0.4573 ns |   1.03 |    0.07 |      - |      - |         - |          NA |
| DryIocCached          | CachedResolve        |         6.7690 ns |       0.3629 ns |      0.2400 ns |   0.87 |    0.05 |      - |      - |         - |          NA |
| StashboxCached        | CachedResolve        |         9.4138 ns |       0.6466 ns |      0.4277 ns |   1.22 |    0.07 |      - |      - |         - |          NA |
| MicrosoftCached       | CachedResolve        |         4.5270 ns |       0.4083 ns |      0.2701 ns |   0.58 |    0.04 |      - |      - |         - |          NA |
| JabCached             | CachedResolve        |         0.4836 ns |       0.0288 ns |      0.0190 ns |   0.06 |    0.00 |      - |      - |         - |          NA |
