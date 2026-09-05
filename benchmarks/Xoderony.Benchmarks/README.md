# Xoderony.Benchmarks

可扩展的 .NET 10 性能基准入口，使用 BenchmarkDotNet。新案例按模块放入子目录，标记 `[Benchmark]` 后自动出现在命令行选择器中。

## 运行

从仓库根目录执行，使用 Release 配置且不附加调试器：

```powershell
dotnet run --project benchmarks/Xoderony.Benchmarks -c Release -- --list flat
dotnet run --project benchmarks/Xoderony.Benchmarks -c Release -- --filter '*Vector3WriteBenchmarks*'
dotnet run --project benchmarks/Xoderony.Benchmarks -c Release -- --filter '*Vector3ReadBenchmarks*'
```

快速比较可增加 `--job Short`；正式比较保留默认采样配置。筛选方法时也包含 `Fields`，以便计算相对基准的 Ratio。

当前 Vector3 案例需要已安装 Unity Editor，并复用 `src/Xoderony.Serialization.Unity/Xoderony.Serialization.Unity.local.props` 中的 `UnityManagedPath`（指向 `Editor/Data/Managed/UnityEngine`）。也可以通过 MSBuild 的 `UnityManagedPath` 属性指定。Unity DLL 仅从本机复制到构建输出，不随仓库分发。

Unity 官方 `CoreModule` 带有未优化标记，因此入口关闭 BenchmarkDotNet 的依赖优化标记校验，并拒绝 Debug 配置运行；基准及本仓库被测库仍按 Release 编译。这不会改变 Unity DLL 自身的编译标记，结果应连同其版本一起解读。

结果默认写入当前目录的 `BenchmarkDotNet.Artifacts/results`，包含 Markdown、CSV、HTML 和反汇编；这些生成文件已由仓库忽略。VS 中可在解决方案的 `benchmarks` 分组找到本项目。

## Vector3 写入比较

| 方法 | 内容 |
| --- | --- |
| Fields（基准） | 固定保留旧的逐字段写法，依次调用三次 `SpanWriter.WriteSingle` |
| Unmanaged | 每个向量调用一次 `SpanWriter.WriteUnmanaged<Vector3>` |
| Codec | 实际调用当前的 `Vector3Codec.Encode`；目前采用一次取得 12 字节切片、固定偏移写入并更新一次位置的实现 |
| BulkCopy | 用 `MemoryMarshal.AsBytes` 将整个向量数组作为字节视图，一次复制全部载荷 |

各方法都从预先构造的真实 `UnityEngine.Vector3[]` 写入同一个可复用缓冲区。`Count` 为 1、1,024 或 65,536；`Offset` 为 0 或 1，用于比较起始偏移的影响。Count 为 1 时前三种方法仍包含数组访问和循环；它代表一个元素的批次，不是剥离所有调用开销后的单条存储指令。

输入生成、分配和正确性校验位于 `GlobalSetup`，不进入计时。输入固定随机种子并包含负零、无穷及带载荷的 NaN；校验按浮点位模式比较全部输出、前后哨兵及最终位置。每种写法计时前都必须通过校验，计时后再次检查缓冲区。方法返回写入位置，输出保存在对象持有的数组中。

每次操作处理 **Count 个向量**：Mean 是整批耗时，Mean / Count 才是平均每个向量的耗时；吞吐量可按 `Count * 12 / Mean秒数` 计算。Ratio 越低越快，Allocated 为每批的托管分配量。不要跨不同 Count 直接比较整批耗时。

这是一组复用相同源、目标缓冲区的预热后吞吐量测试，不代表冷缓存、大文件 I/O 或网络传输。BulkCopy 利用数据本来已连续存储的条件，不能将其结果直接套用于运行时逐个产生向量的场景。

整体写入与批量复制采用本机内存表示；测试仅在小端、12 字节 xyz 布局下运行，以保证与固定小端格式等价。`Fields` 保持固定对照，`Codec` 跟随生产实现，便于在同一轮测量中比较优化前后；修改 Codec 后应重新运行并同步此处的实现说明。

这里测量的是 **.NET 10 JIT**；引用 Unity 类型不意味着使用 Unity Mono、IL2CPP 或 Burst。后续需要在对应 Unity Player 中单独测试。

## Vector3 读取比较

| 方法 | 内容 |
| --- | --- |
| Fields（基准） | 固定的三次 `SpanReader.ReadSingle`，构造并保存 Vector3 |
| Codec | 调用当前 `Vector3Codec.Decode`：取得 12 字节切片，按固定偏移读取并直接初始化 x/y/z 字段，最后更新一次位置 |
| Unmanaged | 每个向量调用一次 `SpanReader.ReadUnmanaged<Vector3>` |
| FixedOffsets | 取得 12 字节切片，在 0、4、8 偏移读取，调用三参数 Vector3 构造函数，最后更新一次位置 |
| BulkCopy | 将整批字节复制到目标 Vector3 数组的字节视图 |

读取采用同样的 Count 和 Offset 参数。源字节按固定随机种子直接生成，包含负零、无穷及带载荷的 NaN，不依赖被测编码器生成输入。每种方法把结果保存到预分配的同一 Vector3 数组；计时包含读取、构造和保存结果，不包含数组分配。这里比较的是物化数组的成本，批量复制也会执行实际拷贝，并非只创建一个零拷贝视图。

计时前逐位校验输出、目标数组前后的哨兵和最终读取位置，计时后再次校验输出。Fields 是固定对照，Codec 跟随生产实现；FixedOffsets 保留调用三参数构造函数的版本，用于与直接初始化字段的 Codec 对比。

读取测试同样只覆盖 .NET 10、小端和 12 字节 xyz 布局，并复用缓冲区。Mean 是整批耗时，单元素组也包含循环和数组操作。

本机 Unity Editor 6000.0.80f1 的 `Vector3` 三参数构造函数在 .NET 10 测量中未内联，生成代码仍带调试检查。Fields 和 FixedOffsets 会调用该构造函数，Codec 直接初始化字段，Unmanaged 和 BulkCopy 直接搬运内存。因而结果同时包含构造方式和依赖程序集优化状态的影响，不能把完整差距归因于字段读取方式；其他 Unity DLL 版本应重新查看反汇编。

本机 6000.7.0a5 的 Count=1024、Offset=0 对比中，构造函数已内联，FixedOffsets 与 Codec 生成相同指令（除进程相关地址）。此时收益主要来自固定偏移读写和减少位置更新，字段初始化器没有额外收益。详见[对比报告](../../artifacts/benchmarks/vector3-read-net10-unity-6000.7.0a5/README.md)。

## 添加案例

- 新增公开基准类，按需要使用 `[Params]`、`[GlobalSetup]` 和 `[MemoryDiagnoser]`。
- 直接调用被测项目的实际实现，将数据准备、分配和结果校验放在计时之外。
- 每组设置语义一致的基准方法，并明确“一次操作”处理的数据量。
- 用返回值或可观察的输出保留计算结果；通过 `--filter` 选择案例，无需修改入口。

参考：[BenchmarkDotNet 命令行](https://benchmarkdotnet.org/articles/guides/console-args.html)、[反汇编诊断](https://benchmarkdotnet.org/articles/features/disassembler.html)。
