using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using Xoderony;

namespace Xoderony.Localization.Editor;

public partial class App : Application {

    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        var preferences = EditorPreferences.Load();
        MigrateLegacyPreferences(preferences);
        var localizationDirectory = Path.Combine(AppContext.BaseDirectory, "Localization");
        var uiCultureName = preferences.Get<string?>(AppearancePreferenceKeys.UiCulture, null);
        var localizer = EditorLocalizer.Load(localizationDirectory, GetPreferredCulture(uiCultureName));
        var services = new ServiceCollection();
        services.AddSingleton(preferences);
        services.AddSingleton(localizer);
        services.AddDelegateChannel<ProjectWorkspaceChangedHandler>();
        services.AddDelegateChannel<ValidationAnalysisRequestedHandler>();
        services.AddDelegateChannel<ValidationResultsChangedHandler>();
        services.AddSingleton<ProjectWorkspace>();
        services.AddSingleton<ValidationResultStore>();
        services.AddSingleton<IValidationResults>(static provider => provider.GetRequiredService<ValidationResultStore>());
        services.AddSingleton<ValidationFeature>();
        services.AddSingleton<MainWindow>(static provider => new MainWindow(
            provider.GetRequiredService<EditorPreferences>(),
            provider.GetRequiredService<EditorLocalizer>(),
            provider.GetRequiredService<ProjectWorkspace>(),
            provider.GetRequiredService<IValidationResults>(),
            provider.GetRequiredService<IDelegateSubscriber<ValidationResultsChangedHandler>>(),
            provider.GetRequiredService<IDelegateDispatcher<ValidationAnalysisRequestedHandler>>()));

        _services = services.BuildServiceProvider();
        _services.GetRequiredService<ValidationFeature>();
        _services.GetRequiredService<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e) {
        _services?.Dispose();
        base.OnExit(e);
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

    private static void MigrateLegacyPreferences(EditorPreferences preferences) {
        preferences.MigrateKey("lastDirectoryPath", ProjectPreferenceKeys.LastDirectory);
        preferences.MigrateKey("theme", AppearancePreferenceKeys.Theme);
        preferences.MigrateKey("uiCultureName", AppearancePreferenceKeys.UiCulture);
        preferences.MigrateKey("placeholderReferenceCultureName", ValidationPreferenceKeys.PlaceholderReferenceCulture);
        preferences.MigrateKey("windowWidth", WindowPreferenceKeys.Width);
        preferences.MigrateKey("windowHeight", WindowPreferenceKeys.Height);
    }
}
