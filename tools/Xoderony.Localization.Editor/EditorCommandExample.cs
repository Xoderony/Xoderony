using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Xoderony.Localization.Editor;

/// <summary>
/// Demonstrates official DI + assembly discovery for context-menu items.
/// Not wired to <see cref="MainWindow"/> yet.
/// </summary>
internal sealed class EditorCommandExample {

    public EditorCommandExample(EditorLocalizer localizer) {
        var services = new ServiceCollection();
        services.AddSingleton(localizer);
        services.AddSingleton(this);
        var provider = services.BuildServiceProvider();

        Menu = new EditorMenuRegistry()
            .AddFromAssembly(Assembly.GetExecutingAssembly(), provider);

        Save = new EditorCommand(() => { /* persist… */ }, gesture: "Ctrl+S", locKey: "toolbar.save");
        OpenDirectory = new EditorCommand(() => { /* dialog… */ }, locKey: "toolbar.open");
    }

    public EditorMenuRegistry Menu { get; }
    public EditorCommand Save { get; }
    public EditorCommand OpenDirectory { get; }

    internal void RequestAddLocale() { }
    internal void RequestAddGroup() { }
    internal void RequestAddEntry() { }
    internal void RequestRename() { }
    internal void RequestMove() { }
    internal void RequestCopyTo() { }
    internal void RequestCopy() { }
    internal void RequestCut() { }
    internal void RequestPaste() { }
    internal void RequestRemove() { }
}

[EditorMenu]
file sealed class AddLocaleMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 10;
    public bool BeginGroup => false;
    public string Gesture => string.Empty;
    public bool IsAvailable(EditorMenuContext context) => context.HasTables;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.AddLocale];
    public void Execute() => host.RequestAddLocale();
}

[EditorMenu]
file sealed class AddGroupMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 20;
    public bool BeginGroup => false;
    public string Gesture => string.Empty;
    public bool IsAvailable(EditorMenuContext context) => context.HasLocales;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.AddGroup];
    public void Execute() => host.RequestAddGroup();
}

[EditorMenu]
file sealed class AddEntryMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 30;
    public bool BeginGroup => false;
    public string Gesture => string.Empty;
    public bool IsAvailable(EditorMenuContext context) => context.HasLocales;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.AddEntry];
    public void Execute() => host.RequestAddEntry();
}

[EditorMenu]
file sealed class RenameMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 40;
    public bool BeginGroup => true;
    public string Gesture => "F2";
    public bool IsAvailable(EditorMenuContext context) => context.HasSelection;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.Rename];
    public void Execute() => host.RequestRename();
}

[EditorMenu]
file sealed class MoveMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 50;
    public bool BeginGroup => false;
    public string Gesture => string.Empty;
    public bool IsAvailable(EditorMenuContext context) => context.HasSelection;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.Move];
    public void Execute() => host.RequestMove();
}

[EditorMenu]
file sealed class CopyToMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 60;
    public bool BeginGroup => false;
    public string Gesture => string.Empty;
    public bool IsAvailable(EditorMenuContext context) => context.HasSelection;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.CopyTo];
    public void Execute() => host.RequestCopyTo();
}

[EditorMenu]
file sealed class CopyMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 70;
    public bool BeginGroup => true;
    public string Gesture => "Ctrl+C";
    public bool IsAvailable(EditorMenuContext context) => context.HasSelection;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.Copy];
    public void Execute() => host.RequestCopy();
}

[EditorMenu]
file sealed class CutMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 80;
    public bool BeginGroup => false;
    public string Gesture => "Ctrl+X";
    public bool IsAvailable(EditorMenuContext context) => context.HasSelection;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.Cut];
    public void Execute() => host.RequestCut();
}

[EditorMenu]
file sealed class PasteMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 90;
    public bool BeginGroup => false;
    public string Gesture => "Ctrl+V";
    public bool IsAvailable(EditorMenuContext context) => context.CanPaste;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.Paste];
    public void Execute() => host.RequestPaste();
}

[EditorMenu]
file sealed class RemoveMenuItem(EditorLocalizer localizer, EditorCommandExample host) : IEditorMenuItem {
    public int Order => 100;
    public bool BeginGroup => true;
    public string Gesture => "Delete";
    public bool IsAvailable(EditorMenuContext context) => context.HasSelection;
    public string GetHeader(EditorMenuContext context) => localizer[EditorStringKeys.Context.Remove];
    public void Execute() => host.RequestRemove();
}
