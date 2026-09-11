using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Xoderony.Localization.Json;

public sealed class JsonLocale {

    public CultureInfo Culture { get; }

    public string FilePath { get; }

    public JsonTranslations Translations { get; }

    public JsonLocale(CultureInfo culture, string filePath, JsonTranslations translations) {
        Debug.Assert(culture is not null);
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The locale culture cannot be invariant.", nameof(culture));
        }

        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));
        Debug.Assert(translations is not null);
        Culture = CultureInfo.GetCultureInfo(culture.Name);
        FilePath = filePath;
        Translations = translations;
    }

    public static JsonLocale Load(string filePath) {
        Debug.Assert(!string.IsNullOrWhiteSpace(filePath));

        var cultureName = Path.GetFileNameWithoutExtension(filePath);
        var culture = CultureInfo.GetCultureInfo(cultureName);
        return new JsonLocale(culture, filePath, JsonTranslations.Load(filePath));
    }

    public void Save() {
        Translations.Save(FilePath);
    }
}
