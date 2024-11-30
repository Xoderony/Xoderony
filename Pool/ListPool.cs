using System.Collections.Generic;

namespace GuestUnion.Pool {

    public class ListPool<T> : GenericPool<List<T>> {
        public static readonly ListPool<T> shared = new();
    }
}