using System.Collections.Generic;
#if NET10_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace Xoderony;

public class ValueChannelMap<T> {
    private readonly Dictionary<int, ValueChannel<T>> _keyToValueChannel = [];

    public ValueChannel<T> GetOrAdd(int key) {
#if NET10_0_OR_GREATER
        ref var valueChannel = ref CollectionsMarshal.GetValueRefOrAddDefault(_keyToValueChannel, key, out _);
#else
        _keyToValueChannel.TryGetValue(key, out var valueChannel);
#endif
        if (valueChannel is null) {
            valueChannel = new ValueChannel<T>();
#if !NET10_0_OR_GREATER
            _keyToValueChannel[key] = valueChannel;
#endif
        }
        return valueChannel;
    }
}
