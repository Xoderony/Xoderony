using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xoderony.ArgumentWrapping;
using Xunit;

namespace Xoderony.ArgumentWrapping.Tests;

public sealed class ArgumentWrappingTests {

    [Theory]
    [InlineData("en-US", "Format call arguments", "Multiline call arguments must use consistent line breaks and indentation", "Normalize call arguments only when their list boundaries already contain a line break.")]
    [InlineData("zh-CN", "规范调用实参布局", "多行调用的实参应使用一致的换行与缩进", "仅在调用实参列表的边界已有换行时，规范其布局与缩进。")]
    [InlineData("fr-FR", "Format call arguments", "Multiline call arguments must use consistent line breaks and indentation", "Normalize call arguments only when their list boundaries already contain a line break.")]
    public async Task DiagnosticsUseRequestedLanguageWithEnglishFallback(string cultureName, string title, string message, string description) {
        var diagnostic = Assert.Single(await AnalyzeAsync("Call(first,\nsecond);"));
        var culture = CultureInfo.GetCultureInfo(cultureName);

        Assert.Equal(title, diagnostic.Descriptor.Title.ToString(culture));
        Assert.Equal(message, diagnostic.GetMessage(culture));
        Assert.Equal(description, diagnostic.Descriptor.Description.ToString(culture));
    }

    [Fact]
    public async Task CodeFixTitlesFollowUiCultureWithoutCachingTheFirstLanguage() {
        const string source = "Call(first,\nsecond);";
        var diagnostic = Assert.Single(await AnalyzeAsync(source));
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("LocalizationTests", LanguageNames.CSharp);
        var document = workspace.AddDocument(project.Id, "Example.cs", SourceText.From(source));
        var provider = new ArgumentWrappingCodeFixProvider();
        var originalCulture = CultureInfo.CurrentUICulture;
        try {
            foreach (var cultureName in new[] { "en-US", "zh-CN", "fr-FR", "en-US" }) {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
                var actions = new List<CodeAction>();
                var context = new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None);
                await provider.RegisterCodeFixesAsync(context);

                var selectedAction = Assert.Single(actions);
                var expected = cultureName == "zh-CN" ? "规范调用实参布局" : "Format call arguments";
                Assert.Equal(expected, selectedAction.Title);
                Assert.Equal(ArgumentWrappingAnalyzer.DiagnosticId, selectedAction.EquivalenceKey);
            }
        } finally {
            CultureInfo.CurrentUICulture = originalCulture;
        }
    }

    [Theory]
    [InlineData(ReportDiagnostic.Default, DiagnosticSeverity.Warning)]
    [InlineData(ReportDiagnostic.Warn, DiagnosticSeverity.Warning)]
    [InlineData(ReportDiagnostic.Error, DiagnosticSeverity.Error)]
    [InlineData(ReportDiagnostic.Info, DiagnosticSeverity.Info)]
    [InlineData(ReportDiagnostic.Hidden, DiagnosticSeverity.Hidden)]
    public async Task StandardSeverityControlsDiagnostics(ReportDiagnostic configured, DiagnosticSeverity expected) {
        var diagnostics = await AnalyzeAsync("Call(first,\nsecond);", configured);

        Assert.Equal(expected, Assert.Single(diagnostics).Severity);
    }

    [Fact]
    public async Task StandardSuppressionDisablesDiagnostics() {
        Assert.Empty(await AnalyzeAsync("Call(first,\nsecond);", ReportDiagnostic.Suppress));
    }

    [Theory]
    [InlineData("Call(    first\n);", "Call(\n    first\n);")]
    [InlineData("Call(    \nfirst);", "Call(\n    first\n);")]
    [InlineData("Call(\n    first,\n       second);", "Call(\n    first,\n    second\n);")]
    [InlineData("Call(first,\nsecond, third);", "Call(\n    first,\n    second,\n    third\n);")]
    [InlineData("Call(first\n, second);", "Call(\n    first,\n    second\n);")]
    [InlineData("Call(first, second\n);", "Call(\n    first,\n    second\n);")]
    [InlineData("Call(\n    first,\n       second,\nthird\n);", "Call(\n    first,\n    second,\n    third\n);")]
    [InlineData("Call(\n    first,\n    second,\n    third\n    );", "Call(\n    first,\n    second,\n    third\n);")]
    [InlineData("Call(\n    );", "Call(\n);")]
    [InlineData("new Item(\nfirst, second);", "new Item(\n    first,\n    second\n);")]
    [InlineData("Item item = new(\nfirst, second);", "Item item = new(\n    first,\n    second\n);")]
    public async Task ExplicitLineBreaksTriggerFormattingWithoutConfiguration(string source, string expected) {
        Assert.Single(await AnalyzeAsync(source));
        var result = Format(CSharpSyntaxTree.ParseText(source).GetRoot()).ToFullString();

        Assert.Equal(expected, result);
        Assert.Empty(await AnalyzeAsync(result));
        Assert.Equal(result, Format(CSharpSyntaxTree.ParseText(result).GetRoot()).ToFullString());
    }

    [Theory]
    [InlineData("Call();")]
    [InlineData("Call(\n);")]
    [InlineData("Call(    first,second );")]
    [InlineData("Call(first, second, third, fourth, fifth, sixth);")]
    [InlineData("var v2 = Mathf.Clamp(a, b, c);")]
    [InlineData("var value = new Vector3Int(BinaryPrimitives.ReadInt32LittleEndian(source), BinaryPrimitives.ReadInt32LittleEndian(source[4..]), BinaryPrimitives.ReadInt32LittleEndian(source[8..]));")]
    [InlineData("Call(\n    first\n);")]
    [InlineData("Call(\n    first,\n    second,\n    third\n);")]
    [InlineData("class C { void M(\nint first, int second) { } }")]
    [InlineData("class C : B { C() : base(\nfirst, second) { } }")]
    public async Task SingleLineCorrectAndUnsupportedListsStayUnchanged(string source) {
        Assert.Empty(await AnalyzeAsync(source));
        Assert.Equal(source, Format(CSharpSyntaxTree.ParseText(source).GetRoot()).ToFullString());
    }

    [Theory]
    [InlineData("Call(() => {\n    Work();\n}, second);")]
    [InlineData("Call(@\"first\nsecond\", third);")]
    [InlineData("Call(first +\nsecond, third);")]
    [InlineData("Outer(Inner(\n    first,\n    second\n), third);")]
    public async Task ArgumentInternalLineBreaksDoNotExpandOuterList(string source) {
        Assert.Empty(await AnalyzeAsync(source));
        Assert.Equal(source, Format(CSharpSyntaxTree.ParseText(source).GetRoot()).ToFullString());
    }

    [Fact]
    public async Task RawStringInternalLineBreaksDoNotTriggerFormatting() {
        const string source = """"
            Call("""
                first
                  second
                """, other);
            """";

        Assert.Empty(await AnalyzeAsync(source));
        Assert.Equal(source, Format(CSharpSyntaxTree.ParseText(source).GetRoot()).ToFullString());
    }

    [Fact]
    public async Task ExplicitlyWrappedLambdaPreservesRelativeIndentation() {
        const string source = """
            Call(() => {
                Work();
            }, second
            );
            """;
        var result = Format(CSharpSyntaxTree.ParseText(source).GetRoot()).ToFullString();

        Assert.Equal("""
            Call(
                () => {
                    Work();
                },
                second
            );
            """, result);
        Assert.Empty(await AnalyzeAsync(result));
    }

    [Fact]
    public async Task FormattingPreservesMultilineStringTokens() {
        const string source = """"
            Call(
            """
                first
                  second
                """, @"third
            fourth");
            """";
        var tree = CSharpSyntaxTree.ParseText(source);
        var result = Format(tree.GetRoot()).ToFullString();
        var reparsed = CSharpSyntaxTree.ParseText(result);

        Assert.NotEqual(source, result);
        Assert.Equal(tree.GetRoot().DescendantTokens().Select(static token => token.Text),
            reparsed.GetRoot().DescendantTokens().Select(static token => token.Text));
        Assert.DoesNotContain(reparsed.GetDiagnostics(), static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        Assert.Empty(await AnalyzeAsync(result));
    }

    [Fact]
    public async Task LineCommentsTriggerMultilineLayoutAndRemainAttached() {
        const string source = "Call(first, // retained\nsecond);";
        var result = Format(CSharpSyntaxTree.ParseText(source).GetRoot()).ToFullString();

        Assert.Equal("Call(\n    first, // retained\n    second\n);", result);
        Assert.Empty(await AnalyzeAsync(result));
        Assert.Equal(result, Format(CSharpSyntaxTree.ParseText(result).GetRoot()).ToFullString());
    }

    [Theory]
    [InlineData("Call(\n    /* retained */ first,\n    second,\n    third\n);")]
    [InlineData("Call(first, // one\n// two\nsecond);")]
    [InlineData("Call(first // before comma\n, second);")]
    [InlineData("Call(first, /* multi\nline */ second);")]
    [InlineData("Call(first,\n#if X\nsecond,\n#endif\nthird);")]
    public async Task MultilineTriviaSurvivesFormattingWithoutRepeatedDiagnostics(string source) {
        var tree = CSharpSyntaxTree.ParseText(source);
        var result = Format(tree.GetRoot()).ToFullString();
        var reparsed = CSharpSyntaxTree.ParseText(result);

        Assert.Equal(tree.GetRoot().DescendantTokens().Select(static token => token.Text),
            reparsed.GetRoot().DescendantTokens().Select(static token => token.Text));
        Assert.Equal(tree.GetRoot().DescendantTrivia().Where(static trivia => !trivia.IsKind(SyntaxKind.WhitespaceTrivia) && !trivia.IsKind(SyntaxKind.EndOfLineTrivia)).Select(static trivia => trivia.ToFullString()),
            reparsed.GetRoot().DescendantTrivia().Where(static trivia => !trivia.IsKind(SyntaxKind.WhitespaceTrivia) && !trivia.IsKind(SyntaxKind.EndOfLineTrivia)).Select(static trivia => trivia.ToFullString()));
        Assert.DoesNotContain(reparsed.GetDiagnostics(), static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        Assert.Empty(await AnalyzeAsync(result));
        Assert.Equal(result, Format(reparsed.GetRoot()).ToFullString());
    }

    [Theory]
    [InlineData("space", "2", "lf", "  Call(\n    first,\n    second\n  );")]
    [InlineData("tab", "4", "crlf", "  Call(\r\n  \tfirst,\r\n  \tsecond\r\n  );")]
    public async Task FormattingUsesStandardIndentationAndLineEndingOptions(string style, string size, string newline, string expected) {
        const string source = "  Call(first,\nsecond);";
        var options = new TestOptions(("indent_style", style), ("indent_size", size), ("tab_width", "4"), ("end_of_line", newline));
        var result = Format(CSharpSyntaxTree.ParseText(source).GetRoot(), options).ToFullString();

        Assert.Equal(expected, result);
        Assert.Empty(await AnalyzeAsync(result, options: options));
    }

    [Theory]
    [InlineData("Outer(Inner(\nfirst, second), third);", "Outer(Inner(\n    first,\n    second\n), third);")]
    [InlineData("Outer(Inner(first,\nsecond), third);", "Outer(Inner(\n    first,\n    second\n), third);")]
    public async Task FixingInnerListDoesNotExpandOuterList(string source, string expected) {
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetRoot();
        var inner = root.DescendantNodes().OfType<InvocationExpressionSyntax>().Single(static invocation => invocation.Expression.ToString() == "Inner");
        var single = ArgumentWrappingFormatter.Format(root, ImmutableHashSet.Create(inner.ArgumentList.SpanStart), new TestOptions(), tree.GetText());
        var all = Format(root);

        Assert.Equal(expected, single.ToFullString());
        Assert.Equal(expected, all.ToFullString());
        Assert.Empty(await AnalyzeAsync(expected));
    }

    [Fact]
    public async Task FixAllFormatsExplicitlyWrappedNestedCallsTogether() {
        const string source = """
            Outer(
            Inner(first,
            second), new Item(third,
            fourth));
            """;
        var result = Format(CSharpSyntaxTree.ParseText(source).GetRoot()).ToFullString();

        Assert.Equal("""
            Outer(
                Inner(
                    first,
                    second
                ),
                new Item(
                    third,
                    fourth
                )
            );
            """, result);
        Assert.Empty(await AnalyzeAsync(result));
        Assert.Equal(result, Format(CSharpSyntaxTree.ParseText(result).GetRoot()).ToFullString());
    }

    [Fact]
    public async Task FixingOuterListKeepsSingleLineInnerList() {
        const string source = "Outer(Inner( first,second ),\nthird);";
        var result = Format(CSharpSyntaxTree.ParseText(source).GetRoot()).ToFullString();

        Assert.Equal("Outer(\n    Inner( first,second ),\n    third\n);", result);
        Assert.Empty(await AnalyzeAsync(result));
    }

    [Fact]
    public void SingleFixLeavesUnrelatedListsUntouched() {
        const string source = "First(\none, two);\nSecond(\none, two);";
        var tree = CSharpSyntaxTree.ParseText(source);
        var root = tree.GetRoot();
        var first = root.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
        var result = ArgumentWrappingFormatter.Format(root, ImmutableHashSet.Create(first.ArgumentList.SpanStart), new TestOptions(), tree.GetText());

        Assert.Equal("First(\n    one,\n    two\n);\nSecond(\none, two);", result.ToFullString());
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source, ReportDiagnostic configured = ReportDiagnostic.Default, AnalyzerConfigOptions? options = null) {
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithSpecificDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic>.Empty.Add(ArgumentWrappingAnalyzer.DiagnosticId, configured));
        var compilation = CSharpCompilation.Create("ArgumentWrappingTests", [tree], options: compilationOptions);
        var analyzerOptions = new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, new TestOptionsProvider(options ?? new TestOptions()));

        return await compilation.WithAnalyzers([new ArgumentWrappingAnalyzer()], analyzerOptions).GetAnalyzerDiagnosticsAsync();
    }

    private static SyntaxNode Format(SyntaxNode root, AnalyzerConfigOptions? options = null) {
        var starts = root.DescendantNodes().OfType<ArgumentListSyntax>()
            .Where(ArgumentWrappingFormatter.IsSupported)
            .Select(static list => list.SpanStart).ToImmutableHashSet();
        return ArgumentWrappingFormatter.Format(root, starts, options ?? new TestOptions(), root.SyntaxTree.GetText());
    }

    private sealed class TestOptionsProvider(AnalyzerConfigOptions options) : AnalyzerConfigOptionsProvider {

        public override AnalyzerConfigOptions GlobalOptions => options;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) {
            return options;
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) {
            return options;
        }
    }

    private sealed class TestOptions(params (string Key, string Value)[] values) : AnalyzerConfigOptions {

        public override bool TryGetValue(string key, out string value) {
            foreach (var pair in values) {
                if (pair.Key == key) {
                    value = pair.Value;
                    return true;
                }
            }

            value = string.Empty;
            return false;
        }
    }
}
