using System.Collections;

namespace Xoderony.Extensions;

public static class CollectionExtensions {

    extension<T>(T[]? array) {

        public bool IsNullOrEmpty => (array is null) || (array.Length == 0);

    }

    extension<T>(T? collection) where T : ICollection {

        public bool IsNullOrEmpty => (collection is null) || (collection.Count == 0);

    }
}
