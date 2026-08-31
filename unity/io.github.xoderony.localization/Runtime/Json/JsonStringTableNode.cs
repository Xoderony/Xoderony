using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Xoderony.Localization.Json;

public enum JsonStringTableNodeKind {
    Group,
    TextEntry
}

public sealed class JsonStringTableNode {

    private readonly SortedDictionary<string, JsonStringTableNode> _childBySegment = new(StringComparer.Ordinal);
    private readonly string _fullKey;
    private readonly JsonStringTableNodeKind _kind;
    private readonly string _segment;

    internal JsonStringTableNode(string segment, string fullKey, JsonStringTableNodeKind kind) {
        _segment = segment;
        _fullKey = fullKey;
        _kind = kind;
    }

    public string Segment => _segment;

    public string FullKey => _fullKey;

    public JsonStringTableNodeKind Kind => _kind;

    public IReadOnlyDictionary<string, JsonStringTableNode> Children => _childBySegment;

    public bool TryGetChild(string segment, [NotNullWhen(true)] out JsonStringTableNode? child) {
        return _childBySegment.TryGetValue(segment, out child);
    }

    internal JsonStringTableNode GetOrAddChild(string segment, JsonStringTableNodeKind kind) {
        Debug.Assert(_kind == JsonStringTableNodeKind.Group);
        if (_childBySegment.TryGetValue(segment, out var child)) {
            if (child._kind != kind) {
                var fullKey = _fullKey.Length == 0 ? segment : $"{_fullKey}.{segment}";
                throw new InvalidDataException($"The localization path '{fullKey}' is used as both a group and a text entry.");
            }

            return child;
        }

        var childFullKey = _fullKey.Length == 0 ? segment : $"{_fullKey}.{segment}";
        child = new JsonStringTableNode(segment, childFullKey, kind);
        _childBySegment.Add(segment, child);
        return child;
    }
}
