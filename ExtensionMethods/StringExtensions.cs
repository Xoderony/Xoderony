using System;
using System.Text;

namespace GuestUnion.StringExtensions {

    public static class StringExtensions {

        public static bool IsNullOrEmpty(this string s) => s?.Length is 0;

        public static bool IsNullOrWhiteSpace(this string s) {
            if (s is null) return true;
            foreach (var c in s.AsSpan()) {
                if (!char.IsWhiteSpace(c)) return false;
            }
            return true;
        }

        public static StringBuilder ToStringBuilder(this string s, int capacity = 16) => new(s, capacity);
    }
}