namespace Xoderony.Extensions;

public static class StringExtensions {

    extension(string? value) {

        public bool IsNullOrEmpty => string.IsNullOrEmpty(value);

        public bool IsNullOrWhiteSpace => string.IsNullOrWhiteSpace(value);
    }
}
