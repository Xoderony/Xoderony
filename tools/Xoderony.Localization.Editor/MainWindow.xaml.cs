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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xoderony.Localization.Json;
using Xoderony.Localization.Tooling;

namespace Xoderony.Localization.Editor;

public partial class MainWindow : Window {

    private readonly Dictionary<string, DataGridColumn> _columnByCultureName = new(StringComparer.OrdinalIgnoreCase);
    private readonly EditorLocalizer _localizer;
    private readonly EditorSettings _settings;
    private JsonStringTableCollection? _tables;
    private string _currentGroupKey = string.Empty;
    private string? _directoryPath;
    private DataGridColumn? _nameColumn;

    public MainWindow() {
        _settings = EditorSettings.Load();
        var localizationDirectory = Path.Combine(AppContext.BaseDirectory, "Localization");
        _localizer = EditorLocalizer.Load(localizationDirectory, GetPreferredCulture(_settings.UiCultureName));
        EditorThemeManager.SetTheme(_settings.Theme);
        InitializeComponent();
        RestoreWindowSize();
        DataContext = _localizer;
        TableGrid.ContextMenu?.DataContext = _localizer;
        ThemeComboBox.SelectedIndex = _settings.Theme == EditorTheme.Dark ? 1 : 0;

        UpdateLocalizedText();
    }

    private void OnLoaded(object sender, RoutedEventArgs e) {
        Loaded -= OnLoaded;
        var directoryPath = _settings.LastDirectoryPath;
        if (directoryPath is not null && Directory.Exists(directoryPath)) {
            TryOpenDirectory(directoryPath, rememberDirectory: false);
        }
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
            var tables = JsonStringTableCollection.LoadDirectory(directoryPath);
            _tables = tables;
            _directoryPath = directoryPath;
            _currentGroupKey = string.Empty;
            RefreshBreadcrumb();
            RebuildColumns();
            if (SearchTextBox.Text.Length > 0) {
                SearchTextBox.Clear();
            } else {
                RefreshRows(previousState: new GridState(null, null, 0, 0));
            }

            if (rememberDirectory && !string.Equals(_settings.LastDirectoryPath, directoryPath, StringComparison.OrdinalIgnoreCase)) {
                _settings.LastDirectoryPath = directoryPath;
                SaveSettings();
            }

            return true;
        } catch (Exception exception) when (exception is ArgumentException or CultureNotFoundException or IOException or InvalidDataException) {
            ShowError(exception.Message);
            return false;
        }
    }

    private void Save(object sender, RoutedEventArgs e) {
        if (_tables is null) {
            return;
        }

        try {
            _tables.Save();
            UpdateStatus();
        } catch (IOException exception) {
            ShowError(exception.Message);
        }
    }

    private void GenerateKeys(object sender, RoutedEventArgs e) {
        if (_tables is null) {
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
            source = StringTableKeyGenerator.Generate(_tables.GetKeys(), namespaceName, typeName);
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
            StatusText.Text = _localizer[EditorStringKeys.Status.GeneratedKeys, dialog.FileName];
        } catch (IOException exception) {
            ShowError(exception.Message);
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

    private void LanguageChanged(object sender, SelectionChangedEventArgs e) {
        if (LanguageComboBox.SelectedItem is CultureInfo culture && _localizer.SetCulture(culture)) {
            _settings.UiCultureName = culture.Name;
            SaveSettings();
            UpdateLocalizedText();
        }
    }

    private void ThemeChanged(object sender, SelectionChangedEventArgs e) {
        if (ThemeComboBox.SelectedItem is not ComboBoxItem { Tag: EditorTheme theme } || _settings.Theme == theme) {
            return;
        }

        EditorThemeManager.SetTheme(theme);
        _settings.Theme = theme;
        SaveSettings();
    }

    private void SaveSettings() {
        try {
            _settings.Save();
        } catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) {
            ShowError(exception.Message);
        }
    }

    private void NavigateBack() {
        if (_currentGroupKey.Length == 0) {
            return;
        }

        NavigateToGroup(JsonStringTableNode.GetParentKey(_currentGroupKey));
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

    private void NavigateToGroup(string groupKey) {
        _currentGroupKey = groupKey;
        RefreshBreadcrumb();
        if (SearchTextBox.Text.Length > 0) {
            SearchTextBox.Clear();
        } else {
            RefreshRows(previousState: new GridState(null, null, 0, 0));
        }
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e) {
        UpdateCommandState();
    }

    private void SelectRowOnRightClick(object sender, MouseButtonEventArgs e) {
        var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
        if (row is not null) {
            if (!row.IsSelected) {
                TableGrid.SelectedItem = row.Item;
            }

            var cell = FindVisualParent<DataGridCell>(e.OriginalSource as DependencyObject);
            if (cell is not null) {
                TableGrid.CurrentCell = new DataGridCellInfo(row.Item, cell.Column);
            }
        }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
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
                    CopySelection(this, new RoutedEventArgs());
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.V) {
                    PasteClipboard(this, new RoutedEventArgs());
                    e.Handled = true;
                    return;
                }
            }
        }

        if (Keyboard.FocusedElement is TextBox) {
            return;
        }

        if (e.Key == Key.Back && _currentGroupKey.Length > 0) {
            NavigateBack();
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
        if (_tables is null) {
            return;
        }

        var localKey = Prompt(_localizer[EditorStringKeys.Dialog.AddGroupTitle], _localizer[EditorStringKeys.Dialog.AddGroupMessage]);
        if (localKey is null) {
            return;
        }

        var key = JsonStringTableNode.CombineKey(_currentGroupKey, localKey);
        ChangeStructure(tables => tables.AddGroup(_currentGroupKey, localKey), key);
    }

    private void AddEntry(object sender, RoutedEventArgs e) {
        if (_tables is null) {
            return;
        }

        var localKey = Prompt(_localizer[EditorStringKeys.Dialog.AddEntryTitle], _localizer[EditorStringKeys.Dialog.AddEntryMessage]);
        if (localKey is null) {
            return;
        }

        var key = JsonStringTableNode.CombineKey(_currentGroupKey, localKey);
        ChangeStructure(tables => tables.AddEntry(_currentGroupKey, localKey), key);
    }

    private void AddLocale(object sender, RoutedEventArgs e) {
        if (_tables is null || _directoryPath is null) {
            return;
        }

        var cultureName = Prompt(_localizer[EditorStringKeys.Dialog.AddLocaleTitle], _localizer[EditorStringKeys.Dialog.AddLocaleMessage]);
        if (cultureName is null) {
            return;
        }

        try {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            var filePath = Path.Combine(_directoryPath, $"{culture.Name}.json");
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

        var newKey = JsonStringTableNode.CombineKey(JsonStringTableNode.GetParentKey(oldKey), localKey);
        ChangeStructure(tables => tables.Rename(oldKey, localKey), newKey);
    }

    private void CopySelection(object sender, RoutedEventArgs e) {
        if (_tables is null || !TryGetSelectedNode(out var node)) {
            return;
        }

        try {
            LocalizationClipboard.Set(_tables, node);
            StatusText.Text = _localizer[EditorStringKeys.Status.Copied, node.FullKey];
            UpdateCommandState();
        } catch (ExternalException exception) {
            ShowError(exception.Message);
        }
    }

    private void PasteClipboard(object sender, RoutedEventArgs e) {
        if (_tables is null) {
            return;
        }

        try {
            if (!LocalizationClipboard.TryGet(out var payload)) {
                ShowError(_localizer[EditorStringKeys.Dialog.InvalidClipboard]);
                return;
            }

            var targetLocalKey = GetAvailableCopyLocalKey(payload.LocalKey);
            var targetKey = JsonStringTableNode.CombineKey(_currentGroupKey, targetLocalKey);
            ChangeStructure(tables => LocalizationClipboard.Paste(tables, _currentGroupKey, targetLocalKey, payload), targetKey);
        } catch (ExternalException exception) {
            ShowError(exception.Message);
        }
    }

    private void MoveSelection(object sender, RoutedEventArgs e) {
        if (_tables is null || !TryGetSelectedNode(out var node)) {
            return;
        }

        var dialog = new MoveDestinationDialog(
            _tables.RootGroup,
            node,
            _localizer[EditorStringKeys.Dialog.MoveRoot],
            _localizer[EditorStringKeys.Dialog.MoveTitle],
            _localizer[EditorStringKeys.Dialog.MoveMessage, node.FullKey],
            _localizer[EditorStringKeys.Dialog.MoveConfirm],
            _localizer[EditorStringKeys.Dialog.Cancel],
            _localizer[EditorStringKeys.Dialog.MoveSameParent],
            _localizer[EditorStringKeys.Dialog.MoveConflict, node.LocalKey]
        ) { Owner = this };
        if (dialog.ShowDialog() != true) {
            return;
        }

        var movedKey = JsonStringTableNode.CombineKey(dialog.SelectedGroupKey, node.LocalKey);
        ChangeStructure(tables => tables.Move(node.FullKey, dialog.SelectedGroupKey), movedKey);
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

    private bool TryGetSelectedNode([NotNullWhen(true)] out JsonStringTableNode? node) {
        if (TableGrid.SelectedItem is LocalizationRow row) {
            node = row.Node;
            return true;
        }

        node = null;
        return false;
    }

    private bool ChangeStructure(Action<JsonStringTableCollection> change, string? selectedKey = null, bool rebuildColumns = false) {
        if (_tables is null) {
            return false;
        }

        try {
            var state = CaptureGridState();
            change(_tables);
            if (rebuildColumns) {
                RebuildColumns();
            }

            RefreshRows(selectedKey ?? state.SelectedKey, state);
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
        _columnByCultureName.Clear();
        _nameColumn = null;
        if (_tables is null) {
            return;
        }

        _nameColumn = CreateNameColumn();
        TableGrid.Columns.Add(_nameColumn);
        foreach (var culture in _tables.Cultures) {
            var column = CreateValueColumn(culture);
            _columnByCultureName.Add(culture.Name, column);
            TableGrid.Columns.Add(column);
        }
    }

    private void RefreshRows(string? selectedKey = null, GridState? previousState = null) {
        if (_tables is null) {
            TableGrid.ItemsSource = null;
            UpdateStatus();
            return;
        }

        var state = previousState ?? CaptureGridState();
        selectedKey ??= state.SelectedKey;
        var searchText = SearchTextBox.Text;
        var currentGroup = GetCurrentGroup();
        var rows = new List<LocalizationRow>(currentGroup.Children.Count);
        if (searchText.Length == 0) {
            foreach (var child in currentGroup.Children.Values) {
                AddRow(rows, child, child.LocalKey);
            }
        } else {
            AddSearchRows(rows, currentGroup, searchText);
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
        UpdateStatus();
    }

    private void AddSearchRows(List<LocalizationRow> rows, JsonStringTableGroup group, string searchText) {
        Debug.Assert(_tables is not null);
        foreach (var child in group.Children.Values) {
            if (MatchesSearch(_tables, child, searchText)) {
                var relativeKey = _currentGroupKey.Length == 0 ? child.FullKey : child.FullKey[(_currentGroupKey.Length + 1)..];
                AddRow(rows, child, relativeKey.Replace(".", " / "));
            }

            if (child is JsonStringTableGroup childGroup) {
                AddSearchRows(rows, childGroup, searchText);
            }
        }
    }

    private void AddRow(List<LocalizationRow> rows, JsonStringTableNode node, string displayName) {
        Debug.Assert(_tables is not null);
        var row = new LocalizationRow(node, displayName);
        foreach (var culture in _tables.Cultures) {
            var value = node is JsonStringTableEntry ? _tables.GetValue(culture, node.FullKey) : string.Empty;
            row.Values.Add(culture.Name, value);
        }

        rows.Add(row);
    }

    private DataGridTemplateColumn CreateNameColumn() {
        return new DataGridTemplateColumn {
            Header = _localizer[EditorStringKeys.Grid.Name],
            CellTemplate = (DataTemplate)FindResource("NameCellTemplate"),
            IsReadOnly = true,
            Width = new DataGridLength(320),
            MinWidth = 260
        };
    }

    private DataGridTextColumn CreateValueColumn(CultureInfo culture) {
        var valuePath = $"Values[{culture.Name}]";
        return new DataGridTextColumn {
            Header = culture.Name,
            Binding = new Binding(valuePath) { Mode = BindingMode.OneWay },
            IsReadOnly = true,
            MinWidth = 210,
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        };
    }

    private JsonStringTableGroup GetCurrentGroup() {
        Debug.Assert(_tables is not null);
        return _tables.RootGroup.GetGroup(_currentGroupKey);
    }

    private static bool MatchesSearch(JsonStringTableCollection tables, JsonStringTableNode node, string searchText) {
        if (searchText.Length == 0 || node.FullKey.Contains(searchText, StringComparison.OrdinalIgnoreCase)) {
            return true;
        }

        if (node is JsonStringTableEntry) {
            foreach (var culture in tables.Cultures) {
                if (tables.GetValue(culture, node.FullKey).Contains(searchText, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }
        }

        return false;
    }

    private bool ConfirmDiscardChanges() {
        if (_tables is null || !_tables.IsDirty) {
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
            return !_tables.IsDirty;
        }

        return true;
    }

    private void OnClosing(object? sender, CancelEventArgs e) {
        if (!ConfirmDiscardChanges()) {
            e.Cancel = true;
            return;
        }

        var restoreBounds = RestoreBounds;
        _settings.WindowWidth = restoreBounds.Width;
        _settings.WindowHeight = restoreBounds.Height;
        SaveSettings();
    }

    private void RestoreWindowSize() {
        var workArea = SystemParameters.WorkArea;
        var maxWidth = Math.Max(MinWidth, workArea.Width);
        var maxHeight = Math.Max(MinHeight, workArea.Height);

        if (_settings.WindowWidth is double width && double.IsFinite(width)) {
            Width = Math.Clamp(width, MinWidth, maxWidth);
        }

        if (_settings.WindowHeight is double height && double.IsFinite(height)) {
            Height = Math.Clamp(height, MinHeight, maxHeight);
        }
    }

    private void UpdateStatus() {
        if (_tables is null) {
            Title = _localizer[EditorStringKeys.Window.Title];
            SummaryText.Text = string.Empty;
            StatusText.Text = _localizer[EditorStringKeys.Status.Unopened];
            EmptyState.Visibility = Visibility.Visible;
            TableGrid.Visibility = Visibility.Collapsed;
            UpdateCommandState();
            return;
        }

        var keyCount = 0;
        foreach (var _ in _tables.GetKeys()) {
            keyCount++;
        }

        var emptyCount = 0;
        foreach (var culture in _tables.Cultures) {
            emptyCount += _tables.GetEmptyValueCount(culture);
        }

        var dirtyMarker = _tables.IsDirty ? " *" : string.Empty;
        Title = _localizer[EditorStringKeys.Window.TitleWithDirectory, Path.GetFileName(_directoryPath), dirtyMarker];
        SummaryText.Text = _localizer[EditorStringKeys.Status.Summary, _tables.Cultures.Count, keyCount, emptyCount];
        StatusText.Text = _tables.IsDirty ? _localizer[EditorStringKeys.Status.Unsaved] : _localizer[EditorStringKeys.Status.Saved];
        EmptyState.Visibility = Visibility.Collapsed;
        TableGrid.Visibility = Visibility.Visible;
        UpdateCommandState();
    }

    private void UpdateCommandState() {
        var tables = _tables;
        var hasTables = tables is not null;
        var hasLocales = tables is not null && tables.Cultures.Count > 0;
        var hasSelection = TableGrid.SelectedItem is LocalizationRow;
        var canPaste = hasLocales && LocalizationClipboard.ContainsData();
        SaveButton.IsEnabled = tables is not null && tables.IsDirty;
        GenerateKeysButton.IsEnabled = hasLocales;
        AddGroupButton.IsEnabled = hasLocales;
        AddEntryButton.IsEnabled = hasLocales;
        AddLocaleButton.IsEnabled = hasTables;
        RenameButton.IsEnabled = hasSelection;
        MoveButton.IsEnabled = hasSelection;
        CopyButton.IsEnabled = hasSelection;
        PasteButton.IsEnabled = canPaste;
        RenameMenuItem.IsEnabled = hasSelection;
        MoveMenuItem.IsEnabled = hasSelection;
        CopyMenuItem.IsEnabled = hasSelection;
        PasteMenuItem.IsEnabled = canPaste;
        RemoveButton.IsEnabled = hasSelection;
        SearchTextBox.IsEnabled = hasTables;
    }

    private void UpdateLocalizedText() {
        _nameColumn?.Header = _localizer[EditorStringKeys.Grid.Name];

        UpdateStatus();
    }

    private void RefreshBreadcrumb() {
        BreadcrumbPanel.Children.Clear();
        var directoryPath = _directoryPath;
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
            key = JsonStringTableNode.CombineKey(key, localKey);
            AddBreadcrumb(localKey, key, key);
        }
    }

    private void AddBreadcrumb(string label, string groupKey, string toolTip, bool isRoot = false) {
        if (!isRoot) {
            BreadcrumbPanel.Children.Add(new TextBlock {
                Text = "\u203A",
                Style = (Style)FindResource("BreadcrumbSeparatorTextStyle")
            });
        }

        var button = new Button {
            Content = label,
            Tag = groupKey,
            ToolTip = toolTip,
            Style = (Style)FindResource("BreadcrumbButtonStyle")
        };
        button.Click += NavigateBreadcrumb;
        BreadcrumbPanel.Children.Add(button);
    }

    private void NavigateBreadcrumb(object sender, RoutedEventArgs e) {
        if (sender is Button { Tag: string groupKey }) {
            NavigateToGroup(groupKey);
        }
    }

    private string? Prompt(string title, string message, string initialValue = "") {
        var dialog = new Window {
            Title = title,
            Owner = this,
            Width = 420,
            Height = 160,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ResizeMode = ResizeMode.NoResize
        };
        var panel = new StackPanel { Margin = new Thickness(16) };
        panel.Children.Add(new TextBlock { Text = message });
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

    private CultureInfo? GetColumnCulture(DataGridColumn column) {
        if (_tables is null) {
            return null;
        }

        foreach (var culture in _tables.Cultures) {
            if (_columnByCultureName.TryGetValue(culture.Name, out var cultureColumn) && ReferenceEquals(cultureColumn, column)) {
                return culture;
            }
        }

        return null;
    }

    private void EditTranslation(LocalizationRow row, CultureInfo culture) {
        if (_tables is null) {
            return;
        }

        var dialog = new Window {
            Title = _localizer[EditorStringKeys.Dialog.EditTranslationTitle],
            Owner = this,
            Width = 720,
            Height = 480,
            MinWidth = 480,
            MinHeight = 320,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ResizeMode = ResizeMode.CanResizeWithGrip
        };
        var layout = new Grid { Margin = new Thickness(16) };
        layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var keyText = new TextBlock {
            Text = _localizer[EditorStringKeys.Dialog.EditTranslationKey, row.Node.FullKey],
            TextTrimming = TextTrimming.CharacterEllipsis
        };
        layout.Children.Add(keyText);

        var localeText = new TextBlock {
            Text = _localizer[EditorStringKeys.Dialog.EditTranslationLocale, culture.Name],
            Margin = new Thickness(0, 4, 0, 10)
        };
        localeText.SetResourceReference(TextBlock.ForegroundProperty, "VsMutedTextBrush");
        Grid.SetRow(localeText, 1);
        layout.Children.Add(localeText);

        var textBox = new TextBox {
            Text = row.Values[culture.Name],
            AcceptsReturn = true,
            AcceptsTab = true,
            TextWrapping = TextWrapping.Wrap,
            VerticalContentAlignment = VerticalAlignment.Top,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };
        Grid.SetRow(textBox, 2);
        layout.Children.Add(textBox);

        var footer = new Grid { Margin = new Thickness(0, 12, 0, 0) };
        footer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        footer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var shortcutText = new TextBlock {
            Text = _localizer[EditorStringKeys.Dialog.EditTranslationShortcut],
            VerticalAlignment = VerticalAlignment.Center
        };
        shortcutText.SetResourceReference(TextBlock.ForegroundProperty, "VsMutedTextBrush");
        footer.Children.Add(shortcutText);

        var buttons = new StackPanel { Orientation = Orientation.Horizontal };
        Grid.SetColumn(buttons, 1);
        var confirmed = false;
        void Confirm() {
            confirmed = true;
            dialog.Close();
        }

        var confirm = new Button { Content = _localizer[EditorStringKeys.Dialog.Confirm], MinWidth = 72, Margin = new Thickness(8, 0, 8, 0) };
        confirm.Click += (_, _) => Confirm();
        var cancel = new Button { Content = _localizer[EditorStringKeys.Dialog.Cancel], IsCancel = true, MinWidth = 72 };
        buttons.Children.Add(confirm);
        buttons.Children.Add(cancel);
        footer.Children.Add(buttons);
        Grid.SetRow(footer, 3);
        layout.Children.Add(footer);

        dialog.Content = layout;
        dialog.Loaded += (_, _) => {
            textBox.Focus();
            textBox.CaretIndex = textBox.Text.Length;
        };
        dialog.PreviewKeyDown += (_, e) => {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) != 0) {
                Confirm();
                e.Handled = true;
            }
        };
        dialog.ShowDialog();
        if (!confirmed || string.Equals(textBox.Text, row.Values[culture.Name], StringComparison.Ordinal)) {
            return;
        }

        _tables.SetValue(culture, row.Node.FullKey, textBox.Text);
        row.Values[culture.Name] = textBox.Text;
        TableGrid.Items.Refresh();
        UpdateStatus();
    }

    private void ShowError(string message) {
        MessageBox.Show(this, message, _localizer[EditorStringKeys.Dialog.ErrorTitle], MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static CultureInfo? GetPreferredCulture(string? cultureName) {
        if (string.IsNullOrEmpty(cultureName)) {
            return null;
        }

        try {
            return CultureInfo.GetCultureInfo(cultureName);
        } catch (CultureNotFoundException) {
            return null;
        }
    }

    private string GetAvailableCopyLocalKey(string sourceLocalKey) {
        var currentGroup = GetCurrentGroup();
        if (!currentGroup.Children.ContainsKey(sourceLocalKey)) {
            return sourceLocalKey;
        }

        var candidate = $"{sourceLocalKey}_copy";
        if (!currentGroup.Children.ContainsKey(candidate)) {
            return candidate;
        }

        for (var suffix = 2; ; suffix++) {
            candidate = $"{sourceLocalKey}_copy_{suffix}";
            if (!currentGroup.Children.ContainsKey(candidate)) {
                return candidate;
            }
        }
    }

    private GridState CaptureGridState() {
        var selectedKey = (TableGrid.SelectedItem as LocalizationRow)?.Node.FullKey;
        string? currentColumnKey = null;
        if (TableGrid.CurrentColumn is not null) {
            currentColumnKey = string.Empty;
            foreach (var pair in _columnByCultureName) {
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
                _columnByCultureName.TryGetValue(state.CurrentColumnKey, out column);
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

    private sealed class LocalizationRow(JsonStringTableNode node, string displayName) {

        public JsonStringTableNode Node { get; } = node;

        public Dictionary<string, string> Values { get; } = new(StringComparer.Ordinal);

        public string DisplayName { get; } = displayName;

        public string IconGlyph { get; } = node is JsonStringTableGroup ? "\uE8B7" : "\uE8A5";

        public bool IsGroup { get; } = node is JsonStringTableGroup;
    }
}
