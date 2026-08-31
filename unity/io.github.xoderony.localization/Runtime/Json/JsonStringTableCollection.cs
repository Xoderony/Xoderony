using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

public sealed class JsonStringTableCollection {

    private readonly Dictionary<string, JsonStringTable> _tableByCultureName = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<JsonStringTable> _dirtyTables = [];
    private readonly List<JsonStringTable> _tables = [];
    private ReadOnlyCollection<CultureInfo> _cultures = new([]);
    private JsonStringTableGroup _rootGroup = new(string.Empty, string.Empty);

    public IReadOnlyList<CultureInfo> Cultures => _cultures;

    public JsonStringTableGroup RootGroup => _rootGroup;

    public bool IsDirty => _dirtyTables.Count != 0;

    internal JsonStringTableCollection(IEnumerable<JsonStringTable> tables) {
        ArgumentNullException.ThrowIfNull(tables);

        foreach (var table in tables) {
            ArgumentNullException.ThrowIfNull(table);
            if (!_tableByCultureName.TryAdd(table.Culture.Name, table)) {
                throw new ArgumentException($"The culture '{table.Culture.Name}' has more than one string table.", nameof(tables));
            }

            _tables.Add(table);
        }

        _tables.Sort(CompareTables);
        UpdateCultures();
        RebuildAndComplete();
    }

    public static JsonStringTableCollection LoadDirectory(string directoryPath) {
        ArgumentException.ThrowIfNullOrEmpty(directoryPath);

        var paths = new List<string>(Directory.EnumerateFiles(directoryPath, "*.json", SearchOption.TopDirectoryOnly));
        paths.Sort(StringComparer.Ordinal);

        var tables = new List<JsonStringTable>(paths.Count);
        foreach (var path in paths) {
            var cultureName = Path.GetFileNameWithoutExtension(path);
            var culture = CultureInfo.GetCultureInfo(cultureName);
            tables.Add(JsonStringTable.Load(culture, path));
        }

        return new JsonStringTableCollection(tables);
    }

    public void Save() {
        foreach (var table in _tables) {
            if (!_dirtyTables.Contains(table)) {
                continue;
            }

            table.Save();
            _dirtyTables.Remove(table);
        }
    }

    public void AddLocale(CultureInfo culture, string filePath) {
        ArgumentNullException.ThrowIfNull(culture);
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The table culture cannot be invariant.", nameof(culture));
        }

        ArgumentException.ThrowIfNullOrEmpty(filePath);

        var table = new JsonStringTable(culture, filePath, new JsonObject());
        if (_tableByCultureName.ContainsKey(table.Culture.Name)) {
            throw new ArgumentException($"The culture '{table.Culture.Name}' already has a string table.", nameof(culture));
        }

        _tableByCultureName.Add(table.Culture.Name, table);
        _tables.Add(table);
        _tables.Sort(CompareTables);
        _dirtyTables.Add(table);
        UpdateCultures();
        RebuildAndComplete();
    }

    public void AddGroup(string parentGroupKey, string localKey) {
        AddNode(parentGroupKey, localKey, isGroup: true);
    }

    public void AddEntry(string parentGroupKey, string localKey) {
        AddNode(parentGroupKey, localKey, isGroup: false);
    }

    public void Rename(string key, string newLocalKey) {
        var node = _rootGroup.Get(key);
        JsonStringTableNode.ValidateLocalKey(newLocalKey, nameof(newLocalKey));
        if (string.Equals(node.LocalKey, newLocalKey, StringComparison.Ordinal)) {
            return;
        }

        var parentKey = JsonStringTableNode.GetParentKey(key);
        if (_rootGroup.GetGroup(parentKey).Children.ContainsKey(newLocalKey)) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(parentKey, newLocalKey)}' already exists.", nameof(newLocalKey));
        }

        foreach (var table in _tables) {
            var parent = GetObject(table.Root, parentKey);
            var value = RemoveValue(parent, node.LocalKey, table, key);
            parent.Add(newLocalKey, value);
            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    public void Move(string key, string newParentGroupKey) {
        var node = _rootGroup.Get(key);
        var oldParentKey = JsonStringTableNode.GetParentKey(key);
        if (string.Equals(oldParentKey, newParentGroupKey, StringComparison.Ordinal)) {
            return;
        }

        ValidateNotDescendant(key, newParentGroupKey);
        if (_rootGroup.GetGroup(newParentGroupKey).Children.ContainsKey(node.LocalKey)) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(newParentGroupKey, node.LocalKey)}' already exists.", nameof(newParentGroupKey));
        }

        foreach (var table in _tables) {
            var oldParent = GetObject(table.Root, oldParentKey);
            var newParent = GetObject(table.Root, newParentGroupKey);
            var value = RemoveValue(oldParent, node.LocalKey, table, key);
            newParent.Add(node.LocalKey, value);
            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    public void Copy(string key, string newParentGroupKey, string newLocalKey) {
        var node = _rootGroup.Get(key);
        JsonStringTableNode.ValidateLocalKey(newLocalKey, nameof(newLocalKey));
        ValidateNotDescendant(key, newParentGroupKey);
        if (_rootGroup.GetGroup(newParentGroupKey).Children.ContainsKey(newLocalKey)) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(newParentGroupKey, newLocalKey)}' already exists.", nameof(newLocalKey));
        }

        var oldParentKey = JsonStringTableNode.GetParentKey(key);
        foreach (var table in _tables) {
            var oldParent = GetObject(table.Root, oldParentKey);
            var value = oldParent[node.LocalKey] ?? throw new InvalidOperationException($"The normalized table '{table.Culture.Name}' does not contain '{key}'.");
            var newParent = GetObject(table.Root, newParentGroupKey);
            newParent.Add(newLocalKey, value.DeepClone());
            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    public void Remove(string key) {
        var node = _rootGroup.Get(key);
        var parentKey = JsonStringTableNode.GetParentKey(key);
        foreach (var table in _tables) {
            var parent = GetObject(table.Root, parentKey);
            RemoveValue(parent, node.LocalKey, table, key);
            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    public string GetValue(CultureInfo culture, string key) {
        if (_rootGroup.Get(key) is not JsonStringTableEntry entry) {
            throw new ArgumentException($"The localization path '{key}' is a group.", nameof(key));
        }

        var table = GetTable(culture);
        var parent = GetObject(table.Root, JsonStringTableNode.GetParentKey(key));
        var value = parent[entry.LocalKey] ?? throw new InvalidOperationException($"The normalized table does not contain '{key}'.");
        return value.GetValue<string>();
    }

    public void SetValue(CultureInfo culture, string key, string value) {
        ArgumentNullException.ThrowIfNull(value);

        if (_rootGroup.Get(key) is not JsonStringTableEntry entry) {
            throw new ArgumentException($"The localization path '{key}' is a group.", nameof(key));
        }

        var table = GetTable(culture);
        var parent = GetObject(table.Root, JsonStringTableNode.GetParentKey(key));
        if (string.Equals(parent[entry.LocalKey]?.GetValue<string>(), value, StringComparison.Ordinal)) {
            return;
        }

        parent[entry.LocalKey] = value;
        _dirtyTables.Add(table);
    }

    public int GetEmptyValueCount(CultureInfo culture) {
        return CountEmptyValues(GetTable(culture).Root, _rootGroup);
    }

    public IEnumerable<string> GetKeys() {
        return _rootGroup.EnumerateEntryKeys();
    }

    private void UpdateCultures() {
        var cultures = new List<CultureInfo>(_tables.Count);
        foreach (var table in _tables) {
            cultures.Add(table.Culture);
        }

        _cultures = cultures.AsReadOnly();
    }

    private static int CompareTables(JsonStringTable left, JsonStringTable right) {
        return StringComparer.Ordinal.Compare(left.Culture.Name, right.Culture.Name);
    }

    private JsonStringTable GetTable(CultureInfo culture) {
        ArgumentNullException.ThrowIfNull(culture);
        if (!_tableByCultureName.TryGetValue(culture.Name, out var table)) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of this string table collection.", nameof(culture));
        }

        return table;
    }

    private void AddNode(string parentGroupKey, string localKey, bool isGroup) {
        JsonStringTableNode.ValidateLocalKey(localKey, nameof(localKey));
        if (_rootGroup.GetGroup(parentGroupKey).Children.ContainsKey(localKey)) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(parentGroupKey, localKey)}' already exists.", nameof(localKey));
        }

        foreach (var table in _tables) {
            var parent = GetObject(table.Root, parentGroupKey);
            if (isGroup) {
                parent.Add(localKey, new JsonObject());
            } else {
                parent.Add(localKey, JsonValue.Create(string.Empty));
            }

            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    private static JsonNode RemoveValue(JsonObject source, string localKey, JsonStringTable table, string fullKey) {
        var value = source[localKey] ?? throw new InvalidOperationException($"The normalized table '{table.Culture.Name}' does not contain '{fullKey}'.");
        source.Remove(localKey);
        return value;
    }

    private static void ValidateNotDescendant(string key, string newParentGroupKey) {
        if (string.Equals(key, newParentGroupKey, StringComparison.Ordinal) || newParentGroupKey.StartsWith($"{key}.", StringComparison.Ordinal)) {
            throw new ArgumentException("A localization node cannot be moved into itself or one of its descendants.", nameof(newParentGroupKey));
        }
    }

    private void RebuildAndComplete() {
        var root = new JsonStringTableGroup(string.Empty, string.Empty);
        foreach (var table in _tables) {
            MergeObject(table, table.Root, root);
        }

        foreach (var table in _tables) {
            CompleteObject(table, table.Root, root);
        }

        _rootGroup = root;
    }

    private static void MergeObject(JsonStringTable table, JsonObject source, JsonStringTableGroup target) {
        foreach (var (localKey, value) in source) {
            JsonStringTableNode.ValidateSourceLocalKey(localKey, table.FilePath);
            if (value is JsonObject child) {
                var childGroup = target.GetOrAddGroup(localKey);
                MergeObject(table, child, childGroup);
                continue;
            }

            if (value is JsonValue jsonValue && jsonValue.TryGetValue<string>(out _)) {
                target.GetOrAddEntry(localKey);
                continue;
            }

            var fullKey = JsonStringTableNode.CombineKey(target.FullKey, localKey);
            throw new InvalidDataException($"The localization value '{fullKey}' in '{table.FilePath}' must be a string or object.");
        }
    }

    private void CompleteObject(JsonStringTable table, JsonObject target, JsonStringTableGroup schema) {
        foreach (var child in schema.Children.Values) {
            var value = target[child.LocalKey];
            if (value is null) {
                value = child is JsonStringTableGroup ? new JsonObject() : JsonValue.Create(string.Empty);
                target.Add(child.LocalKey, value);
                _dirtyTables.Add(table);
            }

            if (child is JsonStringTableGroup childGroup) {
                CompleteObject(table, (JsonObject)value, childGroup);
            }
        }
    }

    private static int CountEmptyValues(JsonObject source, JsonStringTableGroup schema) {
        var count = 0;
        foreach (var child in schema.Children.Values) {
            var value = source[child.LocalKey] ?? throw new InvalidOperationException($"The normalized table does not contain '{child.FullKey}'.");
            switch (child) {
                case JsonStringTableEntry:
                    if (value.GetValue<string>().Length == 0) {
                        count++;
                    }

                    break;
                case JsonStringTableGroup childGroup:
                    count += CountEmptyValues((JsonObject)value, childGroup);
                    break;
            }
        }

        return count;
    }

    private static JsonObject GetObject(JsonObject root, string key) {
        var current = root;
        foreach (var localKey in key.Split('.', StringSplitOptions.RemoveEmptyEntries)) {
            current = (JsonObject)(current[localKey] ?? throw new InvalidOperationException($"The normalized table does not contain '{key}'."));
        }

        return current;
    }
}
