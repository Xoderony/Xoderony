using System.Collections.Generic;

namespace GuestUnion {

    public static class CollectionExtensions {

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => collection is null || collection.Count is 0;
    }
}