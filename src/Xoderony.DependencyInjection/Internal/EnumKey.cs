using System;
using System.Runtime.CompilerServices;

namespace Xoderony.DependencyInjection.Internal;

internal static class EnumKey<TKey> where TKey : struct, Enum {
    private static readonly bool IsInt32 = Enum.GetUnderlyingType(typeof(TKey)) == typeof(int);

    internal static int ToInt32(TKey key) {
        if (!IsInt32) {
            throw new ArgumentException("Service keys require an enum with Int32 as its underlying type.", nameof(key));
        }
        return Unsafe.As<TKey, int>(ref key);
    }
}
