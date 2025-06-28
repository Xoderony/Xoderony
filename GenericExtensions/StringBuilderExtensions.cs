using System.Collections.Generic;
using System.Text;

namespace GuestUnion {

    public static class StringBuilderExtensions {
        private static readonly Stack<StringBuilder> pool = new();

        public static StringBuilder ToStringBuilder<T>(ref this T obj) where T : struct {
            if (pool.TryPop(out var sb)) {
                return sb.Append(obj.ToString());
            }
            return new StringBuilder().Append(obj.ToString());
        }

        public static StringBuilder ToStringBuilder<T>(this T obj) where T : class {
            if (pool.TryPop(out var sb)) {
                return sb.Append(obj);
            }
            return new StringBuilder().Append(obj);
        }

        public static string ToStringAndReturn(this StringBuilder sb) {
            if (sb is null) return string.Empty;
            var str = sb.ToString();
            pool.Push(sb.Clear());
            return str;
        }
    }
}