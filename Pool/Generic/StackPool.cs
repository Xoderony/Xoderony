using System.Collections.Generic;

namespace GuestUnion.ObjectPool.Generic {

    public class StackPool<T> : GenericPool<Stack<T>> {
        public static readonly StackPool<T> shared = new();
    }
}