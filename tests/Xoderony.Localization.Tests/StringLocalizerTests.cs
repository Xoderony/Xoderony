using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Xoderony.Localization.Tests;

public class StringLocalizerTests {

    [Fact]
    public void Constructor_CopiesSourceStrings() {
        var keyToLocalizedString = new Dictionary<string, string> {
            ["Greeting"] = "Hello"
        };
        var localizer = new StringLocalizer(CultureInfo.GetCultureInfo("en-US"), keyToLocalizedString);

        keyToLocalizedString["Greeting"] = "Changed";
        keyToLocalizedString["New"] = "New value";

        Assert.Equal("Hello", localizer["Greeting"]);
        Assert.Equal("New", localizer["New"]);
    }

    [Fact]
    public void Constructor_RejectsInvariantCulture() {
        Assert.Throws<ArgumentException>(() => new StringLocalizer(CultureInfo.InvariantCulture, Array.Empty<KeyValuePair<string, string>>()));
    }

    [Fact]
    public void Constructor_RejectsDuplicateKeys() {
        KeyValuePair<string, string>[] localizedStrings = [
            new("Greeting", "Hello"),
            new("Greeting", "Hi")
        ];

        Assert.Throws<ArgumentException>(() => new StringLocalizer(CultureInfo.GetCultureInfo("en-US"), localizedStrings));
    }

    [Fact]
    public void Constructor_RejectsEmptyKeys() {
        KeyValuePair<string, string>[] localizedStrings = [new(string.Empty, "Value")];

        Assert.Throws<ArgumentException>(() => new StringLocalizer(CultureInfo.GetCultureInfo("en-US"), localizedStrings));
    }

    [Fact]
    public void Constructor_RejectsNullValues() {
        KeyValuePair<string, string>[] localizedStrings = [new("Key", null!)];

        Assert.Throws<ArgumentException>(() => new StringLocalizer(CultureInfo.GetCultureInfo("en-US"), localizedStrings));
    }

    [Fact]
    public void Lookup_IsOrdinalAndReturnsMissingKey() {
        KeyValuePair<string, string>[] localizedStrings = [new("Greeting", "Hello")];
        var localizer = new StringLocalizer(CultureInfo.GetCultureInfo("en-US"), localizedStrings);

        Assert.Equal("Hello", localizer["Greeting"]);
        Assert.Equal("greeting", localizer["greeting"]);
        Assert.Equal("Missing", localizer["Missing"]);
    }

    [Fact]
    public void FormattedLookup_UsesTargetCulture() {
        KeyValuePair<string, string>[] localizedStrings = [new("Number", "{0:N2}")];
        var localizer = new StringLocalizer(CultureInfo.GetCultureInfo("de-DE"), localizedStrings);

        Assert.Equal("1.234,50", localizer["Number", 1234.5]);
    }

    [Fact]
    public void AddLayer_LaterLayerOverridesEarlierLayer() {
        var builder = new StringLocalizerBuilder(CultureInfo.GetCultureInfo("en-US"));
        builder.AddLayer([new("Greeting", "Hello"), new("OnlyFirst", "First")]);
        builder.AddLayer([new("Greeting", "Hi"), new("OnlySecond", "Second")]);

        var localizer = builder.Build();

        Assert.Equal("Hi", localizer["Greeting"]);
        Assert.Equal("First", localizer["OnlyFirst"]);
        Assert.Equal("Second", localizer["OnlySecond"]);
    }

    [Fact]
    public void Build_CreatesIndependentSnapshot() {
        var builder = new StringLocalizerBuilder(CultureInfo.GetCultureInfo("en-US"));
        builder.AddLayer([new("Greeting", "Hello")]);
        var first = builder.Build();

        builder.AddLayer([new("Greeting", "Hi")]);
        var second = builder.Build();

        Assert.Equal("Hello", first["Greeting"]);
        Assert.Equal("Hi", second["Greeting"]);
    }
}
