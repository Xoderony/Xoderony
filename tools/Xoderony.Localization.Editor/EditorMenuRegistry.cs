using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Xoderony.Localization.Editor;

/// <summary>
/// Snapshot passed to menu items when a context menu opens.
/// </summary>
internal readonly struct EditorMenuContext {

    public EditorMenuContext(bool hasTables, bool hasLocales, bool hasSelection, bool clipboardHasData) {
        HasTables = hasTables;
        HasLocales = hasLocales;
        HasSelection = hasSelection;
        ClipboardHasData = clipboardHasData;
    }

    public bool HasTables { get; }
    public bool HasLocales { get; }
    public bool HasSelection { get; }
    public bool ClipboardHasData { get; }

    public bool CanPaste => HasLocales && ClipboardHasData;
}

/// <summary>
/// Marks a concrete <see cref="IEditorMenuItem"/> for assembly discovery.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class EditorMenuAttribute : Attribute {
}

/// <summary>
/// A context-menu row discovered via <see cref="EditorMenuAttribute"/> and constructed with DI.
/// </summary>
internal interface IEditorMenuItem {

    int Order { get; }

    bool BeginGroup { get; }

    string Gesture { get; }

    bool IsAvailable(EditorMenuContext context);

    string GetHeader(EditorMenuContext context);

    void Execute();
}

/// <summary>
/// Discovers <see cref="EditorMenuAttribute"/> types, creates them through
/// <see cref="ActivatorUtilities"/> / <see cref="IServiceProvider"/>, and builds the menu on demand.
/// </summary>
internal sealed class EditorMenuRegistry {

    private readonly List<IEditorMenuItem> _items = [];

    public EditorMenuRegistry Add(IEditorMenuItem item) {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
        _items.Sort(static (a, b) => a.Order.CompareTo(b.Order));
        return this;
    }

    /// <summary>
    /// Scans <paramref name="assembly"/> for non-abstract [<see cref="EditorMenuAttribute"/>] types
    /// implementing <see cref="IEditorMenuItem"/>, constructs each with
    /// <see cref="ActivatorUtilities.CreateInstance(IServiceProvider, Type, object[])"/>,
    /// and registers them ordered by <see cref="IEditorMenuItem.Order"/>.
    /// </summary>
    public EditorMenuRegistry AddFromAssembly(Assembly assembly, IServiceProvider services) {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(services);

        foreach (var type in assembly.GetTypes()) {
            if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition) {
                continue;
            }

            if (type.GetCustomAttribute<EditorMenuAttribute>() is null) {
                continue;
            }

            if (!typeof(IEditorMenuItem).IsAssignableFrom(type)) {
                throw new InvalidOperationException(
                    $"Type '{type.FullName}' has [{nameof(EditorMenuAttribute)}] but does not implement {nameof(IEditorMenuItem)}.");
            }

            _items.Add((IEditorMenuItem)ActivatorUtilities.CreateInstance(services, type));
        }

        _items.Sort(static (a, b) => a.Order.CompareTo(b.Order));
        return this;
    }

    public void Populate(ItemCollection items, EditorMenuContext context) {
        ArgumentNullException.ThrowIfNull(items);
        items.Clear();

        var addedAny = false;
        foreach (var contribution in _items) {
            if (!contribution.IsAvailable(context)) {
                continue;
            }

            if (contribution.BeginGroup && addedAny) {
                items.Add(new Separator());
            }

            var item = new MenuItem {
                Header = contribution.GetHeader(context),
            };
            if (contribution.Gesture.Length > 0) {
                item.InputGestureText = contribution.Gesture;
            }

            var execute = contribution.Execute;
            item.Click += (_, _) => execute();
            items.Add(item);
            addedAny = true;
        }
    }
}
