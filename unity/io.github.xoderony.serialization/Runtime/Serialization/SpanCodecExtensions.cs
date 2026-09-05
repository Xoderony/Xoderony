#if NET10_0_OR_GREATER
namespace Xoderony.Serialization;

public static class SpanCodecExtensions {

    extension(ref SpanWriter writer) {

        public void Write<T, TCodec>(T value) where T : unmanaged where TCodec : ISpanCodec<T> {
            TCodec.Encode(ref writer, value);
        }

        public void Write<T>(T value) where T : unmanaged, ISpanCodec<T> {
            T.Encode(ref writer, value);
        }
    }

    extension(ref SpanReader reader) {

        public T Read<T, TCodec>() where T : unmanaged where TCodec : ISpanCodec<T> {
            return TCodec.Decode(ref reader);
        }

        public T Read<T>() where T : unmanaged, ISpanCodec<T> {
            return T.Decode(ref reader);
        }
    }
}
#endif
