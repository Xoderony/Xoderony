#if NET10_0_OR_GREATER
using System.Buffers.Binary;
#else
using BinaryPrimitives = Xoderony.Serialization.Unity.LittleEndianPrimitives;
#endif
using UnityEngine;

namespace Xoderony.Serialization.Unity;

/// <summary>按 x、y、z、w 顺序，以小端 float 编解码 Vector4。</summary>
public readonly struct Vector4Codec
#if NET10_0_OR_GREATER
    : ISpanCodec<Vector4>
#endif
{

    public const int ByteCount = 16;

    public static void Encode(ref SpanWriter writer, Vector4 value) {
        var destination = writer.Destination.Slice(writer.Position, ByteCount);
        BinaryPrimitives.WriteSingleLittleEndian(destination, value.x);
        BinaryPrimitives.WriteSingleLittleEndian(destination[4..], value.y);
        BinaryPrimitives.WriteSingleLittleEndian(destination[8..], value.z);
        BinaryPrimitives.WriteSingleLittleEndian(destination[12..], value.w);
        writer.Position += ByteCount;
    }

    public static Vector4 Decode(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, ByteCount);
        var value = new Vector4 {
            x = BinaryPrimitives.ReadSingleLittleEndian(source),
            y = BinaryPrimitives.ReadSingleLittleEndian(source[4..]),
            z = BinaryPrimitives.ReadSingleLittleEndian(source[8..]),
            w = BinaryPrimitives.ReadSingleLittleEndian(source[12..])
        };
        reader.Position += ByteCount;
        return value;
    }
}
