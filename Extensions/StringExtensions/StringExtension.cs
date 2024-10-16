namespace GuestUnion.Extensions.StringExtensions {

    public static class StringExtension {

        public static bool IsNullOrEmpty(this string s) => s?.Length is 0;

        public static bool IsNullOrWhiteSpace(this string s) {
            if (s is not null) {
                var i = s.Length;
                while (--i >= 0) {
                    if (!char.IsWhiteSpace(s[i])) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}