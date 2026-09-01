using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xoderony.Localization.Editor;
using Xoderony.Localization.Json;
using Xunit;

namespace Xoderony.Localization.Editor.Tests;

public sealed class EditorLocalizerTests : IDisposable {

    private readonly string _directoryPath = Path.Combine(Path.GetTempPath(), "Xoderony.Localization.Editor.Tests", Guid.NewGuid().ToString("N"));

    public EditorLocalizerTests() {
        Directory.CreateDirectory(_directoryPath);
        File.WriteAllText(Path.Combine(_directoryPath, JsonLocaleTableCollection.KeysFileName), """
            {
              "greeting": null,
              "fallback": null,
              "number": null
            }
            """);
        File.WriteAllText(Path.Combine(_directoryPath, "zh-CN.json"), """
            {
              "fallback": "后备",
              "greeting": "你好",
              "number": "{0:N2}"
            }
            """);
        File.WriteAllText(Path.Combine(_directoryPath, "en-US.json"), """
            {
              "fallback": "",
              "greeting": "Hello",
              "number": "{0:N2}"
            }
            """);
    }

    public void Dispose() {
        Directory.Delete(_directoryPath, recursive: true);
    }

    [Fact]
    public void LoadUsesPreferredCultureFormattingAndFallbackValues() {
        var culture = CultureInfo.GetCultureInfo("en-US");

        var localizer = EditorLocalizer.Load(_directoryPath, culture);

        Assert.Equal(culture, localizer.Culture);
        Assert.Equal("Hello", localizer["greeting"]);
        Assert.Equal("后备", localizer["fallback"]);
        Assert.Equal("1,234.50", localizer["number", 1234.5]);
    }

    [Fact]
    public void LoadFallsBackWhenThePreferredLanguageIsUnavailable() {
        var localizer = EditorLocalizer.Load(_directoryPath, CultureInfo.GetCultureInfo("de-DE"));

        Assert.Equal(CultureInfo.GetCultureInfo("zh-CN"), localizer.Culture);
        Assert.Equal("你好", localizer["greeting"]);
    }

    [Fact]
    public void SetCultureUpdatesLookupsAndNotifiesBindings() {
        var localizer = EditorLocalizer.Load(_directoryPath, CultureInfo.GetCultureInfo("en-US"));
        var changedProperties = new List<string?>();
        localizer.PropertyChanged += (_, eventArgs) => changedProperties.Add(eventArgs.PropertyName);

        var changed = localizer.SetCulture(CultureInfo.GetCultureInfo("zh-CN"));

        Assert.True(changed);
        Assert.Equal("你好", localizer["greeting"]);
        Assert.Contains(nameof(EditorLocalizer.Culture), changedProperties);
        Assert.Contains("Item[]", changedProperties);
    }
}
