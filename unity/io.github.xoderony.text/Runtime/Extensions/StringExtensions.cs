namespace Xoderony.Extensions {

    public static class StringExtensions {

#if NET10_0_OR_GREATER
        extension(string? value) {

            public bool IsNullOrEmpty => string.IsNullOrEmpty(value);

            public bool IsNullOrWhiteSpace => string.IsNullOrWhiteSpace(value);
        }
#else
        public static bool IsNullOrEmpty(this string? value) {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string? value) {
            return string.IsNullOrWhiteSpace(value);
        }
#endif
    }
}

#if !NET10_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis {

    // 供编译器识别两个 Span writer 的 ref this 返回契约。
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
    internal sealed class UnscopedRefAttribute : Attribute {
    }
}
#endif
