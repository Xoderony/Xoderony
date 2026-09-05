using System;
#if NET10_0_OR_GREATER
using System.Diagnostics;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Xoderony.Text;

public ref struct Utf8SpanWriter(Span<byte> destination) {

    public Span<byte> Destination = destination;

    public int Position;

    public readonly int Remaining => Destination.Length - Position;

    public readonly Span<byte> RemainingSpan => Destination[Position..];

    public readonly ReadOnlySpan<byte> WrittenSpan => Destination[..Position];

    [UnscopedRef]
    public ref Utf8SpanWriter Write(byte value) {
        Destination[Position] = value;
        Position++;
        return ref this;
    }

    [UnscopedRef]
    public ref Utf8SpanWriter Write(byte value, int count) {
        Destination.Slice(Position, count).Fill(value);
        Position += count;
        return ref this;
    }

    [UnscopedRef]
    public ref Utf8SpanWriter Write(bool value) {
        return ref Write(value ? "True"u8 : "False"u8);
    }

    [UnscopedRef]
    public ref Utf8SpanWriter Write(bool? value) {
        if (!value.HasValue) {
            return ref this;
        }

        return ref Write(value.Value);
    }

    [UnscopedRef]
    public ref Utf8SpanWriter Write(scoped ReadOnlySpan<byte> value) {
        value.CopyTo(RemainingSpan);
        Position += value.Length;
        return ref this;
    }

    /// <summary>将 UTF-16 文本编码后写入目标缓冲区。</summary>
    /// <remarks>无效的 UTF-16 序列使用 <see cref="Encoding.UTF8"/> 的替换回退。</remarks>
    [UnscopedRef]
    public ref Utf8SpanWriter WriteUtf16(scoped ReadOnlySpan<char> value) {
        var bytesWritten = Encoding.UTF8.GetBytes(value, RemainingSpan);
        Position += bytesWritten;
        return ref this;
    }

#if NET10_0_OR_GREATER
    [UnscopedRef]
    public ref Utf8SpanWriter Write<T>(T value, scoped ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : IUtf8SpanFormattable {
        var success = value.TryFormat(RemainingSpan, out var bytesWritten, format, provider);
        Debug.Assert(success);
        Position += bytesWritten;
        return ref this;
    }

    [UnscopedRef]
    public ref Utf8SpanWriter Write<T>(T? value, scoped ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : struct, IUtf8SpanFormattable {
        if (!value.HasValue) {
            return ref this;
        }

        return ref Write(value.Value, format, provider);
    }
#endif

    [UnscopedRef]
    public ref Utf8SpanWriter WriteLine() {
        return ref WriteUtf16(Environment.NewLine);
    }
}
