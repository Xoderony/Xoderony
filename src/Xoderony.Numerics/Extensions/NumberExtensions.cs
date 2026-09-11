using System.Diagnostics;
using System.Numerics;

namespace Xoderony.Extensions;

public static class NumberExtensions {

    extension<T>(T value) where T : INumber<T> {

        public T Abs() {
            return T.Abs(value);
        }

        public T Clamp(T min, T max) {
            return T.Clamp(value, min, max);
        }

        public T Clamp01() {
            return T.Clamp(value, T.Zero, T.One);
        }
    }

    extension<T>(T from) where T : IFloatingPointIeee754<T> {

        public T LerpTo(T to, T t) {
            return T.Lerp(from, to, t.Clamp01());
        }

        public T LerpToUnclamped(T to, T t) {
            return T.Lerp(from, to, t);
        }

        public T MoveTowards(T to, T maxDelta) {
            Debug.Assert(maxDelta >= T.Zero);
            var delta = to - from;
            if (delta > maxDelta) {
                return from + maxDelta;
            }
            if (delta < -maxDelta) {
                return from - maxDelta;
            }
            return to;
        }
    }
}
