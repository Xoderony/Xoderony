#if NET10_0_OR_GREATER
using System.Buffers.Binary;
#else
using BinaryPrimitives = Xoderony.Serialization.Unity.LittleEndianPrimitives;
#endif
using UnityEngine;

namespace Xoderony.Serialization.Unity;

/// <summary>按 x、y、z 顺序，以小端 float 编解码 Vector3。</summary>
public readonly struct Vector3Codec
#if NET10_0_OR_GREATER
    : ISpanCodec<Vector3>
#endif
{

    public const int ByteCount = 12;

    public static void Encode(ref SpanWriter writer, Vector3 value) {
        var destination = writer.Destination.Slice(writer.Position, ByteCount);
        BinaryPrimitives.WriteSingleLittleEndian(destination, value.x);
        BinaryPrimitives.WriteSingleLittleEndian(destination[4..], value.y);
        BinaryPrimitives.WriteSingleLittleEndian(destination[8..], value.z);
        writer.Position += ByteCount;
    }

    public static Vector3 Decode(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, ByteCount);
        var value = new Vector3 {
            x = BinaryPrimitives.ReadSingleLittleEndian(source),
            y = BinaryPrimitives.ReadSingleLittleEndian(source[4..]),
            z = BinaryPrimitives.ReadSingleLittleEndian(source[8..])
        };
        reader.Position += ByteCount;
        return value;
    }
}

#if !NET10_0_OR_GREATER
internal static class LittleEndianPrimitives {
    public static void WriteSingleLittleEndian(System.Span<byte> destination, float value) {
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(destination, System.BitConverter.SingleToInt32Bits(value));
    }

    public static float ReadSingleLittleEndian(System.ReadOnlySpan<byte> source) {
        return System.BitConverter.Int32BitsToSingle(System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(source));
    }

    public static void WriteInt32LittleEndian(System.Span<byte> destination, int value) {
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(destination, value);
    }

    public static int ReadInt32LittleEndian(System.ReadOnlySpan<byte> source) {
        return System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(source);
    }
}
#endif
