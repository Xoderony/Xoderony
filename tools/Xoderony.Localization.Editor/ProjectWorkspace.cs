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
    private JsonLocalizationDirectory? _directory;
    private CultureInfo? _placeholderReferenceCulture;

    public string? DirectoryPath { get; private set; }

    public CultureInfo? PlaceholderReferenceCulture => _placeholderReferenceCulture;

    public JsonLocalizationDirectory? Directory => _directory;

    public ProjectWorkspace(IDelegateDispatcher<ProjectWorkspaceChangedHandler> changed) {
        _changed = changed;
    }

    public void OpenDirectory(string directoryPath, string? preferredReferenceCultureName) {
        var directory = JsonLocalizationDirectory.LoadDirectory(directoryPath);
        _directory = directory;
        DirectoryPath = directoryPath;
        _placeholderReferenceCulture = ResolveReferenceCulture(directory, preferredReferenceCultureName);
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.ProjectOpened));
    }

    public void Save() {
        Debug.Assert(_directory is not null);
        _directory.Save();
    }

    public void ApplyStructureChange(Action<JsonLocalizationDirectory> change) {
        Debug.Assert(_directory is not null);
        change(_directory);
        _placeholderReferenceCulture ??= ResolveReferenceCulture(_directory, preferredCultureName: null);
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.StructureChanged));
    }

    public void SetTranslation(CultureInfo culture, string entryKey, string translation) {
        Debug.Assert(_directory is not null);
        _directory.SetTranslation(culture, entryKey, translation);
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.TranslationChanged, entryKey));
    }

    public bool SetPlaceholderReferenceCulture(CultureInfo culture) {
        Debug.Assert(_directory is not null);
        CultureInfo? matchingCulture = null;
        foreach (var candidate in _directory.GetCultures()) {
            if (string.Equals(candidate.Name, culture.Name, StringComparison.OrdinalIgnoreCase)) {
                matchingCulture = candidate;
                break;
            }
        }

        if (matchingCulture is null) {
            throw new ArgumentException($"The culture '{culture.Name}' is not part of this localization directory.", nameof(culture));
        }

        if (_placeholderReferenceCulture is not null
            && string.Equals(_placeholderReferenceCulture.Name, matchingCulture.Name, StringComparison.OrdinalIgnoreCase)) {
            return false;
        }

        _placeholderReferenceCulture = matchingCulture;
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.PlaceholderReferenceCultureChanged));
        return true;
    }

    private static CultureInfo? ResolveReferenceCulture(JsonLocalizationDirectory directory, string? preferredCultureName) {
        CultureInfo? firstCulture = null;
        foreach (var culture in directory.GetCultures()) {
            firstCulture ??= culture;
            if (string.Equals(culture.Name, preferredCultureName, StringComparison.OrdinalIgnoreCase)) {
                return culture;
            }
        }

        return firstCulture;
    }
}
