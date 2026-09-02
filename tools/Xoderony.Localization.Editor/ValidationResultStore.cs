using System;
using System.Collections.Generic;
using Xoderony.Localization.Json;

namespace Xoderony.Localization.Editor;

internal interface IValidationResults {

    IReadOnlyList<JsonLocalizationIssue>? Issues { get; }
}

internal sealed class ValidationResultStore : IValidationResults {

    private readonly List<JsonLocalizationIssue> _issues = [];
    private bool _hasProject;

    public IReadOnlyList<JsonLocalizationIssue>? Issues => _hasProject ? _issues : null;

    public void ReplaceAll(IReadOnlyList<JsonLocalizationIssue> issues) {
        _issues.Clear();
        for (var index = 0; index < issues.Count; index++) {
            _issues.Add(issues[index]);
        }

        _hasProject = true;
    }

    public void ReplaceEntry(string entryKey, IReadOnlyList<JsonLocalizationIssue> issues) {
        _issues.RemoveAll(issue => string.Equals(issue.EntryKey, entryKey, StringComparison.Ordinal));
        for (var index = 0; index < issues.Count; index++) {
            _issues.Add(issues[index]);
        }

        SortIssues();
    }

    public void Clear() {
        _issues.Clear();
        _hasProject = false;
    }

    private void SortIssues() {
        _issues.Sort(static (left, right) => {
            var kind = left.Kind.CompareTo(right.Kind);
            if (kind != 0) {
                return kind;
            }

            var key = string.CompareOrdinal(left.EntryKey, right.EntryKey);
            if (key != 0) {
                return key;
            }

            return string.Compare(left.Culture.Name, right.Culture.Name, StringComparison.OrdinalIgnoreCase);
        });
    }
}
