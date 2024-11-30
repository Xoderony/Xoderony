using System.Collections.Generic;

namespace GuestUnion.Pool {

    public class QueuePool<T> : GenericPool<Queue<T>> {
        public static readonly QueuePool<T> shared = new();
    }
}