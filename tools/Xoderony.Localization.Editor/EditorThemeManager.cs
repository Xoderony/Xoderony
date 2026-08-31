using System;
using System.Windows;

namespace Xoderony.Localization.Editor;

internal enum EditorTheme {
    Light,
    Dark
}

internal static class EditorThemeManager {

    private static readonly Uri DarkThemeSource = new("/Xoderony.Localization.Editor;component/Styles/VisualStudioDark.xaml", UriKind.RelativeOrAbsolute);
    private static ResourceDictionary? _darkThemeResources;

    public static void SetTheme(EditorTheme theme) {
        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
        if (_darkThemeResources is not null) {
            mergedDictionaries.Remove(_darkThemeResources);
            _darkThemeResources = null;
        }

        if (theme == EditorTheme.Dark) {
            _darkThemeResources = new ResourceDictionary { Source = DarkThemeSource };
            mergedDictionaries.Add(_darkThemeResources);
        }
    }
}
