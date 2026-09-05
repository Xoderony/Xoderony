using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Xoderony.Localization.Json;

public static class JsonLocalizationValidation {

    public static IReadOnlyList<JsonLocalizationIssue> Validate(JsonLocaleTableCollection collection, CultureInfo placeholderReferenceCulture) {
        Debug.Assert(collection is not null);
        Debug.Assert(placeholderReferenceCulture is not null);

        placeholderReferenceCulture = ResolvePlaceholderReferenceCulture(collection, placeholderReferenceCulture);

        var entryKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entryKey in collection.GetEntryKeys()) {
            entryKeys.Add(entryKey);
        }

        var issues = new List<JsonLocalizationIssue>();
        CollectMissingTranslations(collection, entryKeys, issues);
        CollectInvalidFormatStrings(collection, entryKeys, issues);
        CollectPlaceholderMismatches(collection, entryKeys, placeholderReferenceCulture, issues);
        CollectUnexpectedTranslationKeys(collection, entryKeys, issues);

        SortIssues(issues);

        return issues;
    }

    public static IReadOnlyList<JsonLocalizationIssue> ValidateEntry(JsonLocaleTableCollection collection, CultureInfo placeholderReferenceCulture, string entryKey) {
        Debug.Assert(collection is not null);
        Debug.Assert(placeholderReferenceCulture is not null);
        if (entryKey is null) {
            throw new ArgumentNullException(nameof(entryKey));
        }

        placeholderReferenceCulture = ResolvePlaceholderReferenceCulture(collection, placeholderReferenceCulture);
        if (!collection.RootKeyGroup.TryGet(entryKey, out var node) || node is not JsonKeyEntry) {
            throw new ArgumentException($"The entry key '{entryKey}' is not part of this locale table collection.", nameof(entryKey));
        }

        var entryKeys = new HashSet<string>(StringComparer.Ordinal) { entryKey };
        var issues = new List<JsonLocalizationIssue>();
        CollectMissingTranslations(collection, entryKeys, issues);
        CollectInvalidFormatStrings(collection, entryKeys, issues);
        CollectPlaceholderMismatches(collection, entryKeys, placeholderReferenceCulture, issues);
        SortIssues(issues);

        return issues;
    }

    private static CultureInfo ResolvePlaceholderReferenceCulture(JsonLocaleTableCollection collection, CultureInfo placeholderReferenceCulture) {
        foreach (var culture in collection.GetCultures()) {
            if (string.Equals(culture.Name, placeholderReferenceCulture.Name, StringComparison.OrdinalIgnoreCase)) {
                return culture;
            }
        }

        throw new ArgumentException(
            $"The culture '{placeholderReferenceCulture.Name}' is not part of this locale table collection.",
            nameof(placeholderReferenceCulture));
    }

    private static void SortIssues(List<JsonLocalizationIssue> issues) {
        issues.Sort(static (left, right) => {
            var kind = left.Kind.CompareTo(right.Kind);
            if (kind != 0) {
                return kind;
            }

            var entryKey = string.CompareOrdinal(left.EntryKey, right.EntryKey);
            if (entryKey != 0) {
                return entryKey;
            }

            return string.Compare(left.Culture.Name, right.Culture.Name, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static void CollectMissingTranslations(
        JsonLocaleTableCollection collection,
        HashSet<string> entryKeys,
        List<JsonLocalizationIssue> issues) {
        foreach (var culture in collection.GetCultures()) {
            foreach (var entryKey in entryKeys) {
                if (collection.GetTranslation(culture, entryKey).Length == 0) {
                    issues.Add(new JsonLocalizationIssue(
                        JsonLocalizationIssueKind.MissingTranslation,
                        entryKey,
                        culture,
                        $"The translation for '{entryKey}' in '{culture.Name}' is missing or empty."));
                }
            }
        }
    }

    private static void CollectInvalidFormatStrings(
        JsonLocaleTableCollection collection,
        HashSet<string> entryKeys,
        List<JsonLocalizationIssue> issues) {
        foreach (var culture in collection.GetCultures()) {
            foreach (var entryKey in entryKeys) {
                var translation = collection.GetTranslation(culture, entryKey);
                if (translation.Length == 0 || FormatPlaceholderIndices.TryCollect(translation, out _)) {
                    continue;
                }

                issues.Add(new JsonLocalizationIssue(
                    JsonLocalizationIssueKind.InvalidFormatString,
                    entryKey,
                    culture,
                    $"The translation for '{entryKey}' in '{culture.Name}' is not a valid composite format string."));
            }
        }
    }

    private static void CollectPlaceholderMismatches(
        JsonLocaleTableCollection collection,
        HashSet<string> entryKeys,
        CultureInfo placeholderReferenceCulture,
        List<JsonLocalizationIssue> issues) {
        foreach (var entryKey in entryKeys) {
            var referenceTranslation = collection.GetTranslation(placeholderReferenceCulture, entryKey);
            if (referenceTranslation.Length == 0) {
                continue;
            }

            if (!FormatPlaceholderIndices.TryCollect(referenceTranslation, out var referenceIndices)) {
                continue;
            }

            foreach (var culture in collection.GetCultures()) {
                if (string.Equals(culture.Name, placeholderReferenceCulture.Name, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                var translation = collection.GetTranslation(culture, entryKey);
                if (translation.Length == 0) {
                    continue;
                }

                if (!FormatPlaceholderIndices.TryCollect(translation, out var indices) || indices.SetEquals(referenceIndices)) {
                    continue;
                }

                issues.Add(new JsonLocalizationIssue(
                    JsonLocalizationIssueKind.PlaceholderMismatch,
                    entryKey,
                    culture,
                    $"The placeholders for '{entryKey}' in '{culture.Name}' do not match '{placeholderReferenceCulture.Name}'."));
            }
        }
    }

    private static void CollectUnexpectedTranslationKeys(
        JsonLocaleTableCollection collection,
        HashSet<string> entryKeys,
        List<JsonLocalizationIssue> issues) {
        foreach (var culture in collection.GetCultures()) {
            foreach (var translationKey in collection.GetTranslationKeys(culture)) {
                if (entryKeys.Contains(translationKey)) {
                    continue;
                }

                issues.Add(new JsonLocalizationIssue(
                    JsonLocalizationIssueKind.UnexpectedTranslationKey,
                    translationKey,
                    culture,
                    $"The translation key '{translationKey}' in '{culture.Name}' is not present in the keys document."));
            }
        }
    }
}
