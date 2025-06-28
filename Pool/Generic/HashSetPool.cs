using System.Collections.Generic;

namespace GuestUnion.ObjectPool.Generic {
    public class HashSetPool<T> : GenericPool<HashSet<T>> {
        public static readonly HashSetPool<T> shared = new();

        public static CollectionScope<HashSet<T>, T> Rent(out HashSet<T> set) { 
            set = shared.Rent();
            return new(shared, set);
        }
    }
}
