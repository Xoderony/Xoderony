namespace Xoderony.Serialization;

public interface ISpanCodec<T> where T : unmanaged {

    static abstract void Encode(ref SpanWriter writer, T value);

    static abstract T Decode(ref SpanReader reader);
}
