using System;

namespace GuestUnion.DoubleExtensions {

    public static class DoubleExtensions {

        public static double Clamp(this double value, double min, double max) =>
            value < min
            ? min
            : value > max ? max : value;

        public static double Clamp01(this double value) =>
            value < 0
            ? 0
            : value > 1 ? 1 : value;

        public static double LerpTo(this double from, double to, double t) => from + ((to - from) * Clamp01(t));

        public static double LerpToUnclamped(this double from, double to, double t) => from + ((to - from) * t);

        public static double MoveTowards(this double from, double to, double maxDelta) {
            var v = to - from;
            return v >= 0
                ? (v <= maxDelta ? to : from + maxDelta)
                : (-v <= maxDelta ? to : from - maxDelta);
        }

        public static double NormalizeAngle(this double angle) =>
            angle < 0
            ? (angle % 360) + 360
            : angle % 360;

        /// <summary>若传入角度不在区间[0, 360)，则返回角度向该区间旋转360°后的结果，而不是返回直接旋转到该区间的结果。</summary>
        public static double NormalizeAngleOne360(this double angle) =>
            angle < 0
            ? angle + 360
            : angle > 360 ? angle - 360 : angle;
    }
}