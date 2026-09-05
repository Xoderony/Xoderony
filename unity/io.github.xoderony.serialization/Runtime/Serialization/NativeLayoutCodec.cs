namespace Xoderony.Serialization;

public readonly struct NativeLayoutCodec<T>
#if NET10_0_OR_GREATER
    : ISpanCodec<T>
#endif
    where T : unmanaged {

    public static void Encode(ref SpanWriter writer, T value) {
        writer.WriteUnmanaged(value);
    }

    public static T Decode(ref SpanReader reader) {
        return reader.ReadUnmanaged<T>();
    }
}
