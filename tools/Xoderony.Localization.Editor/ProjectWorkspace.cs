using System;
using System.Diagnostics;
using System.Globalization;
using Xoderony;
using Xoderony.Localization.Json;

namespace Xoderony.Localization.Editor;

internal enum ProjectWorkspaceChangeKind {
    ProjectOpened,
    StructureChanged,
    TranslationChanged,
    PlaceholderReferenceCultureChanged
}

internal sealed class ProjectWorkspace {

    private readonly IDelegateDispatcher<ProjectWorkspaceChangedHandler> _changed;
    private JsonLocaleTableCollection? _tables;
    private CultureInfo? _placeholderReferenceCulture;

    public string? DirectoryPath { get; private set; }

    public CultureInfo? PlaceholderReferenceCulture => _placeholderReferenceCulture;

    public JsonLocaleTableCollection? Tables => _tables;

    public ProjectWorkspace(IDelegateDispatcher<ProjectWorkspaceChangedHandler> changed) {
        _changed = changed;
    }

    public void OpenDirectory(string directoryPath, string? preferredReferenceCultureName) {
        var tables = JsonLocaleTableCollection.LoadDirectory(directoryPath);
        _tables = tables;
        DirectoryPath = directoryPath;
        _placeholderReferenceCulture = ResolveReferenceCulture(tables, preferredReferenceCultureName);
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.ProjectOpened));
    }

    public void Save() {
        Debug.Assert(_tables is not null);
        _tables.Save();
    }

    public void ApplyStructureChange(Action<JsonLocaleTableCollection> change) {
        Debug.Assert(_tables is not null);
        change(_tables);
        _placeholderReferenceCulture ??= ResolveReferenceCulture(_tables, preferredCultureName: null);
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.StructureChanged));
    }

    public void SetTranslation(CultureInfo culture, string entryKey, string translation) {
        Debug.Assert(_tables is not null);
        _tables.SetTranslation(culture, entryKey, translation);
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.TranslationChanged, entryKey));
    }

    public bool SetPlaceholderReferenceCulture(CultureInfo culture) {
        Debug.Assert(_tables is not null);
        CultureInfo? matchingCulture = null;
        foreach (var candidate in _tables.GetCultures()) {
            if (string.Equals(candidate.Name, culture.Name, StringComparison.OrdinalIgnoreCase)) {
                matchingCulture = candidate;
                break;
            }
        }

        if (matchingCulture is null) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of this locale table collection.", nameof(culture));
        }

        if (_placeholderReferenceCulture is not null
            && string.Equals(_placeholderReferenceCulture.Name, matchingCulture.Name, StringComparison.OrdinalIgnoreCase)) {
            return false;
        }

        _placeholderReferenceCulture = matchingCulture;
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.PlaceholderReferenceCultureChanged));
        return true;
    }

    private static CultureInfo? ResolveReferenceCulture(JsonLocaleTableCollection tables, string? preferredCultureName) {
        CultureInfo? firstCulture = null;
        foreach (var culture in tables.GetCultures()) {
            firstCulture ??= culture;
            if (string.Equals(culture.Name, preferredCultureName, StringComparison.OrdinalIgnoreCase)) {
                return culture;
            }
        }

        return firstCulture;
    }
}
