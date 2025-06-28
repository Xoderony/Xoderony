using System.Collections.Generic;

namespace GuestUnion.ObjectPool.Generic {

    public class DictionaryPool<TKey, TValue> : GenericPool<Dictionary<TKey, TValue>> {
        public static readonly DictionaryPool<TKey, TValue> shared = new();

        public static CollectionScope<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> Rent(out Dictionary<TKey, TValue> dictionary) {
            dictionary = shared.Rent();
            return new(shared, dictionary);
        }
    }
}