using System.Diagnostics.CodeAnalysis;

namespace GuestUnion.Pools {

    public interface IPool<T> {

        T Rent();

        bool Return([MaybeNullWhen(true)] ref T value);
    }
}