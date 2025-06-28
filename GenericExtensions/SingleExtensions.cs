using System;

namespace GuestUnion {

    public static class SingleExtensions {

        public static float Abs(this float value) => value < 0 ? -value : value;

        public static float Clamp(this float value, float min, float max) =>
            value < min
            ? min
            : value > max ? max : value;

        public static float Clamp01(this float value) =>
            value < 0
            ? 0
            : value > 1 ? 1 : value;

        public static float LerpTo(this float from, float to, float t) => from + ((to - from) * Clamp01(t));

        public static float LerpToUnclamped(this float from, float to, float t) => from + ((to - from) * t);

        public static float MoveTowards(this float from, float to, float maxDelta) {
            var v = to - from;
            return v >= 0
                ? (v <= maxDelta ? to : from + maxDelta)
                : (-v <= maxDelta ? to : from - maxDelta);
        }

        public static float NormalizeAngle(this float angle) =>
            angle < 0
            ? (angle % 360) + 360
            : angle % 360;

        /// <summary>若传入角度不在区间[0, 360)，则返回角度向该区间旋转360°后的结果，而不是返回直接旋转到该区间的结果。</summary>
        public static float NormalizeAngleOne360(this float angle) =>
            angle < 0
            ? angle + 360
            : angle > 360 ? angle - 360 : angle;
    }
}