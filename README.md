# Xoderony

面向 Unity 7 和 .NET 10 的模块化库集合，各模块按职责独立发布和维护。

## 包

- Xoderony.Channels / io.github.xoderony.channels：委托订阅与分发通道、共享值读写通道。
- Xoderony.Serialization / io.github.xoderony.serialization：二进制 Span Reader/Writer 与 Codec。
- Xoderony.ObjectPool / io.github.xoderony.objectpool：对象池契约、归还作用域与集合池。
- Xoderony.Collections / io.github.xoderony.collections：固定容量 Span 集合与 BCL 集合扩展。
- Xoderony.Numerics / io.github.xoderony.numerics：Q16 定点数与泛型数值扩展。
- Xoderony.Numerics.Unity.Editor / io.github.xoderony.numerics.unity：可选的 Unity Editor 适配，提供 Q16 小数编辑框；单向依赖 Numerics。
- `Xoderony.Text` / `io.github.xoderony.text`：基于 Span 的 UTF-16 与 UTF-8 文本构造、格式化工具和字符串扩展，不依赖 `UnityEngine`。
- `Xoderony.Logging` / `io.github.xoderony.logging`：无 Unity 依赖的通用日志核心，以及自动携带调用位置与 Unity 对象上下文的 Unity 适配。
- `Xoderony.Localization` / `io.github.xoderony.localization`：无 Unity 依赖的本地化核心、强类型字符串键生成工具，以及可选的 Hjson Unity 编辑器数据层。
- `Xoderony.Modding` / `io.github.xoderony.modding`：按约定扫描并加载游戏 Mod，不依赖 `UnityEngine`。
- `Xoderony.Localization.SourceGeneration`：面向标准 .NET/MSBuild 项目的可选 NuGet analyzer，从显式标记的 `keys.json` 生成强类型字符串键；Unity 的 RoslynAnalyzer 分发不在当前阶段支持范围内。

Channels、Serialization、ObjectPool、Collections、Numerics 和 Text 均不依赖 UnityEngine，也不相互依赖；按需安装并直接引用对应程序集。

## 安装

在 Unity Package Manager 中选择 **Add package from git URL**，输入所需包的地址：

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.channels
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.serialization
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.objectpool
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.collections
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.numerics
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.numerics.unity
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.logging
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.localization
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.modding
```

## 从 Core 迁移

Xoderony.Core / io.github.xoderony.core 已移除。原有集合、序列化、对象池、数值和通道能力分别归属上列独立包，StringExtensions 归属 Text；这些迁移类型的命名空间保持不变，调用方需更新程序集引用并重新编译。

SpanExtensions 已删除，调用方改用 System.Runtime.InteropServices.MemoryMarshal 的对应 API，例如 MemoryMarshal.AsBytes、Cast、Read、Write 和 AsRef。Unity 调用方需移除旧 Core 的 asmdef 引用，按需引用各模块；使用程序集限定类型名的持久化数据需同步迁移。

## 兼容性

- Unity 7（`7000.0`）或更高版本
- .NET 10

## 许可证

[MIT](LICENSE)
