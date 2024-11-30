namespace GuestUnion.Pool {

    public class DefaultPooledObjectPolicy<T> : IPooledObjectPolicy<T> where T : class, new() {

        private DefaultPooledObjectPolicy() {
        }

        public static IPooledObjectPolicy<T> DefaultPolicy { get; } = new DefaultPooledObjectPolicy<T>();

        public T Create() => new();

        public bool Return(T obj) => obj is not null;
    }
}