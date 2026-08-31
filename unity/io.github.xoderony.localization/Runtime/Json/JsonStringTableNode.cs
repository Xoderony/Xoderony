using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Xoderony.Localization.Json;

public abstract class JsonStringTableNode {

    private readonly string _fullKey;
    private readonly string _localKey;

    private protected JsonStringTableNode(string localKey, string fullKey) {
        _localKey = localKey;
        _fullKey = fullKey;
    }

    public string LocalKey => _localKey;

    public string FullKey => _fullKey;

    public static string CombineKey(string parentKey, string localKey) {
        return parentKey.Length == 0 ? localKey : $"{parentKey}.{localKey}";
    }

    public static string GetParentKey(string key) {
        var index = key.LastIndexOf('.');
        return index < 0 ? string.Empty : key[..index];
    }

    public static bool IsValidLocalKey(string? localKey) {
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

    internal static void ValidateLocalKey(string localKey, string parameterName) {
        if (!IsValidLocalKey(localKey)) {
            throw new ArgumentException($"'{localKey}' is not a valid lower_snake_case local key.", parameterName);
        }
    }

    internal static void ValidateSourceLocalKey(string localKey, string filePath) {
        if (!IsValidLocalKey(localKey)) {
            throw new InvalidDataException($"The local key '{localKey}' in '{filePath}' is not valid lower_snake_case.");
        }
    }
}

public sealed class JsonStringTableGroup : JsonStringTableNode {

    private readonly SortedDictionary<string, JsonStringTableNode> _childByLocalKey = new(StringComparer.Ordinal);

    internal JsonStringTableGroup(string localKey, string fullKey) : base(localKey, fullKey) {
    }

    public IReadOnlyDictionary<string, JsonStringTableNode> Children => _childByLocalKey;

    public bool TryGet(string relativeKey, [NotNullWhen(true)] out JsonStringTableNode? node) {
        ArgumentException.ThrowIfNullOrEmpty(relativeKey);

        var current = this;
        var localKeys = relativeKey.Split('.');
        for (var index = 0; index < localKeys.Length; index++) {
            if (!current._childByLocalKey.TryGetValue(localKeys[index], out var child)) {
                node = null;
                return false;
            }

            if (index == localKeys.Length - 1) {
                node = child;
                return true;
            }

            if (child is not JsonStringTableGroup group) {
                node = null;
                return false;
            }

            current = group;
        }

        node = null;
        return false;
    }

    public JsonStringTableNode Get(string relativeKey) {
        if (!TryGet(relativeKey, out var node)) {
            throw new KeyNotFoundException($"The localization path '{relativeKey}' does not exist.");
        }

        return node;
    }

    public JsonStringTableGroup GetGroup(string relativeKey) {
        if (relativeKey.Length == 0) {
            return this;
        }

        if (Get(relativeKey) is not JsonStringTableGroup group) {
            throw new ArgumentException($"The localization path '{relativeKey}' is not a group.", nameof(relativeKey));
        }

        return group;
    }

    public IEnumerable<string> EnumerateEntryKeys() {
        foreach (var child in _childByLocalKey.Values) {
            switch (child) {
                case JsonStringTableEntry entry:
                    yield return entry.FullKey;
                    break;
                case JsonStringTableGroup group:
                    foreach (var key in group.EnumerateEntryKeys()) {
                        yield return key;
                    }

                    break;
            }
        }
    }

    internal JsonStringTableGroup GetOrAddGroup(string localKey) {
        if (_childByLocalKey.TryGetValue(localKey, out var child)) {
            if (child is JsonStringTableGroup group) {
                return group;
            }

            throw new InvalidDataException($"The localization path '{CombineKey(FullKey, localKey)}' is used as both a group and an entry.");
        }

        var created = new JsonStringTableGroup(localKey, CombineKey(FullKey, localKey));
        _childByLocalKey.Add(localKey, created);
        return created;
    }

    internal JsonStringTableEntry GetOrAddEntry(string localKey) {
        if (_childByLocalKey.TryGetValue(localKey, out var child)) {
            if (child is JsonStringTableEntry entry) {
                return entry;
            }

            throw new InvalidDataException($"The localization path '{CombineKey(FullKey, localKey)}' is used as both a group and an entry.");
        }

        var created = new JsonStringTableEntry(localKey, CombineKey(FullKey, localKey));
        _childByLocalKey.Add(localKey, created);
        return created;
    }
}

public sealed class JsonStringTableEntry : JsonStringTableNode {

    internal JsonStringTableEntry(string localKey, string fullKey) : base(localKey, fullKey) {
    }
}
