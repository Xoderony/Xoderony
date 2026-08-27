using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xoderony;

public class ValueChannelMap<T> {
    private readonly Dictionary<int, ValueChannel<T>> _keyToValueChannel = [];

    public ValueChannel<T> GetOrAdd(int key) {
        ref var valueChannel = ref CollectionsMarshal.GetValueRefOrAddDefault(_keyToValueChannel, key, out _);
        valueChannel ??= new ValueChannel<T>();
        return valueChannel;
    }
}
