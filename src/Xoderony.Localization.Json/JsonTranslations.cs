using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Xoderony.Localization.Json;

[JsonConverter(typeof(JsonTranslationsConverter))]
public sealed class JsonTranslations {

    private readonly SortedDictionary<string, string> _keyToTranslation;

    public IEnumerable<KeyValuePair<string, string>> KeyToTranslation => _keyToTranslation;

    public IEnumerable<string> TranslationKeys => _keyToTranslation.Keys;

    public JsonTranslations() : this(new SortedDictionary<string, string>(StringComparer.Ordinal)) {
    }

    /// <summary>直接持有传入的 keyToTranslation，不复制字典。</summary>
    public JsonTranslations(SortedDictionary<string, string> keyToTranslation) {
        Debug.Assert(keyToTranslation is not null);
        _keyToTranslation = keyToTranslation;
    }

    public static JsonTranslations Load(string filePath) {
        return JsonFile.Deserialize<JsonTranslations>(filePath);
    }

    public void Save(string filePath) {
        JsonFile.Write(filePath, this);
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
}
