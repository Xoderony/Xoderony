using System;
#if NET10_0_OR_GREATER
using System.Numerics;
#endif

namespace Xoderony.Numerics;

/// <summary>
/// 16.16 定点数，适合倍率等缩放用途。数值通过构造或显式转换读写；底层编码通过 <see cref="RawValue"/> 直接读写。
/// 默认格式化以 <c>RawValue/Scale</c> 分数形式输出精确值；指定的格式和格式提供程序分别应用于分子与分母。
/// 乘除与转换产生无法表示的小数部分时向零截断；
/// 运算符与转换默认不检测溢出，超出表示范围时采用 unchecked 语义。
/// </summary>
[Serializable]
public partial struct Q16 :
    IEquatable<Q16>,
    IComparable<Q16>,
    IComparable,
#if NET10_0_OR_GREATER
    ISpanFormattable,
    IUtf8SpanFormattable,
    IAdditionOperators<Q16, Q16, Q16>,
    ISubtractionOperators<Q16, Q16, Q16>,
    IMultiplyOperators<Q16, Q16, Q16>,
    IDivisionOperators<Q16, Q16, Q16>,
    IUnaryNegationOperators<Q16, Q16>,
    IUnaryPlusOperators<Q16, Q16>,
    IIncrementOperators<Q16>,
    IDecrementOperators<Q16>,
    IComparisonOperators<Q16, Q16, bool>,
    IAdditiveIdentity<Q16, Q16>,
    IMultiplicativeIdentity<Q16, Q16>,
    IMinMaxValue<Q16>
#else
    IFormattable
#endif
{

    /// <summary>底层编码中小数部分占用的位数。</summary>
    public const int FractionalBits = 16;

    /// <summary>每个数值单位对应的 raw 刻度；等于 <c>1 &lt;&lt; FractionalBits</c>。</summary>
    public const int Scale = 1 << FractionalBits;

    /// <summary>将 raw 值转换为 <see cref="float"/> 时使用的乘数；等于 <c>1f / Scale</c>。</summary>
    public const float RawToFloatScale = 1f / Scale;

    /// <summary>将 <see cref="float"/> 转换为 raw 值时使用的乘数；等于 <c>(float)Scale</c>。</summary>
    public const float FloatToRawScale = Scale;

    /// <summary>底层 16.16 编码值；直接读写不会执行数值缩放。</summary>
    public int RawValue;

    public static Q16 Zero => default;

    public static Q16 One { get; } = new Q16 {
        RawValue = Scale
    };

    public static Q16 NegativeOne { get; } = new Q16 {
        RawValue = -Scale
    };

    public static Q16 MinValue { get; } = new Q16 {
        RawValue = int.MinValue
    };

    public static Q16 MaxValue { get; } = new Q16 {
        RawValue = int.MaxValue
    };

    public static Q16 AdditiveIdentity => Zero;

    public static Q16 MultiplicativeIdentity => One;

    /// <summary>使用数值空间整数构造；小数部分为 0。例如 <c>new Q16(1)</c> 表示 1。</summary>
    public Q16(int value) {
        RawValue = value << FractionalBits;
    }

    /// <summary>使用分数构造；结果按 Q16 精度向零截断。</summary>
    public Q16(int numerator, int denominator) {
        RawValue = (int)(((long)numerator) * Scale / denominator);
    }

    /// <summary>使用单精度浮点数构造；结果按 Q16 精度向零截断。</summary>
    public Q16(float value) {
        RawValue = (int)(value * FloatToRawScale);
    }

    public override readonly bool Equals(object? obj) {
        return obj is Q16 other && Equals(other);
    }

    public readonly bool Equals(Q16 other) {
        return RawValue == other.RawValue;
    }

    public override readonly int GetHashCode() {
        return RawValue;
    }

    public readonly int CompareTo(Q16 other) {
        return RawValue.CompareTo(other.RawValue);
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
