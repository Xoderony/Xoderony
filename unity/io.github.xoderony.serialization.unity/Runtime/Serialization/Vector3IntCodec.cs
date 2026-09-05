#if NET10_0_OR_GREATER
using System.Buffers.Binary;
#else
using BinaryPrimitives = Xoderony.Serialization.Unity.LittleEndianPrimitives;
#endif
using UnityEngine;

namespace Xoderony.Serialization.Unity;

/// <summary>按 x、y、z 顺序，以小端 int 编解码 Vector3Int。</summary>
public readonly struct Vector3IntCodec
#if NET10_0_OR_GREATER
    : ISpanCodec<Vector3Int>
#endif
{

    public const int ByteCount = 12;

    public static void Encode(ref SpanWriter writer, Vector3Int value) {
        var destination = writer.Destination.Slice(writer.Position, ByteCount);
        BinaryPrimitives.WriteInt32LittleEndian(destination, value.x);
        BinaryPrimitives.WriteInt32LittleEndian(destination[4..], value.y);
        BinaryPrimitives.WriteInt32LittleEndian(destination[8..], value.z);
        writer.Position += ByteCount;
    }

    public static Vector3Int Decode(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, ByteCount);
        var value = new Vector3Int(
            BinaryPrimitives.ReadInt32LittleEndian(source),
            BinaryPrimitives.ReadInt32LittleEndian(source[4..]),
            BinaryPrimitives.ReadInt32LittleEndian(source[8..])
        );
        reader.Position += ByteCount;
        return value;
    }
}
