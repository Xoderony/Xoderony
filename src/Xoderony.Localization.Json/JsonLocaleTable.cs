using System;
using System.Collections.Generic;
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
        // culture 必须非空，后续访问在写入状态前失败。
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The locale table culture cannot be invariant.", nameof(culture));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(keyToTranslation);
        _culture = culture;
        _filePath = filePath;
        _keyToTranslation = keyToTranslation;
    }

    public static JsonLocaleTable Load(CultureInfo culture, string filePath) {
        // culture 必须非空，后续访问在写入状态前失败。
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
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
        // key 必须非空；字典操作负责拒绝空键。
        return _keyToTranslation.TryGetValue(key, out translation);
    }

    public bool SetTranslation(string key, string translation) {
        // key 必须非空；字典操作负责拒绝空键。
        ArgumentNullException.ThrowIfNull(translation);
        if (_keyToTranslation.TryGetValue(key, out var current) && string.Equals(current, translation, StringComparison.Ordinal)) {
            return false;
        }

        _keyToTranslation[key] = translation;
        return true;
    }

    public bool RemoveTranslation(string key, [NotNullWhen(true)] out string? translation) {
        // key 必须非空；字典操作负责拒绝空键。
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
