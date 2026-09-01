using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Xoderony.Localization;

public sealed class StringLocalizer : IStringLocalizer {

    private readonly FrozenDictionary<string, string> _keyToLocalizedString;
    private readonly CultureInfo _culture;

    public CultureInfo Culture => _culture;

    public StringLocalizer(CultureInfo culture, IEnumerable<KeyValuePair<string, string>> localizedStrings) {
        Debug.Assert(culture is not null);
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The target culture cannot be invariant.", nameof(culture));
        }
        Debug.Assert(localizedStrings is not null);

        var keyToLocalizedString = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in localizedStrings) {
            var key = entry.Key;
            var value = entry.Value;
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentException("A localization key cannot be null or empty.", nameof(localizedStrings));
            }
            if (value is null) {
                throw new ArgumentException("A localized string cannot be null.", nameof(localizedStrings));
            }
            if (!keyToLocalizedString.TryAdd(key, value)) {
                throw new ArgumentException($"Duplicate localization key '{key}'.", nameof(localizedStrings));
            }
        }

        _culture = CultureInfo.GetCultureInfo(culture.Name);
        _keyToLocalizedString = keyToLocalizedString.ToFrozenDictionary(StringComparer.Ordinal);
    }

    /// <summary>直接持有传入的 culture 与字符串表，不解析 culture，也不复制字典。</summary>
    internal StringLocalizer(CultureInfo culture, FrozenDictionary<string, string> keyToLocalizedString) {
        _culture = culture;
        _keyToLocalizedString = keyToLocalizedString;
    }

    public string this[string key] {
        get {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentException("The localization key cannot be null or empty.", nameof(key));
            }
            if (_keyToLocalizedString.TryGetValue(key, out var value)) {
                return value;
            }

            return key;
        }
    }

    public string this[string key, params object?[] arguments] {
        get {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentException("The localization key cannot be null or empty.", nameof(key));
            }
            Debug.Assert(arguments is not null);
            if (_keyToLocalizedString.TryGetValue(key, out var format)) {
                return string.Format(_culture, format, arguments);
            }

            return key;
        }
    }
}
