using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

internal sealed class JsonStringTable {

    private readonly CultureInfo _culture;
    private readonly string _filePath;
    private readonly SortedDictionary<string, string> _values;

    public CultureInfo Culture => _culture;
    public string FilePath => _filePath;
    public SortedDictionary<string, string> Values => _values;

    /// <summary>直接持有传入的 culture 与 values，不解析 culture，也不复制字典。</summary>
    public JsonStringTable(CultureInfo culture, string filePath, SortedDictionary<string, string> values) {
        Debug.Assert(culture is not null);
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The table culture cannot be invariant.", nameof(culture));
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));
        Debug.Assert(values is not null);
        _culture = culture;
        _filePath = filePath;
        _values = values;
    }

    public static JsonStringTable Load(CultureInfo culture, string filePath) {
        Debug.Assert(culture is not null);
        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));
        culture = CultureInfo.GetCultureInfo(culture.Name);

        try {
            using var stream = File.OpenRead(filePath);
            var value = JsonNode.Parse(stream, documentOptions: new JsonDocumentOptions {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow
            });
            if (value is not JsonObject root) {
                throw new InvalidDataException($"The root value in '{filePath}' must be an object.");
            }

            return new JsonStringTable(culture, filePath, ReadValues(root, filePath));
        } catch (JsonException exception) {
            throw new InvalidDataException($"The JSON file '{filePath}' is invalid.", exception);
        }

        static SortedDictionary<string, string> ReadValues(JsonObject root, string path) {
            var values = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var (key, node) in root) {
                if (node is not JsonValue jsonValue || !jsonValue.TryGetValue<string>(out var text)) {
                    throw new InvalidDataException($"The localization value '{key}' in '{path}' must be a string.");
                }

                if (!values.TryAdd(key, text)) {
                    throw new InvalidDataException($"The localization key '{key}' in '{path}' is duplicated.");
                }
            }

            return values;
        }
    }

    public void Save() {
        var root = new JsonObject();
        foreach (var (key, text) in _values) {
            root.Add(key, text);
        }

        WriteJsonFile(_filePath, root);
    }

    internal static void WriteJsonFile(string filePath, JsonObject root) {
        var directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }

        using var stream = File.Create(filePath);
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = true,
            NewLine = "\n"
        });

        root.WriteTo(writer);
        writer.Flush();
        stream.WriteByte((byte)'\n');
    }
}
