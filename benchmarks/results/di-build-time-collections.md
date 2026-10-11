# 构建时自动集合与统一查询：实施结果

2026-09-14。实施 [确认方案](../../docs/specs/di-build-time-collections.md)，现行用法见 [README](../../src/Xoderony.DependencyInjection/README.md)。

## 结构与行为

运行时注册存储由五张表收敛为一张 `(完整请求 Type, Key) → Lazy<object>` 字典，移除 OpenGenericRegistration、开放模板、展开缓存、AsCollection 及泛型服务族判断。成员反射分析缓存独立保留，不参与已创建服务的查找。

Build 按本层声明类型与 Key 判重，为每条注册建立一个原始 Lazy，并按服务类型与 Key 临时分组。每个非空原始服务组生成单个入口及三种集合入口，不递归处理生成的集合。单候选入口直接引用原始 Lazy，多候选入口抛歧义而不创建候选；三个集合入口共享一个只读结果 Lazy。数组请求返回副本。

本层四种入口先完整生成，再以 TryAdd 继承父层，保持整组替换。IResolver 及其集合固定为本层。运行时 ResolveCore 只有一次 TryGetValue；初始化发生在入口命中但 Lazy 未求值时，真正未命中直接返回缺失。别名和集合通过原始 Lazy 共享实例，不再进行第二次注册查询。

## 当前测量

BenchmarkDotNet 0.15.8，Windows 11 25H2，i5-12400F，SDK 10.0.400，.NET 10.0.11，Release、Workstation GC。3 次预热、8 次 300 ms 测量，目标共 12 项。构建项包含注册、Build、首次解析与释放；进程及运行时元数据已预热。

| 场景 | 均值 | 每操作托管分配 |
|---|---:|---:|
| 普通单个缓存命中 | 6.681 ns | 0 B |
| 混合四种闭合 Type 单个命中 | 7.069 ns | 0 B |
| 混合闭合泛型接口 + Key 命中 | 14.67 ns | 0 B |
| 普通只读集合命中 | 10.253 ns | 0 B |
| 混合四种泛型元素只读集合命中 | 6.927 ns | 0 B |
| 两元素数组副本 | 56.804 ns | 40 B |
| 普通七对象图与四个闭合类型注册，首次解析对象图 | 11.742 μs | 20864 B |
| 一条闭合泛型注册，首次请求接口 | 1.665 μs | 3993 B |
| 上述过程再请求具体类型别名 | 1.669 μs | 3993 B |
| 五条闭合注册，先解析有独立别名的一项 | 2.176 μs | 5834 B |
| 父层已预热，创建子层并请求继承服务 | 1.027 μs | 3376 B |
| 十条注册，首次解析两元素闭合泛型集合 | 7.308 μs | 14718 B |

零分配项测量区未记录 GC。数组副本保留预期分配。不同命中案例的类型、键、字典大小及 JIT 结果不同，不能根据普通集合与泛型集合的差别推出泛型本身更快。

## 构建成本与比较范围

每个原始服务别名有四个映射键；集合尚未求值时，已有集合 Lazy 和组引用，首次求值再分配数组及只读包装。构建用的分组字典和声明 HashSet 是临时状态，运行时不保留第二套查询索引。每层 IResolver 也有四个入口。

父层继承复制的是映射引用，不重新创建服务；但需要复制更多映射，因此作用域构建成本不能由查询路径变短推断会下降。当前作用域样本父层为四条闭合注册、每条接口与自身两个别名，共 36 个入口（含 IResolver），子层继承后也有 36 个入口。

此前单个注册图的 Build 约 8.191 μs / 9824 B，当前为 11.742 μs / 20864 B；但此前为七条普通注册加一条开放模板，当前为七条普通注册加四条显式闭合注册。这包含集合自动生成及声明数量变化，不能视为相同注册输入下的纯算法回退比例。

集合基准也从两条普通注册加两条开放模板改为两条普通注册加八条显式闭合注册。旧的首次泛型集合成本与当前样本不能直接等价比较。查询阶段的四种 Type 请求仍然保留，热命中数值可作有限参考；本次未重跑第三方容器，旧第三方开放注册结果不代表当前相同注册语义。

结论：一次字典查询及缓存零分配已满足；构建时自动集合确实增加了入口和构建分配，属于已选设计的明确代价。未引入冻结字典、强制内联、额外共享提供者或提前创建服务。

## 验证与复现

41 项 Release 契约测试通过。移除了仅验证已删除开放泛型和显式成员声明行为的测试，以零/一/多候选、歧义不求值、保留类型检查、声明来源判重、父子四入口替换、IResolver、只读缓存、数组副本及原始实例所有权的测试替代。

原始报告：[单个与对象图](build-time-collections/results/Xoderony.Benchmarks.DependencyInjection.ContainerBenchmarks-report-github.md)、[闭合泛型与作用域](build-time-collections/results/Xoderony.Benchmarks.DependencyInjection.GenericResolutionBenchmarks-report-github.md)、[集合](build-time-collections/results/Xoderony.Benchmarks.DependencyInjection.CollectionBenchmarks-report-github.md)。

```powershell
dotnet test tests/Xoderony.DependencyInjection.Tests/Xoderony.DependencyInjection.Tests.csproj -c Release --no-restore
dotnet run --project benchmarks/Xoderony.Benchmarks/Xoderony.Benchmarks.csproj -c Release --no-restore -- --filter '*CollectionBenchmarks*' '*GenericResolutionBenchmarks*' '*DependencyInjection.ContainerBenchmarks.XoderonyBuild' '*DependencyInjection.ContainerBenchmarks.XoderonyCached' '*DependencyInjection.ContainerBenchmarks.XoderonyGenericType' --warmupCount 3 --iterationCount 8 --iterationTime 300 --artifacts benchmarks/results/build-time-collections
```
