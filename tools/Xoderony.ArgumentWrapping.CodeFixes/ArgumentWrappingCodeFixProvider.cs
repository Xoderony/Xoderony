using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace Xoderony.ArgumentWrapping;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArgumentWrappingCodeFixProvider)), Shared]
public sealed class ArgumentWrappingCodeFixProvider : CodeFixProvider {

    public override ImmutableArray<string> FixableDiagnosticIds => [ArgumentWrappingAnalyzer.DiagnosticId];

    public override FixAllProvider GetFixAllProvider() {
        return ArgumentWrappingFixAllProvider.Instance;
    }

    public override Task RegisterCodeFixesAsync(CodeFixContext context) {
        context.RegisterCodeFix(
            CodeAction.Create(
                ArgumentWrappingResources.CodeFixTitle,
                cancellationToken => FormatAsync(context.Document, context.Diagnostics[0], cancellationToken),
                equivalenceKey: ArgumentWrappingAnalyzer.DiagnosticId
            ),
            context.Diagnostics
        );
        return Task.CompletedTask;
    }

    internal static async Task<Document> FormatAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken) {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null) {
            return document;
        }

        var argumentList = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ArgumentListSyntax>();
        if (argumentList is null) {
            return document;
        }

        var options = document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(argumentList.SyntaxTree);
        var source = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        var formattedRoot = ArgumentWrappingFormatter.Format(root, [argumentList.SpanStart], options, source);
        return document.WithSyntaxRoot(formattedRoot);
    }
}
