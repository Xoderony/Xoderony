using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Xoderony.ArgumentWrapping;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ArgumentWrappingAnalyzer : DiagnosticAnalyzer {

    internal const string DiagnosticId = "XoderonyArgumentLayout";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        ArgumentWrappingResources.Title,
        ArgumentWrappingResources.Message,
        "Formatting",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: ArgumentWrappingResources.Description
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeArgumentList, SyntaxKind.ArgumentList);
    }

    internal static bool NeedsFormatting(ArgumentListSyntax argumentList, AnalyzerConfigOptionsProvider optionsProvider) {
        if (!ArgumentWrappingFormatter.IsSupported(argumentList)) {
            return false;
        }

        var options = optionsProvider.GetOptions(argumentList.SyntaxTree);
        return !ArgumentWrappingFormatter.IsFormatted(argumentList, options);
    }

    private static void AnalyzeArgumentList(SyntaxNodeAnalysisContext context) {
        var argumentList = (ArgumentListSyntax)context.Node;
        if (!NeedsFormatting(argumentList, context.Options.AnalyzerConfigOptionsProvider)) {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, argumentList.GetLocation()));
    }
}
