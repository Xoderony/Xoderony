using System.Globalization;

namespace Xoderony.Localization.Json;

public sealed class JsonLocalizationIssue {

    public JsonLocalizationIssue(JsonLocalizationIssueKind kind, string entryKey, CultureInfo culture, string message) {
        Kind = kind;
        EntryKey = entryKey;
        Culture = culture;
        Message = message;
    }

    public JsonLocalizationIssueKind Kind { get; }

    public string EntryKey { get; }

    public CultureInfo Culture { get; }

    public string Message { get; }
}
