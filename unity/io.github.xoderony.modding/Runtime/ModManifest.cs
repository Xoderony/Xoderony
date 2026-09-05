using System.Collections.Generic;

namespace Xoderony.Modding;

public sealed class ModManifest {

    public string Id { get; set; } = "";

    public string Version { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Name { get; set; } = "";

    public string Author { get; set; } = "";

    public string Description { get; set; } = "";

    public Dictionary<string, string> Dependencies { get; set; } = [];
}

#if !NET10_0_OR_GREATER
internal sealed class OrderedModMap<T> where T : class {
    private readonly Dictionary<string, T> _items = new(System.StringComparer.Ordinal);
    private readonly List<string> _keys = [];
    private readonly List<T> _values = [];
    private readonly System.Collections.ObjectModel.ReadOnlyCollection<T> _valueView;

    public IReadOnlyList<string> Keys => _keys;
    public IReadOnlyList<T> Values => _valueView;

    public OrderedModMap() {
        _valueView = _values.AsReadOnly();
    }

    public bool ContainsKey(string key) {
        return _items.ContainsKey(key);
    }

    public bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) {
        return _items.TryGetValue(key, out value);
    }

    public void Add(string key, T value) {
        _items.Add(key, value);
        _keys.Add(key);
        _values.Add(value);
    }

    public void Remove(string key) {
        if (!_items.Remove(key)) {
            return;
        }
        var index = _keys.IndexOf(key);
        _keys.RemoveAt(index);
        _values.RemoveAt(index);
    }

    public void Clear() {
        _items.Clear();
        _keys.Clear();
        _values.Clear();
    }
}
#endif
