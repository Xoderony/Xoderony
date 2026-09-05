# Xoderony Serialization Unity

为 Unity 常用非托管值类型提供 `ISpanCodec<T>` 实现。仅单向依赖 `io.github.xoderony.serialization`，通过基础包的 `SpanReader` / `SpanWriter` 读写调用方缓冲区。

## 安装

先安装 Serialization，再安装本包。通过 Git URL 安装时，在 Unity Package Manager 中分别添加：

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.serialization
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.serialization.unity
```

使用本地仓库时，选择 **Add package from disk**，先选择 Serialization 包的 `package.json`，再选择本包的 `package.json`。

最低 Unity 版本与基础包一致，为 7000.0（Unity 7）。使用自定义 asmdef 的调用方应引用 `Xoderony.Serialization` 和 `Xoderony.Serialization.Unity`。

## 使用

Codec 位于命名空间 `Xoderony.Serialization.Unity`。每个 Codec 都公开固定的 `ByteCount`，支持基础包现有的 `Write<T, TCodec>` / `Read<T, TCodec>` API；无需注册或创建 Codec 实例。

```csharp
using System;
using UnityEngine;
using Xoderony.Serialization;
using Xoderony.Serialization.Unity;

Span<byte> buffer = stackalloc byte[Vector3Codec.ByteCount + QuaternionCodec.ByteCount];
var writer = new SpanWriter(buffer);
writer.Write<Vector3, Vector3Codec>(new Vector3(1f, 2f, 3f));
writer.Write<Quaternion, QuaternionCodec>(Quaternion.identity);

var reader = new SpanReader(writer.WrittenSpan);
var position = reader.Read<Vector3, Vector3Codec>();
var rotation = reader.Read<Quaternion, QuaternionCodec>();
```

也可以在自定义 Codec 中直接组合 `Vector3Codec.Encode(ref writer, value)` / `Vector3Codec.Decode(ref reader)` 等静态方法。

## 二进制表示

下表中的字段连续写入，不包含类型标记、版本、长度前缀或填充。所有 `float` 都使用 32 位 IEEE 754 小端表示，`int` 使用 32 位有符号整数小端表示，`byte` 占一个字节。读写双方必须约定相同的 Codec 与字段顺序。

| Unity 类型 / Codec | 字段顺序 | 字段类型 | ByteCount |
| --- | --- | --- | --- |
| `Vector2` / `Vector2Codec` | x, y | float | 8 |
| `Vector3` / `Vector3Codec` | x, y, z | float | 12 |
| `Vector4` / `Vector4Codec` | x, y, z, w | float | 16 |
| `Vector2Int` / `Vector2IntCodec` | x, y | int | 8 |
| `Vector3Int` / `Vector3IntCodec` | x, y, z | int | 12 |
| `Quaternion` / `QuaternionCodec` | x, y, z, w | float | 16 |
| `Color` / `ColorCodec` | r, g, b, a | float | 16 |
| `Color32` / `Color32Codec` | r, g, b, a | byte | 4 |
| `Rect` / `RectCodec` | x, y, width, height | float | 16 |
| `Bounds` / `BoundsCodec` | center.x, center.y, center.z, extents.x, extents.y, extents.z | float | 24 |

- 向量和四元数逐分量保存，不进行归一化、量化或欧拉角转换。
- `Color` 保留 HDR 和超出 [0, 1] 的分量，不进行颜色空间转换；`Color32` 保存其原始四个通道字节。两者的格式不能互换。
- `Rect` 保留原始宽高及其符号，不转换成 min/max。
- `Bounds` 使用半尺寸 `extents`，解码时直接赋给 `extents`，避免通过 `size` 乘除二造成额外的浮点溢出或精度损失。
- Codec 不校验数值的业务有效性，不拒绝 NaN、Infinity、负尺寸或非单位四元数；调用方在使用解码值之前负责按业务规则检查。

协议由以上分量顺序定义，不依赖 Unity 结构体的内存布局。`NativeLayoutCodec<T>` 使用本机布局，不能据此假定与这些格式兼容。

## 缓冲区与职责

调用方负责有效的 `Position`、数据格式和足够的连续容量。读取外部数据前，用 `reader.CanRead(所需字节数)` 检查完整记录；写入前可用 `writer.CanWrite(所需字节数)` 检查容量。每次成功调用按对应 `ByteCount` 推进位置。

容量不足时沿用基础包的失败行为，可能已推进位置或写入部分字段，不保证整条记录的原子性，也不自动回滚。Span 的存储所有权和生命周期始终归调用方。

本包处理值类型数据，不序列化 `UnityEngine.Object` 引用、场景对象、资源身份或对象图；集合长度、消息边界、版本和业务校验由调用方协议定义。

## 程序集与源码

`Runtime/Xoderony.Serialization.Unity.asmdef` 同时用于 Editor 和 Player，引用基础程序集 `Xoderony.Serialization` 和 UnityEngine。权威源码位于 `Runtime/`，不需要 Editor 程序集或额外的 .NET 项目。本包不依赖 Numerics.Unity 或其他 Xoderony 包。

## netstandard2.1 DLL 兼容版

项目同时生成 `net10.0` 和 `netstandard2.1`；Unity 6000.7 同时引用 Serialization 与 Serialization.Unity 的兼容版 DLL。

全部具体 codec、`ByteCount` 和二进制表示保持一致。兼容版不实现 .NET 10 的静态抽象接口，调用形式为 `Vector3Codec.Encode(ref writer, value)` / `Vector3Codec.Decode(ref reader)`；上面的泛型扩展调用示例仅适用于 .NET 10。UPM 源码安装仍面向 Unity 7。
