using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Xoderony.Localization.Json;

public static class JsonLocalizationValidation {

    public static IReadOnlyList<JsonLocalizationIssue> Validate(JsonLocaleTableCollection collection, CultureInfo placeholderReferenceCulture) {
        Debug.Assert(collection is not null);
        Debug.Assert(placeholderReferenceCulture is not null);

        var referenceCultureFound = false;
        foreach (var culture in collection.GetCultures()) {
            if (string.Equals(culture.Name, placeholderReferenceCulture.Name, StringComparison.OrdinalIgnoreCase)) {
                referenceCultureFound = true;
                placeholderReferenceCulture = culture;
                break;
            }
        }

        if (!referenceCultureFound) {
            throw new ArgumentException(
                $"The culture '{placeholderReferenceCulture.Name}' is not part of this locale table collection.",
                nameof(placeholderReferenceCulture));
        }

        var entryKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entryKey in collection.GetEntryKeys()) {
            entryKeys.Add(entryKey);
        }

        var issues = new List<JsonLocalizationIssue>();
        CollectMissingTranslations(collection, entryKeys, issues);
        CollectPlaceholderMismatches(collection, entryKeys, placeholderReferenceCulture, issues);
        CollectUnexpectedTranslationKeys(collection, entryKeys, issues);

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

        return issues;
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

            var referenceIndices = FormatPlaceholderIndices.Collect(referenceTranslation);
            foreach (var culture in collection.GetCultures()) {
                if (string.Equals(culture.Name, placeholderReferenceCulture.Name, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                var translation = collection.GetTranslation(culture, entryKey);
                if (translation.Length == 0) {
                    continue;
                }

                var indices = FormatPlaceholderIndices.Collect(translation);
                if (indices.SetEquals(referenceIndices)) {
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
