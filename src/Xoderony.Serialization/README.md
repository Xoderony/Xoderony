# Xoderony Serialization

提供基于调用方缓冲区的二进制读写和编解码能力。不依赖其他 Xoderony 包。

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

通过 NuGet 引用 `Xoderony.Serialization`；本地开发可直接引用同目录的项目文件。

目标框架为 `net10.0`。

## 从 Core 迁移

这些类型从 Xoderony.Core 移入本程序集，类型名、命名空间和二进制读写行为保持不变。调用方改为引用 Xoderony.Serialization 并重新编译。
