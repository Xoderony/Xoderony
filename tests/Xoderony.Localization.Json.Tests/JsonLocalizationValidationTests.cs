using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace Xoderony.Localization.Json.Tests;

public sealed class JsonLocalizationValidationTests {

    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo SimplifiedChinese = CultureInfo.GetCultureInfo("zh-CN");

    [Fact]
    public void ValidateReportsMissingTranslation() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocaleTableCollection.KeysFileName, """
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

            var collection = JsonLocaleTableCollection.LoadDirectory(directoryPath);
            var issues = JsonLocalizationValidation.Validate(collection, SimplifiedChinese);

            var missing = Assert.Single(issues, issue => issue.Kind == JsonLocalizationIssueKind.MissingTranslation);
            Assert.Equal("menu.quit", missing.EntryKey);
            Assert.Equal(English.Name, missing.Culture.Name);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void ValidateReportsPlaceholderMismatch() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocaleTableCollection.KeysFileName, """
                {
                  "greeting": null
                }
                """);
            WriteAllText(directoryPath, "en-US.json", """
                {
                  "greeting": "Hello {0}"
                }
                """);
            WriteAllText(directoryPath, "zh-CN.json", """
                {
                  "greeting": "你好 {0} {1}"
                }
                """);

            var collection = JsonLocaleTableCollection.LoadDirectory(directoryPath);
            var issues = JsonLocalizationValidation.Validate(collection, English);

            var mismatch = Assert.Single(issues);
            Assert.Equal(JsonLocalizationIssueKind.PlaceholderMismatch, mismatch.Kind);
            Assert.Equal("greeting", mismatch.EntryKey);
            Assert.Equal(SimplifiedChinese.Name, mismatch.Culture.Name);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void ValidateTreatsEscapedBracesAsLiterals() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocaleTableCollection.KeysFileName, """
                {
                  "sample": null
                }
                """);
            WriteAllText(directoryPath, "en-US.json", """
                {
                  "sample": "Use {{0}} as {0}"
                }
                """);
            WriteAllText(directoryPath, "zh-CN.json", """
                {
                  "sample": "将 {{0}} 用作 {0}"
                }
                """);

            var collection = JsonLocaleTableCollection.LoadDirectory(directoryPath);
            var issues = JsonLocalizationValidation.Validate(collection, English);

            Assert.Empty(issues);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void ValidateReportsUnexpectedTranslationKey() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocaleTableCollection.KeysFileName, """
                {
                  "title": null
                }
                """);
            WriteAllText(directoryPath, "en-US.json", """
                {
                  "title": "Title",
                  "orphan.key": "Orphan"
                }
                """);
            WriteAllText(directoryPath, "zh-CN.json", """
                {
                  "title": "标题"
                }
                """);

            var collection = JsonLocaleTableCollection.LoadDirectory(directoryPath);
            var issues = JsonLocalizationValidation.Validate(collection, English);

            var unexpected = Assert.Single(issues);
            Assert.Equal(JsonLocalizationIssueKind.UnexpectedTranslationKey, unexpected.Kind);
            Assert.Equal("orphan.key", unexpected.EntryKey);
            Assert.Equal(English.Name, unexpected.Culture.Name);
        } finally {
            Directory.Delete(directoryPath, recursive: true);
        }
    }

    [Fact]
    public void ValidateReturnsNoIssuesForConsistentTables() {
        var directoryPath = CreateTempDirectory();
        try {
            WriteAllText(directoryPath, JsonLocaleTableCollection.KeysFileName, """
                {
                  "menu": {
                    "start": null
                  }
                }
                """);
            WriteAllText(directoryPath, "en-US.json", """
                {
                  "menu.start": "Start {0}"
                }
                """);
            WriteAllText(directoryPath, "zh-CN.json", """
                {
                  "menu.start": "开始 {0}"
                }
                """);

            var collection = JsonLocaleTableCollection.LoadDirectory(directoryPath);
            var issues = JsonLocalizationValidation.Validate(collection, English);

            Assert.Empty(issues);
            Assert.Equal(["menu.start"], collection.GetTranslationKeys(English).ToArray());
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
