# DI 泛型分路实施结果

此报告对应初版带 GenericRegistrationGroup 的实现。后续已采用泛型服务族层级约束及 AsCollection 显式集合成员关系，单个服务只保存一个 Lazy；本报告保留为历史数据。当前契约见 [容器设计](../../docs/specs/di-container.md)，最新数据见 [集合成员调整结果](di-collection-membership.md)。

2026-09-13。采用 [已确认方案](../../docs/specs/di-generic-resolution-refactor.md)：普通与闭合泛型服务分别查表，开放注册仅在未命中时展开整组 Lazy；保留 OpenGenericRegistration 的实例所有权和别名共享。

## 同机前后结果

BenchmarkDotNet 0.15.8，Windows 11 25H2，i5-12400F，SDK 10.0.400，.NET 10.0.11，Release、Workstation GC。两轮均为 3 次预热、8 次 300 ms 测量，依次运行，未并行执行测试或其他基准。类型与运行时元数据已预热，首次请求指新容器内首次请求，不是进程冷启动。

| 场景 | 调整前均值 | 调整后均值 | 前 / 后托管分配 |
|---|---:|---:|---:|
| 普通单例命中 | 7.598 ns | 8.390 ns | 0 / 0 B |
| 混合四种闭合实现 Type 命中 | 29.429 ns | 9.016 ns | 0 / 0 B |
| 混合四种泛型接口 Type + Key 命中 | 40.49 ns | 15.26 ns | 0 / 0 B |
| 注册、Build、首次解析普通七对象图并释放 | 8.023 μs | 8.062 μs | 9760 / 9848 B |
| 注册、Build、首次泛型接口请求并释放 | 1.390 μs | 1.385 μs | 2464 / 2936 B |
| 上述过程再请求第二个别名 | 1.416 μs | 1.468 μs | 2464 / 3136 B |
| 四条开放注册，Build 后仅解析最后一项并释放 | 2.678 μs | 2.599 μs | 3856 / 4761 B |
| 父层已预热，创建子层、首次请求继承泛型并释放 | 185.52 ns | 276.83 ns | 696 / 1168 B |

泛型实现命中用时减少约 69%，接口加 Key 减少约 62%，差距超过本次测量噪声。两个测量区均无托管分配和已记录 GC。普通路径均值增加约 0.79 ns（10%）；前后 99.9% 置信区间部分重叠，不将精确百分比视为稳定保证。普通图构建及各泛型构建场景的时间差处于本次误差范围，不能声称构建加速。

新增闭合字典及容器引用使普通图构建多 88 B。单注册首次请求多 472 B；第二个别名首次请求再多 200 B；四注册但仅求值末项的整体分配约多 905 B，包含未求值槽位。上述都是完整操作的分配量，第二别名的单独耗时没有隔离测量，不能直接把两项均值差当作精确耗时。

子作用域首次请求回退明确：即使父层已有实例，子层仍建立自己的请求组，多约 91 ns、472 B。实例本身仍共享，子层不会再次构造或登记释放。

## 缓存驻留与作用域

每个容器新增一张闭合泛型字典。显式闭合组只保存原始 Lazy；运行时每个实际请求的 `(闭合服务类型, Key)` 增加一个展开组，组内有与可见开放注册数相同的外层 Lazy。不同别名分别建立请求组，原始实例仍由 OpenGenericRegistration 共享。

按代码存储关系计算：S 个存活作用域，各自请求 C 个闭合类型/别名/Key 组合，每组 R 条注册，最多增加 S×C 个展开组与 S×C×R 个请求槽位。例如 8 个子层分别请求 4 个组合、每组 4 条注册，会有 32 个组和 128 个外层槽位，即使每组只解析最后一项。父层实例缓存不因此复制 8 份。未求值槽位保留工厂捕获状态；求值后 Lazy 保留结果或失败，槽位随所属容器缓存存活。

创建子层还会遍历父层闭合表、过滤展开项，因此父层已请求类型很多时，CreateScope 也有随条目数增长的扫描成本。本次作用域基准父层只有四个已展开组合，不能代表大缓存规模。

这些驻留关系依据源码，未做堆快照测量；MemoryDiagnoser 的 Allocated 是操作分配量，不是存活堆大小。Dispose 清空闭合表中的引用，但 Dictionary 仍可保留已分配容量，直到容器本身可回收。此方案适合重复解析多于一次性作用域请求的场景，不是无成本优化。

## 验证与结论

目标 Release 测试共 36 项全部通过。新加的两项约束异常身份测试在旧实现失败、调整后通过；其余新增回归在前后均通过。覆盖跨普通/泛型别名、类型/实例/工厂来源、整组集合顺序、延迟约束校验、父层预热与子层覆盖、所有权、显式集合优先、可见性及泛型跨别名循环/构造失败。

保留本次实现：稳定泛型命中改善明显，零分配保持，新增结构限于闭合表、小型组结构体和展开方法。接受已确认的首次槽位分配与约束失败缓存；普通路径及作用域首次请求回退如上。未改公共 API、并发模型、注入分析和 OpenGenericRegistration，也未重新运行第三方容器基准；此前第三方结果仅作历史参照。

## 原始报告与复现

- 调整前：[普通图与缓存](generic-routing-before/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)、[接口与首次请求](generic-routing-before/results/Xoderony.Benchmarks.DependencyInjection.GenericResolutionBenchmarks-report-github.md)。
- 调整后：[普通图与缓存](generic-routing-after/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)、[接口与首次请求](generic-routing-after/results/Xoderony.Benchmarks.DependencyInjection.GenericResolutionBenchmarks-report-github.md)。
- 基准源码：[ContainerBenchmarks](../Xoderony.Benchmarks/DependencyInjection/ContainerBenchmarks.cs)、[GenericResolutionBenchmarks](../Xoderony.Benchmarks/DependencyInjection/GenericResolutionBenchmarks.cs)。

```powershell
dotnet test tests/Xoderony.DependencyInjection.Tests/Xoderony.DependencyInjection.Tests.csproj -c Release --no-restore
dotnet run --project benchmarks/Xoderony.Benchmarks/Xoderony.Benchmarks.csproj -c Release --no-restore -- --filter '*GenericResolutionBenchmarks*' '*DependencyInjection.ContainerBenchmarks.XoderonyBuild' '*DependencyInjection.ContainerBenchmarks.XoderonyCached' '*DependencyInjection.ContainerBenchmarks.XoderonyGenericType' --warmupCount 3 --iterationCount 8 --iterationTime 300 --artifacts benchmarks/results/generic-routing-after
```

调整前使用同一基准源码，容器仍为方案实施前的三次查找版本，输出目录为 generic-routing-before；当前检出直接运行得到调整后的结果。
