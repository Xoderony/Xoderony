#if NET10_0_OR_GREATER
using System.Buffers.Binary;
#else
using BinaryPrimitives = Xoderony.Serialization.Unity.LittleEndianPrimitives;
#endif
using UnityEngine;

namespace Xoderony.Serialization.Unity;

/// <summary>按 x、y 顺序，以小端 int 编解码 Vector2Int。</summary>
public readonly struct Vector2IntCodec
#if NET10_0_OR_GREATER
    : ISpanCodec<Vector2Int>
#endif
{

    public const int ByteCount = 8;

    public static void Encode(ref SpanWriter writer, Vector2Int value) {
        var destination = writer.Destination.Slice(writer.Position, ByteCount);
        BinaryPrimitives.WriteInt32LittleEndian(destination, value.x);
        BinaryPrimitives.WriteInt32LittleEndian(destination[4..], value.y);
        writer.Position += ByteCount;
    }

    public static Vector2Int Decode(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, ByteCount);
        var value = new Vector2Int(BinaryPrimitives.ReadInt32LittleEndian(source), BinaryPrimitives.ReadInt32LittleEndian(source[4..]));
        reader.Position += ByteCount;
        return value;
    }
}
