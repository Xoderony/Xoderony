using GuestUnion.ObjectPool.Generic;
using UnityEngine;

namespace GuestUnion {

    public static class GameObjectExtensions {

        public static void RemoveComponent<T>(this GameObject gameObject) where T : Component {
            if (gameObject.TryGetComponent<T>(out var component)) {
                Object.Destroy(component);
            }
        }

        public static void RemoveComponents<T>(this GameObject gameObject) where T : Component {
            using (ListPool<T>.Rent(out var list)) {
                gameObject.GetComponents(list);
                foreach (var component in list) {
                    Object.Destroy(component);
                }
            }
        }
    }
}