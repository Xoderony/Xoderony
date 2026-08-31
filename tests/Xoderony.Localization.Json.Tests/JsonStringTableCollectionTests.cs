using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using Xunit;

namespace Xoderony.Localization.Json.Tests;

public sealed class JsonStringTableCollectionTests {

    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo SimplifiedChinese = CultureInfo.GetCultureInfo("zh-CN");

    [Fact]
    public void LoadDirectoryUsesSharedKeysAndFlatLocaleValues() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonStringTableCollection.KeysFileName, """
                {
                  "menu": {
                    "start": null,
                    "quit": null
                  }
                }
                """);
            WriteAllText(directoryPath, "zh-CN.json", """
                {
                  "menu.quit": "退出",
                  "menu.start": "开始"
                }
                """);
            WriteAllText(directoryPath, "en-US.json", """
                {
                  "menu.start": "Start"
                }
                """);

            var collection = JsonStringTableCollection.LoadDirectory(directoryPath);

            Assert.Equal(["menu.quit", "menu.start"], collection.GetKeys());
            Assert.Equal(string.Empty, collection.GetValue(English, "menu.quit"));
            Assert.Equal("开始", collection.GetValue(SimplifiedChinese, "menu.start"));
            Assert.False(collection.IsDirty);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void StructuralChangesUpdateKeysAndEveryLocale() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonStringTableCollection.KeysFileName, """
                {
                  "menu": {
                    "start": null
                  }
                }
                """);
            WriteAllText(directoryPath, "en-US.json", """
                {
                  "menu.start": "Start"
                }
                """);
            WriteAllText(directoryPath, "zh-CN.json", """
                {
                  "menu.start": "开始"
                }
                """);

            var collection = JsonStringTableCollection.LoadDirectory(directoryPath);
            collection.AddGroup("menu", "settings");
            collection.AddEntry("menu.settings", "title");
            collection.SetValue(English, "menu.settings.title", "Settings");
            collection.Copy("menu.settings", "", "preferences");
            collection.Rename("preferences", "options");
            collection.Move("options", "menu");
            collection.Remove("menu.settings");

            Assert.Equal("Settings", collection.GetValue(English, "menu.options.title"));
            Assert.Equal(string.Empty, collection.GetValue(SimplifiedChinese, "menu.options.title"));
            Assert.Equal(["menu.options.title", "menu.start"], collection.GetKeys());
            Assert.True(collection.IsDirty);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Theory]
    [InlineData("{ \"menu.start\": 1 }")]
    [InlineData("{ \"menu.start\": true }")]
    [InlineData("{ \"menu.start\": null }")]
    [InlineData("{ \"menu.start\": {} }")]
    public void LocaleLoadRejectsNonStringValues(string json) {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonStringTableCollection.KeysFileName, "{ \"menu\": { \"start\": null } }");
            WriteAllText(directoryPath, "en-US.json", json);
            Assert.Throws<InvalidDataException>(() => JsonStringTableCollection.LoadDirectory(directoryPath));
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Theory]
    [InlineData("{ \"menu\": { \"start\": \"text\" } }")]
    [InlineData("{ \"menu\": { \"start\": 1 } }")]
    [InlineData("{ \"menu\": [] }")]
    public void KeysLoadRejectsNonNullNonObjectLeaves(string json) {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonStringTableCollection.KeysFileName, json);
            Assert.Throws<InvalidDataException>(() => JsonStringTableCollection.LoadDirectory(directoryPath));
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Theory]
    [InlineData("{ // comment\n \"menu.start\": \"text\" }")]
    [InlineData("{ \"menu.start\": \"text\", }")]
    [InlineData("{ menu.start: \"text\" }")]
    public void LocaleLoadRejectsNonStandardJson(string json) {
        var directoryPath = CreateTempDirectory();
        var filePath = Path.Combine(directoryPath, "en-US.json");
        try {
            WriteAllText(directoryPath, JsonStringTableCollection.KeysFileName, "{ \"menu\": { \"start\": null } }");
            File.WriteAllText(filePath, json);
            Assert.Throws<InvalidDataException>(() => JsonStringTable.Load(English, filePath));
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void LoadDirectoryAllowsEmptyDirectoryThenAddLocale() {
        var directoryPath = CreateTempDirectory();
        var filePath = Path.Combine(directoryPath, "zh-CN.json");
        try {
            var collection = JsonStringTableCollection.LoadDirectory(directoryPath);

            Assert.Empty(collection.Cultures);
            Assert.Empty(collection.GetKeys());
            Assert.False(collection.IsDirty);

            collection.AddLocale(SimplifiedChinese, filePath);
            collection.AddEntry(string.Empty, "title");
            collection.SetValue(SimplifiedChinese, "title", "标题");
            collection.Save();

            Assert.Equal([SimplifiedChinese], collection.Cultures);
            Assert.Equal(["title"], collection.GetKeys());
            Assert.True(File.Exists(filePath));
            Assert.True(File.Exists(Path.Combine(directoryPath, JsonStringTableCollection.KeysFileName)));
            Assert.Contains("标题", File.ReadAllText(filePath), StringComparison.Ordinal);
            Assert.Contains("\"title\": null", File.ReadAllText(Path.Combine(directoryPath, JsonStringTableCollection.KeysFileName)), StringComparison.Ordinal);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void SaveWritesIndentedUtf8JsonWithReadableUnicode() {
        var directoryPath = CreateTempDirectory();
        var filePath = Path.Combine(directoryPath, "zh-CN.json");
        try {
            WriteAllText(directoryPath, JsonStringTableCollection.KeysFileName, """
                {
                  "menu": {
                    "title": null
                  }
                }
                """);
            WriteAllText(directoryPath, "zh-CN.json", """
                {
                  "menu.title": "标题"
                }
                """);

            var collection = JsonStringTableCollection.LoadDirectory(directoryPath);
            collection.SetValue(SimplifiedChinese, "menu.title", "本地化标题");
            collection.Save();

            var source = File.ReadAllText(filePath);
            Assert.Contains("\n  \"menu.title\":", source, StringComparison.Ordinal);
            Assert.Contains("本地化标题", source, StringComparison.Ordinal);
            Assert.EndsWith("\n", source, StringComparison.Ordinal);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    private static string CreateTempDirectory() {
        var directoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directoryPath);
        return directoryPath;
    }

    private static void WriteAllText(string directoryPath, string fileName, string contents) {
        File.WriteAllText(Path.Combine(directoryPath, fileName), contents);
    }
}
