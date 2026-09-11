using System.Globalization;

namespace Xoderony.Localization.Json;

public sealed class JsonLocalizationIssue(JsonLocalizationIssueKind kind, string entryKey, CultureInfo culture, string message) {
    public JsonLocalizationIssueKind Kind { get; } = kind;

    public string EntryKey { get; } = entryKey;

    public CultureInfo Culture { get; } = culture;

    public string Message { get; } = message;
}
