using System;

namespace GuestUnion {

    public static class StringExtensions {

        public static bool IsNullOrEmpty(this string s) => s is null || s.Length is 0;

        public static bool IsNullOrWhiteSpace(this string s) {
            if (s is null) return true;
            foreach (var c in s.AsSpan()) {
                if (!char.IsWhiteSpace(c)) return false;
            }
            return true;
        }
    }
}