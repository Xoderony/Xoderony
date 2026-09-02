namespace Xoderony.Localization.Editor;

internal delegate void ProjectWorkspaceChangedHandler(ProjectWorkspaceChange change);

internal delegate void ValidationResultsChangedHandler();

internal readonly record struct ProjectWorkspaceChange(ProjectWorkspaceChangeKind Kind, string? EntryKey = null);
