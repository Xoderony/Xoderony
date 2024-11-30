using System;
using System.Collections.Generic;

namespace GuestUnion.Pool {

    public class ObjectPool<T> : IPool<T>, IDisposable where T : class {
        public ushort capacity;
        private readonly Stack<T> pool;
        private readonly IPooledObjectPolicy<T> policy;

        public ObjectPool(IPooledObjectPolicy<T> policy, ushort capacity = 15) {
            this.policy = policy;
            this.capacity = capacity;
            pool = new(capacity);
        }

        public T Rent() => pool.TryPop(out var result) ? result : policy.Create();

        public bool Return(T value) {
            if (policy.Return(value) && pool.Count < capacity) {
                pool.Push(value);
                return true;
            }
            return false;
        }

        public void Clear() => pool.Clear();

        void IDisposable.Dispose() => pool.Clear();
    }
}