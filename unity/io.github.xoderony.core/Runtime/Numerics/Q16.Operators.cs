using System;

namespace Xoderony.Numerics;

public partial struct Q16 {

    public static explicit operator float(Q16 value) {
        return value.rawValue * Raw2Value;
    }

    /// <summary>转为数值空间整数（截断小数部分）。底层 raw 请读 <see cref="rawValue"/>。</summary>
    public static explicit operator int(Q16 value) {
        return value.rawValue >> FractionalBitCount;
    }

    /// <summary>从数值空间整数转换；小数部分为 0。底层 raw 请写 <see cref="rawValue"/>。</summary>
    public static explicit operator Q16(int value) {
        return new Q16(value);
    }

    public static explicit operator Q16(float value) {
        return new Q16(value);
    }

    public static int operator *(int value, Q16 valueScale) {
        return (int)((((long)value) * valueScale.rawValue) >> FractionalBitCount);
    }

    public static int operator *(Q16 valueScale, int value) {
        return (int)((((long)value) * valueScale.rawValue) >> FractionalBitCount);
    }

    public static long operator *(long value, Q16 valueScale) {
        return (value * valueScale.rawValue) >> FractionalBitCount;
    }

    public static long operator *(Q16 valueScale, long value) {
        return (value * valueScale.rawValue) >> FractionalBitCount;
    }

    public static Q16 operator +(Q16 left, Q16 right) {
        return new Q16 {
            rawValue = left.rawValue + right.rawValue
        };
    }

    public static Q16 operator -(Q16 left, Q16 right) {
        return new Q16 {
            rawValue = left.rawValue - right.rawValue
        };
    }

    public static Q16 operator -(Q16 value) {
        return new Q16 {
            rawValue = -value.rawValue
        };
    }

    public static Q16 operator +(Q16 value) {
        return value;
    }

    public static Q16 operator ++(Q16 value) {
        return value + One;
    }

    public static Q16 operator --(Q16 value) {
        return value - One;
    }

    public static Q16 operator *(Q16 left, Q16 right) {
        return new Q16 {
            rawValue = (int)((((long)left.rawValue) * right.rawValue) >> FractionalBitCount)
        };
    }

    public static Q16 operator /(Q16 left, Q16 right) {
        return new Q16 {
            rawValue = (int)((((long)left.rawValue) << FractionalBitCount) / right.rawValue)
        };
    }

    public static bool operator ==(Q16 left, Q16 right) {
        return left.rawValue == right.rawValue;
    }

    public static bool operator !=(Q16 left, Q16 right) {
        return left.rawValue != right.rawValue;
    }

    public static bool operator <(Q16 left, Q16 right) {
        return left.rawValue < right.rawValue;
    }

    public static bool operator >(Q16 left, Q16 right) {
        return left.rawValue > right.rawValue;
    }

    public static bool operator <=(Q16 left, Q16 right) {
        return left.rawValue <= right.rawValue;
    }

    public static bool operator >=(Q16 left, Q16 right) {
        return left.rawValue >= right.rawValue;
    }
}
