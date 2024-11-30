using GuestUnion.Pool;
using System.Collections.Generic;

namespace GuestUnion.GenericCollectionsExtensions {

    public static class GenericCollectionsExtensions {

        public static bool ReturnToListPool<T>(this List<T> collection) {
            collection.Clear();
            return ListPool<T>.shared.Return(collection);
        }

        public static bool ReturnToStackPool<T>(this Stack<T> collection) {
            collection.Clear();
            return StackPool<T>.shared.Return(collection);
        }

        public static bool ReturnToQueuePool<T>(this Queue<T> collection) {
            collection.Clear();
            return QueuePool<T>.shared.Return(collection);
        }

        public static bool ReturnToDictionaryPool<TKey, TValue>(this Dictionary<TKey, TValue> collection) {
            collection.Clear();
            return DictionaryPool<TKey, TValue>.shared.Return(collection);
        }
    }
}