using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

internal static class JsonObjectFile {

    public static JsonObject Read(string filePath) {
        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));

        try {
            using var stream = File.OpenRead(filePath);
            var value = JsonNode.Parse(stream, documentOptions: new JsonDocumentOptions {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow
            });
            if (value is not JsonObject root) {
                throw new InvalidDataException($"The root value in '{filePath}' must be an object.");
            }

            return root;
        } catch (JsonException exception) {
            throw new InvalidDataException($"The JSON file '{filePath}' is invalid.", exception);
        }
    }

    public static void Write(string filePath, JsonObject root) {
        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));
        Debug.Assert(root is not null);

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
