using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Xoderony.Localization.Json;

public sealed class JsonLocales {

    private readonly SortedDictionary<string, JsonLocale> _cultureNameToLocale;
    private readonly HashSet<JsonLocale> _dirtyLocales = [];

    public int Count => _cultureNameToLocale.Count;

    public bool IsDirty => _dirtyLocales.Count != 0;

    public JsonLocales() : this(new SortedDictionary<string, JsonLocale>(StringComparer.OrdinalIgnoreCase)) {
    }

    /// <summary>接管 cultureNameToLocale 的所有权，不复制。</summary>
    public JsonLocales(SortedDictionary<string, JsonLocale> cultureNameToLocale) {
        Debug.Assert(cultureNameToLocale is not null);
        _cultureNameToLocale = cultureNameToLocale;
    }

    public static JsonLocales LoadDirectory(string directoryPath, string keysFileName) {
        Debug.Assert(!string.IsNullOrWhiteSpace(directoryPath));
        Debug.Assert(!string.IsNullOrWhiteSpace(keysFileName));

        var cultureNameToLocale = new SortedDictionary<string, JsonLocale>(StringComparer.OrdinalIgnoreCase);
        foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*.json", SearchOption.TopDirectoryOnly)) {
            if (string.Equals(Path.GetFileName(filePath), keysFileName, StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            var locale = JsonLocale.Load(filePath);
            if (!cultureNameToLocale.TryAdd(locale.Culture.Name, locale)) {
                throw new InvalidDataException($"The culture '{locale.Culture.Name}' has more than one locale in '{directoryPath}'.");
            }
        }

        return new JsonLocales(cultureNameToLocale);
    }

    public void Add(JsonLocale locale) {
        Debug.Assert(locale is not null);
        if (!_cultureNameToLocale.TryAdd(locale.Culture.Name, locale)) {
            throw new ArgumentException($"The culture '{locale.Culture.Name}' already has a locale.", nameof(locale));
        }

        _dirtyLocales.Add(locale);
    }

    public JsonLocale Get(CultureInfo culture) {
        // culture 必须非空；访问名称时会在查询或写入状态前失败。
        if (!_cultureNameToLocale.TryGetValue(culture.Name, out var locale)) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of these locales.", nameof(culture));
        }

        return locale;
    }

    public IEnumerable<JsonLocale> GetLocales() {
        return _cultureNameToLocale.Values;
    }

    public IEnumerable<CultureInfo> GetCultures() {
        foreach (var locale in _cultureNameToLocale.Values) {
            yield return locale.Culture;
        }
    }

    public string GetTranslation(CultureInfo culture, string key) {
        return Get(culture).Translations.TryGetTranslation(key, out var translation) ? translation : string.Empty;
    }

    public bool SetTranslation(CultureInfo culture, string key, string translation) {
        var locale = Get(culture);
        if (!locale.Translations.SetTranslation(key, translation)) {
            return false;
        }

        _dirtyLocales.Add(locale);
        return true;
    }

    public IEnumerable<string> GetTranslationKeys(CultureInfo culture) {
        return Get(culture).Translations.TranslationKeys;
    }

    public void EnsureEntry(string key) {
        foreach (var locale in _cultureNameToLocale.Values) {
            if (locale.Translations.SetTranslation(key, string.Empty)) {
                _dirtyLocales.Add(locale);
            }
        }
    }

    public void CopyEntry(string fromKey, string toKey) {
        foreach (var locale in _cultureNameToLocale.Values) {
            var text = locale.Translations.TryGetTranslation(fromKey, out var sourceText) ? sourceText : string.Empty;
            if (locale.Translations.SetTranslation(toKey, text)) {
                _dirtyLocales.Add(locale);
            }
        }
    }

    public void CopyGroup(JsonKeyGroup fromGroup, string toKey) {
        Debug.Assert(fromGroup is not null);
        foreach (var locale in _cultureNameToLocale.Values) {
            var changed = false;
            foreach (var fromEntryKey in fromGroup.GetDescendantEntryKeys()) {
                var toEntryKey = toKey + fromEntryKey[fromGroup.FullKey.Length..];
                var translation = locale.Translations.TryGetTranslation(fromEntryKey, out var sourceTranslation) ? sourceTranslation : string.Empty;
                changed |= locale.Translations.SetTranslation(toEntryKey, translation);
            }

            if (changed) {
                _dirtyLocales.Add(locale);
            }
        }
    }

    public void RemapEntry(string fromKey, string toKey) {
        foreach (var locale in _cultureNameToLocale.Values) {
            if (locale.Translations.RemoveTranslation(fromKey, out var translation)) {
                locale.Translations.SetTranslation(toKey, translation);
                _dirtyLocales.Add(locale);
            }
        }
    }

    public void RemapGroup(string fromKey, string toKey) {
        foreach (var locale in _cultureNameToLocale.Values) {
            var translations = locale.Translations;
            var replacements = new List<(string FromKey, string ToKey)>();
            foreach (var key in translations.TranslationKeys) {
                if (string.Equals(key, fromKey, StringComparison.Ordinal) || key.StartsWith($"{fromKey}.", StringComparison.Ordinal)) {
                    replacements.Add((key, toKey + key[fromKey.Length..]));
                }
            }

            if (replacements.Count == 0) {
                continue;
            }

            foreach (var (from, to) in replacements) {
                if (!translations.RemoveTranslation(from, out var translation)) {
                    continue;
                }

                translations.SetTranslation(to, translation);
            }

            _dirtyLocales.Add(locale);
        }
    }

    public void RemoveEntry(string entryKey) {
        foreach (var locale in _cultureNameToLocale.Values) {
            if (locale.Translations.RemoveTranslation(entryKey, out _)) {
                _dirtyLocales.Add(locale);
            }
        }
    }

    public void RemoveGroup(string groupKey) {
        foreach (var locale in _cultureNameToLocale.Values) {
            var translations = locale.Translations;
            var removals = new List<string>();
            foreach (var valueKey in translations.TranslationKeys) {
                if (string.Equals(valueKey, groupKey, StringComparison.Ordinal) || valueKey.StartsWith($"{groupKey}.", StringComparison.Ordinal)) {
                    removals.Add(valueKey);
                }
            }

            if (removals.Count == 0) {
                continue;
            }

            foreach (var valueKey in removals) {
                translations.RemoveTranslation(valueKey, out _);
            }

            _dirtyLocales.Add(locale);
        }
    }

    public void Save() {
        foreach (var locale in _dirtyLocales) {
            locale.Save();
        }

        _dirtyLocales.Clear();
    }
}
