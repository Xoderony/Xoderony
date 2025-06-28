using System;
using System.Collections.Generic;

namespace GuestUnion.ObjectPool.Generic {

    public class GenericPool<T> : IPool<T>, IDisposable where T : class, new() {
        public ushort capacity;
        private readonly Stack<T> pool;

        public GenericPool(ushort capacity = 16) => pool = new(capacity);

        public T Rent() => pool.TryPop(out var result) ? result : new();

        public bool Return(T value) {
            if (value is not null && pool.Count < capacity) {
                pool.Push(value);
                return true;
            }
            return false;
        }

        public void Clear() => pool.Clear();

        void IDisposable.Dispose() => Clear();
    }
}