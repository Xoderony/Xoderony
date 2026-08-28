using System;

namespace Xoderony.Numerics;

public partial struct Q16 {

    public override readonly string ToString() {
        return ToString(null, null);
    }

    public readonly string ToString(string? format) {
        return ToString(format, null);
    }

    public readonly string ToString(string? format, IFormatProvider? formatProvider) {
        Span<char> buffer = stackalloc char[32];
        if (!TryFormat(buffer, out var charsWritten, format, formatProvider)) {
            throw new FormatException("The format of Q16 is invalid or too long for the destination buffer.");
        }
        return new string(buffer[..charsWritten]);
    }

    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
        if (!rawValue.TryFormat(destination, out var rawWritten, format, provider) ) {
            charsWritten = 0;
            return false;
        }
        if (rawWritten == destination.Length){
            charsWritten = 0;
            return false;
        }
        destination[rawWritten] = '/';
        if (!OneRawValue.TryFormat(destination[(rawWritten + 1)..], out var denomWritten, format, provider)) {
            charsWritten = 0;
            return false;
        }
        charsWritten = rawWritten + 1 + denomWritten;
        return true;
    }
}
