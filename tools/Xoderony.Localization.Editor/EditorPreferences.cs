using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xoderony.Localization.Editor;

internal sealed class EditorPreferences {

    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
    private static readonly string PreferencesPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
    private readonly Dictionary<string, JsonElement> _keyToValue;
    private bool _isDirty;

    private EditorPreferences(Dictionary<string, JsonElement> keyToValue) {
        _keyToValue = keyToValue;
    }

    public static EditorPreferences Load() {
        if (!File.Exists(PreferencesPath)) {
            return new EditorPreferences(new Dictionary<string, JsonElement>(StringComparer.Ordinal));
        }

        try {
            var json = File.ReadAllText(PreferencesPath);
            var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, SerializerOptions);
            return new EditorPreferences(values is null
                ? new Dictionary<string, JsonElement>(StringComparer.Ordinal)
                : new Dictionary<string, JsonElement>(values, StringComparer.Ordinal));
        } catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException) {
            return new EditorPreferences(new Dictionary<string, JsonElement>(StringComparer.Ordinal));
        }
    }

    public T Get<T>(string key, T defaultValue) {
        ArgumentException.ThrowIfNullOrEmpty(key);
        if (!_keyToValue.TryGetValue(key, out var element)) {
            return defaultValue;
        }

        try {
            var value = element.Deserialize<T>(SerializerOptions);
            return value is null ? defaultValue : value;
        } catch (Exception exception) when (exception is JsonException or NotSupportedException) {
            return defaultValue;
        }
    }

    public void Set<T>(string key, T value) {
        ArgumentException.ThrowIfNullOrEmpty(key);
        var element = JsonSerializer.SerializeToElement(value, SerializerOptions);
        if (_keyToValue.TryGetValue(key, out var currentElement) && JsonElement.DeepEquals(currentElement, element)) {
            return;
        }

        _keyToValue[key] = element;
        _isDirty = true;
    }

    public bool DeleteKey(string key) {
        ArgumentException.ThrowIfNullOrEmpty(key);
        if (!_keyToValue.Remove(key)) {
            return false;
        }

        _isDirty = true;
        return true;
    }

    public void Save() {
        if (!_isDirty) {
            return;
        }

        var json = JsonSerializer.Serialize(_keyToValue, SerializerOptions);
        var normalizedJson = json.Replace("\r\n", "\n", StringComparison.Ordinal);
        File.WriteAllText(PreferencesPath, $"{normalizedJson}\n", new UTF8Encoding(false));
        _isDirty = false;
    }

    public void MigrateKey(string oldKey, string newKey) {
        ArgumentException.ThrowIfNullOrEmpty(oldKey);
        ArgumentException.ThrowIfNullOrEmpty(newKey);
        if (!_keyToValue.Remove(oldKey, out var oldValue)) {
            return;
        }

        _keyToValue.TryAdd(newKey, oldValue);
        _isDirty = true;
    }
}
