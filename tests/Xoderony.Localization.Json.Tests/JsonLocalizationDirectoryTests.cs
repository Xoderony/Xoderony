using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using Xunit;

namespace Xoderony.Localization.Json.Tests;

public sealed class JsonLocalizationDirectoryTests {

    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo SimplifiedChinese = CultureInfo.GetCultureInfo("zh-CN");

    [Fact]
    public void InvalidTranslationAndLocalePathDoNotDirtyTheCollection() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, "keys.json", "{\"title\":null}");
            WriteAllText(directoryPath, "en-US.json", "{\"title\":\"Title\"}");
            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);
            Assert.Throws<ArgumentNullException>(() => collection.SetTranslation(English, "title", null!));
            Assert.Throws<ArgumentException>(() => collection.AddLocale(SimplifiedChinese, " "));
            Assert.Equal("Title", collection.GetTranslation(English, "title"));
            Assert.Equal(1, collection.CultureCount);
            Assert.False(collection.IsDirty);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void LoadDirectoryUsesSharedKeysAndFlatLocaleValues() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, """
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

            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);

            Assert.Equal(["menu.quit", "menu.start"], collection.GetEntryKeys());
            Assert.Equal(string.Empty, collection.GetTranslation(English, "menu.quit"));
            Assert.Equal("开始", collection.GetTranslation(SimplifiedChinese, "menu.start"));
            Assert.False(collection.IsDirty);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void StructuralChangesUpdateKeysAndEveryLocale() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, """
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

            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);
            collection.AddGroup("menu", "settings");
            collection.AddEntry("menu.settings", "title");
            collection.SetTranslation(English, "menu.settings.title", "Settings");
            collection.Copy("menu.settings", "");
            collection.Rename("settings", "options");
            collection.Move("options", "menu");
            collection.Remove("menu.settings");

            Assert.Equal("Settings", collection.GetTranslation(English, "menu.options.title"));
            Assert.Equal(string.Empty, collection.GetTranslation(SimplifiedChinese, "menu.options.title"));
            Assert.Equal(["menu.options.title", "menu.start"], collection.GetEntryKeys());
            Assert.True(collection.IsDirty);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void CopyAndMoveUsesUnusedLocalKeyOnConflict() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, """
                {
                  "menu": {
                    "title": null
                  },
                  "title": null
                }
                """);
            WriteAllText(directoryPath, "en-US.json", """
                {
                  "menu.title": "Menu Title",
                  "title": "Root Title"
                }
                """);

            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);

            var copiedKey = collection.Copy("menu.title", "");
            Assert.Equal("title_1", copiedKey);
            Assert.Equal("Menu Title", collection.GetTranslation(English, "title_1"));
            Assert.Equal("Root Title", collection.GetTranslation(English, "title"));

            var movedKey = collection.Move("menu.title", "");
            Assert.Equal("title_2", movedKey);
            Assert.Equal("Menu Title", collection.GetTranslation(English, "title_2"));
            Assert.False(collection.RootKeyGroup.TryGet("menu.title", out _));
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
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, "{ \"menu\": { \"start\": null } }");
            WriteAllText(directoryPath, "en-US.json", json);
            Assert.Throws<InvalidDataException>(() => JsonLocalizationDirectory.LoadDirectory(directoryPath));
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
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, json);
            Assert.Throws<InvalidDataException>(() => JsonLocalizationDirectory.LoadDirectory(directoryPath));
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
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, "{ \"menu\": { \"start\": null } }");
            File.WriteAllText(filePath, json);
            Assert.Throws<InvalidDataException>(() => JsonTranslations.Load(filePath));
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void LoadDirectoryAllowsEmptyDirectoryThenAddLocale() {
        var directoryPath = CreateTempDirectory();
        var filePath = Path.Combine(directoryPath, "zh-CN.json");
        try {
            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);

            Assert.Equal(0, collection.CultureCount);
            Assert.Empty(collection.GetEntryKeys());
            Assert.False(collection.IsDirty);

            collection.AddLocale(SimplifiedChinese, filePath);
            collection.AddEntry(string.Empty, "title");
            collection.SetTranslation(SimplifiedChinese, "title", "标题");
            collection.Save();

            Assert.Equal([SimplifiedChinese], collection.GetCultures());
            Assert.Equal(["title"], collection.GetEntryKeys());
            Assert.True(File.Exists(filePath));
            Assert.True(File.Exists(Path.Combine(directoryPath, JsonLocalizationDirectory.KeysFileName)));
            Assert.Contains("标题", File.ReadAllText(filePath), StringComparison.Ordinal);
            Assert.Contains("\"title\": null", File.ReadAllText(Path.Combine(directoryPath, JsonLocalizationDirectory.KeysFileName)), StringComparison.Ordinal);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void SaveWritesIndentedUtf8JsonWithReadableUnicode() {
        var directoryPath = CreateTempDirectory();
        var filePath = Path.Combine(directoryPath, "zh-CN.json");
        try {
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, """
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

            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);
            collection.SetTranslation(SimplifiedChinese, "menu.title", "本地化标题");
            collection.Save();

            var source = File.ReadAllText(filePath);
            Assert.Contains("\n  \"menu.title\":", source, StringComparison.Ordinal);
            Assert.Contains("本地化标题", source, StringComparison.Ordinal);
            Assert.EndsWith("\n", source, StringComparison.Ordinal);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void KeysLoadAllowsNonSnakeCaseLocalKeys() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, """
                {
                  "Start": null
                }
                """);

            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);

            Assert.Equal(["Start"], collection.GetEntryKeys());
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Theory]
    [InlineData("{ \"\": null }")]
    [InlineData("{ \"menu.start\": null }")]
    public void KeysLoadRejectsEmptyOrDottedLocalKeys(string json) {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocalizationDirectory.KeysFileName, json);
            Assert.Throws<InvalidDataException>(() => JsonLocalizationDirectory.LoadDirectory(directoryPath));
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void AddEntryAllowsNonSnakeCaseLocalKey() {
        var directoryPath = CreateTempDirectory();
        try {
            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);
            collection.AddEntry(string.Empty, "Start");
            Assert.Equal(["Start"], collection.GetEntryKeys());
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("menu.start")]
    public void AddEntryRejectsEmptyOrDottedLocalKey(string localKey) {
        var directoryPath = CreateTempDirectory();
        try {
            var collection = JsonLocalizationDirectory.LoadDirectory(directoryPath);
            Assert.Throws<ArgumentException>(() => collection.AddEntry(string.Empty, localKey));
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
