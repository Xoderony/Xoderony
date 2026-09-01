using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using Xoderony.Localization.Json;

namespace Xoderony.Localization.Editor;

internal static class LocalizationClipboard {

    private const string DataFormat = "Xoderony.Localization.JsonNode";
    private const string FormatName = "xoderony.localization.node";
    private const int FormatVersion = 1;

    public static bool ContainsData() {
        try {
            return Clipboard.ContainsData(DataFormat);
        } catch (ExternalException) {
            return false;
        }
    }

    public static void Set(JsonLocaleTableCollection tables, JsonKeyNode node) {
        ArgumentNullException.ThrowIfNull(tables);
        ArgumentNullException.ThrowIfNull(node);

        var payload = new ClipboardPayload {
            Format = FormatName,
            Version = FormatVersion,
            LocalKey = node.LocalKey,
            Node = CreateNode(tables, node)
        };
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        var data = new DataObject();
        data.SetData(DataFormat, json);
        data.SetText(json, TextDataFormat.UnicodeText);
        Clipboard.SetDataObject(data, copy: true);
    }

    public static bool TryGet([NotNullWhen(true)] out ClipboardPayload? payload) {
        IDataObject? data;
        try {
            data = Clipboard.GetDataObject();
        } catch (ExternalException) {
            payload = null;
            return false;
        }

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
            || !JsonKeyNode.IsLowerSnakeCaseLocalKey(payload.LocalKey)
            || !IsValidNode(payload.Node)) {
            payload = null;
            return false;
        }

        return true;
    }

    public static void Paste(JsonLocaleTableCollection tables, string parentGroupKey, string localKey, ClipboardPayload payload) {
        ArgumentNullException.ThrowIfNull(tables);
        ArgumentNullException.ThrowIfNull(payload);

        AddNode(tables, parentGroupKey, localKey, payload.Node);
    }

    private static ClipboardNode CreateNode(JsonLocaleTableCollection tables, JsonKeyNode node) {
        if (node is JsonKeyGroup group) {
            var localKeyToChild = new Dictionary<string, ClipboardNode>(StringComparer.Ordinal);
            foreach (var child in group.LocalKeyToChild.Values) {
                localKeyToChild.Add(child.LocalKey, CreateNode(tables, child));
            }

            return new ClipboardNode { LocalKeyToChild = localKeyToChild };
        }

        var cultureNameToTranslation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var culture in tables.GetCultures()) {
            cultureNameToTranslation.Add(culture.Name, tables.GetTranslation(culture, node.FullKey));
        }

        return new ClipboardNode { CultureNameToTranslation = cultureNameToTranslation };
    }

    private static void AddNode(JsonLocaleTableCollection tables, string parentGroupKey, string localKey, ClipboardNode node) {
        var key = JsonKeyNode.CombineKey(parentGroupKey, localKey);
        if (node.LocalKeyToChild is not null) {
            tables.AddGroup(parentGroupKey, localKey);
            foreach (var child in node.LocalKeyToChild) {
                AddNode(tables, key, child.Key, child.Value);
            }

            return;
        }

        tables.AddEntry(parentGroupKey, localKey);
        foreach (var culture in tables.GetCultures()) {
            if (TryGetValue(node.CultureNameToTranslation!, culture, out var value)) {
                tables.SetTranslation(culture, key, value);
            }
        }
    }

    private static bool TryGetValue(Dictionary<string, string> cultureNameToTranslation, CultureInfo culture, [NotNullWhen(true)] out string? value) {
        foreach (var pair in cultureNameToTranslation) {
            if (string.Equals(pair.Key, culture.Name, StringComparison.OrdinalIgnoreCase)) {
                value = pair.Value;
                return true;
            }
        }

        value = null;
        return false;
    }

    private static bool IsValidNode(ClipboardNode? node) {
        if (node is null || (node.LocalKeyToChild is null) == (node.CultureNameToTranslation is null)) {
            return false;
        }

        if (node.CultureNameToTranslation is not null) {
            foreach (var value in node.CultureNameToTranslation.Values) {
                if (value is null) {
                    return false;
                }
            }

            return true;
        }

        foreach (var child in node.LocalKeyToChild!) {
            if (!JsonKeyNode.IsLowerSnakeCaseLocalKey(child.Key) || !IsValidNode(child.Value)) {
                return false;
            }
        }

        return true;
    }

    internal sealed class ClipboardPayload {

        public string Format { get; init; } = string.Empty;

        public int Version { get; init; }

        public string LocalKey { get; init; } = string.Empty;

        public ClipboardNode Node { get; init; } = new();
    }

    internal sealed class ClipboardNode {

        [JsonPropertyName("Children")]
        public Dictionary<string, ClipboardNode>? LocalKeyToChild { get; init; }

        [JsonPropertyName("Values")]
        public Dictionary<string, string>? CultureNameToTranslation { get; init; }
    }
}
