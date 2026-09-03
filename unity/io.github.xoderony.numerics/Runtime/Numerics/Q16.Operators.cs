namespace Xoderony.Numerics;

public partial struct Q16 {

    public static explicit operator float(Q16 value) {
        return value.RawValue * RawToFloatScale;
    }

    /// <summary>转为数值空间整数（向零截断小数部分）。底层 raw 值请读 <see cref="RawValue"/>。</summary>
    public static explicit operator int(Q16 value) {
        return value.RawValue / Scale;
    }

    /// <summary>从数值空间整数转换；小数部分为 0。底层 raw 值请写 <see cref="RawValue"/>。</summary>
    public static explicit operator Q16(int value) {
        return new Q16(value);
    }

    public static explicit operator Q16(float value) {
        return new Q16(value);
    }

    public static int operator *(int left, Q16 right) {
        return (int)(((long)left) * right.RawValue / Scale);
    }

    public static int operator *(Q16 left, int right) {
        return (int)(((long)right) * left.RawValue / Scale);
    }

    public static long operator *(long left, Q16 right) {
        return left * right.RawValue / Scale;
    }

    public static long operator *(Q16 left, long right) {
        return right * left.RawValue / Scale;
    }

    public static Q16 operator +(Q16 left, Q16 right) {
        return new Q16 {
            RawValue = left.RawValue + right.RawValue
        };
    }

    public static Q16 operator -(Q16 left, Q16 right) {
        return new Q16 {
            RawValue = left.RawValue - right.RawValue
        };
    }

    public static Q16 operator -(Q16 value) {
        return new Q16 {
            RawValue = -value.RawValue
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
            RawValue = (int)(((long)left.RawValue) * right.RawValue / Scale)
        };
    }

    public static Q16 operator /(Q16 left, Q16 right) {
        return new Q16 {
            RawValue = (int)((((long)left.RawValue) << FractionalBits) / right.RawValue)
        };
    }

    public static bool operator ==(Q16 left, Q16 right) {
        return left.RawValue == right.RawValue;
    }

    public static bool operator !=(Q16 left, Q16 right) {
        return left.RawValue != right.RawValue;
    }

    public static bool operator <(Q16 left, Q16 right) {
        return left.RawValue < right.RawValue;
    }

    public static bool operator >(Q16 left, Q16 right) {
        return left.RawValue > right.RawValue;
    }

    public static bool operator <=(Q16 left, Q16 right) {
        return left.RawValue <= right.RawValue;
    }

    public static bool operator >=(Q16 left, Q16 right) {
        return left.RawValue >= right.RawValue;
    }
}
