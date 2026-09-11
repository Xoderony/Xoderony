using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;

namespace Xoderony.Localization.Json;

internal sealed class JsonLocaleTable {

    private readonly CultureInfo _culture;
    private readonly string _filePath;
    private readonly SortedDictionary<string, string> _keyToTranslation;

    public CultureInfo Culture => _culture;
    public IEnumerable<KeyValuePair<string, string>> KeyToTranslation => _keyToTranslation;
    public IEnumerable<string> TranslationKeys => _keyToTranslation.Keys;

    /// <summary>直接持有传入的 culture 与 keyToTranslation，不解析 culture，也不复制字典。</summary>
    public JsonLocaleTable(CultureInfo culture, string filePath, SortedDictionary<string, string> keyToTranslation) {
        Debug.Assert(culture is not null);
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The locale table culture cannot be invariant.", nameof(culture));
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));
        Debug.Assert(keyToTranslation is not null);
        _culture = culture;
        _filePath = filePath;
        _keyToTranslation = keyToTranslation;
    }

    public static JsonLocaleTable Load(CultureInfo culture, string filePath) {
        Debug.Assert(culture is not null);
        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));
        culture = CultureInfo.GetCultureInfo(culture.Name);

        var root = JsonObjectFile.Read(filePath);
        var keyToTranslation = ParseTranslations(root, filePath);
        return new JsonLocaleTable(culture, filePath, keyToTranslation);

        static SortedDictionary<string, string> ParseTranslations(JsonObject root, string path) {
            var keyToTranslation = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var (key, node) in root) {
                if (node is not JsonValue jsonValue || !jsonValue.TryGetValue<string>(out var text)) {
                    throw new InvalidDataException($"The localization value '{key}' in '{path}' must be a string.");
                }

                if (!keyToTranslation.TryAdd(key, text)) {
                    throw new InvalidDataException($"The localization key '{key}' in '{path}' is duplicated.");
                }
            }

            return keyToTranslation;
        }
    }

    public bool TryGetTranslation(string key, [NotNullWhen(true)] out string? translation) {
        Debug.Assert(key is not null);
        return _keyToTranslation.TryGetValue(key, out translation);
    }

    public bool SetTranslation(string key, string translation) {
        Debug.Assert(key is not null);
        Debug.Assert(translation is not null);
        if (_keyToTranslation.TryGetValue(key, out var current) && string.Equals(current, translation, StringComparison.Ordinal)) {
            return false;
        }

        _keyToTranslation[key] = translation;
        return true;
    }

    public bool RemoveTranslation(string key, [NotNullWhen(true)] out string? translation) {
        Debug.Assert(key is not null);
        return _keyToTranslation.Remove(key, out translation);
    }

    public void Save() {
        var root = new JsonObject();
        foreach (var (key, text) in _keyToTranslation) {
            root.Add(key, text);
        }

        JsonObjectFile.Write(_filePath, root);
    }
}
