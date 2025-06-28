namespace GuestUnion.ObjectPool {

    public interface IPool<T> where T : class {

        T Rent();

        bool Return(T value);
    }
}