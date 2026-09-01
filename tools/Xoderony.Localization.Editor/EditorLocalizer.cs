using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using Xoderony.Localization.Json;

namespace Xoderony.Localization.Editor;

internal sealed class EditorLocalizer : INotifyPropertyChanged {

    private const string FallbackCultureName = "zh-CN";
    private readonly CultureInfo _fallbackCulture;
    private readonly CultureInfo[] _cultures;
    private readonly JsonLocaleTableCollection _tables;
    private StringLocalizer _localizer;
    private CultureInfo _culture;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<CultureInfo> Cultures => _cultures;

    public CultureInfo Culture => _culture;

    public string this[string key] => _localizer[key];

    public string this[string key, params object?[] arguments] => _localizer[key, arguments];

    private EditorLocalizer(JsonLocaleTableCollection tables, CultureInfo preferredCulture) {
        _tables = tables;
        _cultures = CaptureCultures(tables);
        _fallbackCulture = FindCulture(FallbackCultureName) ?? throw new InvalidDataException($"The editor localization directory must contain '{FallbackCultureName}.json'.");
        _culture = ResolveCulture(preferredCulture);
        _localizer = BuildLocalizer(_culture);
    }

    public static EditorLocalizer Load(string directoryPath, CultureInfo? preferredCulture = null) {
        var tables = JsonLocaleTableCollection.LoadDirectory(directoryPath);
        return new EditorLocalizer(tables, preferredCulture ?? CultureInfo.CurrentUICulture);
    }

    public bool SetCulture(CultureInfo culture) {
        ArgumentNullException.ThrowIfNull(culture);
        var targetCulture = FindCulture(culture.Name) ?? throw new ArgumentException($"The editor culture '{culture.Name}' is not available.", nameof(culture));
        if (_culture.Equals(targetCulture)) {
            return false;
        }

        _culture = targetCulture;
        _localizer = BuildLocalizer(targetCulture);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Culture)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        return true;
    }

    private StringLocalizer BuildLocalizer(CultureInfo culture) {
        var builder = new StringLocalizerBuilder(culture);
        builder.AddLayer(GetNonEmptyValues(_fallbackCulture));
        if (!_fallbackCulture.Equals(culture)) {
            builder.AddLayer(GetNonEmptyValues(culture));
        }

        return builder.Build();
    }

    private List<KeyValuePair<string, string>> GetNonEmptyValues(CultureInfo culture) {
        var values = new List<KeyValuePair<string, string>>();
        foreach (var key in _tables.GetEntryKeys()) {
            var value = _tables.GetTranslation(culture, key);
            if (value.Length > 0) {
                values.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        return values;
    }

    private CultureInfo ResolveCulture(CultureInfo preferredCulture) {
        var culture = FindCulture(preferredCulture.Name);
        if (culture is not null) {
            return culture;
        }

        foreach (var candidate in _cultures) {
            if (string.Equals(candidate.TwoLetterISOLanguageName, preferredCulture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase)) {
                return candidate;
            }
        }

        return _fallbackCulture;
    }

    private CultureInfo? FindCulture(string cultureName) {
        foreach (var culture in _cultures) {
            if (string.Equals(culture.Name, cultureName, StringComparison.OrdinalIgnoreCase)) {
                return culture;
            }
        }

        return null;
    }

    private static CultureInfo[] CaptureCultures(JsonLocaleTableCollection tables) {
        var cultures = new CultureInfo[tables.CultureCount];
        var index = 0;
        foreach (var culture in tables.GetCultures()) {
            cultures[index++] = culture;
        }

        return cultures;
    }
}
