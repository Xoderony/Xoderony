using System.Collections.Generic;

namespace Xoderony.Localization.Json;

internal static class FormatPlaceholderIndices {

    public static SortedSet<int> Collect(string format) {
        var indices = new SortedSet<int>();
        for (var index = 0; index < format.Length; index++) {
            var character = format[index];
            if (character == '{') {
                if (index + 1 < format.Length && format[index + 1] == '{') {
                    index++;
                    continue;
                }

                if (!TryReadPlaceholderIndex(format, index + 1, out var placeholderIndex, out var endIndex)) {
                    continue;
                }

                indices.Add(placeholderIndex);
                index = endIndex;
                continue;
            }

            if (character == '}' && index + 1 < format.Length && format[index + 1] == '}') {
                index++;
            }
        }

        return indices;
    }

    private static bool TryReadPlaceholderIndex(string format, int startIndex, out int placeholderIndex, out int endIndex) {
        placeholderIndex = 0;
        endIndex = startIndex;
        if (startIndex >= format.Length || format[startIndex] is < '0' or > '9') {
            return false;
        }

        var value = 0;
        var index = startIndex;
        while (index < format.Length && format[index] is >= '0' and <= '9') {
            value = (value * 10) + (format[index] - '0');
            index++;
        }

        while (index < format.Length) {
            var character = format[index];
            if (character == '}') {
                placeholderIndex = value;
                endIndex = index;
                return true;
            }

            if (character is ',' or ':') {
                index++;
                continue;
            }

            if (character == '{') {
                return false;
            }

            index++;
        }

        return false;
    }
}
