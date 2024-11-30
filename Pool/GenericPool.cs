using System.Collections.Generic;

namespace GuestUnion.Pool {

    public class GenericPool<T> : IPool<T> where T : class, new() {
        public ushort capacity;
        private readonly Stack<T> pool;

        public GenericPool(ushort capacity = 15) => pool = new(capacity);

        public T Rent() => pool.TryPop(out var result) ? result : new();

        public bool Return(T value) {
            if (value != null && pool.Count < capacity) {
                pool.Push(value);
                return true;
            }
            return false;
        }

        public void Clear() => pool.Clear();
    }
}