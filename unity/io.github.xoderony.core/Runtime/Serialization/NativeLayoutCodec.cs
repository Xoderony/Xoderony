namespace Xoderony.Serialization;

public readonly struct NativeLayoutCodec<T> : ISpanCodec<T> where T : unmanaged {

    public static void Encode(ref SpanWriter writer, T value) {
        writer.WriteUnmanaged(value);
    }

    public static T Decode(ref SpanReader reader) {
        return reader.ReadUnmanaged<T>();
    }
}
