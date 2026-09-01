using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xoderony.Localization.Json;

namespace Xoderony.Localization.Editor;

public partial class MoveDestinationDialog : Window {

    private readonly string _currentParentKey;
    private readonly string? _sameParentMessage;

    public string SelectedGroupKey { get; private set; } = string.Empty;

    // sameParentMessage 非 null 时禁止选择当前父键组；null 则允许（复制）。
    public MoveDestinationDialog(
        JsonKeyGroup rootGroup,
        JsonKeyNode source,
        string rootDisplayName,
        string title,
        string message,
        string confirmText,
        string cancelText,
        string? sameParentMessage) {
        ArgumentNullException.ThrowIfNull(rootGroup);
        ArgumentNullException.ThrowIfNull(source);

        InitializeComponent();
        Title = title;
        MessageText.Text = message;
        MoveButton.Content = confirmText;
        CancelButton.Content = cancelText;
        _currentParentKey = JsonKeyNode.GetParentKey(source.FullKey);
        _sameParentMessage = sameParentMessage;

        var root = CreateItem(rootGroup, source.FullKey, _currentParentKey, rootDisplayName);
        GroupTree.ItemsSource = new[] { root };
        UpdateSelection(root);
    }

    private void GroupSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
        if (e.NewValue is MoveDestinationItem item) {
            UpdateSelection(item);
        }
    }

    private void GroupTreeDoubleClick(object sender, MouseButtonEventArgs e) {
        if (MoveButton.IsEnabled) {
            DialogResult = true;
        }
    }

    private void ConfirmMove(object sender, RoutedEventArgs e) {
        DialogResult = true;
    }

    private void UpdateSelection(MoveDestinationItem item) {
        SelectedGroupKey = item.Node.FullKey;
        if (_sameParentMessage is not null && string.Equals(SelectedGroupKey, _currentParentKey, StringComparison.Ordinal)) {
            MoveButton.IsEnabled = false;
            ValidationText.Text = _sameParentMessage;
            return;
        }

        MoveButton.IsEnabled = true;
        ValidationText.Text = string.Empty;
    }

    private static MoveDestinationItem CreateItem(JsonKeyGroup group, string excludedGroupKey, string selectedGroupKey, string rootDisplayName) {
        var children = new List<MoveDestinationItem>();
        foreach (var child in group.LocalKeyToChild.Values) {
            if (child is not JsonKeyGroup childGroup || string.Equals(child.FullKey, excludedGroupKey, StringComparison.Ordinal)) {
                continue;
            }

            children.Add(CreateItem(childGroup, excludedGroupKey, selectedGroupKey, rootDisplayName));
        }

        var isRoot = group.FullKey.Length == 0;
        var isOnSelectedPath = selectedGroupKey.Length > 0
            && (string.Equals(group.FullKey, selectedGroupKey, StringComparison.Ordinal)
                || selectedGroupKey.StartsWith($"{group.FullKey}.", StringComparison.Ordinal));
        return new MoveDestinationItem(
            group,
            isRoot ? rootDisplayName : group.LocalKey,
            children,
            isRoot || isOnSelectedPath,
            string.Equals(group.FullKey, selectedGroupKey, StringComparison.Ordinal)
        );
    }

    private sealed class MoveDestinationItem(JsonKeyGroup node, string displayName, IReadOnlyList<MoveDestinationItem> children, bool isExpanded, bool isSelected) {

        public JsonKeyGroup Node { get; } = node;

        public string DisplayName { get; } = displayName;

        public IReadOnlyList<MoveDestinationItem> Children { get; } = children;

        public bool IsExpanded { get; set; } = isExpanded;

        public bool IsSelected { get; set; } = isSelected;
    }
}
