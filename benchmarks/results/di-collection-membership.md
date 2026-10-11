# 显式集合成员注册：实现与测量

历史版本记录：2026-09-14 已移除 AsCollection 与开放泛型，改为构建时自动集合和统一字典查询。当前数据见 [构建时集合结果](di-build-time-collections.md)，本文保留原版本测量，不代表当前实现。

2026-09-13。当前契约见 [容器设计](../../docs/specs/di-container.md)，用法见 [README](../../src/Xoderony.DependencyInjection/README.md)。

## 实现变化

- AsCollection 为已有注册添加成员关系，保留默认自身入口，可与 As、AsSelf 混用。同一条注册的单个别名和多个集合共享实例，只登记一次释放。
- 普通与闭合泛型单个服务直接保存 Lazy<object>；开放单个模板只对应一条注册，取消最后一项选择和整组请求槽位。
- 集合才保存注册引用列表。普通集合引用原始 Lazy，开放集合引用同一个 OpenGenericRegistration，首次求值组装只读结果。没有独立集合元素提供者、元素所有权或共享注册句柄。
- 集合缓存表一次登记 IReadOnlyList<T>、IEnumerable<T>、T[] 三种形式，均引用一个集合 Lazy。前两者直接返回只读结果，数组每次复制。稳定命中不再提取泛型参数。
- 同层单个服务类型与 Key 重复时 Build 报错。没有 AsCollection 成员声明就不自动组成集合；未注册集合按目标缺失处理。直接集合服务与可匹配的成员声明同时可见时报错。
- 泛型层级检查区分单个与集合，集合按元素泛型定义判断。普通集合可以子层整组覆盖；继承的集合保持指向原注册，不因子层覆盖单个服务而改换元素。

这次简化集中在单个解析及实例关系；整体实现增加了集合映射、只读结果缓存和配置冲突检查，不能称为所有代码都变少。

## 最终测量

BenchmarkDotNet 0.15.8，Windows 11 25H2，i5-12400F，SDK 10.0.400，.NET 10.0.11，Release、Workstation GC。3 次预热、8 次 300 ms 测量。缓存类型预先创建；构建基准包含注册、Build、首次解析与释放，进程和运行时元数据已预热。

| 场景 | 均值 | 每操作托管分配 |
|---|---:|---:|
| 普通单个缓存命中 | 7.342 ns | 0 B |
| 混合四种闭合 Type 单个缓存命中 | 8.259 ns | 0 B |
| 混合泛型接口 + Key 缓存命中 | 14.97 ns | 0 B |
| 普通元素只读集合缓存命中 | 14.64 ns | 0 B |
| 混合四种泛型元素只读集合缓存命中 | 18.10 ns | 0 B |
| 两元素数组副本 | 78.53 ns | 40 B |
| 普通七对象图构建与首次解析 | 8.191 μs | 9824 B |
| 单个开放注册构建与首次请求 | 1.478 μs | 3024 B |
| 上述过程再请求第二个单个别名 | 1.780 μs | 3160 B |
| 一个单个服务及四个集合贡献项，构建后只解析单个 | 2.975 μs | 5522 B |
| 父层已预热，创建子层并请求继承单个服务 | 512.71 ns | 1656 B |
| 四条注册构建并首次解析两元素泛型集合 | 4.402 μs | 6939 B |

零分配项测量区没有记录到 GC；数组副本和构建按预期产生分配。集合需要查询单个服务映射与集合映射，不把单个请求的一次查表承诺扩展到集合。

最初集合缓存以元素类型查询，每次请求都调用 GetGenericArguments，实测只读集合命中约 73–80 ns、32 B。改成登记完整集合请求形式后，复测为上表的零分配结果。代价是每组多两个映射条目和首次登记成本；两元素泛型集合的构建及首次请求从约 3.782 μs / 6377 B 变为 4.402 μs / 6939 B。

此前分路版本的泛型单个命中约 9 ns，当前仍处于相近量级，不能把小幅变化都归因于本次调整。初版单个泛型构建分配为 2936 B，当前为 3024 B；新增集合表与 Build 检查仍有成本，删除整组单个槽位不等于所有冷路径分配都下降。作用域创建也包含新增映射继承与冲突检查，不承诺优于此前版本。

原 BuildFourAndResolveLast 使用四条重复单个注册，已不符合新契约，替换为一个单个注册加四个集合贡献项，只解析单个服务；这个案例不能与旧案例直接计算性能比例。第三方容器本次没有重跑。

## 验证

63 项 Release 契约测试通过，包含单个先解析／集合先解析的共享身份、默认自身入口、多接口集合去重、工厂执行一次、外部实例所有权、只读结果与数组隔离、集合失败缓存及已创建元素释放、显式集合冲突、开放泛型、父子缓存和类别隔离。

## 原始报告

- [单个服务与普通图](explicit-collection-registration-final/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)
- [泛型接口、首次请求与作用域](explicit-collection-registration-final/results/Xoderony.Benchmarks.DependencyInjection.GenericResolutionBenchmarks-report-github.md)
- [集合缓存、数组副本与首次请求](explicit-collection-registration-final/results/Xoderony.Benchmarks.DependencyInjection.CollectionBenchmarks-report-github.md)
- [中间版本集合分配报告](explicit-collection-registration/results/Xoderony.Benchmarks.DependencyInjection.CollectionBenchmarks-report-github.md)

```powershell
dotnet test tests/Xoderony.DependencyInjection.Tests/Xoderony.DependencyInjection.Tests.csproj -c Release --no-restore
dotnet run --project benchmarks/Xoderony.Benchmarks/Xoderony.Benchmarks.csproj -c Release --no-restore -- --filter '*CollectionBenchmarks*' '*GenericResolutionBenchmarks*' '*DependencyInjection.ContainerBenchmarks.XoderonyBuild' '*DependencyInjection.ContainerBenchmarks.XoderonyCached' '*DependencyInjection.ContainerBenchmarks.XoderonyGenericType' --warmupCount 3 --iterationCount 8 --iterationTime 300 --artifacts benchmarks/results/explicit-collection-registration-final
```
