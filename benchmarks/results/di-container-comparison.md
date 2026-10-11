# DI 容器基准对比（2026-09-13）

本次仅增加基准与依赖准备脚本，未改变 Xoderony 容器实现。GitHub 已发布结果与本机结果分别解释，不跨机器、运行时或负载直接排名。

以上说明及主表记录最初对比时的实现。随后开放泛型优化的结果见文末，保留原始数据作为优化前基线。

## 环境与版本

- Windows 11 25H2，Intel Core i5-12400F，6 核 12 线程。
- .NET SDK 10.0.400，运行时 .NET 10.0.11，Release，x64 RyuJIT，Workstation GC。
- BenchmarkDotNet 0.15.8，1 个 launch、5 次预热、10 次测量，目标迭代时长 500 ms。
- DryIoc.dll 5.4.3；Stashbox 5.20.0；Microsoft.Extensions.DependencyInjection 10.0.11；Jab 0.12.0。
- VContainer 1.19.0，官方提交 `5401e5a7ebc4980a2b82141ffc26391a6547edd7`，按官方 Standalone 文件范围排除 Unity 集成后以 .NET 10 编译，不启用源码生成器。源码保持原样；其未使用字段 CS0169 警告保留。
- Xoderony 使用本工作区源码。运行时库不新增第三方依赖；比较包只属于基准项目。

## 测量契约

所有容器使用相同的 7 个对象、3 层构造函数依赖图：Root → Left/Right → 4 个 Leaf，Left/Right 共享其中两个 Leaf。所有注册都使用容器级单例；对象不实现 IDisposable。另注册 Box<> 供泛型场景使用。

1. `BuildAndFirstResolve`：每次创建新容器，注册、构建、首次解析 Root，再释放容器。Jab 注册与依赖分析在编译期完成，运行时没有对应注册工作。这里测的是预热进程中反复创建新容器，不是应用进程冷启动；各库静态元数据缓存可保留。
2. `CachedResolve`：预热后反复使用泛型 API 解析同一个 Root。每批 1024 次，按操作数归一化。包含各库正常内联优化；Jab 极小数字是固定服务连续访问的优化后吞吐，不代表任意动态请求延迟。
3. `CachedGenericType`：从预先构造的数组中读取 1024 个请求类型，Random(42) 混合 Box<int>/Box<long>/Box<float>/Box<double>，全部提前解析过。计时区不包含随机数、类型数组创建或首次创建。Jab 显式声明这四个 RootServices，测其已生成类型的 IServiceProvider 路径，不表示它支持运行时未知的泛型组合。
4. `XoderonyFactory`：7 个注册全部使用静态 lambda 手写 new，依赖仍通过 IResolver 解析，与反射版本保持相同对象图和共享关系。只比较首次创建及缓存解析，没有新增静态抽象接口。

GlobalSetup 校验共享单例对象图、泛型返回类型和重复解析实例一致性。未测 transient、作用域覆盖、成员注入、集合、注册规模、多线程或可释放服务，不能据此给出全功能性能排名。Xoderony 本身要求串行访问，其余库的并发保证不同。

## 本机结果

主比较的完整均值、误差、分配见 [BenchmarkDotNet 报告](container-comparison-mixed/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)。

| 容器 / 创建方式 | 注册至首次解析及释放 | 该流程分配 | 普通缓存解析 | 混合泛型 Type 缓存解析 | 泛型每次分配 |
|---|---:|---:|---:|---:|---:|
| Xoderony 反射 | 7.87 μs | 9760 B | 7.76 ns | 168.14 ns | 96 B |
| Xoderony 委托 | 1.11 μs | 4864 B | 7.97 ns | 未单独测 | 未单独测 |
| DryIoc 5.4.3 | 2.14 μs | 4520 B | 6.77 ns | 7.88 ns | 0 B |
| Stashbox 5.20.0 | 1175.24 μs | 42712 B | 9.41 ns | 11.01 ns | 0 B |
| Microsoft DI 10.0.11 | 4.59 μs | 9888 B | 4.53 ns | 4.67 ns | 0 B |
| Jab 0.12.0 | 0.15 μs | 328 B | 0.48 ns | 2.35 ns | 0 B |
| VContainer 1.19.0 独立核心（第二组） | 3.40 μs | 8712 B | 6.79 ns | 32.14 ns | 32 B |

VContainer 在随后单独一组中运行，[完整报告](container-comparison-vcontainer/results/Xoderony.Benchmarks.DependencyInjection.VContainerBenchmarks-report-github.md) 同时重测 Xoderony：7.61 μs / 9760 B、普通缓存 7.45 ns、泛型缓存 190.74 ns / 96 B。泛型时间两组有约 13% 差异，因此应看 168–191 ns 的量级和稳定的 96 B 分配，不将两组拼成精确的倍数排名。

普通缓存解析的所有条目均为 0 B。Stashbox 的默认配置在反复创建新容器、首次构造该图时成本明显较高，不能将其外推为稳态解析慢；没有做分阶段分析来精确拆分这部分时间。

首次探索使用固定 typeof(Box<int>) 和 250 ms 迭代。发现动态场景不充分及一次短迭代警告后，改成上述混合请求，并将全部主比较重新运行至 500 ms；本报告只采用修订后的结果。

## GitHub 发布数据

| 来源 | 发布的负载与数据 | 解读限制 |
|---|---|---|
| [DryIoc README](https://github.com/dadhi/DryIoc#benchmarks) | README 标注 DryIoc 6.0.0、.NET 9、i9-8950HK；40 个依赖、4 层图。注册、开作用域、首次解析约 65.70 μs / 32.74 KB；预热后开作用域并解析约 1.357 μs / 2.91 KB。 | 后者会创建作用域及对象，不能与本次取已缓存 Root 的纳秒值比较。官方版本也不同于本次 5.4.3。 |
| [Stashbox README](https://github.com/z4kn4fein/stashbox#benchmarks) 指向 [IocPerformance](https://github.com/danielpalme/IocPerformance) | 表中的单线程 Singleton 总耗时：Stashbox 5.11.0 为 40 ms，DryIoc 4.8.5 为 63 ms，Microsoft DI 7.0.0 为 68 ms。 | 是旧版本整批耗时，并非一次 Resolve；仓库已于 2023-07-20 归档。不能作为 2026 年最新排名。 |
| [VContainer README](https://github.com/hadashiA/VContainer#performance) / [基准源码](https://github.com/hadashiA/VContainer/blob/5401e5a7ebc4980a2b82141ffc26391a6547edd7/tests/VContainer.Benchmark/Assets/VContainer.Benchmark/ContainerPerformanceTest.cs) | README 声称通常比 Zenject 快 5–10 倍，排除创建的服务对象后 Resolve 零分配；Unity 基准采用 10000 次循环，包含单例、瞬态、混合和复杂图。 | 这是 Unity 中对特定框架和负载的描述；不能据此认定本次所有 .NET 路径均零分配。 |
| [Jab README](https://github.com/pakrym/jab#performance) | 启动项 Jab 11.03 ns、Microsoft DI 2437.88 ns；GetService 项 Jab 5.619 ns、Microsoft DI 39.340 ns，后者两者均分配 24 B。 | 24 B 表示该条目并非本次零分配单例缓存查询；README 表格没有完整机器与版本信息，宣传倍数不能直接套用本项目。 |

## 对当前实现的意义

- 委托工厂把本例的注册至首次解析成本从约 7.87 μs 降至 1.11 μs，约 7.1 倍，分配约减半；缓存后两条路径差异处于测量波动范围。现有工厂 API 已能获得这部分收益，不需要为此增加 IService 静态抽象契约。
- 普通单例缓存查询已在个位数纳秒附近，应优先关注真实热路径而非微小名次。
- VContainer 本次泛型路径也有 32 B/次分配，源码 GetClosedRegistration 每次创建 TypeParametersKey；其普通缓存解析无分配。README 的零分配表述不能直接外推到所有泛型路径。
- 开放泛型缓存命中仍约 168 ns、96 B/次。源码在查实例缓存前执行 GetGenericArguments 和 MakeGenericType，并存在捕获闭合类型的 lambda；本次尚未用分配分析器拆分 96 B 的具体来源。最值得继续验证的是以闭合请求类型缓存解析结果，让命中路径跳过类型闭合，同时保留不同服务别名共享同一闭合实现实例的契约。

## 复现

在仓库根目录运行。VContainer 依赖下载与构建只需首次准备；保存在被忽略的 benchmarks/.dependencies，保留官方 LICENSE。

```powershell
./benchmarks/Prepare-VContainer.ps1
dotnet run --project benchmarks/Xoderony.Benchmarks/Xoderony.Benchmarks.csproj -c Release -- --filter '*DependencyInjection.ContainerBenchmarks*' --warmupCount 5 --iterationCount 10 --iterationTime 500 --artifacts benchmarks/results/container-comparison-mixed
dotnet run --project benchmarks/Xoderony.Benchmarks/Xoderony.Benchmarks.csproj -c Release -- --filter '*VContainerBenchmarks*' --warmupCount 5 --iterationCount 10 --iterationTime 500 --artifacts benchmarks/results/container-comparison-vcontainer
```

源码见 benchmarks/Xoderony.Benchmarks/DependencyInjection/ContainerBenchmarks.cs 和 VContainerBenchmarks.cs。未准备 VContainer 时，该比较类不会参与编译，其余容器仍可正常测量。

## 后续：开放泛型缓存优化

在 OpenGenericRegistration 原有字典中，闭合服务类型与闭合实现类型指向同一个 Lazy<object>。先按请求类型查询，只有未命中才进入独立方法完成泛型闭合、创建闭包及登记别名。没有修改 Container 的注册表，因此缓存不会被当作显式闭合注册继承到子作用域。

| 相同混合泛型请求基准 | 平均耗时 | 每次托管分配 |
|---|---:|---:|
| 优化前主比较 / VContainer 对照组 | 168.14 / 190.74 ns | 96 B |
| 优化后 | 27.74 ns | 0 B |

沿用相同机器、运行时、Random(42) 请求、5 次预热及 10 次 500 ms 测量。优化后 Error 为 1.648 ns（99.9% 置信区间半宽），吞吐约提升 6–7 倍。额外驻留成本是每个实际请求过的闭合服务别名多一条字典映射，不额外创建服务实例或 Lazy。首次闭合成本没有在本轮单独测量。

[优化后原始报告](open-generic-cache/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)。25 项 Release 单元测试通过，包含别名解析顺序、Key、集合、释放一次、闭合类型隔离和预热后子作用域覆盖。

随后进一步限定在 OpenGenericRegistration 内调整首次展开路径：请求实现类型自身时直接复用传入的 Type；使用 CollectionsMarshal.GetValueRefOrAddDefault 合并实现缓存的查找和插入；将 Lazy 的闭包创建移入独立方法，已有实例的新别名不再创建该闭包。添加别名前保存 Lazy 引用，字典扩容后不再使用取得的 ref。

25 项测试再次通过。同一命中基准复测为 28.86 ns，Error 2.416 ns，0 B/次，与上一轮 27.74 ns 的置信区间重叠，不能认定热路径进一步加速。本轮主要减少首次展开的工作，未单独测量首次展开收益。[复测报告](open-generic-cache-refined/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)。

后续将 Lazy 创建方法合并回缓存未命中方法，最终保留两个方法。捕获变量声明在 `cachedInstance is null` 分支内，限制闭包创建范围；25 项测试通过，该结构整理后没有重新运行基准，上述数字对应整理前版本。

最终进一步合并为单个 GetOrCreateLazyInstance 方法。首次合并仍直接捕获 this，实测命中路径出现 32 B 分配；随后将 owner 和 implementationType 都保存为创建分支内的局部变量，使 lambda 不直接捕获 this。修正后 25 项测试通过，命中基准为 28.73 ns、0 B，与此前拆分方法版本相近。[最终单方法报告](open-generic-cache-single-method-fixed/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)。

随后将方法改名为 GetOrCreateInstance 并直接返回 object，把两个调用方的 `.Value` 访问收进注册对象内部。Lazy 缓存及求值顺序保持不变，25 项测试通过；该接口整理后未重新测量，上述基准对应返回 Lazy 的版本。

## 最新缓存复测

本次使用直接返回 object、单方法、if/else 类型选择及索引器覆盖别名映射的最终实现。运行相同环境、5 次预热、10 次 500 ms 测量，共 15 项缓存基准；结束后 25 项 Release 单元测试通过。没有进一步修改容器实现。

| 容器 | 普通单例缓存解析 | 混合泛型 Type 缓存解析 | 普通 / 泛型每次托管分配 |
|---|---:|---:|---:|
| Xoderony | 7.88 ns | 24.40 ns | 0 / 0 B |
| Xoderony 委托注册 | 7.58 ns | 未单独测 | 0 / — |
| DryIoc 5.4.3 | 6.38 ns | 9.27 ns | 0 / 0 B |
| Stashbox 5.20.0 | 9.09 ns | 11.19 ns | 0 / 0 B |
| Microsoft DI 10.0.11 | 4.37 ns | 4.70 ns | 0 / 0 B |
| Jab 0.12.0 | 0.48 ns | 2.15 ns | 0 / 0 B |
| VContainer 1.19.0 独立核心 | 6.64 ns | 34.91 ns | 0 / 32 B |

两类基准仍分别输出报告；VContainer 对照组内的 Xoderony 为普通 8.04 ns、泛型 29.29 ns，分配同为 0 B。泛型时间存在组间波动，应报告约 24–29 ns，不能将 24.40 ns 与此前 28.73 ns 的差异归因于此次语法调整。Jab 的固定服务查询受生成代码和 JIT 优化影响，不作为动态容器的平均水平参照。

结论：本次测量的 Xoderony 缓存命中均无托管分配，测量区没有记录到其引发的 GC；普通缓存解析在运行时容器常见的 4–9 ns 范围。开放泛型已经优于本次 VContainer，但仍慢于 DryIoc、Stashbox、Microsoft DI 的约 5–11 ns，不能认为所有路径都达到对照组平均水平。仅针对已预热的单服务解析，不涵盖首次构建、首次请求新类型及会创建结果数组的集合解析。

[主比较原始报告](container-cache-recheck/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)；[VContainer 对照原始报告](container-cache-recheck/results/Xoderony.Benchmarks.DependencyInjection.VContainerBenchmarks-report-github.md)。

```powershell
dotnet run --project benchmarks/Xoderony.Benchmarks/Xoderony.Benchmarks.csproj -c Release -- --filter '*ContainerBenchmarks*' --anyCategories CachedResolve CachedGenericType --warmupCount 5 --iterationCount 10 --iterationTime 500 --artifacts benchmarks/results/container-cache-recheck
```

```powershell
dotnet run --project benchmarks/Xoderony.Benchmarks/Xoderony.Benchmarks.csproj -c Release -- --filter '*DependencyInjection.ContainerBenchmarks.XoderonyGenericType' --warmupCount 5 --iterationCount 10 --iterationTime 500 --artifacts benchmarks/results/open-generic-cache
```
