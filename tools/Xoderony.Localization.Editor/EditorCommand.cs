using System;
using System.Windows.Input;

namespace Xoderony.Localization.Editor;

/// <summary>
/// Minimal <see cref="ICommand"/>: always executable. Use for toolbar / menu / key bindings.
/// Guard invalid operations inside the execute action if needed.
/// </summary>
internal sealed class EditorCommand : ICommand {

    private readonly Action _execute;

    public EditorCommand(Action execute, string? gesture = null, string? locKey = null) {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        Gesture = gesture ?? string.Empty;
        LocKey = locKey ?? string.Empty;
    }

    public string Gesture { get; }

    public string LocKey { get; }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _execute();

    public event EventHandler? CanExecuteChanged {
        add { }
        remove { }
    }
}
