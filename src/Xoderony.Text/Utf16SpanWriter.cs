using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Xoderony.Text;

public ref struct Utf16SpanWriter(Span<char> destination) {

    public Span<char> Destination = destination;

    public int Position;

    public readonly int Remaining => Destination.Length - Position;

    public readonly Span<char> RemainingSpan => Destination[Position..];

    public readonly ReadOnlySpan<char> WrittenSpan => Destination[..Position];

    [UnscopedRef]
    public ref Utf16SpanWriter Write(char value) {
        Destination[Position] = value;
        Position++;
        return ref this;
    }

    [UnscopedRef]
    public ref Utf16SpanWriter Write(char value, int count) {
        Destination.Slice(Position, count).Fill(value);
        Position += count;
        return ref this;
    }

    [UnscopedRef]
    public ref Utf16SpanWriter Write(bool value) {
        return ref Write(value ? "True" : "False");
    }

    [UnscopedRef]
    public ref Utf16SpanWriter Write(bool? value) {
        if (!value.HasValue) {
            return ref this;
        }

        return ref Write(value.Value);
    }

    [UnscopedRef]
    public ref Utf16SpanWriter Write(scoped ReadOnlySpan<char> value) {
        value.CopyTo(RemainingSpan);
        Position += value.Length;
        return ref this;
    }

    /// <summary>将 UTF-8 文本解码后写入目标缓冲区。</summary>
    /// <remarks>无效的 UTF-8 序列使用 <see cref="Encoding.UTF8"/> 的替换回退。</remarks>
    [UnscopedRef]
    public ref Utf16SpanWriter WriteUtf8(scoped ReadOnlySpan<byte> value) {
        var charsWritten = Encoding.UTF8.GetChars(value, RemainingSpan);
        Position += charsWritten;
        return ref this;
    }

    [UnscopedRef]
    public ref Utf16SpanWriter Write<T>(T value, scoped ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : ISpanFormattable {
        var success = value.TryFormat(RemainingSpan, out var charsWritten, format, provider);
        Debug.Assert(success);
        Position += charsWritten;
        return ref this;
    }

    [UnscopedRef]
    public ref Utf16SpanWriter Write<T>(T? value, scoped ReadOnlySpan<char> format = default, IFormatProvider? provider = null) where T : struct, ISpanFormattable {
        if (!value.HasValue) {
            return ref this;
        }

        return ref Write(value.Value, format, provider);
    }

    [UnscopedRef]
    public ref Utf16SpanWriter WriteLine() {
        return ref Write(Environment.NewLine);
    }
}
