using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xoderony.ArgumentWrapping;

internal sealed class ArgumentWrappingFixAllProvider : FixAllProvider {

    public static ArgumentWrappingFixAllProvider Instance { get; } = new();

    public override IEnumerable<FixAllScope> GetSupportedFixAllScopes() {
        yield return FixAllScope.Document;
        yield return FixAllScope.Project;
        yield return FixAllScope.Solution;
    }

    public override Task<CodeAction?> GetFixAsync(FixAllContext fixAllContext) {
        return Task.FromResult<CodeAction?>(CodeAction.Create(
            ArgumentWrappingResources.CodeFixTitle,
            cancellationToken => FixAllAsync(fixAllContext, cancellationToken),
            equivalenceKey: ArgumentWrappingAnalyzer.DiagnosticId
        ));
    }

    private static async Task<Solution> FixAllAsync(FixAllContext context, CancellationToken cancellationToken) {
        var solution = context.Solution;
        foreach (var document in GetDocuments(context, solution)) {
            var diagnostics = await context.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
            if (diagnostics.IsDefaultOrEmpty) {
                continue;
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null) {
                continue;
            }

            var spans = diagnostics.Select(static diagnostic => diagnostic.Location.SourceSpan.Start).ToImmutableHashSet();
            var source = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var options = document.Project.AnalyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(root.SyntaxTree);
            var rewrittenRoot = ArgumentWrappingFormatter.Format(root, spans, options, source);
            solution = solution.WithDocumentSyntaxRoot(document.Id, rewrittenRoot);
        }

        return solution;
    }

    private static IEnumerable<Document> GetDocuments(FixAllContext context, Solution solution) {
        return context.Scope switch {
            FixAllScope.Document => [context.Document!],
            FixAllScope.Project => solution.GetProject(context.Project.Id)!.Documents,
            _ => solution.Projects.SelectMany(static project => project.Documents)
        };
    }
}
