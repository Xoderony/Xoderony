using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuestUnion.Pools {

    public class ListPool<T> : IPool<IList<T>> {
        private readonly Stack<IList<T>> pool;

        public ListPool(ushort capacity = 15) {
            pool = new Stack<IList<T>>(capacity);
        }

        public IList<T> Rent() {
            if (!pool.TryPop(out var list)) {
                list = new List<T>();
            }
            return list;
        }

        public bool Return([MaybeNullWhen(true)] ref IList<T> value) {
            pool.Push(value);
            value = null;
            return true;
        }
    }
}