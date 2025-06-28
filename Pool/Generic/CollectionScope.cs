using System;
using System.Collections.Generic;

namespace GuestUnion.ObjectPool.Generic {

    public struct CollectionScope<TCollection, TElement> : IDisposable where TCollection : class, ICollection<TElement> {
        private readonly IPool<TCollection> _pool;
        private TCollection? _collection;
        public readonly TCollection? Collection => _collection;

        public CollectionScope(IPool<TCollection> pool, TCollection? collection) {
            if(pool is null) throw new ArgumentNullException(nameof(pool));
            _pool = pool;
            _collection = collection;
        }

        public void Dispose() {
            if (_collection is null) return;
            _collection.Clear();
            _pool.Return(_collection);
            _collection = null;
        }
    }
}