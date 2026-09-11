using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

public abstract class JsonKeyNode {

    private string _localKey;
    private string _fullKey;

    public string LocalKey => _localKey;

    public string FullKey => _fullKey;

    private protected JsonKeyNode(string localKey, string fullKey) {
        _localKey = localKey;
        _fullKey = fullKey;
    }

    public static string CombineKey(string parentKey, string localKey) {
        return parentKey.Length == 0 ? localKey : $"{parentKey}.{localKey}";
    }

    public static string GetParentKey(string key) {
        var index = key.LastIndexOf('.');
        return index < 0 ? string.Empty : key[..index];
    }

    /// <summary>
    /// 是否为合法的 lower_snake_case 局部键：非空；首字符 a-z；其余为 a-z、0-9 或单个 _；禁止连续 _ 与结尾 _。
    /// </summary>
    public static bool IsLowerSnakeCaseLocalKey(string? localKey) {
        if (string.IsNullOrEmpty(localKey) || localKey[0] is < 'a' or > 'z') {
            return false;
        }

        var previousUnderscore = false;
        for (var index = 1; index < localKey.Length; index++) {
            var character = localKey[index];
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

    internal abstract JsonKeyNode Clone();

    internal virtual void SetKeys(string localKey, string parentFullKey) {
        Debug.Assert(localKey is not null);
        Debug.Assert(parentFullKey is not null);
        _localKey = localKey;
        _fullKey = CombineKey(parentFullKey, localKey);
    }
}

public sealed class JsonKeyGroup : JsonKeyNode {

    private readonly SortedDictionary<string, JsonKeyNode> _localKeyToChild = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, JsonKeyNode> LocalKeyToChild => _localKeyToChild;

    internal JsonKeyGroup(string localKey, string fullKey) : base(localKey, fullKey) {
    }

    public bool TryGet(string relativeKey, [NotNullWhen(true)] out JsonKeyNode? node) {
        Debug.Assert(!string.IsNullOrWhiteSpace(relativeKey));
        var current = this;
        var localKeys = relativeKey.Split('.');
        for (var index = 0; index < localKeys.Length; index++) {
            if (!current._localKeyToChild.TryGetValue(localKeys[index], out var child)) {
                node = null;
                return false;
            }

            if (index == localKeys.Length - 1) {
                node = child;
                return true;
            }

            if (child is not JsonKeyGroup group) {
                node = null;
                return false;
            }

            current = group;
        }

        node = null;
        return false;
    }

    public JsonKeyNode Get(string relativeKey) {
        if (!TryGet(relativeKey, out var node)) {
            throw new KeyNotFoundException($"The localization path '{relativeKey}' does not exist.");
        }

        return node;
    }

    public JsonKeyGroup GetGroup(string relativeKey) {
        if (relativeKey.Length == 0) {
            return this;
        }

        if (Get(relativeKey) is not JsonKeyGroup group) {
            throw new ArgumentException($"The localization path '{relativeKey}' is not a group.", nameof(relativeKey));
        }

        return group;
    }

    public IEnumerable<string> GetDescendantEntryKeys() {
        foreach (var child in _localKeyToChild.Values) {
            switch (child) {
                case JsonKeyEntry entry:
                    yield return entry.FullKey;
                    break;
                case JsonKeyGroup group:
                    foreach (var key in group.GetDescendantEntryKeys()) {
                        yield return key;
                    }
                    break;
            }
        }
    }

    public string AllocateLocalKey(string localKey) {
        Debug.Assert(localKey is not null);
        if (!_localKeyToChild.ContainsKey(localKey)) {
            return localKey;
        }

        for (var suffix = 1; ; suffix++) {
            var candidate = $"{localKey}_{suffix}";
            if (!_localKeyToChild.ContainsKey(candidate)) {
                return candidate;
            }
        }
    }

    internal override JsonKeyNode Clone() {
        var group = new JsonKeyGroup(LocalKey, FullKey);
        foreach (var child in _localKeyToChild.Values) {
            group.AddChild(child.LocalKey, child.Clone());
        }

        return group;
    }

    internal JsonKeyGroup AddGroup(string localKey) {
        var group = new JsonKeyGroup(localKey, CombineKey(FullKey, localKey));
        _localKeyToChild.Add(localKey, group);
        return group;
    }

    internal void AddEntry(string localKey) {
        var entry = new JsonKeyEntry(localKey, CombineKey(FullKey, localKey));
        _localKeyToChild.Add(localKey, entry);
    }

    internal void AddChild(string localKey, JsonKeyNode child) {
        Debug.Assert(child is not null);
        child.SetKeys(localKey, FullKey);
        _localKeyToChild.Add(localKey, child);
    }

    internal bool RemoveChild(string localKey, [NotNullWhen(true)] out JsonKeyNode? child) {
        return _localKeyToChild.Remove(localKey, out child);
    }

    internal static JsonKeyGroup Parse(JsonObject keys, string keysFilePath) {
        var root = new JsonKeyGroup(string.Empty, string.Empty);
        Populate(root, keys, keysFilePath);
        return root;

        static void Populate(JsonKeyGroup group, JsonObject keysObject, string filePath) {
            foreach (var (localKey, value) in keysObject) {
                if (!IsLowerSnakeCaseLocalKey(localKey)) {
                    throw new InvalidDataException($"The local key '{localKey}' in '{filePath}' is not valid lower_snake_case.");
                }

                if (value is null || (value is JsonValue jsonValue && jsonValue.GetValueKind() == JsonValueKind.Null)) {
                    group.AddEntry(localKey);
                    continue;
                }

                if (value is JsonObject child) {
                    Populate(group.AddGroup(localKey), child, filePath);
                    continue;
                }

                var fullKey = CombineKey(group.FullKey, localKey);
                throw new InvalidDataException($"The keys value '{fullKey}' in '{filePath}' must be null or an object.");
            }
        }
    }

    internal JsonObject ToJsonObject() {
        var root = new JsonObject();
        foreach (var (localKey, child) in _localKeyToChild) {
            if (child is JsonKeyGroup group) {
                root.Add(localKey, group.ToJsonObject());
                continue;
            }

            root.Add(localKey, null);
        }

        return root;
    }

    internal override void SetKeys(string localKey, string parentFullKey) {
        base.SetKeys(localKey, parentFullKey);
        foreach (var child in _localKeyToChild.Values) {
            child.SetKeys(child.LocalKey, FullKey);
        }
    }
}

public sealed class JsonKeyEntry : JsonKeyNode {

    internal JsonKeyEntry(string localKey, string fullKey) : base(localKey, fullKey) {
    }

    internal override JsonKeyNode Clone() {
        return new JsonKeyEntry(LocalKey, FullKey);
    }
}
