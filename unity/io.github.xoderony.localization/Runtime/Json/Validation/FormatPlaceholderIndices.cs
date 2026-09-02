using System.Collections.Generic;

namespace Xoderony.Localization.Json;

internal static class FormatPlaceholderIndices {

    public static bool TryCollect(string format, out SortedSet<int> indices) {
        indices = [];
        for (var index = 0; index < format.Length; index++) {
            var character = format[index];
            if (character == '{') {
                if (index + 1 < format.Length && format[index + 1] == '{') {
                    index++;
                    continue;
                }

                if (!TryReadPlaceholder(format, index + 1, out var placeholderIndex, out var endIndex)) {
                    return false;
                }

                indices.Add(placeholderIndex);
                index = endIndex;
                continue;
            }

            if (character == '}') {
                if (index + 1 >= format.Length || format[index + 1] != '}') {
                    return false;
                }

                index++;
            }
        }

        return true;
    }

    private static bool TryReadPlaceholder(string format, int startIndex, out int placeholderIndex, out int endIndex) {
        placeholderIndex = 0;
        endIndex = startIndex;
        if (startIndex >= format.Length || format[startIndex] is < '0' or > '9') {
            return false;
        }

        var value = 0;
        var index = startIndex;
        while (index < format.Length && format[index] is >= '0' and <= '9') {
            var digit = format[index] - '0';
            if (value > (int.MaxValue - digit) / 10) {
                return false;
            }

            value = (value * 10) + digit;
            index++;
        }

        SkipSpaces(format, ref index);
        if (index < format.Length && format[index] == ',') {
            index++;
            SkipSpaces(format, ref index);
            if (index < format.Length && format[index] == '-') {
                index++;
            }

            if (index >= format.Length || format[index] is < '0' or > '9') {
                return false;
            }

            var alignment = 0;
            while (index < format.Length && format[index] is >= '0' and <= '9') {
                var digit = format[index] - '0';
                if (alignment > (int.MaxValue - digit) / 10) {
                    return false;
                }

                alignment = (alignment * 10) + digit;
                index++;
            }

            SkipSpaces(format, ref index);
        }

        if (index < format.Length && format[index] == ':') {
            index++;
            while (index < format.Length) {
                var character = format[index];
                if (character == '{') {
                    return false;
                }

                if (character == '}') {
                    placeholderIndex = value;
                    endIndex = index;
                    return true;
                }

                index++;
            }

            return false;
        }

        if (index >= format.Length || format[index] != '}') {
            return false;
        }

        placeholderIndex = value;
        endIndex = index;
        return true;

        static void SkipSpaces(string text, ref int position) {
            while (position < text.Length && text[position] == ' ') {
                position++;
            }
        }
    }
}
