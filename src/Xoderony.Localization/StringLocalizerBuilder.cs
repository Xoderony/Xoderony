using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;

namespace Xoderony.Localization;

public sealed class StringLocalizerBuilder {

    private readonly Dictionary<string, string> _keyToLocalizedString = new(StringComparer.Ordinal);
    private readonly CultureInfo _culture;

    public CultureInfo Culture => _culture;

    public StringLocalizerBuilder(CultureInfo culture) {
        // culture 必须非空，下面的 Equals 会在创建对象前拒绝空引用。
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The target culture cannot be invariant.", nameof(culture));
        }

        _culture = CultureInfo.GetCultureInfo(culture.Name);
    }

    public void AddLayer(IEnumerable<KeyValuePair<string, string>> localizedStrings) {
        // localizedStrings 必须非空；完整枚举验证后才合并此层。

        var layerKeyToLocalizedString = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in localizedStrings) {
            var key = entry.Key;
            var value = entry.Value;
            if (string.IsNullOrEmpty(key)) {
                throw new ArgumentException("A localization key cannot be null or empty.", nameof(localizedStrings));
            }
            if (value is null) {
                throw new ArgumentException("A localized string cannot be null.", nameof(localizedStrings));
            }
            if (!layerKeyToLocalizedString.TryAdd(key, value)) {
                throw new ArgumentException($"Duplicate localization key '{key}'.", nameof(localizedStrings));
            }
        }

        foreach (var entry in layerKeyToLocalizedString) {
            _keyToLocalizedString[entry.Key] = entry.Value;
        }
    }

    public StringLocalizer Build() {
        var keyToLocalizedString = _keyToLocalizedString.ToFrozenDictionary(StringComparer.Ordinal);
        return new StringLocalizer(_culture, keyToLocalizedString);
    }
}
