using UnityEngine;

namespace Xoderony.Serialization.Unity;

/// <summary>按 r、g、b、a 顺序，以四个字节编解码 Color32。</summary>
public readonly struct Color32Codec
#if NET10_0_OR_GREATER
    : ISpanCodec<Color32>
#endif
{

    public const int ByteCount = 4;

    public static void Encode(ref SpanWriter writer, Color32 value) {
        var destination = writer.Destination.Slice(writer.Position, ByteCount);
        destination[0] = value.r;
        destination[1] = value.g;
        destination[2] = value.b;
        destination[3] = value.a;
        writer.Position += ByteCount;
    }

    public static Color32 Decode(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, ByteCount);
        var value = new Color32 {
            r = source[0],
            g = source[1],
            b = source[2],
            a = source[3]
        };
        reader.Position += ByteCount;
        return value;
    }
}
