using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

internal static class JsonObjectFile {

    public static JsonObject Read(string filePath) {
        // filePath 必须是有效文件路径；文件 API 负责拒绝不可访问的路径。

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
        // filePath 必须是有效文件路径；文件 API 负责拒绝不可访问的路径。
        ArgumentNullException.ThrowIfNull(root);

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
