using System;
using Xoderony;
using Xoderony.Localization.Json;

namespace Xoderony.Localization.Editor;

internal sealed class ValidationFeature : IDisposable {

    private readonly IDelegateSubscriber<ProjectWorkspaceChangedHandler> _projectChanged;
    private readonly ValidationResultStore _resultStore;
    private readonly IDelegateDispatcher<ValidationResultsChangedHandler> _resultsChanged;
    private readonly ProjectWorkspace _workspace;

    public ValidationFeature(ProjectWorkspace workspace, ValidationResultStore resultStore, IDelegateSubscriber<ProjectWorkspaceChangedHandler> projectChanged, IDelegateDispatcher<ValidationResultsChangedHandler> resultsChanged) {
        _workspace = workspace;
        _resultStore = resultStore;
        _projectChanged = projectChanged;
        _resultsChanged = resultsChanged;
        _projectChanged.Subscribe(WorkspaceChanged);
    }

    public void Dispose() {
        _projectChanged.Unsubscribe(WorkspaceChanged);
    }

    private void AnalyzeAll() {
        var tables = _workspace.Tables;
        var referenceCulture = _workspace.PlaceholderReferenceCulture;
        if (tables is null) {
            _resultStore.Clear();
        } else if (referenceCulture is null) {
            _resultStore.ReplaceAll(Array.Empty<JsonLocalizationIssue>());
        } else {
            _resultStore.ReplaceAll(JsonLocalizationValidation.Validate(tables, referenceCulture));
        }

        _resultsChanged.Handlers?.Invoke();
    }

    private void WorkspaceChanged(ProjectWorkspaceChange change) {
        if (change.Kind != ProjectWorkspaceChangeKind.TranslationChanged || change.EntryKey is null) {
            AnalyzeAll();
            return;
        }

        AnalyzeEntry(change.EntryKey);
    }

    private void AnalyzeEntry(string entryKey) {
        var tables = _workspace.Tables;
        var referenceCulture = _workspace.PlaceholderReferenceCulture;
        if (tables is null || referenceCulture is null) {
            AnalyzeAll();
            return;
        }

        _resultStore.ReplaceEntry(entryKey, JsonLocalizationValidation.ValidateEntry(tables, referenceCulture, entryKey));
        _resultsChanged.Handlers?.Invoke();
    }
}
