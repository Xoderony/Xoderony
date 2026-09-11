using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using YamlDotNet.RepresentationModel;

namespace Xoderony.Stride.UI.SourceGeneration;

internal static class UIAssetReader {
    public static Asset Read(string text, bool library, CancellationToken cancellationToken) {
        var stream = new YamlStream();
        stream.Load(new StringReader(text));
        if (stream.Documents.Count != 1) {
            throw new AssetException(2, "Expected exactly one YAML document.");
        }

        var document = Mapping(stream.Documents[0].RootNode);
        if (document.Tag.ToString() != (library ? "!UILibraryAsset" : "!UIPageAsset")) {
            throw new AssetException(2, "The asset tag does not match the file extension.");
        }

        var hierarchy = Mapping(Required(document, "Hierarchy"));
        var elements = new Dictionary<Guid, Element>();
        foreach (var part in Sequence(Required(hierarchy, "Parts")).Children) {
            cancellationToken.ThrowIfCancellationRequested();
            var node = Mapping(Required(Mapping(part), "UIElement"));
            var id = ParseId(Scalar(Required(node, "Id")));
            var element = new Element(id, Scalar(Optional(node, "Name")), node.Tag.ToString());
            if (elements.ContainsKey(id)) {
                throw new AssetException(2, $"Duplicate element ID '{id}'.");
            }

            elements.Add(id, element);
            var children = Optional(node, "Children");
            if (children is not null) {
                var index = 0;
                foreach (var child in Mapping(children).Children.Values) {
                    element.Children.Add(new ChildEdge(Reference(child), ChildRelation.Children, index));
                    index++;
                }
            }

            var content = Optional(node, "Content");
            if (content is not null && Scalar(content) is not "" and not "null" and not "~") {
                element.Children.Add(new ChildEdge(Reference(content), ChildRelation.Content, 0));
            }
        }

        var roots = new List<Element>();
        var visited = new HashSet<Guid>();
        foreach (var root in Sequence(Required(hierarchy, "RootParts")).Children) {
            var id = Reference(root);
            if (!elements.TryGetValue(id, out var element)) {
                throw new AssetException(2, $"Unknown root ID '{id}'.");
            }

            roots.Add(element);
            var pending = new Stack<Element>();
            pending.Push(element);
            while (pending.Count > 0) {
                cancellationToken.ThrowIfCancellationRequested();
                var current = pending.Pop();
                if (!visited.Add(current.Id)) {
                    throw new AssetException(2, $"Element '{current.Id}' has multiple parents or forms a cycle.");
                }

                foreach (var edge in current.Children) {
                    if (!elements.TryGetValue(edge.ChildId, out var child)) {
                        throw new AssetException(2, $"Unknown child ID '{edge.ChildId}'.");
                    }

                    pending.Push(child);
                }
            }
        }

        if (!library && roots.Count != 1) {
            throw new AssetException(2, "A page must have exactly one root.");
        }

        if (visited.Count != elements.Count) {
            throw new AssetException(2, "Some Parts are unreachable through Children/Content. Unsupported hierarchy or unresolved inheritance.");
        }

        return new Asset(elements, roots);
    }

    private static YamlMappingNode Mapping(YamlNode node) {
        return node as YamlMappingNode ?? throw new AssetException(2, "Expected a YAML mapping.");
    }

    private static YamlSequenceNode Sequence(YamlNode node) {
        return node as YamlSequenceNode ?? throw new AssetException(2, "Expected a YAML sequence.");
    }

    private static string Scalar(YamlNode? node) {
        if (node is null) {
            return string.Empty;
        }

        return node is YamlScalarNode scalar ? scalar.Value ?? string.Empty : throw new AssetException(2, "Expected a YAML scalar.");
    }

    private static YamlNode? Optional(YamlMappingNode node, string key) {
        YamlNode? result = null;
        foreach (var pair in node.Children) {
            // Stride 的覆盖标记属于成员键，不属于业务名称。
            if (Scalar(pair.Key).TrimEnd('*', '!') == key) {
                if (result is not null) {
                    throw new AssetException(2, $"Duplicate member '{key}'.");
                }

                result = pair.Value;
            }
        }

        return result;
    }

    private static YamlNode Required(YamlMappingNode node, string key) {
        return Optional(node, key) ?? throw new AssetException(2, $"Missing '{key}'.");
    }

    private static Guid Reference(YamlNode node) {
        var value = Scalar(node);
        if (!value.StartsWith("ref!! ", StringComparison.Ordinal)) {
            throw new AssetException(2, $"Expected an element reference, found '{value}'.");
        }

        return ParseId(value.Substring(6).Trim());
    }

    private static Guid ParseId(string value) {
        return Guid.TryParse(value, out var id) ? id : throw new AssetException(2, $"Invalid GUID '{value}'.");
    }
}

internal sealed class Asset(Dictionary<Guid, Element> elements, List<Element> roots) {
    public Dictionary<Guid, Element> Elements { get; } = elements;
    public List<Element> Roots { get; } = roots;
}

internal sealed class Element(Guid id, string name, string tag) {
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Tag { get; } = tag;
    public List<ChildEdge> Children { get; } = [];
}

internal enum ChildRelation {
    Children,
    Content
}

internal readonly struct ChildEdge(Guid childId, ChildRelation relation, int index) {
    public Guid ChildId { get; } = childId;
    public ChildRelation Relation { get; } = relation;
    public int Index { get; } = index;
}

internal sealed class AssetException(int diagnostic, string message) : Exception(message) {
    public int Diagnostic { get; } = diagnostic;
}
