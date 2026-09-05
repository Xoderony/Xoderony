using UnityEngine;

namespace Xoderony.Serialization.Unity;

/// <summary>依次编解码 Bounds 的 center 和 extents，各使用 Vector3Codec 的表示方式。</summary>
public readonly struct BoundsCodec
#if NET10_0_OR_GREATER
    : ISpanCodec<Bounds>
#endif
{

    public const int ByteCount = Vector3Codec.ByteCount * 2;

    public static void Encode(ref SpanWriter writer, Bounds value) {
        Vector3Codec.Encode(ref writer, value.center);
        Vector3Codec.Encode(ref writer, value.extents);
    }

    public static Bounds Decode(ref SpanReader reader) {
        var center = Vector3Codec.Decode(ref reader);
        var extents = Vector3Codec.Decode(ref reader);
        return new Bounds {
            center = center,
            extents = extents
        };
    }
}
