using System;
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

    // 编辑操作要求已打开项目，在回调或写入状态前拒绝未打开状态。
    private JsonLocalizationDirectory OpenProject => _directory ?? throw new InvalidOperationException("No project is open.");

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
        var directory = OpenProject;
        directory.Save();
    }

    public void ApplyStructureChange(Action<JsonLocalizationDirectory> change) {
        var directory = OpenProject;
        change(directory);
        _placeholderReferenceCulture ??= ResolveReferenceCulture(directory, preferredCultureName: null);
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.StructureChanged));
    }

    public void SetTranslation(CultureInfo culture, string entryKey, string translation) {
        var directory = OpenProject;
        directory.SetTranslation(culture, entryKey, translation);
        _changed.Handlers?.Invoke(new ProjectWorkspaceChange(ProjectWorkspaceChangeKind.TranslationChanged, entryKey));
    }

    public bool SetPlaceholderReferenceCulture(CultureInfo culture) {
        var directory = OpenProject;
        CultureInfo? matchingCulture = null;
        foreach (var candidate in directory.GetCultures()) {
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
