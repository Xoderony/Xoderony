using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using Xunit;

namespace Xoderony.Localization.Json.Tests;

public sealed class JsonStringTableCollectionTests {

    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo Japanese = CultureInfo.GetCultureInfo("ja-JP");
    private static readonly CultureInfo SimplifiedChinese = CultureInfo.GetCultureInfo("zh-CN");

    [Fact]
    public void ConstructorMergesKeysAndCompletesMissingValues() {
        var collection = CreateCollection((SimplifiedChinese, "{ \"menu\": { \"start\": \"开始\", \"quit\": \"退出\" } }"), (English, "{ \"menu\": { \"start\": \"Start\" } }"));

        Assert.Equal(["menu.quit", "menu.start"], collection.GetKeys());
        Assert.Equal(string.Empty, collection.GetValue(English, "menu.quit"));
        Assert.True(collection.IsDirty);
    }

    [Fact]
    public void StructuralChangesApplyToEveryLocale() {
        var collection = CreateCollection((English, "{ \"menu\": { \"start\": \"Start\" } }"), (SimplifiedChinese, "{ \"menu\": { \"start\": \"开始\" } }"));

        collection.AddGroup("menu", "settings");
        collection.AddTextEntry("menu.settings", "title");
        collection.SetValue(English, "menu.settings.title", "Settings");
        collection.Copy("menu.settings", "", "preferences");
        collection.Rename("preferences", "options");
        collection.Move("options", "menu");
        collection.Remove("menu.settings");

        Assert.Equal("Settings", collection.GetValue(English, "menu.options.title"));
        Assert.Equal(string.Empty, collection.GetValue(SimplifiedChinese, "menu.options.title"));
        Assert.Equal(["menu.options.title", "menu.start"], collection.GetKeys());
    }

    [Theory]
    [InlineData("{ \"value\": 1 }")]
    [InlineData("{ \"value\": true }")]
    [InlineData("{ \"value\": null }")]
    [InlineData("{ \"value\": [] }")]
    public void ConstructorRejectsNonStringLeafValues(string json) {
        var table = new JsonStringTable(English, "en-US.json", JsonNode.Parse(json)!.AsObject());

        Assert.Throws<InvalidDataException>(() => new JsonStringTableCollection([table]));
    }

    [Theory]
    [InlineData("{ // comment\n \"value\": \"text\" }")]
    [InlineData("{ \"value\": \"text\", }")]
    [InlineData("{ value: \"text\" }")]
    public void LoadRejectsNonStandardJson(string json) {
        var directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directoryPath);
        var filePath = Path.Combine(directoryPath, "en-US.json");
        try {
            File.WriteAllText(filePath, json);
            Assert.Throws<InvalidDataException>(() => JsonStringTable.Load(English, filePath));
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void SaveWritesIndentedUtf8JsonWithReadableUnicode() {
        var directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directoryPath);
        var filePath = Path.Combine(directoryPath, "zh-CN.json");
        try {
            var collection = new JsonStringTableCollection([new JsonStringTable(SimplifiedChinese, filePath, JsonNode.Parse("{ \"menu\": { \"title\": \"标题\" } }")!.AsObject())]);
            collection.SetValue(SimplifiedChinese, "menu.title", "本地化标题");
            collection.Save();

            var source = File.ReadAllText(filePath);
            Assert.Contains("\n  \"menu\":", source, StringComparison.Ordinal);
            Assert.Contains("本地化标题", source, StringComparison.Ordinal);
            Assert.EndsWith("\n", source, StringComparison.Ordinal);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    private static JsonStringTableCollection CreateCollection(params (CultureInfo Culture, string Json)[] sources) {
        var tables = new JsonStringTable[sources.Length];
        for (var index = 0; index < sources.Length; index++) {
            var source = sources[index];
            tables[index] = new JsonStringTable(source.Culture, $"{source.Culture.Name}.json", JsonNode.Parse(source.Json)!.AsObject());
        }

        return new JsonStringTableCollection(tables);
    }
}
