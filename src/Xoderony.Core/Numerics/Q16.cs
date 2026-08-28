using System;
using System.Numerics;

namespace Xoderony.Numerics;

/// <summary>
/// 16.16 定点数，适合倍率等缩放用途。数值用构造或显式转换；底层 raw 读写 <see cref="rawValue"/>。
/// 运算符与转换默认不检测溢出，行为与同宽 <see cref="int"/> 的 unchecked 运算一致。
/// </summary>
[Serializable]
public partial struct Q16 :
    IEquatable<Q16>,
    IComparable<Q16>,
    IComparable,
    IFormattable,
    ISpanFormattable,
    IAdditionOperators<Q16, Q16, Q16>,
    ISubtractionOperators<Q16, Q16, Q16>,
    IMultiplyOperators<Q16, Q16, Q16>,
    IDivisionOperators<Q16, Q16, Q16>,
    IUnaryNegationOperators<Q16, Q16>,
    IUnaryPlusOperators<Q16, Q16>,
    IIncrementOperators<Q16>,
    IDecrementOperators<Q16>,
    IComparisonOperators<Q16, Q16, bool>,
    IEqualityOperators<Q16, Q16, bool>,
    IAdditiveIdentity<Q16, Q16>,
    IMultiplicativeIdentity<Q16, Q16>,
    IMinMaxValue<Q16> {

    /// <summary>小数部分位数。</summary>
    public const int FractionalBits = 16;

    /// <summary>数值 1 对应的 raw；等于 <c>1 &lt;&lt; FractionalBits</c>。</summary>
    public const int OneRaw = 1 << FractionalBits;

    /// <summary>raw 转到 float 的系数；等于 <c>1f / OneRaw</c>。</summary>
    public const float Raw2Float = 1f / OneRaw;

    /// <summary>float 转到 raw 的系数；等于 <c>(float)OneRaw</c>。</summary>
    public const float Float2Raw = OneRaw;

    public int rawValue;

    public static Q16 Zero => default;

    public static Q16 One { get; } = new Q16 {
        rawValue = OneRaw
    };

    public static Q16 NegativeOne { get; } = new Q16 {
        rawValue = -OneRaw
    };

    public static Q16 MinValue { get; } = new Q16 {
        rawValue = int.MinValue
    };

    public static Q16 MaxValue { get; } = new Q16 {
        rawValue = int.MaxValue
    };

    public static Q16 AdditiveIdentity => Zero;

    public static Q16 MultiplicativeIdentity => One;

    /// <summary>使用数值空间整数构造；小数部分为 0。例如 <c>new Q16(1)</c> 表示 1。</summary>
    public Q16(int value) {
        rawValue = value << FractionalBits;
    }

    public Q16(int numerator, int denominator) {
        rawValue = (int)(((long)numerator) * OneRaw / denominator);
    }

    public Q16(float value) {
        rawValue = (int)(value * Float2Raw);
    }

    public override readonly bool Equals(object? obj) {
        return obj is Q16 other && Equals(other);
    }

    public readonly bool Equals(Q16 other) {
        return rawValue == other.rawValue;
    }

    public override readonly int GetHashCode() {
        return rawValue;
    }

    public readonly int CompareTo(Q16 other) {
        return rawValue.CompareTo(other.rawValue);
    }

    public readonly int CompareTo(object? obj) {
        if (obj is null) {
            return 1;
        }
        if (obj is Q16 other) {
            return CompareTo(other);
        }
        throw new ArgumentException($"Object must be of type {nameof(Q16)}.", nameof(obj));
    }
}
