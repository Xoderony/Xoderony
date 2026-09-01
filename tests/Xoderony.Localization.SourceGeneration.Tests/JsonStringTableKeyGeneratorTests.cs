using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xoderony.Localization.SourceGeneration;
using Xunit;

namespace Xoderony.Localization.SourceGeneration.Tests;

public sealed class JsonStringTableKeyGeneratorTests {

    private const string GenerateMetadataName = "build_metadata.AdditionalFiles.XoderonyLocalizationGenerate";
    private const string NamespaceMetadataName = "build_metadata.AdditionalFiles.XoderonyLocalizationNamespace";
    private const string TypeNameMetadataName = "build_metadata.AdditionalFiles.XoderonyLocalizationTypeName";

    [Fact]
    public void GenerateCreatesNestedKeyTypes() {
        var result = Run("""
            {
              "main_menu": {
                "quit": {
                  "message": null
                },
                "start": null
              }
            }
            """);

        var source = Assert.Single(result.Results[0].GeneratedSources).SourceText.ToString();
        Assert.Contains("namespace Example.Localization;", source, StringComparison.Ordinal);
        Assert.Contains("public static partial class L10nKeys", source, StringComparison.Ordinal);
        Assert.Contains("public static class MainMenu", source, StringComparison.Ordinal);
        Assert.Contains("public const string Message = \"main_menu.quit.message\";", source, StringComparison.Ordinal);
        Assert.Contains("public const string Start = \"main_menu.start\";", source, StringComparison.Ordinal);
    }

    [Fact]
    public void GenerateChangesWhenKeysChange() {
        var first = Run("{ \"menu\": { \"start\": null } }");
        var second = Run("{ \"menu\": { \"quit\": null } }");

        var firstSource = Assert.Single(first.Results[0].GeneratedSources).SourceText.ToString();
        var secondSource = Assert.Single(second.Results[0].GeneratedSources).SourceText.ToString();
        Assert.NotEqual(firstSource, secondSource);
        Assert.Contains("public const string Start = \"menu.start\";", firstSource, StringComparison.Ordinal);
        Assert.Contains("public const string Quit = \"menu.quit\";", secondSource, StringComparison.Ordinal);
    }

    [Fact]
    public void GenerateReportsInvalidJson() {
        var result = Run("{ \"menu\":");

        Assert.Contains(result.Results[0].Diagnostics, static diagnostic => diagnostic.Id == "XLG002");
    }

    [Fact]
    public void GenerateReportsNonNullLeaves() {
        var result = Run("{ \"menu\": { \"start\": \"Start\" } }");

        Assert.Contains(result.Results[0].Diagnostics, static diagnostic => diagnostic.Id == "XLG003");
    }

    [Theory]
    [InlineData("{ \"Main_menu\": null }")]
    [InlineData("{ \"item_2\": { \"name\": null }, \"item2\": { \"name\": null } }")]
    public void GenerateReportsInvalidOrConflictingKeys(string text) {
        var result = Run(text);

        Assert.Contains(result.Results[0].Diagnostics, static diagnostic => diagnostic.Id == "XLG004");
    }

    [Fact]
    public void GenerateReportsInvalidConfiguration() {
        var result = Run("{ \"menu\": { \"start\": null } }", typeName: "1Keys");

        Assert.Contains(result.Results[0].Diagnostics, static diagnostic => diagnostic.Id == "XLG001");
    }

    private static GeneratorDriverRunResult Run(string text, string namespaceName = "Example.Localization", string typeName = "L10nKeys") {
        const string path = "Localization/keys.json";
        var additionalText = new TestAdditionalText(path, text);
        var metadataNameToValue = ImmutableDictionary<string, string>.Empty
            .Add(GenerateMetadataName, "true")
            .Add(NamespaceMetadataName, namespaceName)
            .Add(TypeNameMetadataName, typeName);
        var provider = new TestAnalyzerConfigOptionsProvider(path, metadataNameToValue);
        var generator = new JsonStringTableKeyGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [generator.AsSourceGenerator()],
            [additionalText],
            parseOptions: new CSharpParseOptions(),
            optionsProvider: provider);
        var compilation = CSharpCompilation.Create("GeneratorTests");
        driver = driver.RunGenerators(compilation);
        return driver.GetRunResult();
    }

    private sealed class TestAdditionalText(string path, string text) : AdditionalText {

        public override string Path { get; } = path;

        public override SourceText GetText(System.Threading.CancellationToken cancellationToken = default) {
            return SourceText.From(text);
        }
    }

    private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider {

        private readonly AnalyzerConfigOptions _metadataNameToValue;

        public TestAnalyzerConfigOptionsProvider(string path, ImmutableDictionary<string, string> metadataNameToValue) {
            _metadataNameToValue = new TestAnalyzerConfigOptions(metadataNameToValue);
            Path = path;
        }

        public string Path { get; }

        public override AnalyzerConfigOptions GlobalOptions { get; } = new TestAnalyzerConfigOptions(ImmutableDictionary<string, string>.Empty);

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) {
            return GlobalOptions;
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) {
            return string.Equals(textFile.Path, Path, StringComparison.Ordinal) ? _metadataNameToValue : GlobalOptions;
        }
    }

    private sealed class TestAnalyzerConfigOptions(ImmutableDictionary<string, string> metadataNameToValue) : AnalyzerConfigOptions {

        public override bool TryGetValue(string key, out string value) {
            return metadataNameToValue.TryGetValue(key, out value);
        }
    }
}
