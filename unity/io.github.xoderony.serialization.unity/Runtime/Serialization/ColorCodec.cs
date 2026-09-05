#if NET10_0_OR_GREATER
using System.Buffers.Binary;
#else
using BinaryPrimitives = Xoderony.Serialization.Unity.LittleEndianPrimitives;
#endif
using UnityEngine;

namespace Xoderony.Serialization.Unity;

/// <summary>按 r、g、b、a 顺序，以小端 float 编解码 Color，不进行颜色空间转换或截断。</summary>
public readonly struct ColorCodec
#if NET10_0_OR_GREATER
    : ISpanCodec<Color>
#endif
{

    public const int ByteCount = 16;

    public static void Encode(ref SpanWriter writer, Color value) {
        var destination = writer.Destination.Slice(writer.Position, ByteCount);
        BinaryPrimitives.WriteSingleLittleEndian(destination, value.r);
        BinaryPrimitives.WriteSingleLittleEndian(destination[4..], value.g);
        BinaryPrimitives.WriteSingleLittleEndian(destination[8..], value.b);
        BinaryPrimitives.WriteSingleLittleEndian(destination[12..], value.a);
        writer.Position += ByteCount;
    }

    public static Color Decode(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, ByteCount);
        var value = new Color {
            r = BinaryPrimitives.ReadSingleLittleEndian(source),
            g = BinaryPrimitives.ReadSingleLittleEndian(source[4..]),
            b = BinaryPrimitives.ReadSingleLittleEndian(source[8..]),
            a = BinaryPrimitives.ReadSingleLittleEndian(source[12..])
        };
        reader.Position += ByteCount;
        return value;
    }
}
