#if NET10_0_OR_GREATER
using System.Buffers.Binary;
#else
using BinaryPrimitives = Xoderony.Serialization.Unity.LittleEndianPrimitives;
#endif
using UnityEngine;

namespace Xoderony.Serialization.Unity;

/// <summary>按 x、y 顺序，以小端 float 编解码 Vector2。</summary>
public readonly struct Vector2Codec
#if NET10_0_OR_GREATER
    : ISpanCodec<Vector2>
#endif
{

    public const int ByteCount = 8;

    public static void Encode(ref SpanWriter writer, Vector2 value) {
        var destination = writer.Destination.Slice(writer.Position, ByteCount);
        BinaryPrimitives.WriteSingleLittleEndian(destination, value.x);
        BinaryPrimitives.WriteSingleLittleEndian(destination[4..], value.y);
        writer.Position += ByteCount;
    }

    public static Vector2 Decode(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, ByteCount);
        var value = new Vector2 {
            x = BinaryPrimitives.ReadSingleLittleEndian(source),
            y = BinaryPrimitives.ReadSingleLittleEndian(source[4..])
        };
        reader.Position += ByteCount;
        return value;
    }
}
