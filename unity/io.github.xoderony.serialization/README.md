# Xoderony Serialization

提供基于调用方缓冲区的二进制读写和编解码能力。不依赖 UnityEngine 或其他 Xoderony 包。

## 程序集与 API

- 程序集与命名空间：`Xoderony.Serialization`。
- `SpanReader` / `SpanWriter`：维护调用方缓冲区和当前位置。
- `ISpanCodec<T>`：通过静态 Encode/Decode 实现非托管类型的编解码。
- `NativeLayoutCodec<T>`：直接使用类型的本机内存布局。
- `SpanCodecExtensions`：连接 Reader/Writer 与具体 Codec。

## 数据约定

数值读写使用小端字节序；WriteUnmanaged/ReadUnmanaged 使用本机内存布局，调用方负责类型布局一致性。

WriteBytes/ReadBytes 使用 ushort 字节长度前缀；WriteChars/ReadChars 使用 ushort 字符数量前缀，字符载荷使用本机 char 内存布局。调用方负责缓冲区容量、有效位置和数据格式，可通过 CanRead/CanWrite 查询剩余空间是否足够。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.serialization
```

支持 Unity 7（7000.0）和 .NET 10。权威源码位于 Runtime/，src/Xoderony.Serialization/Xoderony.Serialization.csproj 链接同一份源码。

## 从 Core 迁移

这些类型从 Xoderony.Core 移入本程序集，类型名、命名空间和二进制读写行为保持不变。调用方改为引用 Xoderony.Serialization 并重新编译。

## netstandard2.1 DLL 兼容版

.NET 项目同时生成 `net10.0` 和 `netstandard2.1`；Unity 6000.7 使用兼容版 DLL。基础读写、字节序、长度前缀与本机布局约定保持一致。

`ISpanCodec<T>` 和泛型 `SpanCodecExtensions` 仅在 .NET 10 提供。兼容版通过 `NativeLayoutCodec<T>.Encode(ref writer, value)` / `Decode(ref reader)` 或具体 codec 的同名静态方法调用，不要求创建 codec 实例。UPM 源码安装仍面向 Unity 7。
