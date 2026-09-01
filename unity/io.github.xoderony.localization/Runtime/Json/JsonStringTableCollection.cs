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

    private readonly string _keysFilePath;
    private readonly HashSet<JsonStringTable> _dirtyTables = [];
    private readonly SortedDictionary<string, JsonStringTable> _cultureNameToTable;
    private bool _keysDirty;
    private JsonObject _keysRoot;
    private JsonStringTableGroup _rootGroup;

    public IReadOnlyList<CultureInfo> Cultures {
        get {
            if (_cultureNameToTable.Count == 0) {
                return [];
            }

            var cultures = new CultureInfo[_cultureNameToTable.Count];
            var index = 0;
            foreach (var table in _cultureNameToTable.Values) {
                cultures[index++] = table.Culture;
            }

            return cultures;
        }
    }

    public JsonStringTableGroup RootGroup => _rootGroup;

    public bool IsDirty => _keysDirty || _dirtyTables.Count != 0;

    /// <summary>接管 keysRoot 与 cultureNameToTable 的所有权，不重新解析或复制。</summary>
    private JsonStringTableCollection(string keysFilePath, JsonObject keysRoot, SortedDictionary<string, JsonStringTable> cultureNameToTable) {
        Debug.Assert(!string.IsNullOrWhiteSpace(keysFilePath));
        Debug.Assert(keysRoot is not null);
        Debug.Assert(cultureNameToTable is not null);

        _keysFilePath = keysFilePath;
        _keysRoot = keysRoot;
        _cultureNameToTable = cultureNameToTable;
        _rootGroup = BuildRootGroup();
    }

    public static JsonStringTableCollection LoadDirectory(string directoryPath) {
        Debug.Assert(!string.IsNullOrWhiteSpace(directoryPath));

        var keysFilePath = Path.Combine(directoryPath, KeysFileName);
        var keysRoot = LoadKeysRoot(keysFilePath);
        var cultureNameToTable = LoadTables(directoryPath);
        return new JsonStringTableCollection(keysFilePath, keysRoot, cultureNameToTable);

        static JsonObject LoadKeysRoot(string path) {
            if (!File.Exists(path)) {
                return [];
            }

            return JsonObjectFile.Read(path);
        }

        static SortedDictionary<string, JsonStringTable> LoadTables(string path) {
            var cultureNameToTable = new SortedDictionary<string, JsonStringTable>(StringComparer.OrdinalIgnoreCase);
            foreach (var filePath in Directory.EnumerateFiles(path, "*.json", SearchOption.TopDirectoryOnly)) {
                if (string.Equals(Path.GetFileName(filePath), KeysFileName, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                var cultureName = Path.GetFileNameWithoutExtension(filePath);
                var culture = CultureInfo.GetCultureInfo(cultureName);
                var table = JsonStringTable.Load(culture, filePath);
                if (!cultureNameToTable.TryAdd(table.Culture.Name, table)) {
                    throw new InvalidDataException($"The culture '{table.Culture.Name}' has more than one string table in '{path}'.");
                }
            }

            return cultureNameToTable;
        }
    }

    public void Save() {
        if (_keysDirty) {
            JsonObjectFile.Write(_keysFilePath, _keysRoot);
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
        if (!_cultureNameToTable.TryAdd(table.Culture.Name, table)) {
            throw new ArgumentException($"The culture '{table.Culture.Name}' already has a string table.", nameof(culture));
        }

        foreach (var key in _rootGroup.GetDescendantEntryKeys()) {
            table.SetValue(key, string.Empty);
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
        foreach (var table in _cultureNameToTable.Values) {
            if (table.SetValue(key, string.Empty)) {
                _dirtyTables.Add(table);
            }
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
        if (node is JsonStringTableGroup group) {
            CopyGroupValueKeys(group, newKey);
        } else {
            CopyEntryValueKeys(key, newKey);
        }

        _keysDirty = true;
        _rootGroup = BuildRootGroup();

        void CopyGroupValueKeys(JsonStringTableGroup sourceGroup, string targetGroupKey) {
            foreach (var table in _cultureNameToTable.Values) {
                var changed = false;
                foreach (var sourceKey in sourceGroup.GetDescendantEntryKeys()) {
                    var targetKey = targetGroupKey + sourceKey[sourceGroup.FullKey.Length..];
                    var translation = table.TryGetValue(sourceKey, out var sourceTranslation) ? sourceTranslation : string.Empty;
                    changed |= table.SetValue(targetKey, translation);
                }

                if (changed) {
                    _dirtyTables.Add(table);
                }
            }
        }

        void CopyEntryValueKeys(string sourceKey, string targetKey) {
            foreach (var table in _cultureNameToTable.Values) {
                var text = table.TryGetValue(sourceKey, out var sourceText) ? sourceText : string.Empty;
                if (table.SetValue(targetKey, text)) {
                    _dirtyTables.Add(table);
                }
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
            foreach (var table in _cultureNameToTable.Values) {
                var removals = new List<string>();
                foreach (var valueKey in table.Keys) {
                    if (string.Equals(valueKey, groupKey, StringComparison.Ordinal) || valueKey.StartsWith($"{groupKey}.", StringComparison.Ordinal)) {
                        removals.Add(valueKey);
                    }
                }

                if (removals.Count == 0) {
                    continue;
                }

                foreach (var valueKey in removals) {
                    table.RemoveValue(valueKey, out _);
                }

                _dirtyTables.Add(table);
            }
        }

        void RemoveEntryValueKeys(string entryKey) {
            foreach (var table in _cultureNameToTable.Values) {
                if (table.RemoveValue(entryKey, out _)) {
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
        return table.TryGetValue(key, out var value) ? value : string.Empty;
    }

    public void SetValue(CultureInfo culture, string key, string value) {
        Debug.Assert(value is not null);

        if (_rootGroup.Get(key) is not JsonStringTableEntry) {
            throw new ArgumentException($"The localization path '{key}' is a group.", nameof(key));
        }

        var table = GetTable(culture);
        if (table.SetValue(key, value)) {
            _dirtyTables.Add(table);
        }
    }

    public int GetEmptyValueCount(CultureInfo culture) {
        var table = GetTable(culture);
        var count = 0;
        foreach (var key in _rootGroup.GetDescendantEntryKeys()) {
            if (!table.TryGetValue(key, out var value) || value.Length == 0) {
                count++;
            }
        }

        return count;
    }

    public IEnumerable<string> GetKeys() {
        return _rootGroup.GetDescendantEntryKeys();
    }

    private JsonStringTable GetTable(CultureInfo culture) {
        Debug.Assert(culture is not null);
        if (!_cultureNameToTable.TryGetValue(culture.Name, out var table)) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of this string table collection.", nameof(culture));
        }

        return table;
    }

    private void RewriteGroupValueKeys(string oldKey, string newKey) {
        foreach (var table in _cultureNameToTable.Values) {
            var replacements = new List<(string OldKey, string NewKey)>();
            foreach (var key in table.Keys) {
                if (string.Equals(key, oldKey, StringComparison.Ordinal) || key.StartsWith($"{oldKey}.", StringComparison.Ordinal)) {
                    replacements.Add((key, newKey + key[oldKey.Length..]));
                }
            }

            if (replacements.Count == 0) {
                continue;
            }

            foreach (var (from, to) in replacements) {
                if (!table.RemoveValue(from, out var translation)) {
                    continue;
                }

                table.SetValue(to, translation);
            }

            _dirtyTables.Add(table);
        }
    }

    private void RewriteEntryValueKeys(string oldKey, string newKey) {
        foreach (var table in _cultureNameToTable.Values) {
            if (table.RemoveValue(oldKey, out var translation)) {
                table.SetValue(newKey, translation);
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
                group.AddEntry(localKey);
                continue;
            }

            if (value is JsonObject child) {
                var childGroup = group.AddGroup(localKey);
                PopulateGroup(child, childGroup);
                continue;
            }

            var fullKey = JsonStringTableNode.CombineKey(group.FullKey, localKey);
            throw new InvalidDataException($"The keys value '{fullKey}' in '{_keysFilePath}' must be null or an object.");
        }
    }

    private JsonObject GetGroupObject(string groupKey) {
        if (groupKey.Length == 0) {
            return _keysRoot;
        }

        var current = _keysRoot;
        foreach (var localKey in groupKey.Split('.')) {
            if (!current.TryGetPropertyValue(localKey, out var child) || child is not JsonObject group) {
                throw new InvalidOperationException($"The keys document does not contain '{groupKey}'.");
            }

            current = group;
        }

        return current;
    }
}
