using System;
using System.Globalization;
using System.IO;

namespace Xoderony.Localization.Json;

public sealed class JsonLocale {

    public CultureInfo Culture { get; }

    public string FilePath { get; }

    public JsonTranslations Translations { get; }

    public JsonLocale(CultureInfo culture, string filePath, JsonTranslations translations) {
        // culture 必须非空，后续访问在写入状态前失败。
        if (culture.Equals(CultureInfo.InvariantCulture)) {
            throw new ArgumentException("The locale culture cannot be invariant.", nameof(culture));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(translations);
        Culture = CultureInfo.GetCultureInfo(culture.Name);
        FilePath = filePath;
        Translations = translations;
    }

    public static JsonLocale Load(string filePath) {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var cultureName = Path.GetFileNameWithoutExtension(filePath);
        var culture = CultureInfo.GetCultureInfo(cultureName);
        return new JsonLocale(culture, filePath, JsonTranslations.Load(filePath));
    }

    public void Save() {
        Translations.Save(FilePath);
    }
}
