using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

public sealed class JsonLocaleTableCollection {

    public const string KeysFileName = "keys.json";

    private readonly string _keysFilePath;
    private readonly SortedDictionary<string, JsonLocaleTable> _cultureNameToTable;
    private readonly HashSet<JsonLocaleTable> _dirtyTables = [];
    private bool _keysDirty;
    private JsonKeyGroup _rootKeyGroup;

    public int CultureCount => _cultureNameToTable.Count;

    public JsonKeyGroup RootKeyGroup => _rootKeyGroup;

    public bool IsDirty => _keysDirty || _dirtyTables.Count != 0;

    /// <summary>接管 rootKeyGroup 与 cultureNameToTable 的所有权，不重新解析或复制。</summary>
    private JsonLocaleTableCollection(string keysFilePath, JsonKeyGroup rootKeyGroup, SortedDictionary<string, JsonLocaleTable> cultureNameToTable) {
        Debug.Assert(!string.IsNullOrWhiteSpace(keysFilePath));
        Debug.Assert(rootKeyGroup is not null);
        Debug.Assert(cultureNameToTable is not null);

        _keysFilePath = keysFilePath;
        _rootKeyGroup = rootKeyGroup;
        _cultureNameToTable = cultureNameToTable;
    }

    public static JsonLocaleTableCollection LoadDirectory(string directoryPath) {
        Debug.Assert(!string.IsNullOrWhiteSpace(directoryPath));

        var keysFilePath = Path.Combine(directoryPath, KeysFileName);
        var rootKeyGroup = LoadRootKeyGroup(keysFilePath);
        var cultureNameToTable = LoadTables(directoryPath);
        return new JsonLocaleTableCollection(keysFilePath, rootKeyGroup, cultureNameToTable);

        static JsonKeyGroup LoadRootKeyGroup(string path) {
            if (!File.Exists(path)) {
                return new JsonKeyGroup(string.Empty, string.Empty);
            }

            return JsonKeyGroup.Parse(JsonObjectFile.Read(path), path);
        }

        static SortedDictionary<string, JsonLocaleTable> LoadTables(string path) {
            var cultureNameToTable = new SortedDictionary<string, JsonLocaleTable>(StringComparer.OrdinalIgnoreCase);
            foreach (var filePath in Directory.EnumerateFiles(path, "*.json", SearchOption.TopDirectoryOnly)) {
                if (string.Equals(Path.GetFileName(filePath), KeysFileName, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                var cultureName = Path.GetFileNameWithoutExtension(filePath);
                var culture = CultureInfo.GetCultureInfo(cultureName);
                var table = JsonLocaleTable.Load(culture, filePath);
                if (!cultureNameToTable.TryAdd(table.Culture.Name, table)) {
                    throw new InvalidDataException($"The culture '{table.Culture.Name}' has more than one locale table in '{path}'.");
                }
            }

            return cultureNameToTable;
        }
    }

    public void Save() {
        if (_keysDirty) {
            JsonObjectFile.Write(_keysFilePath, _rootKeyGroup.ToJsonObject());
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
            throw new ArgumentException("The locale table culture cannot be invariant.", nameof(culture));
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));

        culture = CultureInfo.GetCultureInfo(culture.Name);
        var table = new JsonLocaleTable(culture, filePath, new SortedDictionary<string, string>(StringComparer.Ordinal));
        if (!_cultureNameToTable.TryAdd(table.Culture.Name, table)) {
            throw new ArgumentException($"The culture '{table.Culture.Name}' already has a locale table.", nameof(culture));
        }

        foreach (var key in _rootKeyGroup.GetDescendantEntryKeys()) {
            table.SetTranslation(key, string.Empty);
        }

        _dirtyTables.Add(table);
    }

    public void AddGroup(string parentGroupKey, string localKey) {
        if (!JsonKeyNode.IsLowerSnakeCaseLocalKey(localKey)) {
            throw new ArgumentException($"'{localKey}' is not a valid lower_snake_case local key.", nameof(localKey));
        }

        var parent = _rootKeyGroup.GetGroup(parentGroupKey);
        if (parent.LocalKeyToChild.ContainsKey(localKey)) {
            throw new ArgumentException($"The localization path '{JsonKeyNode.CombineKey(parentGroupKey, localKey)}' already exists.", nameof(localKey));
        }

        parent.AddGroup(localKey);
        _keysDirty = true;
    }

    public void AddEntry(string parentGroupKey, string localKey) {
        if (!JsonKeyNode.IsLowerSnakeCaseLocalKey(localKey)) {
            throw new ArgumentException($"'{localKey}' is not a valid lower_snake_case local key.", nameof(localKey));
        }

        var parent = _rootKeyGroup.GetGroup(parentGroupKey);
        if (parent.LocalKeyToChild.ContainsKey(localKey)) {
            throw new ArgumentException($"The localization path '{JsonKeyNode.CombineKey(parentGroupKey, localKey)}' already exists.", nameof(localKey));
        }

        parent.AddEntry(localKey);
        var key = JsonKeyNode.CombineKey(parentGroupKey, localKey);
        foreach (var table in _cultureNameToTable.Values) {
            if (table.SetTranslation(key, string.Empty)) {
                _dirtyTables.Add(table);
            }
        }

        _keysDirty = true;
    }

    public void Rename(string fullKey, string localKey) {
        var node = _rootKeyGroup.Get(fullKey);
        if (!JsonKeyNode.IsLowerSnakeCaseLocalKey(localKey)) {
            throw new ArgumentException($"'{localKey}' is not a valid lower_snake_case local key.", nameof(localKey));
        }

        if (string.Equals(node.LocalKey, localKey, StringComparison.Ordinal)) {
            return;
        }

        var parentKey = JsonKeyNode.GetParentKey(fullKey);
        var parent = _rootKeyGroup.GetGroup(parentKey);
        if (parent.LocalKeyToChild.ContainsKey(localKey)) {
            throw new ArgumentException($"The localization path '{JsonKeyNode.CombineKey(parentKey, localKey)}' already exists.", nameof(localKey));
        }

        if (!parent.RemoveChild(node.LocalKey, out var removed)) {
            throw new InvalidOperationException($"The keys tree does not contain '{fullKey}'.");
        }

        parent.AddChild(localKey, removed);

        if (removed is JsonKeyGroup) {
            RemapGroupTranslationKeys(fullKey, removed.FullKey);
        } else {
            RemapEntryTranslationKey(fullKey, removed.FullKey);
        }

        _keysDirty = true;
    }

    public string Move(string fullKey, string parentGroupKey) {
        var node = _rootKeyGroup.Get(fullKey);
        var oldParentKey = JsonKeyNode.GetParentKey(fullKey);
        if (string.Equals(oldParentKey, parentGroupKey, StringComparison.Ordinal)) {
            return fullKey;
        }

        if (string.Equals(fullKey, parentGroupKey, StringComparison.Ordinal) || parentGroupKey.StartsWith($"{fullKey}.", StringComparison.Ordinal)) {
            throw new ArgumentException("A localization node cannot be moved into itself or one of its descendants.", nameof(parentGroupKey));
        }

        var parent = _rootKeyGroup.GetGroup(parentGroupKey);
        var localKey = parent.AllocateLocalKey(node.LocalKey);
        var oldParent = _rootKeyGroup.GetGroup(oldParentKey);
        if (!oldParent.RemoveChild(node.LocalKey, out var removed)) {
            throw new InvalidOperationException($"The keys tree does not contain '{fullKey}'.");
        }

        parent.AddChild(localKey, removed);

        if (removed is JsonKeyGroup) {
            RemapGroupTranslationKeys(fullKey, removed.FullKey);
        } else {
            RemapEntryTranslationKey(fullKey, removed.FullKey);
        }

        _keysDirty = true;
        return removed.FullKey;
    }

    public string Copy(string fullKey, string parentGroupKey) {
        var node = _rootKeyGroup.Get(fullKey);
        if (string.Equals(fullKey, parentGroupKey, StringComparison.Ordinal) || parentGroupKey.StartsWith($"{fullKey}.", StringComparison.Ordinal)) {
            throw new ArgumentException("A localization node cannot be copied into itself or one of its descendants.", nameof(parentGroupKey));
        }

        var parent = _rootKeyGroup.GetGroup(parentGroupKey);
        var localKey = parent.AllocateLocalKey(node.LocalKey);
        var clone = node.Clone();
        parent.AddChild(localKey, clone);

        if (node is JsonKeyGroup group) {
            CopyGroupTranslationKeys(group, clone.FullKey);
        } else {
            CopyEntryTranslationKey(fullKey, clone.FullKey);
        }

        _keysDirty = true;
        return clone.FullKey;

        void CopyGroupTranslationKeys(JsonKeyGroup fromGroup, string toKey) {
            foreach (var table in _cultureNameToTable.Values) {
                var changed = false;
                foreach (var fromEntryKey in fromGroup.GetDescendantEntryKeys()) {
                    var toEntryKey = toKey + fromEntryKey[fromGroup.FullKey.Length..];
                    var translation = table.TryGetTranslation(fromEntryKey, out var sourceTranslation) ? sourceTranslation : string.Empty;
                    changed |= table.SetTranslation(toEntryKey, translation);
                }

                if (changed) {
                    _dirtyTables.Add(table);
                }
            }
        }

        void CopyEntryTranslationKey(string fromKey, string toKey) {
            foreach (var table in _cultureNameToTable.Values) {
                var text = table.TryGetTranslation(fromKey, out var sourceText) ? sourceText : string.Empty;
                if (table.SetTranslation(toKey, text)) {
                    _dirtyTables.Add(table);
                }
            }
        }
    }

    public void Remove(string fullKey) {
        var node = _rootKeyGroup.Get(fullKey);
        var parentKey = JsonKeyNode.GetParentKey(fullKey);
        var parent = _rootKeyGroup.GetGroup(parentKey);
        if (!parent.RemoveChild(node.LocalKey, out _)) {
            throw new InvalidOperationException($"The keys tree does not contain '{fullKey}'.");
        }

        if (node is JsonKeyGroup) {
            RemoveGroupTranslationKeys(fullKey);
        } else {
            RemoveEntryTranslationKey(fullKey);
        }

        _keysDirty = true;

        void RemoveGroupTranslationKeys(string groupKey) {
            foreach (var table in _cultureNameToTable.Values) {
                var removals = new List<string>();
                foreach (var valueKey in table.TranslationKeys) {
                    if (string.Equals(valueKey, groupKey, StringComparison.Ordinal) || valueKey.StartsWith($"{groupKey}.", StringComparison.Ordinal)) {
                        removals.Add(valueKey);
                    }
                }

                if (removals.Count == 0) {
                    continue;
                }

                foreach (var valueKey in removals) {
                    table.RemoveTranslation(valueKey, out _);
                }

                _dirtyTables.Add(table);
            }
        }

        void RemoveEntryTranslationKey(string entryKey) {
            foreach (var table in _cultureNameToTable.Values) {
                if (table.RemoveTranslation(entryKey, out _)) {
                    _dirtyTables.Add(table);
                }
            }
        }
    }

    public string GetTranslation(CultureInfo culture, string entryKey) {
        if (_rootKeyGroup.Get(entryKey) is not JsonKeyEntry) {
            throw new ArgumentException($"The localization path '{entryKey}' is a group.", nameof(entryKey));
        }

        Debug.Assert(culture is not null);
        if (!_cultureNameToTable.TryGetValue(culture.Name, out var table)) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of this locale table collection.", nameof(culture));
        }

        return table.TryGetTranslation(entryKey, out var translation) ? translation : string.Empty;
    }

    public void SetTranslation(CultureInfo culture, string entryKey, string translation) {
        Debug.Assert(translation is not null);

        if (_rootKeyGroup.Get(entryKey) is not JsonKeyEntry) {
            throw new ArgumentException($"The localization path '{entryKey}' is a group.", nameof(entryKey));
        }

        Debug.Assert(culture is not null);
        if (!_cultureNameToTable.TryGetValue(culture.Name, out var table)) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of this locale table collection.", nameof(culture));
        }

        if (table.SetTranslation(entryKey, translation)) {
            _dirtyTables.Add(table);
        }
    }

    public IEnumerable<string> GetEntryKeys() {
        return _rootKeyGroup.GetDescendantEntryKeys();
    }

    public IEnumerable<string> GetTranslationKeys(CultureInfo culture) {
        Debug.Assert(culture is not null);
        if (!_cultureNameToTable.TryGetValue(culture.Name, out var table)) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of this locale table collection.", nameof(culture));
        }

        return table.TranslationKeys;
    }

    public IEnumerable<CultureInfo> GetCultures() {
        foreach (var table in _cultureNameToTable.Values) {
            yield return table.Culture;
        }
    }

    private void RemapGroupTranslationKeys(string fromKey, string toKey) {
        foreach (var table in _cultureNameToTable.Values) {
            var replacements = new List<(string FromKey, string ToKey)>();
            foreach (var key in table.TranslationKeys) {
                if (string.Equals(key, fromKey, StringComparison.Ordinal) || key.StartsWith($"{fromKey}.", StringComparison.Ordinal)) {
                    replacements.Add((key, toKey + key[fromKey.Length..]));
                }
            }

            if (replacements.Count == 0) {
                continue;
            }

            foreach (var (from, to) in replacements) {
                if (!table.RemoveTranslation(from, out var translation)) {
                    continue;
                }

                table.SetTranslation(to, translation);
            }

            _dirtyTables.Add(table);
        }
    }

    private void RemapEntryTranslationKey(string fromKey, string toKey) {
        foreach (var table in _cultureNameToTable.Values) {
            if (table.RemoveTranslation(fromKey, out var translation)) {
                table.SetTranslation(toKey, translation);
                _dirtyTables.Add(table);
            }
        }
    }
}
