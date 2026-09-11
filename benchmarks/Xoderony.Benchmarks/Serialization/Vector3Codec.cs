using System.Buffers.Binary;
using Xoderony.Serialization;
using System.Numerics;

namespace Xoderony.Benchmarks.Serialization;

/// <summary>按 X、Y、Z 顺序，以小端 float 编解码 Vector3。</summary>
internal readonly struct Vector3Codec
    : ISpanCodec<Vector3>
{

    public const int ByteCount = 12;

    public static void Encode(ref SpanWriter writer, Vector3 value) {
        var destination = writer.Destination.Slice(writer.Position, ByteCount);
        BinaryPrimitives.WriteSingleLittleEndian(destination, value.X);
        BinaryPrimitives.WriteSingleLittleEndian(destination[4..], value.Y);
        BinaryPrimitives.WriteSingleLittleEndian(destination[8..], value.Z);
        writer.Position += ByteCount;
    }

    public static Vector3 Decode(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, ByteCount);
        var value = new Vector3 {
            X = BinaryPrimitives.ReadSingleLittleEndian(source),
            Y = BinaryPrimitives.ReadSingleLittleEndian(source[4..]),
            Z = BinaryPrimitives.ReadSingleLittleEndian(source[8..])
        };
        reader.Position += ByteCount;
        return value;
    }
}

