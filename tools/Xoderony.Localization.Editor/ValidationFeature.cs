using System;
using Xoderony;
using Xoderony.Localization.Json;

namespace Xoderony.Localization.Editor;

internal sealed class ValidationFeature : IDisposable {

    private readonly IDelegateSubscriber<ValidationAnalysisRequestedHandler> _analysisRequested;
    private readonly IDelegateSubscriber<ProjectWorkspaceChangedHandler> _projectChanged;
    private readonly ValidationResultStore _resultStore;
    private readonly IDelegateDispatcher<ValidationResultsChangedHandler> _resultsChanged;
    private readonly ProjectWorkspace _workspace;

    public ValidationFeature(ProjectWorkspace workspace, ValidationResultStore resultStore, IDelegateSubscriber<ProjectWorkspaceChangedHandler> projectChanged, IDelegateSubscriber<ValidationAnalysisRequestedHandler> analysisRequested, IDelegateDispatcher<ValidationResultsChangedHandler> resultsChanged) {
        _workspace = workspace;
        _resultStore = resultStore;
        _projectChanged = projectChanged;
        _analysisRequested = analysisRequested;
        _resultsChanged = resultsChanged;
        _projectChanged.Subscribe(WorkspaceChanged);
        _analysisRequested.Subscribe(AnalyzeAll);
    }

    public void Dispose() {
        _projectChanged.Unsubscribe(WorkspaceChanged);
        _analysisRequested.Unsubscribe(AnalyzeAll);
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
