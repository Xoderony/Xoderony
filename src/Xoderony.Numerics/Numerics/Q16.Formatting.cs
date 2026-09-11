using System;

namespace Xoderony.Numerics;

public partial struct Q16 {

    private const int ToStringBufferLength = 32;

    public override readonly string ToString() {
        return ToString(null, null);
    }

    public readonly string ToString(string? format) {
        return ToString(format, null);
    }

    public readonly string ToString(string? format, IFormatProvider? formatProvider) {
        Span<char> buffer = stackalloc char[ToStringBufferLength];
        if (TryFormat(buffer, out var charsWritten, format, formatProvider)) {
            return new string(buffer[..charsWritten]);
        }
        var numerator = RawValue.ToString(format, formatProvider);
        var denominator = Scale.ToString(format, formatProvider);
        return string.Concat(numerator, "/", denominator);
    }

    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
        if (!RawValue.TryFormat(destination, out var numeratorWritten, format, provider)) {
            charsWritten = 0;
            return false;
        }
        if (numeratorWritten == destination.Length) {
            charsWritten = 0;
            return false;
        }
        destination[numeratorWritten] = '/';
        if (!Scale.TryFormat(destination[(numeratorWritten + 1)..], out var denominatorWritten, format, provider)) {
            charsWritten = 0;
            return false;
        }
        charsWritten = numeratorWritten + 1 + denominatorWritten;
        return true;
    }

    public readonly bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
        if (!RawValue.TryFormat(utf8Destination, out var numeratorWritten, format, provider)) {
            bytesWritten = 0;
            return false;
        }
        if (numeratorWritten == utf8Destination.Length) {
            bytesWritten = 0;
            return false;
        }
        utf8Destination[numeratorWritten] = (byte)'/';
        if (!Scale.TryFormat(utf8Destination[(numeratorWritten + 1)..], out var denominatorWritten, format, provider)) {
            bytesWritten = 0;
            return false;
        }
        bytesWritten = numeratorWritten + 1 + denominatorWritten;
        return true;
    }
}
