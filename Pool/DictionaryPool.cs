using System.Collections.Generic;

namespace GuestUnion.Pool {

    public class DictionaryPool<TKey, TValue> : GenericPool<Dictionary<TKey, TValue>> {
        public static readonly DictionaryPool<TKey, TValue> shared = new();
    }
}