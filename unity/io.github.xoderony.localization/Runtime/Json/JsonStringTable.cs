using System;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

internal sealed class JsonStringTable {

    private readonly CultureInfo _culture;
    private readonly string _filePath;
    private readonly JsonObject _root;

    public CultureInfo Culture => _culture;
    public string FilePath => _filePath;
    public JsonObject Root => _root;

    public JsonStringTable(CultureInfo culture, string filePath, JsonObject root) {
        ArgumentNullException.ThrowIfNull(culture);
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The table culture cannot be invariant.", nameof(culture));
        }

        ArgumentException.ThrowIfNullOrEmpty(filePath);
        ArgumentNullException.ThrowIfNull(root);
        _culture = CultureInfo.GetCultureInfo(culture.Name);
        _filePath = filePath;
        _root = root;
    }

    public static JsonStringTable Load(CultureInfo culture, string filePath) {
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentException.ThrowIfNullOrEmpty(filePath);
        try {
            using var stream = File.OpenRead(filePath);
            var value = JsonNode.Parse(stream, documentOptions: new JsonDocumentOptions {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow
            });
            if (value is not JsonObject root) {
                throw new InvalidDataException($"The root value in '{filePath}' must be an object.");
            }

            return new JsonStringTable(culture, filePath, root);
        } catch (JsonException exception) {
            throw new InvalidDataException($"The JSON file '{filePath}' is invalid.", exception);
        }
    }

    public void Save() {
        var directoryPath = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }

        using var stream = File.Create(_filePath);
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = true,
            NewLine = "\n"
        });

        _root.WriteTo(writer);
        writer.Flush();
        stream.WriteByte((byte)'\n');
    }
}
