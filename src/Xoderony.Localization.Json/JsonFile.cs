using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Xoderony.Localization.Json;

internal static class JsonFile {

    public static T Deserialize<T>(string filePath) where T : class {
        // filePath 必须是有效文件路径；文件 API 负责拒绝不可访问的路径。
        try {
            using var stream = File.OpenRead(filePath);
            return JsonSerializer.Deserialize<T>(stream)                ?? throw new InvalidDataException($"The root value in '{filePath}' must be an object.");
        } catch (JsonException exception) {
            throw new InvalidDataException($"The JSON file '{filePath}' is invalid.", exception);
        }
    }

    public static void Write<T>(string filePath, T value) {
        // filePath 必须是有效文件路径；文件 API 负责拒绝不可访问的路径。
        ArgumentNullException.ThrowIfNull(value);
        var directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }
        var writerOptions = new JsonWriterOptions {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = true,
            NewLine = "\n"
        };
        using var stream = File.Create(filePath);
        using var writer = new Utf8JsonWriter(stream, writerOptions);
        JsonSerializer.Serialize(writer, value);
        writer.Flush();
        stream.WriteByte((byte)'\n');
    }
}
