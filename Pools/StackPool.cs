using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuestUnion.Pools
{
    public class StackPool<T> : IPool<Stack<T>>
    {
        private readonly Stack<Stack<T>> pool;

        public StackPool(ushort capacity)
        {
            pool = new Stack<Stack<T>>(capacity);
        }

        public Stack<T> Rent()
        {
            if (!pool.TryPop(out var stack))
            {
                stack = new Stack<T>();
            }
            return stack;
        }

        public bool Return([MaybeNullWhen(true)] ref Stack<T> value)
        {
            pool.Push(value);
            value = null;
            return true;
        }
    }
}
