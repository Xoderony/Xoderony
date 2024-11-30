using GuestUnion.Factory;

namespace GuestUnion.Pool {

    public interface IPooledObjectPolicy<T> : IFactory<T> {

        /// <summary>
        /// Runs some processing when an object was returned to the pool. Can be
        /// used to reset the state of an object and indicate if the object
        /// should be returned to the pool.
        /// </summary>
        /// <param name="obj">The object to return to the pool.</param>
        /// <returns>
        /// <see langword="true"/> if the object should be returned to the pool.
        /// <see langword="false"/> if it's not possible/desirable for the pool
        /// to keep the object.
        /// </returns>
        bool Return(T obj);
    }
}