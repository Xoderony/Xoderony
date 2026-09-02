using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xoderony;
using Xoderony.Localization.Json;
using Xoderony.Localization.Tooling;

namespace Xoderony.Localization.Editor;

public partial class MainWindow : Window {

    private readonly Dictionary<string, DataGridColumn> _cultureNameToColumn = new(StringComparer.OrdinalIgnoreCase);
    private readonly EditorLocalizer _localizer;
    private readonly EditorPreferences _preferences;
    private readonly IDelegateDispatcher<ValidationAnalysisRequestedHandler> _validationAnalysisRequested;
    private readonly IValidationResults _validationResults;
    private readonly IDelegateSubscriber<ValidationResultsChangedHandler> _validationResultsChanged;
    private readonly ProjectWorkspace _workspace;
    private string _currentGroupKey = string.Empty;
    private DataGridColumn? _nameColumn;
    private JsonLocalizationIssueKind? _validationKindFilter;
    private string? _validationCultureFilter;
    private bool _rebuildingValidationFilters;

    internal MainWindow(EditorPreferences preferences, EditorLocalizer localizer, ProjectWorkspace workspace, IValidationResults validationResults, IDelegateSubscriber<ValidationResultsChangedHandler> validationResultsChanged, IDelegateDispatcher<ValidationAnalysisRequestedHandler> validationAnalysisRequested) {
        _preferences = preferences;
        _localizer = localizer;
        _workspace = workspace;
        _validationResults = validationResults;
        _validationResultsChanged = validationResultsChanged;
        _validationAnalysisRequested = validationAnalysisRequested;
        var theme = _preferences.Get(AppearancePreferenceKeys.Theme, EditorTheme.Light);
        EditorThemeManager.SetTheme(theme);
        InitializeComponent();
        _validationResultsChanged.Subscribe(ValidationResultsChanged);
        RestoreWindowSize();
        DataContext = _localizer;
        ThemeComboBox.SelectedIndex = theme == EditorTheme.Dark ? 1 : 0;
    }

    protected override void OnClosed(EventArgs e) {
        _validationResultsChanged.Unsubscribe(ValidationResultsChanged);
        base.OnClosed(e);
    }

    private void OnLoaded(object sender, RoutedEventArgs e) {
        Loaded -= OnLoaded;
        var directoryPath = _preferences.Get<string?>(ProjectPreferenceKeys.LastDirectory, null);
        if (directoryPath is not null && Directory.Exists(directoryPath)) {
            TryOpenDirectory(directoryPath, rememberDirectory: false);
        }
    }

    private void OnClosing(object? sender, CancelEventArgs e) {
        if (!ConfirmDiscardChanges()) {
            e.Cancel = true;
            return;
        }

        var restoreBounds = RestoreBounds;
        _preferences.Set(WindowPreferenceKeys.Width, restoreBounds.Width);
        _preferences.Set(WindowPreferenceKeys.Height, restoreBounds.Height);
        SavePreferences();
    }

    private void OpenDirectory(object sender, RoutedEventArgs e) {
        if (!ConfirmDiscardChanges()) {
            return;
        }

        var dialog = new OpenFolderDialog { Title = _localizer[EditorStringKeys.Dialog.OpenDirectoryTitle] };
        if (dialog.ShowDialog(this) != true) {
            return;
        }

        TryOpenDirectory(dialog.FolderName, rememberDirectory: true);
    }

    private bool TryOpenDirectory(string directoryPath, bool rememberDirectory) {
        try {
            _validationKindFilter = null;
            _validationCultureFilter = null;
            var referenceCultureName = _preferences.Get<string?>(ValidationPreferenceKeys.PlaceholderReferenceCulture, null);
            _workspace.OpenDirectory(directoryPath, referenceCultureName);
            _currentGroupKey = string.Empty;
            RebuildBreadcrumb();
            RebuildColumns();
            if (SearchTextBox.Text.Length > 0) {
                SearchTextBox.Clear();
            } else {
                RefreshRows(previousState: new GridState(null, null, 0, 0));
            }

            EmptyState.Visibility = Visibility.Collapsed;
            TableGrid.Visibility = Visibility.Visible;

            var lastDirectory = _preferences.Get<string?>(ProjectPreferenceKeys.LastDirectory, null);
            if (rememberDirectory && !string.Equals(lastDirectory, directoryPath, StringComparison.OrdinalIgnoreCase)) {
                _preferences.Set(ProjectPreferenceKeys.LastDirectory, directoryPath);
                SavePreferences();
            }

            return true;
        } catch (Exception exception) when (exception is ArgumentException or CultureNotFoundException or IOException or InvalidDataException) {
            ShowError(exception.Message);
            return false;
        }
    }

    private void Save(object sender, RoutedEventArgs e) {
        if (_workspace.Tables is null) {
            return;
        }

        try {
            _workspace.Save();
        } catch (IOException exception) {
            ShowError(exception.Message);
        }
    }

    private void GenerateKeys(object sender, RoutedEventArgs e) {
        var tables = _workspace.Tables;
        if (tables is null) {
            return;
        }

        var namespaceName = Prompt(
            _localizer[EditorStringKeys.Dialog.GenerateKeysNamespaceTitle],
            _localizer[EditorStringKeys.Dialog.GenerateKeysNamespaceMessage],
            "Xoderony.Localization");
        if (namespaceName is null) {
            return;
        }

        var typeName = Prompt(
            _localizer[EditorStringKeys.Dialog.GenerateKeysTypeTitle],
            _localizer[EditorStringKeys.Dialog.GenerateKeysTypeMessage],
            "StringTableKeys");
        if (typeName is null) {
            return;
        }

        string source;
        try {
            source = StringTableKeyGenerator.Generate(tables.GetEntryKeys(), namespaceName, typeName);
        } catch (ArgumentException exception) {
            ShowError(exception.Message);
            return;
        }

        var dialog = new SaveFileDialog {
            Title = _localizer[EditorStringKeys.Dialog.GenerateKeysOutputTitle],
            Filter = _localizer[EditorStringKeys.Dialog.GenerateKeysFileFilter],
            FileName = $"{typeName}.g.cs",
            DefaultExt = ".cs",
            AddExtension = true
        };
        if (dialog.ShowDialog(this) != true) {
            return;
        }

        try {
            File.WriteAllText(dialog.FileName, source, new UTF8Encoding(false));
        } catch (IOException exception) {
            ShowError(exception.Message);
        }
    }

    private bool ConfirmDiscardChanges() {
        var tables = _workspace.Tables;
        if (tables is null || !tables.IsDirty) {
            return true;
        }

        var result = MessageBox.Show(
            this,
            _localizer[EditorStringKeys.Dialog.UnsavedMessage],
            _localizer[EditorStringKeys.Dialog.UnsavedTitle],
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Warning
        );
        if (result == MessageBoxResult.Cancel) {
            return false;
        }

        if (result == MessageBoxResult.Yes) {
            Save(this, new RoutedEventArgs());
            return !tables.IsDirty;
        }

        return true;
    }

    private void SavePreferences() {
        try {
            _preferences.Save();
        } catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) {
            ShowError(exception.Message);
        }
    }

    private void RestoreWindowSize() {
        var workArea = SystemParameters.WorkArea;
        var maxWidth = Math.Max(MinWidth, workArea.Width);
        var maxHeight = Math.Max(MinHeight, workArea.Height);

        if (_preferences.Get<double?>(WindowPreferenceKeys.Width, null) is double width && double.IsFinite(width)) {
            Width = Math.Clamp(width, MinWidth, maxWidth);
        }

        if (_preferences.Get<double?>(WindowPreferenceKeys.Height, null) is double height && double.IsFinite(height)) {
            Height = Math.Clamp(height, MinHeight, maxHeight);
        }
    }

    private void LanguageChanged(object sender, SelectionChangedEventArgs e) {
        if (LanguageComboBox.SelectedItem is CultureInfo culture && _localizer.SetCulture(culture)) {
            _preferences.Set(AppearancePreferenceKeys.UiCulture, culture.Name);
            SavePreferences();
            RebuildValidationFilters();
            RefreshValidationPresentation();
        }
    }

    private void ThemeChanged(object sender, SelectionChangedEventArgs e) {
        if (ThemeComboBox.SelectedItem is not ComboBoxItem { Tag: EditorTheme theme }
            || _preferences.Get(AppearancePreferenceKeys.Theme, EditorTheme.Light) == theme) {
            return;
        }

        EditorThemeManager.SetTheme(theme);
        _preferences.Set(AppearancePreferenceKeys.Theme, theme);
        SavePreferences();
    }

    private void NavigateToGroup(string groupKey) {
        _currentGroupKey = groupKey;
        RebuildBreadcrumb();
        if (SearchTextBox.Text.Length > 0) {
            SearchTextBox.Clear();
        } else {
            RefreshRows(previousState: new GridState(null, null, 0, 0));
        }
    }

    private void RebuildBreadcrumb() {
        BreadcrumbPanel.Children.Clear();
        var directoryPath = _workspace.DirectoryPath;
        if (directoryPath is null) {
            PathPlaceholderText.Visibility = Visibility.Visible;
            BreadcrumbScrollViewer.Visibility = Visibility.Collapsed;
            return;
        }

        PathPlaceholderText.Visibility = Visibility.Collapsed;
        BreadcrumbScrollViewer.Visibility = Visibility.Visible;
        var directoryName = Path.GetFileName(Path.TrimEndingDirectorySeparator(directoryPath));
        AddBreadcrumb(directoryName.Length == 0 ? directoryPath : directoryName, string.Empty, directoryPath, isRoot: true);

        var key = string.Empty;
        foreach (var localKey in _currentGroupKey.Split('.', StringSplitOptions.RemoveEmptyEntries)) {
            key = JsonKeyNode.CombineKey(key, localKey);
            AddBreadcrumb(localKey, key, key);
        }

        void AddBreadcrumb(string label, string groupKey, string toolTip, bool isRoot = false) {
            if (!isRoot) {
                BreadcrumbPanel.Children.Add(new TextBlock {
                    Text = "\u203A",
                    Style = (Style)FindResource("BreadcrumbSeparatorTextStyle")
                });
            }

            var button = new Button {
                Content = label,
                ToolTip = toolTip,
                Style = (Style)FindResource("BreadcrumbButtonStyle")
            };
            button.Click += (_, _) => NavigateToGroup(groupKey);
            BreadcrumbPanel.Children.Add(button);
        }
    }

    private void SearchChanged(object sender, TextChangedEventArgs e) {
        if (!IsInitialized) {
            return;
        }

        RefreshRows(previousState: new GridState(null, null, 0, 0));
    }

    private void ClearSearch(object sender, RoutedEventArgs e) {
        SearchTextBox.Clear();
        SearchTextBox.Focus();
    }

    private void OpenProjectMenu(object sender, RoutedEventArgs e) {
        if (ProjectMenuButton.ContextMenu is not ContextMenu menu) {
            return;
        }

        menu.PlacementTarget = ProjectMenuButton;
        menu.Placement = PlacementMode.Bottom;
        menu.IsOpen = true;
    }

    private void PopulateProjectMenu(object sender, RoutedEventArgs e) {
        if (sender is not ContextMenu menu) {
            return;
        }

        var items = menu.Items;
        items.Clear();
        AddMenuItem(items, _localizer[EditorStringKeys.Toolbar.Open], OpenDirectory, "Ctrl+O");

        var tables = _workspace.Tables;
        if (tables?.IsDirty == true) {
            AddMenuItem(items, _localizer[EditorStringKeys.Toolbar.Save], Save, "Ctrl+S");
        }

        if (tables is null) {
            return;
        }

        items.Add(new Separator());
        AddMenuItem(items, _localizer[EditorStringKeys.Context.AddLocale], AddLocale);
        if (tables.CultureCount > 0) {
            var referenceCulture = _workspace.PlaceholderReferenceCulture;
            Debug.Assert(referenceCulture is not null);
            AddMenuItem(items, _localizer[EditorStringKeys.Validation.ReferenceLocale, referenceCulture.Name], ChoosePlaceholderReferenceCulture);
            AddMenuItem(items, _localizer[EditorStringKeys.Validation.Run], RunValidation);
            items.Add(new Separator());
            AddMenuItem(items, _localizer[EditorStringKeys.Toolbar.GenerateKeys], GenerateKeys);
        }
    }

    private void PopulateTableContextMenu(object sender, RoutedEventArgs e) {
        if (sender is not ContextMenu menu) {
            return;
        }

        var items = menu.Items;
        items.Clear();
        if (_workspace.Tables is not { CultureCount: > 0 }) {
            return;
        }

        AddMenuItem(items, _localizer[EditorStringKeys.Context.AddGroup], AddGroup);
        AddMenuItem(items, _localizer[EditorStringKeys.Context.AddEntry], AddEntry);
        if (LocalizationClipboard.ContainsData()) {
            AddMenuItem(items, _localizer[EditorStringKeys.Context.Paste], ClipboardPaste, "Ctrl+V");
        }

        if (!TryGetSelectedNode(out _)) {
            return;
        }

        items.Add(new Separator());
        AddMenuItem(items, _localizer[EditorStringKeys.Context.Rename], Rename, "F2");
        AddMenuItem(items, _localizer[EditorStringKeys.Context.Move], MoveSelection);
        AddMenuItem(items, _localizer[EditorStringKeys.Context.CopyTo], CopyToSelection);
        items.Add(new Separator());
        AddMenuItem(items, _localizer[EditorStringKeys.Context.Copy], ClipboardCopy, "Ctrl+C");
        AddMenuItem(items, _localizer[EditorStringKeys.Context.Cut], ClipboardCut, "Ctrl+X");
        items.Add(new Separator());
        AddMenuItem(items, _localizer[EditorStringKeys.Context.Remove], Remove, "Delete", isDangerous: true);
    }

    private void SelectRowOnRightClick(object sender, MouseButtonEventArgs e) {
        var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
        if (row is null) {
            TableGrid.SelectedItem = null;
            TableGrid.CurrentCell = default;
            return;
        }

        if (!row.IsSelected) {
            TableGrid.SelectedItem = row.Item;
        }

        var cell = FindVisualParent<DataGridCell>(e.OriginalSource as DependencyObject);
        if (cell is not null) {
            TableGrid.CurrentCell = new DataGridCellInfo(row.Item, cell.Column);
        }
    }

    private void HandleCellDoubleClick(object sender, MouseButtonEventArgs e) {
        var cell = FindVisualParent<DataGridCell>(e.OriginalSource as DependencyObject);
        if (cell?.DataContext is not LocalizationRow row) {
            return;
        }

        if (row.IsGroup) {
            NavigateToGroup(row.Node.FullKey);
            e.Handled = true;
            return;
        }

        if (ReferenceEquals(cell.Column, _nameColumn)) {
            return;
        }

        TableGrid.CurrentCell = new DataGridCellInfo(row, cell.Column);
        var culture = GetColumnCulture(cell.Column);
        if (culture is not null) {
            EditTranslation(row, culture);
        }

        e.Handled = true;
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter && ValidationGrid.IsKeyboardFocusWithin) {
            ActivateSelectedValidationIssue();
            e.Handled = true;
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) {
            if (e.Key == Key.O) {
                OpenDirectory(this, new RoutedEventArgs());
                e.Handled = true;
                return;
            }

            if (e.Key == Key.S) {
                Save(this, new RoutedEventArgs());
                e.Handled = true;
                return;
            }

            if (Keyboard.FocusedElement is not TextBox) {
                if (e.Key == Key.C) {
                    ClipboardCopy(this, new RoutedEventArgs());
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.X) {
                    ClipboardCut(this, new RoutedEventArgs());
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.V) {
                    ClipboardPaste(this, new RoutedEventArgs());
                    e.Handled = true;
                    return;
                }
            }
        }

        if (Keyboard.FocusedElement is TextBox) {
            return;
        }

        if (e.Key == Key.Back && _currentGroupKey.Length > 0) {
            NavigateToGroup(JsonKeyNode.GetParentKey(_currentGroupKey));
            e.Handled = true;
        } else if (e.Key == Key.Enter && TableGrid.SelectedItem is LocalizationRow row) {
            if (row.IsGroup) {
                NavigateToGroup(row.Node.FullKey);
                e.Handled = true;
            } else if (TableGrid.CurrentColumn is not null) {
                var culture = GetColumnCulture(TableGrid.CurrentColumn);
                if (culture is not null) {
                    EditTranslation(row, culture);
                    e.Handled = true;
                }
            }
        } else if (e.Key == Key.F2) {
            Rename(this, new RoutedEventArgs());
            e.Handled = true;
        } else if (e.Key == Key.Delete) {
            Remove(this, new RoutedEventArgs());
            e.Handled = true;
        }
    }

    private void AddGroup(object sender, RoutedEventArgs e) {
        if (_workspace.Tables is null) {
            return;
        }

        var localKey = Prompt(_localizer[EditorStringKeys.Dialog.AddGroupTitle], _localizer[EditorStringKeys.Dialog.AddGroupMessage]);
        if (localKey is null) {
            return;
        }

        var key = JsonKeyNode.CombineKey(_currentGroupKey, localKey);
        ChangeStructure(tables => tables.AddGroup(_currentGroupKey, localKey), () => key);
    }

    private void AddEntry(object sender, RoutedEventArgs e) {
        if (_workspace.Tables is null) {
            return;
        }

        var localKey = Prompt(_localizer[EditorStringKeys.Dialog.AddEntryTitle], _localizer[EditorStringKeys.Dialog.AddEntryMessage]);
        if (localKey is null) {
            return;
        }

        var key = JsonKeyNode.CombineKey(_currentGroupKey, localKey);
        ChangeStructure(tables => tables.AddEntry(_currentGroupKey, localKey), () => key);
    }

    private void AddLocale(object sender, RoutedEventArgs e) {
        var directoryPath = _workspace.DirectoryPath;
        if (_workspace.Tables is null || directoryPath is null) {
            return;
        }

        var cultureName = Prompt(_localizer[EditorStringKeys.Dialog.AddLocaleTitle], _localizer[EditorStringKeys.Dialog.AddLocaleMessage]);
        if (cultureName is null) {
            return;
        }

        try {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            var filePath = Path.Combine(directoryPath, $"{culture.Name}.json");
            ChangeStructure(tables => tables.AddLocale(culture, filePath), rebuildColumns: true);
        } catch (CultureNotFoundException exception) {
            ShowError(exception.Message);
        }
    }

    private void Rename(object sender, RoutedEventArgs e) {
        if (!TryGetSelectedNode(out var node)) {
            return;
        }

        var oldKey = node.FullKey;
        var localKey = Prompt(_localizer[EditorStringKeys.Dialog.RenameTitle], _localizer[EditorStringKeys.Dialog.RenameMessage], node.LocalKey);
        if (localKey is null || string.Equals(localKey, node.LocalKey, StringComparison.Ordinal)) {
            return;
        }

        var newKey = JsonKeyNode.CombineKey(JsonKeyNode.GetParentKey(oldKey), localKey);
        ChangeStructure(tables => tables.Rename(oldKey, localKey), () => newKey);
    }

    private void ClipboardCopy(object sender, RoutedEventArgs e) {
        var tables = _workspace.Tables;
        if (tables is null || !TryGetSelectedNode(out var node)) {
            return;
        }

        try {
            LocalizationClipboard.Set(tables, node);
        } catch (ExternalException exception) {
            ShowError(exception.Message);
        }
    }

    private void ClipboardCut(object sender, RoutedEventArgs e) {
        var tables = _workspace.Tables;
        if (tables is null || !TryGetSelectedNode(out var node)) {
            return;
        }

        try {
            LocalizationClipboard.Set(tables, node);
        } catch (ExternalException exception) {
            ShowError(exception.Message);
            return;
        }

        var key = node.FullKey;
        ChangeStructure(tables => tables.Remove(key));
    }

    private void ClipboardPaste(object sender, RoutedEventArgs e) {
        if (_workspace.Tables is null) {
            return;
        }

        try {
            if (!LocalizationClipboard.TryGet(out var payload)) {
                ShowError(_localizer[EditorStringKeys.Dialog.InvalidClipboard]);
                return;
            }

            var targetLocalKey = GetCurrentGroup().AllocateLocalKey(payload.LocalKey);
            var targetKey = JsonKeyNode.CombineKey(_currentGroupKey, targetLocalKey);
            ChangeStructure(tables => LocalizationClipboard.Paste(tables, _currentGroupKey, targetLocalKey, payload), () => targetKey);
        } catch (ExternalException exception) {
            ShowError(exception.Message);
        }
    }

    private void CopyToSelection(object sender, RoutedEventArgs e) {
        var tables = _workspace.Tables;
        if (tables is null || !TryGetSelectedNode(out var node)) {
            return;
        }

        var dialog = new MoveDestinationDialog(
            tables.RootKeyGroup,
            node,
            _localizer[EditorStringKeys.Dialog.MoveRoot],
            _localizer[EditorStringKeys.Dialog.CopyTitle],
            _localizer[EditorStringKeys.Dialog.CopyMessage, node.FullKey],
            _localizer[EditorStringKeys.Dialog.CopyConfirm],
            _localizer[EditorStringKeys.Dialog.Cancel],
            sameParentMessage: null
        ) { Owner = this };
        if (dialog.ShowDialog() != true) {
            return;
        }

        string? copiedKey = null;
        ChangeStructure(tables => copiedKey = tables.Copy(node.FullKey, dialog.SelectedGroupKey), () => copiedKey);
    }

    private void MoveSelection(object sender, RoutedEventArgs e) {
        var tables = _workspace.Tables;
        if (tables is null || !TryGetSelectedNode(out var node)) {
            return;
        }

        var dialog = new MoveDestinationDialog(
            tables.RootKeyGroup,
            node,
            _localizer[EditorStringKeys.Dialog.MoveRoot],
            _localizer[EditorStringKeys.Dialog.MoveTitle],
            _localizer[EditorStringKeys.Dialog.MoveMessage, node.FullKey],
            _localizer[EditorStringKeys.Dialog.MoveConfirm],
            _localizer[EditorStringKeys.Dialog.Cancel],
            _localizer[EditorStringKeys.Dialog.MoveSameParent]
        ) { Owner = this };
        if (dialog.ShowDialog() != true) {
            return;
        }

        string? movedKey = null;
        ChangeStructure(tables => movedKey = tables.Move(node.FullKey, dialog.SelectedGroupKey), () => movedKey);
    }

    private void Remove(object sender, RoutedEventArgs e) {
        if (!TryGetSelectedNode(out var node)) {
            return;
        }

        var result = MessageBox.Show(
            this,
            _localizer[EditorStringKeys.Dialog.RemoveMessage, node.FullKey],
            _localizer[EditorStringKeys.Dialog.RemoveTitle],
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );
        if (result != MessageBoxResult.Yes) {
            return;
        }

        var key = node.FullKey;
        ChangeStructure(tables => tables.Remove(key));
    }

    private bool TryGetSelectedNode([NotNullWhen(true)] out JsonKeyNode? node) {
        if (TableGrid.SelectedItem is LocalizationRow row) {
            node = row.Node;
            return true;
        }

        node = null;
        return false;
    }

    private bool ChangeStructure(Action<JsonLocaleTableCollection> change, Func<string?>? getSelectedKey = null, bool rebuildColumns = false) {
        if (_workspace.Tables is null) {
            return false;
        }

        try {
            var state = CaptureGridState();
            _workspace.ApplyStructureChange(change);
            if (rebuildColumns) {
                RebuildColumns();
            }

            RefreshRows(getSelectedKey?.Invoke() ?? state.SelectedKey, state);
            return true;
        } catch (ArgumentException exception) {
            ShowError(exception.Message);
            return false;
        } catch (InvalidOperationException exception) {
            ShowError(exception.Message);
            return false;
        }
    }

    private void RebuildColumns() {
        TableGrid.Columns.Clear();
        _cultureNameToColumn.Clear();
        _nameColumn = null;
        var tables = _workspace.Tables;
        if (tables is null) {
            return;
        }

        _nameColumn = CreateNameColumn();
        TableGrid.Columns.Add(_nameColumn);
        foreach (var culture in tables.GetCultures()) {
            var column = CreateValueColumn(culture);
            _cultureNameToColumn.Add(culture.Name, column);
            TableGrid.Columns.Add(column);
        }

        DataGridTemplateColumn CreateNameColumn() {
            var header = new TextBlock();
            header.SetBinding(TextBlock.TextProperty, new Binding($"[{EditorStringKeys.Grid.Name}]") { Source = _localizer });
            return new DataGridTemplateColumn {
                Header = header,
                CellTemplate = (DataTemplate)FindResource("NameCellTemplate"),
                IsReadOnly = true,
                Width = new DataGridLength(320),
                MinWidth = 260
            };
        }

        DataGridTextColumn CreateValueColumn(CultureInfo culture) {
            var valuePath = $"CultureNameToTranslation[{culture.Name}]";
            return new DataGridTextColumn {
                Header = culture.Name,
                Binding = new Binding(valuePath) { Mode = BindingMode.OneWay },
                ElementStyle = (Style)FindResource("TranslationCellTextStyle"),
                IsReadOnly = true,
                MinWidth = 210,
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            };
        }
    }

    private void RefreshRows(string? selectedKey = null, GridState? previousState = null) {
        var tables = _workspace.Tables;
        if (tables is null) {
            TableGrid.ItemsSource = null;
            return;
        }

        var state = previousState ?? CaptureGridState();
        selectedKey ??= state.SelectedKey;
        var searchText = SearchTextBox.Text;
        var currentGroup = GetCurrentGroup();
        var rows = new List<LocalizationRow>(currentGroup.LocalKeyToChild.Count);
        if (searchText.Length == 0) {
            foreach (var child in currentGroup.LocalKeyToChild.Values) {
                AddRow(child, child.LocalKey);
            }
        } else {
            AddSearchRows(currentGroup);
        }

        TableGrid.ItemsSource = rows;
        if (selectedKey is not null) {
            foreach (var row in rows) {
                if (string.Equals(row.Node.FullKey, selectedKey, StringComparison.Ordinal)) {
                    TableGrid.SelectedItem = row;
                    break;
                }
            }
        }

        RestoreGridState(state);

        void AddSearchRows(JsonKeyGroup group) {
            foreach (var child in group.LocalKeyToChild.Values) {
                if (MatchesSearch(child)) {
                    var relativeKey = _currentGroupKey.Length == 0 ? child.FullKey : child.FullKey[(_currentGroupKey.Length + 1)..];
                    AddRow(child, relativeKey.Replace(".", " / "));
                }

                if (child is JsonKeyGroup childGroup) {
                    AddSearchRows(childGroup);
                }
            }
        }

        void AddRow(JsonKeyNode node, string displayName) {
            var row = new LocalizationRow(node, displayName);
            foreach (var culture in tables.GetCultures()) {
                var value = node is JsonKeyEntry ? tables.GetTranslation(culture, node.FullKey) : string.Empty;
                row.CultureNameToTranslation.Add(culture.Name, value);
            }

            rows.Add(row);
        }

        bool MatchesSearch(JsonKeyNode node) {
            if (node.FullKey.Contains(searchText, StringComparison.OrdinalIgnoreCase)) {
                return true;
            }

            if (node is JsonKeyEntry) {
                foreach (var culture in tables.GetCultures()) {
                    if (tables.GetTranslation(culture, node.FullKey).Contains(searchText, StringComparison.OrdinalIgnoreCase)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    private JsonKeyGroup GetCurrentGroup() {
        var tables = _workspace.Tables;
        Debug.Assert(tables is not null);
        return tables.RootKeyGroup.GetGroup(_currentGroupKey);
    }

    private CultureInfo? GetColumnCulture(DataGridColumn column) {
        var tables = _workspace.Tables;
        if (tables is null) {
            return null;
        }

        foreach (var culture in tables.GetCultures()) {
            if (_cultureNameToColumn.TryGetValue(culture.Name, out var cultureColumn) && ReferenceEquals(cultureColumn, column)) {
                return culture;
            }
        }

        return null;
    }

    private GridState CaptureGridState() {
        var selectedKey = (TableGrid.SelectedItem as LocalizationRow)?.Node.FullKey;
        string? currentColumnKey = null;
        if (TableGrid.CurrentColumn is not null) {
            currentColumnKey = string.Empty;
            foreach (var pair in _cultureNameToColumn) {
                if (ReferenceEquals(pair.Value, TableGrid.CurrentColumn)) {
                    currentColumnKey = pair.Key;
                    break;
                }
            }
        }

        var scrollViewer = FindVisualChild<ScrollViewer>(TableGrid);
        return new GridState(selectedKey, currentColumnKey, scrollViewer?.HorizontalOffset ?? 0, scrollViewer?.VerticalOffset ?? 0);
    }

    private void RestoreGridState(GridState state) {
        if (TableGrid.SelectedItem is LocalizationRow row && state.CurrentColumnKey is not null) {
            DataGridColumn? column;
            if (state.CurrentColumnKey.Length == 0) {
                column = TableGrid.Columns.Count > 0 ? TableGrid.Columns[0] : null;
            } else {
                _cultureNameToColumn.TryGetValue(state.CurrentColumnKey, out column);
            }

            if (column is not null) {
                TableGrid.CurrentCell = new DataGridCellInfo(row, column);
            }
        }

        var scrollViewer = FindVisualChild<ScrollViewer>(TableGrid);
        if (scrollViewer is null) {
            return;
        }

        TableGrid.UpdateLayout();
        scrollViewer.ScrollToHorizontalOffset(state.HorizontalOffset);
        scrollViewer.ScrollToVerticalOffset(state.VerticalOffset);
    }

    private void EditTranslation(LocalizationRow row, CultureInfo culture) {
        if (_workspace.Tables is null) {
            return;
        }

        var currentTranslation = row.CultureNameToTranslation[culture.Name];
        var dialog = new TranslationEditDialog(
            _localizer[EditorStringKeys.Dialog.EditTranslationTitle],
            _localizer[EditorStringKeys.Dialog.EditTranslationKey, row.Node.FullKey],
            _localizer[EditorStringKeys.Dialog.EditTranslationLocale, culture.Name],
            _localizer[EditorStringKeys.Dialog.EditTranslationShortcut],
            currentTranslation,
            _localizer[EditorStringKeys.Dialog.Confirm],
            _localizer[EditorStringKeys.Dialog.Cancel]
        ) { Owner = this };
        if (dialog.ShowDialog() != true || string.Equals(dialog.Translation, currentTranslation, StringComparison.Ordinal)) {
            return;
        }

        _workspace.SetTranslation(culture, row.Node.FullKey, dialog.Translation);
        row.CultureNameToTranslation[culture.Name] = dialog.Translation;
        TableGrid.Items.Refresh();
    }

    private void SelectPlaceholderReferenceCulture(CultureInfo culture) {
        Debug.Assert(_workspace.Tables is not null);
        _workspace.SetPlaceholderReferenceCulture(culture);
        var referenceCultureName = _preferences.Get<string?>(ValidationPreferenceKeys.PlaceholderReferenceCulture, null);
        if (!string.Equals(referenceCultureName, culture.Name, StringComparison.OrdinalIgnoreCase)) {
            _preferences.Set(ValidationPreferenceKeys.PlaceholderReferenceCulture, culture.Name);
            SavePreferences();
        }
    }

    private void ChoosePlaceholderReferenceCulture(object sender, RoutedEventArgs e) {
        var tables = _workspace.Tables;
        if (tables is null) {
            return;
        }

        var cultures = new List<CultureInfo>(tables.CultureCount);
        foreach (var culture in tables.GetCultures()) {
            cultures.Add(culture);
        }

        var dialog = new Window {
            Title = _localizer[EditorStringKeys.Validation.ReferenceDialogTitle],
            Owner = this,
            MinWidth = 420,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ResizeMode = ResizeMode.NoResize
        };
        var panel = new StackPanel { Margin = new Thickness(16) };
        panel.Children.Add(new TextBlock {
            Text = _localizer[EditorStringKeys.Validation.ReferenceDialogMessage],
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 560
        });
        var comboBox = new ComboBox {
            ItemsSource = cultures,
            DisplayMemberPath = nameof(CultureInfo.NativeName),
            SelectedItem = _workspace.PlaceholderReferenceCulture,
            Margin = new Thickness(0, 8, 0, 12),
            MinWidth = 360
        };
        panel.Children.Add(comboBox);
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var confirm = new Button {
            Content = _localizer[EditorStringKeys.Dialog.Confirm],
            IsDefault = true,
            MinWidth = 72,
            Margin = new Thickness(0, 0, 8, 0)
        };
        confirm.Click += (_, _) => {
            if (comboBox.SelectedItem is CultureInfo selectedCulture) {
                SelectPlaceholderReferenceCulture(selectedCulture);
                dialog.DialogResult = true;
            }
        };
        var cancel = new Button {
            Content = _localizer[EditorStringKeys.Dialog.Cancel],
            IsCancel = true,
            MinWidth = 72
        };
        buttons.Children.Add(confirm);
        buttons.Children.Add(cancel);
        panel.Children.Add(buttons);
        dialog.Content = panel;
        dialog.ShowDialog();
    }

    private void RunValidation(object sender, RoutedEventArgs e) {
        var referenceCulture = _workspace.PlaceholderReferenceCulture;
        if (_workspace.Tables is null || referenceCulture is null) {
            return;
        }

        var referenceCultureName = _preferences.Get<string?>(ValidationPreferenceKeys.PlaceholderReferenceCulture, null);
        if (!string.Equals(referenceCultureName, referenceCulture.Name, StringComparison.OrdinalIgnoreCase)) {
            _preferences.Set(ValidationPreferenceKeys.PlaceholderReferenceCulture, referenceCulture.Name);
            SavePreferences();
        }

        _validationKindFilter = null;
        _validationCultureFilter = null;
        _validationAnalysisRequested.Handlers?.Invoke();
        ValidationPanelRow.Height = new GridLength(220);
        ValidationGridSplitter.Visibility = Visibility.Visible;
        ValidationPanel.Visibility = Visibility.Visible;
    }

    private void RefreshValidationPresentation() {
        var issues = _validationResults.Issues;
        var referenceCulture = _workspace.PlaceholderReferenceCulture;
        if (issues is null || referenceCulture is null) {
            ValidationGrid.ItemsSource = null;
            ValidationGrid.Visibility = Visibility.Collapsed;
            ValidationEmptyText.Text = _localizer[EditorStringKeys.Validation.NoIssues];
            ValidationEmptyText.Visibility = Visibility.Visible;
            ValidationSummaryText.Text = string.Empty;
            ValidationCompletionText.Text = string.Empty;
            return;
        }

        var rows = new List<ValidationIssueRow>(issues.Count);
        foreach (var issue in issues) {
            if (_validationKindFilter is JsonLocalizationIssueKind kind && issue.Kind != kind) {
                continue;
            }

            if (_validationCultureFilter is not null && !string.Equals(issue.Culture.Name, _validationCultureFilter, StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            rows.Add(new ValidationIssueRow(issue, GetIssueKindText(issue.Kind)));
        }

        ValidationGrid.ItemsSource = rows;
        ValidationGrid.Visibility = rows.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        ValidationEmptyText.Visibility = rows.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ValidationEmptyText.Text = _localizer[issues.Count == 0
            ? EditorStringKeys.Validation.NoIssues
            : EditorStringKeys.Validation.NoMatchingIssues];
        ValidationSummaryText.Text = _localizer[EditorStringKeys.Validation.Summary, rows.Count, issues.Count, referenceCulture.Name];
        ValidationCompletionText.Text = BuildCompletionText();

        string BuildCompletionText() {
            var tables = _workspace.Tables;
            Debug.Assert(tables is not null);
            var entryKeys = new List<string>();
            foreach (var entryKey in tables.GetEntryKeys()) {
                entryKeys.Add(entryKey);
            }

            var items = new List<string>(tables.CultureCount);
            foreach (var culture in tables.GetCultures()) {
                var translatedCount = 0;
                foreach (var entryKey in entryKeys) {
                    if (tables.GetTranslation(culture, entryKey).Length > 0) {
                        translatedCount++;
                    }
                }

                var percent = entryKeys.Count == 0
                    ? 100
                    : (int)Math.Round(translatedCount * 100.0 / entryKeys.Count, MidpointRounding.AwayFromZero);
                items.Add(_localizer[EditorStringKeys.Validation.CompletionItem, culture.Name, translatedCount, entryKeys.Count, percent]);
            }

            return _localizer[EditorStringKeys.Validation.Completion, string.Join(" · ", items)];
        }
    }

    private void RebuildValidationFilters() {
        var tables = _workspace.Tables;
        if (_validationResults.Issues is null || tables is null) {
            return;
        }

        _rebuildingValidationFilters = true;
        try {
            ValidationKindFilterComboBox.Items.Clear();
            ValidationKindFilterComboBox.Items.Add(new ComboBoxItem {
                Content = _localizer[EditorStringKeys.Validation.FilterAll]
            });
            var selectedKindIndex = 0;
            foreach (var kind in Enum.GetValues<JsonLocalizationIssueKind>()) {
                ValidationKindFilterComboBox.Items.Add(new ComboBoxItem {
                    Content = GetIssueKindText(kind),
                    Tag = kind
                });
                if (_validationKindFilter == kind) {
                    selectedKindIndex = ValidationKindFilterComboBox.Items.Count - 1;
                }
            }
            ValidationKindFilterComboBox.SelectedIndex = selectedKindIndex;

            ValidationLocaleFilterComboBox.Items.Clear();
            ValidationLocaleFilterComboBox.Items.Add(new ComboBoxItem {
                Content = _localizer[EditorStringKeys.Validation.FilterAll]
            });
            var selectedCultureIndex = 0;
            foreach (var culture in tables.GetCultures()) {
                ValidationLocaleFilterComboBox.Items.Add(new ComboBoxItem {
                    Content = $"{culture.NativeName} ({culture.Name})",
                    Tag = culture.Name
                });
                if (string.Equals(_validationCultureFilter, culture.Name, StringComparison.OrdinalIgnoreCase)) {
                    selectedCultureIndex = ValidationLocaleFilterComboBox.Items.Count - 1;
                }
            }
            ValidationLocaleFilterComboBox.SelectedIndex = selectedCultureIndex;
        } finally {
            _rebuildingValidationFilters = false;
        }
    }

    private void ValidationFilterChanged(object sender, SelectionChangedEventArgs e) {
        if (_rebuildingValidationFilters) {
            return;
        }

        var kindTag = (ValidationKindFilterComboBox.SelectedItem as ComboBoxItem)?.Tag;
        _validationKindFilter = kindTag is JsonLocalizationIssueKind kind ? kind : null;
        _validationCultureFilter = (ValidationLocaleFilterComboBox.SelectedItem as ComboBoxItem)?.Tag as string;
        RefreshValidationPresentation();
    }

    private string GetIssueKindText(JsonLocalizationIssueKind kind) {
        return kind switch {
            JsonLocalizationIssueKind.MissingTranslation => _localizer[EditorStringKeys.Validation.KindMissingTranslation],
            JsonLocalizationIssueKind.PlaceholderMismatch => _localizer[EditorStringKeys.Validation.KindPlaceholderMismatch],
            JsonLocalizationIssueKind.UnexpectedTranslationKey => _localizer[EditorStringKeys.Validation.KindUnexpectedTranslationKey],
            JsonLocalizationIssueKind.InvalidFormatString => _localizer[EditorStringKeys.Validation.KindInvalidFormatString],
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }

    private void ValidationResultsChanged() {
        RebuildValidationFilters();
        RefreshValidationPresentation();
    }

    private void CloseValidationPanel(object sender, RoutedEventArgs e) {
        ValidationPanel.Visibility = Visibility.Collapsed;
        ValidationGridSplitter.Visibility = Visibility.Collapsed;
        ValidationPanelRow.Height = new GridLength(0);
    }

    private void NavigateToValidationIssue(object sender, MouseButtonEventArgs e) {
        if (FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject) is null) {
            return;
        }

        ActivateSelectedValidationIssue();
        e.Handled = true;
    }

    private void ActivateSelectedValidationIssue() {
        var tables = _workspace.Tables;
        if (tables is null || ValidationGrid.SelectedItem is not ValidationIssueRow row) {
            return;
        }

        var issue = row.Issue;
        if (issue.Kind == JsonLocalizationIssueKind.UnexpectedTranslationKey
            || !tables.RootKeyGroup.TryGet(issue.EntryKey, out var node)
            || node is not JsonKeyEntry) {
            MessageBox.Show(
                this,
                _localizer[EditorStringKeys.Validation.CannotNavigate, issue.EntryKey],
                _localizer[EditorStringKeys.Validation.CannotNavigateTitle],
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
            return;
        }

        _currentGroupKey = JsonKeyNode.GetParentKey(issue.EntryKey);
        RebuildBreadcrumb();
        if (SearchTextBox.Text.Length > 0) {
            SearchTextBox.Clear();
        } else {
            RefreshRows(issue.EntryKey, new GridState(issue.EntryKey, issue.Culture.Name, 0, 0));
        }

        foreach (var item in TableGrid.Items) {
            if (item is not LocalizationRow localizationRow || !string.Equals(localizationRow.Node.FullKey, issue.EntryKey, StringComparison.Ordinal)) {
                continue;
            }

            TableGrid.SelectedItem = localizationRow;
            if (_cultureNameToColumn.TryGetValue(issue.Culture.Name, out var column)) {
                TableGrid.CurrentCell = new DataGridCellInfo(localizationRow, column);
                TableGrid.ScrollIntoView(localizationRow, column);
            } else {
                TableGrid.ScrollIntoView(localizationRow);
            }

            TableGrid.Focus();
            break;
        }
    }

    private string? Prompt(string title, string message, string initialValue = "") {
        var dialog = new Window {
            Title = title,
            Owner = this,
            MinWidth = 420,
            MaxWidth = 720,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ResizeMode = ResizeMode.NoResize
        };
        var panel = new StackPanel { Margin = new Thickness(16) };
        panel.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, MaxWidth = 680 });
        var textBox = new TextBox { Text = initialValue, Margin = new Thickness(0, 8, 0, 12) };
        panel.Children.Add(textBox);
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var confirmed = false;
        var confirm = new Button { Content = _localizer[EditorStringKeys.Dialog.Confirm], IsDefault = true, MinWidth = 72, Margin = new Thickness(0, 0, 8, 0) };
        confirm.Click += (_, _) => { confirmed = true; dialog.Close(); };
        var cancel = new Button { Content = _localizer[EditorStringKeys.Dialog.Cancel], IsCancel = true, MinWidth = 72 };
        buttons.Children.Add(confirm);
        buttons.Children.Add(cancel);
        panel.Children.Add(buttons);
        dialog.Content = panel;
        dialog.Loaded += (_, _) => { textBox.Focus(); textBox.SelectAll(); };
        dialog.ShowDialog();
        return confirmed ? textBox.Text : null;
    }

    private void ShowError(string message) {
        MessageBox.Show(this, message, _localizer[EditorStringKeys.Dialog.ErrorTitle], MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static void AddMenuItem(ItemCollection items, string header, RoutedEventHandler click, string? gesture = null, bool isDangerous = false) {
        var item = new MenuItem {
            Header = header,
            InputGestureText = gesture ?? string.Empty
        };
        item.Click += click;
        if (isDangerous) {
            item.SetResourceReference(Control.ForegroundProperty, "VsDangerBrush");
        }

        items.Add(item);
    }

    private static T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject {
        while (child is not null) {
            if (child is T parent) {
                return parent;
            }

            child = VisualTreeHelper.GetParent(child);
        }

        return null;
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject {
        var childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var index = 0; index < childCount; index++) {
            var child = VisualTreeHelper.GetChild(parent, index);
            if (child is T matchingChild) {
                return matchingChild;
            }

            var descendant = FindVisualChild<T>(child);
            if (descendant is not null) {
                return descendant;
            }
        }

        return null;
    }

    private sealed record GridState(string? SelectedKey, string? CurrentColumnKey, double HorizontalOffset, double VerticalOffset);

    private sealed class LocalizationRow(JsonKeyNode node, string displayName) {

        public JsonKeyNode Node { get; } = node;

        public Dictionary<string, string> CultureNameToTranslation { get; } = new(StringComparer.Ordinal);

        public string DisplayName { get; } = displayName;

        public bool IsGroup { get; } = node is JsonKeyGroup;
    }

    private sealed class ValidationIssueRow(JsonLocalizationIssue issue, string kindText) {

        public JsonLocalizationIssue Issue { get; } = issue;

        public string KindText { get; } = kindText;

        public string EntryKey => Issue.EntryKey;

        public string CultureName => Issue.Culture.Name;
    }
}
