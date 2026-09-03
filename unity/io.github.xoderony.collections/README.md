# Xoderony Collections

提供固定容量的 Span 集合，以及 BCL 集合访问扩展。不依赖 UnityEngine 或其他 Xoderony 包。

## 程序集与 API

- 程序集：`Xoderony.Collections`。
- `Xoderony.Collections`：SpanList、SpanIntMap、SpanIntSet、EqualityComparerDelegate 和配套状态类型。
- `Xoderony.Extensions.CollectionExtensions`：集合判空、List 的 Span 视图与槽位操作、Dictionary 的值引用访问和 BitArray 字节视图。

Span 集合使用调用方提供的缓冲区，不自动扩容。SpanIntMap 的值必须为 unmanaged；SpanIntSet 不允许存储空槽哨兵 -1。集合视图和引用借用底层存储，调用方负责其生命周期和结构修改约束。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.collections
```

支持 Unity 7（7000.0）和 .NET 10。权威源码位于 Runtime/，src/Xoderony.Collections/Xoderony.Collections.csproj 链接同一份源码。

## 从 Core 迁移

这些类型从 Xoderony.Core 移入本程序集，类型名和命名空间保持不变。调用方改为引用 Xoderony.Collections 并重新编译。
