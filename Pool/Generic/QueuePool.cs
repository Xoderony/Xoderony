using System.Collections.Generic;

namespace GuestUnion.ObjectPool.Generic {

    public class QueuePool<T> : GenericPool<Queue<T>> {
        public static readonly QueuePool<T> shared = new();
    }
}