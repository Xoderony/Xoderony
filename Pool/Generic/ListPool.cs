using System.Collections.Generic;

namespace GuestUnion.ObjectPool.Generic {

    public class ListPool<T> : GenericPool<List<T>> {
        public static readonly ListPool<T> shared = new();

        public static CollectionScope<List<T>, T> Rent(out List<T> list) {
            list = shared.Rent();
            return new(shared, list);
        }
    }
}