using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

public sealed class JsonStringTableCollection {

    public const string KeysFileName = "keys.json";

    private readonly HashSet<JsonStringTable> _dirtyTables = [];
    private readonly string _keysFilePath;
    private readonly SortedDictionary<string, JsonStringTable> _tables;
    private bool _keysDirty;
    private JsonObject _keysRoot;
    private JsonStringTableGroup _rootGroup;

    public IReadOnlyList<CultureInfo> Cultures {
        get {
            if (_tables.Count == 0) {
                return [];
            }

            var cultures = new CultureInfo[_tables.Count];
            var index = 0;
            foreach (var table in _tables.Values) {
                cultures[index++] = table.Culture;
            }

            return cultures;
        }
    }

    public JsonStringTableGroup RootGroup => _rootGroup;

    public bool IsDirty => _keysDirty || _dirtyTables.Count != 0;

    /// <summary>接管 keysRoot 与 tables 的所有权，不重新解析或复制。</summary>
    private JsonStringTableCollection(string keysFilePath, JsonObject keysRoot, SortedDictionary<string, JsonStringTable> tables) {
        Debug.Assert(!string.IsNullOrWhiteSpace(keysFilePath));
        Debug.Assert(keysRoot is not null);
        Debug.Assert(tables is not null);

        _keysFilePath = keysFilePath;
        _keysRoot = keysRoot;
        _tables = tables;
        _rootGroup = BuildRootGroup();
    }

    public static JsonStringTableCollection LoadDirectory(string directoryPath) {
        Debug.Assert(!string.IsNullOrWhiteSpace(directoryPath));

        var keysFilePath = Path.Combine(directoryPath, KeysFileName);
        var keysRoot = LoadKeysRoot(keysFilePath);
        var tables = LoadTables(directoryPath);
        return new JsonStringTableCollection(keysFilePath, keysRoot, tables);

        static JsonObject LoadKeysRoot(string path) {
            if (!File.Exists(path)) {
                return [];
            }

            try {
                using var stream = File.OpenRead(path);
                var value = JsonNode.Parse(stream, documentOptions: new JsonDocumentOptions {
                    AllowTrailingCommas = false,
                    CommentHandling = JsonCommentHandling.Disallow
                });
                if (value is not JsonObject root) {
                    throw new InvalidDataException($"The root value in '{path}' must be an object.");
                }

                return root;
            } catch (JsonException exception) {
                throw new InvalidDataException($"The JSON file '{path}' is invalid.", exception);
            }
        }

        static SortedDictionary<string, JsonStringTable> LoadTables(string path) {
            var tables = new SortedDictionary<string, JsonStringTable>(StringComparer.OrdinalIgnoreCase);
            foreach (var filePath in Directory.EnumerateFiles(path, "*.json", SearchOption.TopDirectoryOnly)) {
                if (string.Equals(Path.GetFileName(filePath), KeysFileName, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                var cultureName = Path.GetFileNameWithoutExtension(filePath);
                var culture = CultureInfo.GetCultureInfo(cultureName);
                var table = JsonStringTable.Load(culture, filePath);
                if (!tables.TryAdd(table.Culture.Name, table)) {
                    throw new InvalidDataException($"The culture '{table.Culture.Name}' has more than one string table in '{path}'.");
                }
            }

            return tables;
        }
    }

    public void Save() {
        if (_keysDirty) {
            JsonStringTable.WriteJsonFile(_keysFilePath, _keysRoot);
            _keysDirty = false;
        }

        foreach (var table in _dirtyTables) {
            table.Save();
        }

        _dirtyTables.Clear();
    }

    public void AddLocale(CultureInfo culture, string filePath) {
        Debug.Assert(culture is not null);
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The table culture cannot be invariant.", nameof(culture));
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));

        culture = CultureInfo.GetCultureInfo(culture.Name);
        var table = new JsonStringTable(culture, filePath, new SortedDictionary<string, string>(StringComparer.Ordinal));
        if (!_tables.TryAdd(table.Culture.Name, table)) {
            throw new ArgumentException($"The culture '{table.Culture.Name}' already has a string table.", nameof(culture));
        }

        foreach (var key in _rootGroup.GetDescendantKeys()) {
            table.Values.Add(key, string.Empty);
        }

        _dirtyTables.Add(table);
    }

    public void AddGroup(string parentGroupKey, string localKey) {
        JsonStringTableNode.ValidateLocalKey(localKey, nameof(localKey));
        var parent = GetGroupObject(parentGroupKey);
        if (!parent.TryAdd(localKey, new JsonObject())) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(parentGroupKey, localKey)}' already exists.", nameof(localKey));
        }

        _keysDirty = true;
        _rootGroup = BuildRootGroup();
    }

    public void AddEntry(string parentGroupKey, string localKey) {
        JsonStringTableNode.ValidateLocalKey(localKey, nameof(localKey));
        var parent = GetGroupObject(parentGroupKey);
        if (!parent.TryAdd(localKey, null)) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(parentGroupKey, localKey)}' already exists.", nameof(localKey));
        }

        var key = JsonStringTableNode.CombineKey(parentGroupKey, localKey);
        foreach (var table in _tables.Values) {
            table.Values[key] = string.Empty;
            _dirtyTables.Add(table);
        }

        _keysDirty = true;
        _rootGroup = BuildRootGroup();
    }

    public void Rename(string key, string newLocalKey) {
        var node = _rootGroup.Get(key);
        JsonStringTableNode.ValidateLocalKey(newLocalKey, nameof(newLocalKey));
        if (string.Equals(node.LocalKey, newLocalKey, StringComparison.Ordinal)) {
            return;
        }

        var parentKey = JsonStringTableNode.GetParentKey(key);
        var parent = GetGroupObject(parentKey);
        if (parent.ContainsKey(newLocalKey)) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(parentKey, newLocalKey)}' already exists.", nameof(newLocalKey));
        }

        if (!parent.TryGetPropertyValue(node.LocalKey, out var value)) {
            throw new InvalidOperationException($"The keys document does not contain '{key}'.");
        }

        parent.Remove(node.LocalKey);
        parent.Add(newLocalKey, value?.DeepClone());

        var newKey = JsonStringTableNode.CombineKey(parentKey, newLocalKey);
        if (node is JsonStringTableGroup) {
            RewriteGroupValueKeys(key, newKey);
        } else {
            RewriteEntryValueKeys(key, newKey);
        }

        _keysDirty = true;
        _rootGroup = BuildRootGroup();
    }

    public void Move(string key, string newParentGroupKey) {
        var node = _rootGroup.Get(key);
        var oldParentKey = JsonStringTableNode.GetParentKey(key);
        if (string.Equals(oldParentKey, newParentGroupKey, StringComparison.Ordinal)) {
            return;
        }

        ValidateNotDescendant(key, newParentGroupKey);
        var newParent = GetGroupObject(newParentGroupKey);
        if (newParent.ContainsKey(node.LocalKey)) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(newParentGroupKey, node.LocalKey)}' already exists.", nameof(newParentGroupKey));
        }

        var oldParent = GetGroupObject(oldParentKey);
        if (!oldParent.TryGetPropertyValue(node.LocalKey, out var value)) {
            throw new InvalidOperationException($"The keys document does not contain '{key}'.");
        }

        oldParent.Remove(node.LocalKey);
        newParent.Add(node.LocalKey, value?.DeepClone());

        var newKey = JsonStringTableNode.CombineKey(newParentGroupKey, node.LocalKey);
        if (node is JsonStringTableGroup) {
            RewriteGroupValueKeys(key, newKey);
        } else {
            RewriteEntryValueKeys(key, newKey);
        }

        _keysDirty = true;
        _rootGroup = BuildRootGroup();
    }

    public void Copy(string key, string newParentGroupKey, string newLocalKey) {
        var node = _rootGroup.Get(key);
        JsonStringTableNode.ValidateLocalKey(newLocalKey, nameof(newLocalKey));
        ValidateNotDescendant(key, newParentGroupKey);
        var newParent = GetGroupObject(newParentGroupKey);
        if (newParent.ContainsKey(newLocalKey)) {
            throw new ArgumentException($"The localization path '{JsonStringTableNode.CombineKey(newParentGroupKey, newLocalKey)}' already exists.", nameof(newLocalKey));
        }

        var oldParentKey = JsonStringTableNode.GetParentKey(key);
        var oldParent = GetGroupObject(oldParentKey);
        if (!oldParent.TryGetPropertyValue(node.LocalKey, out var value)) {
            throw new InvalidOperationException($"The keys document does not contain '{key}'.");
        }

        newParent.Add(newLocalKey, value?.DeepClone());

        var newKey = JsonStringTableNode.CombineKey(newParentGroupKey, newLocalKey);
        if (node is JsonStringTableGroup) {
            CopyGroupValueKeys(key, newKey);
        } else {
            CopyEntryValueKeys(key, newKey);
        }

        _keysDirty = true;
        _rootGroup = BuildRootGroup();

        void CopyGroupValueKeys(string sourceKey, string targetKey) {
            foreach (var table in _tables.Values) {
                var additions = new List<(string Key, string Text)>();
                foreach (var (valueKey, text) in table.Values) {
                    if (string.Equals(valueKey, sourceKey, StringComparison.Ordinal) || valueKey.StartsWith($"{sourceKey}.", StringComparison.Ordinal)) {
                        additions.Add((targetKey + valueKey[sourceKey.Length..], text));
                    }
                }

                if (additions.Count == 0) {
                    continue;
                }

                foreach (var (valueKey, text) in additions) {
                    table.Values[valueKey] = text;
                }

                _dirtyTables.Add(table);
            }
        }

        void CopyEntryValueKeys(string sourceKey, string targetKey) {
            foreach (var table in _tables.Values) {
                if (table.Values.TryGetValue(sourceKey, out var text)) {
                    table.Values[targetKey] = text;
                } else {
                    table.Values[targetKey] = string.Empty;
                }

                _dirtyTables.Add(table);
            }
        }
    }

    public void Remove(string key) {
        var node = _rootGroup.Get(key);
        var parentKey = JsonStringTableNode.GetParentKey(key);
        var parent = GetGroupObject(parentKey);
        if (!parent.Remove(node.LocalKey)) {
            throw new InvalidOperationException($"The keys document does not contain '{key}'.");
        }

        if (node is JsonStringTableGroup) {
            RemoveGroupValueKeys(key);
        } else {
            RemoveEntryValueKeys(key);
        }

        _keysDirty = true;
        _rootGroup = BuildRootGroup();

        void RemoveGroupValueKeys(string groupKey) {
            foreach (var table in _tables.Values) {
                var removals = new List<string>();
                foreach (var valueKey in table.Values.Keys) {
                    if (string.Equals(valueKey, groupKey, StringComparison.Ordinal) || valueKey.StartsWith($"{groupKey}.", StringComparison.Ordinal)) {
                        removals.Add(valueKey);
                    }
                }

                if (removals.Count == 0) {
                    continue;
                }

                foreach (var valueKey in removals) {
                    table.Values.Remove(valueKey);
                }

                _dirtyTables.Add(table);
            }
        }

        void RemoveEntryValueKeys(string entryKey) {
            foreach (var table in _tables.Values) {
                if (table.Values.Remove(entryKey)) {
                    _dirtyTables.Add(table);
                }
            }
        }
    }

    public string GetValue(CultureInfo culture, string key) {
        if (_rootGroup.Get(key) is not JsonStringTableEntry) {
            throw new ArgumentException($"The localization path '{key}' is a group.", nameof(key));
        }

        var table = GetTable(culture);
        return table.Values.TryGetValue(key, out var value) ? value : string.Empty;
    }

    public void SetValue(CultureInfo culture, string key, string value) {
        Debug.Assert(value is not null);

        if (_rootGroup.Get(key) is not JsonStringTableEntry) {
            throw new ArgumentException($"The localization path '{key}' is a group.", nameof(key));
        }

        var table = GetTable(culture);
        if (table.Values.TryGetValue(key, out var current) && string.Equals(current, value, StringComparison.Ordinal)) {
            return;
        }

        table.Values[key] = value;
        _dirtyTables.Add(table);
    }

    public int GetEmptyValueCount(CultureInfo culture) {
        var table = GetTable(culture);
        var count = 0;
        foreach (var key in _rootGroup.GetDescendantKeys()) {
            if (!table.Values.TryGetValue(key, out var value) || value.Length == 0) {
                count++;
            }
        }

        return count;
    }

    public IEnumerable<string> GetKeys() {
        return _rootGroup.GetDescendantKeys();
    }

    private JsonStringTable GetTable(CultureInfo culture) {
        Debug.Assert(culture is not null);
        if (!_tables.TryGetValue(culture.Name, out var table)) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of this string table collection.", nameof(culture));
        }

        return table;
    }

    private void RewriteGroupValueKeys(string oldKey, string newKey) {
        foreach (var table in _tables.Values) {
            var replacements = new List<(string OldKey, string NewKey)>();
            foreach (var key in table.Values.Keys) {
                if (string.Equals(key, oldKey, StringComparison.Ordinal) || key.StartsWith($"{oldKey}.", StringComparison.Ordinal)) {
                    replacements.Add((key, newKey + key[oldKey.Length..]));
                }
            }

            if (replacements.Count == 0) {
                continue;
            }

            foreach (var (from, to) in replacements) {
                if (!table.Values.Remove(from, out var text)) {
                    continue;
                }

                table.Values[to] = text;
            }

            _dirtyTables.Add(table);
        }
    }

    private void RewriteEntryValueKeys(string oldKey, string newKey) {
        foreach (var table in _tables.Values) {
            if (table.Values.Remove(oldKey, out var text)) {
                table.Values[newKey] = text;
                _dirtyTables.Add(table);
            }
        }
    }

    private static void ValidateNotDescendant(string key, string newParentGroupKey) {
        if (string.Equals(key, newParentGroupKey, StringComparison.Ordinal) || newParentGroupKey.StartsWith($"{key}.", StringComparison.Ordinal)) {
            throw new ArgumentException("A localization node cannot be moved into itself or one of its descendants.", nameof(newParentGroupKey));
        }
    }

    private JsonStringTableGroup BuildRootGroup() {
        var root = new JsonStringTableGroup(string.Empty, string.Empty);
        PopulateGroup(_keysRoot, root);
        return root;
    }

    private void PopulateGroup(JsonObject keys, JsonStringTableGroup group) {
        foreach (var (localKey, value) in keys) {
            JsonStringTableNode.ValidateSourceLocalKey(localKey, _keysFilePath);
            if (value is null || (value is JsonValue jsonValue && jsonValue.GetValueKind() == JsonValueKind.Null)) {
                group.GetOrAddEntry(localKey);
                continue;
            }

            if (value is JsonObject child) {
                PopulateGroup(child, group.GetOrAddGroup(localKey));
                continue;
            }

            var fullKey = JsonStringTableNode.CombineKey(group.FullKey, localKey);
            throw new InvalidDataException($"The keys value '{fullKey}' in '{_keysFilePath}' must be null or an object.");
        }
    }

    private JsonObject GetGroupObject(string groupKey) {
        var current = _keysRoot;
        foreach (var localKey in groupKey.Split('.', StringSplitOptions.RemoveEmptyEntries)) {
            if (!current.TryGetPropertyValue(localKey, out var child) || child is not JsonObject group) {
                throw new InvalidOperationException($"The keys document does not contain '{groupKey}'.");
            }

            current = group;
        }

        return current;
    }
}
