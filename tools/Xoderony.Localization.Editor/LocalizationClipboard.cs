using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using Xoderony.Localization.Json;

namespace Xoderony.Localization.Editor;

internal static class LocalizationClipboard {

    private const string DataFormat = "Xoderony.Localization.JsonNode";
    private const string FormatName = "xoderony.localization.node";
    private const int FormatVersion = 1;

    public static bool ContainsData() {
        try {
            return Clipboard.ContainsData(DataFormat) || Clipboard.ContainsText(TextDataFormat.UnicodeText);
        } catch (ExternalException) {
            return false;
        }
    }

    public static void Set(JsonStringTableCollection tables, JsonStringTableNode node) {
        ArgumentNullException.ThrowIfNull(tables);
        ArgumentNullException.ThrowIfNull(node);

        var payload = new ClipboardPayload {
            Format = FormatName,
            Version = FormatVersion,
            Segment = node.Segment,
            Node = CreateNode(tables, node)
        };
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        var data = new DataObject();
        data.SetData(DataFormat, json);
        data.SetText(json, TextDataFormat.UnicodeText);
        Clipboard.SetDataObject(data, copy: true);
    }

    public static bool TryGet([NotNullWhen(true)] out ClipboardPayload? payload) {
        var data = Clipboard.GetDataObject();
        var json = data?.GetData(DataFormat) as string;
        if (json is null && data?.GetDataPresent(DataFormats.UnicodeText) == true) {
            json = data.GetData(DataFormats.UnicodeText) as string;
        }

        if (json is null) {
            payload = null;
            return false;
        }

        try {
            payload = JsonSerializer.Deserialize<ClipboardPayload>(json);
        } catch (JsonException) {
            payload = null;
            return false;
        } catch (NotSupportedException) {
            payload = null;
            return false;
        }

        if (payload is null
            || !string.Equals(payload.Format, FormatName, StringComparison.Ordinal)
            || payload.Version != FormatVersion
            || !IsValidSegment(payload.Segment)
            || !IsValidNode(payload.Node)) {
            payload = null;
            return false;
        }

        return true;
    }

    public static void Paste(JsonStringTableCollection tables, string parentGroupKey, string segment, ClipboardPayload payload) {
        ArgumentNullException.ThrowIfNull(tables);
        ArgumentNullException.ThrowIfNull(payload);

        AddNode(tables, parentGroupKey, segment, payload.Node);
    }

    private static ClipboardNode CreateNode(JsonStringTableCollection tables, JsonStringTableNode node) {
        if (node.Kind == JsonStringTableNodeKind.Group) {
            var children = new Dictionary<string, ClipboardNode>(StringComparer.Ordinal);
            foreach (var child in node.Children.Values) {
                children.Add(child.Segment, CreateNode(tables, child));
            }

            return new ClipboardNode { Children = children };
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var culture in tables.Cultures) {
            values.Add(culture.Name, tables.GetValue(culture, node.FullKey));
        }

        return new ClipboardNode { Values = values };
    }

    private static void AddNode(JsonStringTableCollection tables, string parentGroupKey, string segment, ClipboardNode node) {
        var key = CombineKey(parentGroupKey, segment);
        if (node.Children is not null) {
            tables.AddGroup(parentGroupKey, segment);
            foreach (var child in node.Children) {
                AddNode(tables, key, child.Key, child.Value);
            }

            return;
        }

        tables.AddTextEntry(parentGroupKey, segment);
        foreach (var culture in tables.Cultures) {
            if (TryGetValue(node.Values!, culture, out var value)) {
                tables.SetValue(culture, key, value);
            }
        }
    }

    private static bool TryGetValue(Dictionary<string, string> values, CultureInfo culture, [NotNullWhen(true)] out string? value) {
        foreach (var pair in values) {
            if (string.Equals(pair.Key, culture.Name, StringComparison.OrdinalIgnoreCase)) {
                value = pair.Value;
                return true;
            }
        }

        value = null;
        return false;
    }

    private static bool IsValidNode(ClipboardNode? node) {
        if (node is null || (node.Children is null) == (node.Values is null)) {
            return false;
        }

        if (node.Values is not null) {
            foreach (var value in node.Values.Values) {
                if (value is null) {
                    return false;
                }
            }

            return true;
        }

        foreach (var child in node.Children!) {
            if (!IsValidSegment(child.Key) || !IsValidNode(child.Value)) {
                return false;
            }
        }

        return true;
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

    private static string CombineKey(string parentKey, string segment) {
        return parentKey.Length == 0 ? segment : $"{parentKey}.{segment}";
    }

    internal sealed class ClipboardPayload {

        public ClipboardPayload() { }

        public string Format { get; init; } = string.Empty;

        public int Version { get; init; }

        public string Segment { get; init; } = string.Empty;

        public ClipboardNode Node { get; init; } = new();
    }

    internal sealed class ClipboardNode {

        public ClipboardNode() { }

        public Dictionary<string, ClipboardNode>? Children { get; init; }

        public Dictionary<string, string>? Values { get; init; }
    }
}
