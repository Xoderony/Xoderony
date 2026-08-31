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
    private JsonStringTableNode _rootGroup = new(string.Empty, string.Empty, JsonStringTableNodeKind.Group);

    public IReadOnlyList<CultureInfo> Cultures => _cultures;

    public JsonStringTableNode RootGroup => _rootGroup;

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

        if (_tables.Count == 0) {
            throw new ArgumentException("At least one JSON string table is required.", nameof(tables));
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

    public void AddGroup(string parentGroupKey, string segment) {
        AddNode(parentGroupKey, segment, JsonStringTableNodeKind.Group);
    }

    public void AddTextEntry(string parentGroupKey, string segment) {
        AddNode(parentGroupKey, segment, JsonStringTableNodeKind.TextEntry);
    }

    public void Rename(string key, string newSegment) {
        var node = GetNode(key);
        ValidateSegment(newSegment, nameof(newSegment));
        if (string.Equals(node.Segment, newSegment, StringComparison.Ordinal)) {
            return;
        }

        var parentKey = GetParentKey(key);
        if (GetGroupNode(parentKey).TryGetChild(newSegment, out _)) {
            throw new ArgumentException($"The localization path '{CombineKey(parentKey, newSegment)}' already exists.", nameof(newSegment));
        }

        foreach (var table in _tables) {
            var parent = GetObject(table.Root, parentKey);
            var value = RemoveValue(parent, node.Segment, table, key);
            parent.Add(newSegment, value);
            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    public void Move(string key, string newParentGroupKey) {
        var node = GetNode(key);
        var oldParentKey = GetParentKey(key);
        if (string.Equals(oldParentKey, newParentGroupKey, StringComparison.Ordinal)) {
            return;
        }

        ValidateNotDescendant(key, newParentGroupKey);
        if (GetGroupNode(newParentGroupKey).TryGetChild(node.Segment, out _)) {
            throw new ArgumentException($"The localization path '{CombineKey(newParentGroupKey, node.Segment)}' already exists.", nameof(newParentGroupKey));
        }

        foreach (var table in _tables) {
            var oldParent = GetObject(table.Root, oldParentKey);
            var newParent = GetObject(table.Root, newParentGroupKey);
            var value = RemoveValue(oldParent, node.Segment, table, key);
            newParent.Add(node.Segment, value);
            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    public void Copy(string key, string newParentGroupKey, string newSegment) {
        var node = GetNode(key);
        ValidateSegment(newSegment, nameof(newSegment));
        ValidateNotDescendant(key, newParentGroupKey);
        if (GetGroupNode(newParentGroupKey).TryGetChild(newSegment, out _)) {
            throw new ArgumentException($"The localization path '{CombineKey(newParentGroupKey, newSegment)}' already exists.", nameof(newSegment));
        }

        var oldParentKey = GetParentKey(key);
        foreach (var table in _tables) {
            var oldParent = GetObject(table.Root, oldParentKey);
            var value = oldParent[node.Segment] ?? throw new InvalidOperationException($"The normalized table '{table.Culture.Name}' does not contain '{key}'.");
            var newParent = GetObject(table.Root, newParentGroupKey);
            newParent.Add(newSegment, value.DeepClone());
            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    public void Remove(string key) {
        var node = GetNode(key);
        var parentKey = GetParentKey(key);
        foreach (var table in _tables) {
            var parent = GetObject(table.Root, parentKey);
            RemoveValue(parent, node.Segment, table, key);
            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    public string GetValue(CultureInfo culture, string key) {
        var node = GetNode(key);
        if (node.Kind != JsonStringTableNodeKind.TextEntry) {
            throw new ArgumentException($"The localization path '{key}' is a group.", nameof(key));
        }

        var table = GetTable(culture);
        var parent = GetObject(table.Root, GetParentKey(key));
        var value = parent[node.Segment] ?? throw new InvalidOperationException($"The normalized table does not contain '{key}'.");
        return value.GetValue<string>();
    }

    public void SetValue(CultureInfo culture, string key, string value) {
        ArgumentNullException.ThrowIfNull(value);

        var node = GetNode(key);
        if (node.Kind != JsonStringTableNodeKind.TextEntry) {
            throw new ArgumentException($"The localization path '{key}' is a group.", nameof(key));
        }

        var table = GetTable(culture);
        var parent = GetObject(table.Root, GetParentKey(key));
        if (string.Equals(parent[node.Segment]?.GetValue<string>(), value, StringComparison.Ordinal)) {
            return;
        }

        parent[node.Segment] = value;
        _dirtyTables.Add(table);
    }

    public int GetEmptyValueCount(CultureInfo culture) {
        return CountEmptyValues(GetTable(culture).Root, _rootGroup);
    }

    public IEnumerable<string> GetKeys() {
        return EnumerateKeys(_rootGroup);
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

    private void AddNode(string parentGroupKey, string segment, JsonStringTableNodeKind kind) {
        ValidateSegment(segment, nameof(segment));
        if (GetGroupNode(parentGroupKey).TryGetChild(segment, out _)) {
            throw new ArgumentException($"The localization path '{CombineKey(parentGroupKey, segment)}' already exists.", nameof(segment));
        }

        foreach (var table in _tables) {
            var parent = GetObject(table.Root, parentGroupKey);
            if (kind == JsonStringTableNodeKind.Group) {
                parent.Add(segment, new JsonObject());
            } else {
                parent.Add(segment, JsonValue.Create(string.Empty));
            }

            _dirtyTables.Add(table);
        }

        RebuildAndComplete();
    }

    private static JsonNode RemoveValue(JsonObject source, string segment, JsonStringTable table, string fullKey) {
        var value = source[segment] ?? throw new InvalidOperationException($"The normalized table '{table.Culture.Name}' does not contain '{fullKey}'.");
        source.Remove(segment);
        return value;
    }

    private static void ValidateNotDescendant(string key, string newParentGroupKey) {
        if (string.Equals(key, newParentGroupKey, StringComparison.Ordinal) || newParentGroupKey.StartsWith($"{key}.", StringComparison.Ordinal)) {
            throw new ArgumentException("A localization node cannot be moved into itself or one of its descendants.", nameof(newParentGroupKey));
        }
    }

    private void RebuildAndComplete() {
        var root = new JsonStringTableNode(string.Empty, string.Empty, JsonStringTableNodeKind.Group);
        foreach (var table in _tables) {
            MergeObject(table, table.Root, root);
        }

        foreach (var table in _tables) {
            CompleteObject(table, table.Root, root);
        }

        _rootGroup = root;
    }

    private static void MergeObject(JsonStringTable table, JsonObject source, JsonStringTableNode target) {
        foreach (var (segment, value) in source) {
            ValidateSourceSegment(segment, table.FilePath);
            if (value is JsonObject child) {
                var childNode = target.GetOrAddChild(segment, JsonStringTableNodeKind.Group);
                MergeObject(table, child, childNode);
                continue;
            }

            if (value is JsonValue jsonValue && jsonValue.TryGetValue<string>(out _)) {
                target.GetOrAddChild(segment, JsonStringTableNodeKind.TextEntry);
                continue;
            }

            var fullKey = CombineKey(target.FullKey, segment);
            throw new InvalidDataException($"The localization value '{fullKey}' in '{table.FilePath}' must be a string or object.");
        }
    }

    private void CompleteObject(JsonStringTable table, JsonObject target, JsonStringTableNode schema) {
        foreach (var child in schema.Children.Values) {
            var value = target[child.Segment];
            if (value is null) {
                value = child.Kind == JsonStringTableNodeKind.Group ? new JsonObject() : JsonValue.Create(string.Empty);
                target.Add(child.Segment, value);
                _dirtyTables.Add(table);
            }

            if (child.Kind == JsonStringTableNodeKind.Group) {
                CompleteObject(table, (JsonObject)value, child);
            }
        }
    }

    private static int CountEmptyValues(JsonObject source, JsonStringTableNode schema) {
        var count = 0;
        foreach (var child in schema.Children.Values) {
            var value = source[child.Segment] ?? throw new InvalidOperationException($"The normalized table does not contain '{child.FullKey}'.");
            if (child.Kind == JsonStringTableNodeKind.TextEntry) {
                if (value.GetValue<string>().Length == 0) {
                    count++;
                }
            } else {
                count += CountEmptyValues((JsonObject)value, child);
            }
        }

        return count;
    }

    private static IEnumerable<string> EnumerateKeys(JsonStringTableNode node) {
        foreach (var child in node.Children.Values) {
            if (child.Kind == JsonStringTableNodeKind.TextEntry) {
                yield return child.FullKey;
                continue;
            }

            foreach (var key in EnumerateKeys(child)) {
                yield return key;
            }
        }
    }

    private JsonStringTableNode GetNode(string key) {
        ArgumentException.ThrowIfNullOrEmpty(key);

        var current = _rootGroup;
        foreach (var segment in key.Split('.')) {
            if (!current.TryGetChild(segment, out current)) {
                throw new KeyNotFoundException($"The localization path '{key}' does not exist.");
            }
        }

        return current;
    }

    private JsonStringTableNode GetGroupNode(string key) {
        var node = key.Length == 0 ? _rootGroup : GetNode(key);
        if (node.Kind != JsonStringTableNodeKind.Group) {
            throw new ArgumentException($"The localization path '{key}' is not a group.", nameof(key));
        }

        return node;
    }

    private static JsonObject GetObject(JsonObject root, string key) {
        var current = root;
        foreach (var segment in key.Split('.', StringSplitOptions.RemoveEmptyEntries)) {
            current = (JsonObject)(current[segment] ?? throw new InvalidOperationException($"The normalized table does not contain '{key}'."));
        }

        return current;
    }

    private static string GetParentKey(string key) {
        var index = key.LastIndexOf('.');
        return index < 0 ? string.Empty : key[..index];
    }

    private static string CombineKey(string parent, string segment) {
        return parent.Length == 0 ? segment : $"{parent}.{segment}";
    }

    private static void ValidateSegment(string segment, string parameterName) {
        if (!IsValidSegment(segment)) {
            throw new ArgumentException($"'{segment}' is not a valid lower_snake_case localization key segment.", parameterName);
        }
    }

    private static void ValidateSourceSegment(string segment, string filePath) {
        if (!IsValidSegment(segment)) {
            throw new InvalidDataException($"The key segment '{segment}' in '{filePath}' is not valid lower_snake_case.");
        }
    }

    private static bool IsValidSegment(string? segment) {
        if (string.IsNullOrEmpty(segment) || segment[0] is < 'a' or > 'z') {
            return false;
        }

        var previousUnderscore = false;
        for (var index = 1; index < segment.Length; index++) {
            var character = segment[index];
            if (character == '_') {
                if (previousUnderscore) {
                    return false;
                }

                previousUnderscore = true;
                continue;
            }

            if (character is not (>= 'a' and <= 'z') and not (>= '0' and <= '9')) {
                return false;
            }

            previousUnderscore = false;
        }

        return !previousUnderscore;
    }
}
