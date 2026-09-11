using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Xoderony.Localization.Json;

public sealed class JsonLocalizationDirectory {

    public const string KeysFileName = "keys.json";

    private readonly string _keysFilePath;
    private readonly JsonKeyGroup _rootKeyGroup;
    private readonly JsonLocales _locales;
    private bool _keysDirty;

    public int CultureCount => _locales.Count;

    public JsonKeyGroup RootKeyGroup => _rootKeyGroup;

    public JsonLocales Locales => _locales;

    public bool IsDirty => _keysDirty || _locales.IsDirty;

    /// <summary>接管 rootKeyGroup 与 locales 的所有权，不重新解析或复制。</summary>
    private JsonLocalizationDirectory(string keysFilePath, JsonKeyGroup rootKeyGroup, JsonLocales locales) {
        Debug.Assert(!string.IsNullOrWhiteSpace(keysFilePath));
        Debug.Assert(rootKeyGroup is not null);
        Debug.Assert(locales is not null);

        _keysFilePath = keysFilePath;
        _rootKeyGroup = rootKeyGroup;
        _locales = locales;
    }

    public static JsonLocalizationDirectory LoadDirectory(string directoryPath) {
        Debug.Assert(!string.IsNullOrWhiteSpace(directoryPath));

        var keysFilePath = Path.Combine(directoryPath, KeysFileName);
        var rootKeyGroup = File.Exists(keysFilePath)
            ? JsonFile.Deserialize<JsonKeyGroup>(keysFilePath)
            : new JsonKeyGroup(string.Empty, string.Empty);
        var locales = JsonLocales.LoadDirectory(directoryPath, KeysFileName);
        return new JsonLocalizationDirectory(keysFilePath, rootKeyGroup, locales);
    }

    public void Save() {
        if (_keysDirty) {
            JsonFile.Write(_keysFilePath, _rootKeyGroup);
            _keysDirty = false;
        }

        _locales.Save();
    }

    public void AddLocale(CultureInfo culture, string filePath) {
        var translations = new JsonTranslations();
        _locales.Add(new JsonLocale(culture, filePath, translations));
        foreach (var key in _rootKeyGroup.GetDescendantEntryKeys()) {
            translations.SetTranslation(key, string.Empty);
        }
    }

    public void AddGroup(string parentGroupKey, string localKey) {
        EnsureValidLocalKey(localKey);

        var parent = _rootKeyGroup.GetGroup(parentGroupKey);
        if (parent.LocalKeyToChild.ContainsKey(localKey)) {
            throw new ArgumentException($"The localization path '{JsonKeyNode.CombineKey(parentGroupKey, localKey)}' already exists.", nameof(localKey));
        }

        parent.AddGroup(localKey);
        _keysDirty = true;
    }

    public void AddEntry(string parentGroupKey, string localKey) {
        EnsureValidLocalKey(localKey);

        var parent = _rootKeyGroup.GetGroup(parentGroupKey);
        if (parent.LocalKeyToChild.ContainsKey(localKey)) {
            throw new ArgumentException($"The localization path '{JsonKeyNode.CombineKey(parentGroupKey, localKey)}' already exists.", nameof(localKey));
        }

        parent.AddEntry(localKey);
        _locales.EnsureEntry(JsonKeyNode.CombineKey(parentGroupKey, localKey));
        _keysDirty = true;
    }

    public void Rename(string fullKey, string localKey) {
        var node = _rootKeyGroup.Get(fullKey);
        EnsureValidLocalKey(localKey);

        if (string.Equals(node.LocalKey, localKey, StringComparison.Ordinal)) {
            return;
        }

        var parentKey = JsonKeyNode.GetParentKey(fullKey);
        var parent = _rootKeyGroup.GetGroup(parentKey);
        if (parent.LocalKeyToChild.ContainsKey(localKey)) {
            throw new ArgumentException($"The localization path '{JsonKeyNode.CombineKey(parentKey, localKey)}' already exists.", nameof(localKey));
        }

        if (!parent.RemoveChild(node.LocalKey, out var removed)) {
            throw new InvalidOperationException($"The keys tree does not contain '{fullKey}'.");
        }

        parent.AddChild(localKey, removed);
        RemapTranslations(fullKey, removed);
        _keysDirty = true;
    }

    public string Move(string fullKey, string parentGroupKey) {
        var node = _rootKeyGroup.Get(fullKey);
        var oldParentKey = JsonKeyNode.GetParentKey(fullKey);
        if (string.Equals(oldParentKey, parentGroupKey, StringComparison.Ordinal)) {
            return fullKey;
        }

        if (string.Equals(fullKey, parentGroupKey, StringComparison.Ordinal) || parentGroupKey.StartsWith($"{fullKey}.", StringComparison.Ordinal)) {
            throw new ArgumentException("A localization node cannot be moved into itself or one of its descendants.", nameof(parentGroupKey));
        }

        var parent = _rootKeyGroup.GetGroup(parentGroupKey);
        var localKey = parent.GetUnusedLocalKey(node.LocalKey);
        var oldParent = _rootKeyGroup.GetGroup(oldParentKey);
        if (!oldParent.RemoveChild(node.LocalKey, out var removed)) {
            throw new InvalidOperationException($"The keys tree does not contain '{fullKey}'.");
        }

        parent.AddChild(localKey, removed);
        RemapTranslations(fullKey, removed);
        _keysDirty = true;
        return removed.FullKey;
    }

    public string Copy(string fullKey, string parentGroupKey) {
        var node = _rootKeyGroup.Get(fullKey);
        if (string.Equals(fullKey, parentGroupKey, StringComparison.Ordinal) || parentGroupKey.StartsWith($"{fullKey}.", StringComparison.Ordinal)) {
            throw new ArgumentException("A localization node cannot be copied into itself or one of its descendants.", nameof(parentGroupKey));
        }

        var parent = _rootKeyGroup.GetGroup(parentGroupKey);
        var localKey = parent.GetUnusedLocalKey(node.LocalKey);
        var clone = node.Clone();
        parent.AddChild(localKey, clone);

        if (node is JsonKeyGroup group) {
            _locales.CopyGroup(group, clone.FullKey);
        } else {
            _locales.CopyEntry(fullKey, clone.FullKey);
        }

        _keysDirty = true;
        return clone.FullKey;
    }

    public void Remove(string fullKey) {
        var node = _rootKeyGroup.Get(fullKey);
        var parentKey = JsonKeyNode.GetParentKey(fullKey);
        var parent = _rootKeyGroup.GetGroup(parentKey);
        if (!parent.RemoveChild(node.LocalKey, out _)) {
            throw new InvalidOperationException($"The keys tree does not contain '{fullKey}'.");
        }

        if (node is JsonKeyGroup) {
            _locales.RemoveGroup(fullKey);
        } else {
            _locales.RemoveEntry(fullKey);
        }

        _keysDirty = true;
    }

    public string GetTranslation(CultureInfo culture, string entryKey) {
        if (_rootKeyGroup.Get(entryKey) is not JsonKeyEntry) {
            throw new ArgumentException($"The localization path '{entryKey}' is a group.", nameof(entryKey));
        }

        return _locales.GetTranslation(culture, entryKey);
    }

    public void SetTranslation(CultureInfo culture, string entryKey, string translation) {
        Debug.Assert(translation is not null);

        if (_rootKeyGroup.Get(entryKey) is not JsonKeyEntry) {
            throw new ArgumentException($"The localization path '{entryKey}' is a group.", nameof(entryKey));
        }

        _locales.SetTranslation(culture, entryKey, translation);
    }

    public IEnumerable<string> GetEntryKeys() {
        return _rootKeyGroup.GetDescendantEntryKeys();
    }

    public IEnumerable<string> GetTranslationKeys(CultureInfo culture) {
        return _locales.GetTranslationKeys(culture);
    }

    public IEnumerable<CultureInfo> GetCultures() {
        return _locales.GetCultures();
    }

    private static void EnsureValidLocalKey(string localKey) {
        if (!JsonKeyNode.IsValidLocalKey(localKey)) {
            throw new ArgumentException($"'{localKey}' cannot be empty or contain '.'.", nameof(localKey));
        }
    }

    private void RemapTranslations(string fromKey, JsonKeyNode removed) {
        if (removed is JsonKeyGroup) {
            _locales.RemapGroup(fromKey, removed.FullKey);
            return;
        }

        _locales.RemapEntry(fromKey, removed.FullKey);
    }
}
