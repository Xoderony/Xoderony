using System.Collections.Generic;
using Xoderony.Extensions;

namespace Xoderony;

public class ValueChannelMap<T> {
    private readonly Dictionary<int, ValueChannel<T>> _keyToValueChannel = [];

    public ValueChannel<T> GetOrAdd(int key) {
        ref var valueChannel = ref _keyToValueChannel.GetValueRefOrAddDefault(key, out _);
        valueChannel ??= new ValueChannel<T>();
        return valueChannel;
    }
}
