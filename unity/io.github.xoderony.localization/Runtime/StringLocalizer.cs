using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;

namespace Xoderony.Localization;

public sealed class StringLocalizer : IStringLocalizer {

    private readonly FrozenDictionary<string, string> _localizedStringByKey;
    private readonly CultureInfo _culture;

    public CultureInfo Culture => _culture;

    public StringLocalizer(CultureInfo culture, IEnumerable<KeyValuePair<string, string>> localizedStrings) {
        ArgumentNullException.ThrowIfNull(culture);
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The target culture cannot be invariant.", nameof(culture));
        }
        ArgumentNullException.ThrowIfNull(localizedStrings);

        var localizedStringByKey = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in localizedStrings) {
            var key = entry.Key;
            var value = entry.Value;
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentException("A localization key cannot be null or empty.", nameof(localizedStrings));
            }
            if (value is null) {
                throw new ArgumentException("A localized string cannot be null.", nameof(localizedStrings));
            }
            if (!localizedStringByKey.TryAdd(key, value)) {
                throw new ArgumentException($"Duplicate localization key '{key}'.", nameof(localizedStrings));
            }
        }

        _culture = CultureInfo.GetCultureInfo(culture.Name);
        _localizedStringByKey = localizedStringByKey.ToFrozenDictionary(StringComparer.Ordinal);
    }

    internal StringLocalizer(CultureInfo culture, FrozenDictionary<string, string> localizedStringByKey) {
        _culture = culture;
        _localizedStringByKey = localizedStringByKey;
    }

    public string this[string key] {
        get {
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentException("The localization key cannot be null or empty.", nameof(key));
            }
            if (_localizedStringByKey.TryGetValue(key, out var value)) {
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
            ArgumentNullException.ThrowIfNull(arguments);
            if (_localizedStringByKey.TryGetValue(key, out var format)) {
                return string.Format(_culture, format, arguments);
            }

            return key;
        }
    }
}
