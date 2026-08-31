using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xoderony.Localization.Editor;

internal sealed class EditorSettings {

    private static readonly JsonSerializerOptions SerializerOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter<EditorTheme>() }
    };
    private static readonly string SettingsPath = Path.Combine(AppContext.BaseDirectory, "settings.json");

    public string? LastDirectoryPath { get; set; }

    public EditorTheme Theme { get; set; } = EditorTheme.Light;

    public string? UiCultureName { get; set; }

    public double? WindowWidth { get; set; }

    public double? WindowHeight { get; set; }

    public static EditorSettings Load() {
        if (!File.Exists(SettingsPath)) {
            return new EditorSettings();
        }

        try {
            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<EditorSettings>(json, SerializerOptions) ?? new EditorSettings();
        } catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException) {
            return new EditorSettings();
        }
    }

    public void Save() {
        var json = JsonSerializer.Serialize(this, SerializerOptions);
        var normalizedJson = json.Replace("\r\n", "\n", StringComparison.Ordinal);
        File.WriteAllText(SettingsPath, $"{normalizedJson}\n", new UTF8Encoding(false));
    }
}
