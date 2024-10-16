using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GuestUnion.Extensions.StringExtensions {

    public static class StringExtension {
        public static void A() {
            System.Buffers.ArrayPool<string>.Shared.GetType();
        }
    }
}