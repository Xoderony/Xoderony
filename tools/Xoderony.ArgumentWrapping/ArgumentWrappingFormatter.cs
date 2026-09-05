using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Xoderony.ArgumentWrapping;

internal static class ArgumentWrappingFormatter {

    public static bool IsSupported(ArgumentListSyntax argumentList) {
        return argumentList.Parent is InvocationExpressionSyntax or ObjectCreationExpressionSyntax or ImplicitObjectCreationExpressionSyntax;
    }

    public static bool IsFormatted(ArgumentListSyntax argumentList, AnalyzerConfigOptions options) {
        var source = argumentList.SyntaxTree.GetText();
        if (!HasBoundaryLineBreaks(argumentList, source)) {
            return true;
        }

        var arguments = argumentList.Arguments;
        var previousLine = source.Lines.GetLineFromPosition(argumentList.OpenParenToken.SpanStart).LineNumber;
        var closingLine = source.Lines.GetLineFromPosition(argumentList.CloseParenToken.SpanStart).LineNumber;
        var closingIndentation = GetIndentation(source, argumentList.Parent?.SpanStart ?? argumentList.SpanStart);
        var indentation = closingIndentation + GetIndentUnit(options);
        foreach (var argument in arguments) {
            var line = source.Lines.GetLineFromPosition(argument.SpanStart).LineNumber;
            if (line <= previousLine || !HasIndentation(source, argument.SpanStart, indentation)) {
                return false;
            }

            previousLine = source.Lines.GetLineFromPosition(argument.Span.End).LineNumber;
        }

        return closingLine > previousLine && HasIndentation(source, argumentList.CloseParenToken.SpanStart, closingIndentation);
    }

    public static SyntaxNode Format(SyntaxNode root, ImmutableHashSet<int> diagnosticStarts, AnalyzerConfigOptions options, SourceText source) {
        return new ArgumentListFormattingRewriter(diagnosticStarts, options, source).Visit(root)!;
    }

    private static bool HasBoundaryLineBreaks(ArgumentListSyntax argumentList, SourceText source) {
        var previousEnd = argumentList.OpenParenToken.Span.End;
        foreach (var item in argumentList.Arguments.GetWithSeparators()) {
            if (source.Lines.GetLineFromPosition(previousEnd).LineNumber != source.Lines.GetLineFromPosition(item.SpanStart).LineNumber) {
                return true;
            }

            previousEnd = item.Span.End;
        }

        return source.Lines.GetLineFromPosition(previousEnd).LineNumber != source.Lines.GetLineFromPosition(argumentList.CloseParenToken.SpanStart).LineNumber;
    }

    private static bool HasLineBreak(string text) {
        return text.IndexOf('\n') >= 0 || text.IndexOf('\r') >= 0;
    }

    private static ArgumentListSyntax Wrap(ArgumentListSyntax original, ArgumentListSyntax argumentList, AnalyzerConfigOptions options, SourceText source) {
        var newline = GetNewline(source, options);
        var closingIndentation = GetIndentation(source, original.Parent?.SpanStart ?? original.SpanStart);
        var indentation = closingIndentation + GetIndentUnit(options);
        var arguments = argumentList.Arguments;
        var rewrittenArguments = new List<ArgumentSyntax>(arguments.Count);
        var tabWidth = GetTabWidth(options);
        var indentationWidth = GetIndentationWidth(indentation, tabWidth);

        for (var index = 0; index < arguments.Count; index++) {
            var originalIndentation = GetIndentation(source, original.Arguments[index].SpanStart);
            var offset = indentationWidth - GetIndentationWidth(originalIndentation, tabWidth);
            var argument = arguments[index];
            if (offset != 0) {
                var leading = argument.GetLeadingTrivia();
                var trailing = argument.GetTrailingTrivia();
                argument = (ArgumentSyntax)new IndentationRewriter(offset, tabWidth, options).Visit(argument.WithoutLeadingTrivia().WithoutTrailingTrivia())!;
                argument = argument.WithLeadingTrivia(leading).WithTrailingTrivia(trailing);
            }

            var trailingTrivia = new List<SyntaxTrivia>();
            AppendSignificantTrivia(trailingTrivia, argument.GetTrailingTrivia(), indentation, newline, terminateLineComments: index < arguments.SeparatorCount);
            rewrittenArguments.Add(argument
                .WithLeadingTrivia(CreateLeadingTrivia(argument.GetLeadingTrivia(), indentation, newline))
                .WithTrailingTrivia(SyntaxFactory.TriviaList(trailingTrivia)));
        }

        var separators = new List<SyntaxToken>(arguments.SeparatorCount);
        for (var index = 0; index < arguments.SeparatorCount; index++) {
            var separator = arguments.GetSeparator(index);
            var leadingTrivia = new List<SyntaxTrivia>();
            AppendSignificantTrivia(leadingTrivia, separator.LeadingTrivia, indentation, newline, terminateLineComments: true);
            separators.Add(separator
                .WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTrivia))
                .WithTrailingTrivia(CreateInlineTrivia(separator.TrailingTrivia, indentation, newline)));
        }

        var rewrittenList = argumentList
            .WithOpenParenToken(argumentList.OpenParenToken.WithTrailingTrivia(CreateInlineTrivia(argumentList.OpenParenToken.TrailingTrivia, indentation, newline)))
            .WithArguments(SyntaxFactory.SeparatedList(rewrittenArguments, separators))
            .WithCloseParenToken(argumentList.CloseParenToken.WithLeadingTrivia(CreateLeadingTrivia(argumentList.CloseParenToken.LeadingTrivia, closingIndentation, newline)));
        return rewrittenList;
    }

    private static SyntaxTriviaList CreateLeadingTrivia(SyntaxTriviaList original, string indentation, string newline) {
        var trivia = new List<SyntaxTrivia> {
            SyntaxFactory.EndOfLine(newline),
            SyntaxFactory.Whitespace(indentation)
        };
        AppendSignificantTrivia(trivia, original, indentation, newline, terminateLineComments: true);
        return SyntaxFactory.TriviaList(trivia);
    }

    private static SyntaxTriviaList CreateInlineTrivia(SyntaxTriviaList original, string indentation, string newline) {
        var trivia = new List<SyntaxTrivia>();
        AppendSignificantTrivia(trivia, original, indentation, newline, terminateLineComments: false);
        return SyntaxFactory.TriviaList(trivia);
    }

    private static void AppendSignificantTrivia(List<SyntaxTrivia> destination, SyntaxTriviaList original, string indentation, string newline, bool terminateLineComments) {
        if (GetLastDirectiveIndex(original) >= 0) {
            // 指令之间的空白可能属于未激活代码，整段保留，只调整有效代码一侧的边界。
            while (destination.Count > 0 && destination[destination.Count - 1].IsKind(SyntaxKind.WhitespaceTrivia)) {
                destination.RemoveAt(destination.Count - 1);
            }

            if (destination.Count == 0 || !destination[destination.Count - 1].IsKind(SyntaxKind.EndOfLineTrivia)) {
                destination.Add(SyntaxFactory.EndOfLine(newline));
            }

            var first = 0;
            var last = original.Count - 1;
            while (IsWhitespace(original[first])) {
                first++;
            }

            while (IsWhitespace(original[last])) {
                last--;
            }

            for (var index = first; index <= last; index++) {
                destination.Add(original[index]);
            }

            if (terminateLineComments) {
                if (!EndsWithNewline(original[last])) {
                    destination.Add(SyntaxFactory.EndOfLine(newline));
                }

                destination.Add(SyntaxFactory.Whitespace(indentation));
            }

            return;
        }

        var needsSpaceBeforeComment = false;
        var needsNewline = false;
        var endsWithNewline = false;
        foreach (var trivia in original) {
            if (trivia.IsKind(SyntaxKind.WhitespaceTrivia)) {
                needsSpaceBeforeComment = true;
                continue;
            }

            if (trivia.IsKind(SyntaxKind.EndOfLineTrivia)) {
                continue;
            }

            if (needsNewline) {
                if (!endsWithNewline) {
                    destination.Add(SyntaxFactory.EndOfLine(newline));
                }

                destination.Add(SyntaxFactory.Whitespace(indentation));
                needsSpaceBeforeComment = false;
            }

            if (needsSpaceBeforeComment && (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)) &&
                (destination.Count == 0 || !destination[destination.Count - 1].IsKind(SyntaxKind.WhitespaceTrivia))) {
                destination.Add(SyntaxFactory.Whitespace(" "));
            }

            destination.Add(trivia);
            needsNewline = trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                HasLineBreak(trivia.ToFullString());
            endsWithNewline = EndsWithNewline(trivia);
            if (!needsNewline) {
                destination.Add(SyntaxFactory.Whitespace(" "));
            }

            needsSpaceBeforeComment = false;
        }

        if (needsNewline && terminateLineComments) {
            if (!endsWithNewline) {
                destination.Add(SyntaxFactory.EndOfLine(newline));
            }

            destination.Add(SyntaxFactory.Whitespace(indentation));
        }
    }

    private static bool EndsWithNewline(SyntaxTrivia trivia) {
        var text = trivia.ToFullString();
        return text.EndsWith("\n", StringComparison.Ordinal) || text.EndsWith("\r", StringComparison.Ordinal);
    }

    private static bool IsWhitespace(SyntaxTrivia trivia) {
        return trivia.IsKind(SyntaxKind.WhitespaceTrivia) || trivia.IsKind(SyntaxKind.EndOfLineTrivia);
    }

    private static int GetLastDirectiveIndex(SyntaxTriviaList triviaList) {
        for (var index = triviaList.Count - 1; index >= 0; index--) {
            if (triviaList[index].IsDirective || triviaList[index].IsKind(SyntaxKind.DisabledTextTrivia)) {
                return index;
            }
        }

        return -1;
    }

    private static string GetIndentation(SourceText source, int position) {
        var line = source.Lines.GetLineFromPosition(position);
        var end = line.Start;
        while (end < line.End && (source[end] == ' ' || source[end] == '\t')) {
            end++;
        }

        return source.ToString(TextSpan.FromBounds(line.Start, end));
    }

    private static bool HasIndentation(SourceText source, int position, string indentation) {
        var line = source.Lines.GetLineFromPosition(position);
        if (line.End - line.Start < indentation.Length) {
            return false;
        }

        for (var index = 0; index < indentation.Length; index++) {
            if (source[line.Start + index] != indentation[index]) {
                return false;
            }
        }

        var end = line.Start + indentation.Length;
        return end == line.End || (source[end] != ' ' && source[end] != '\t');
    }

    private static int GetTabWidth(AnalyzerConfigOptions options) {
        return options.TryGetValue("tab_width", out var value) && int.TryParse(value, out var width) && width > 0 ? width : 4;
    }

    private static int GetIndentationWidth(string indentation, int tabWidth) {
        var width = 0;
        foreach (var character in indentation) {
            width += character == '\t' ? tabWidth - (width % tabWidth) : 1;
        }

        return width;
    }

    private static string GetIndentUnit(AnalyzerConfigOptions options) {
        if (options.TryGetValue("indent_style", out var style) && string.Equals(style, "tab", StringComparison.OrdinalIgnoreCase)) {
            return "\t";
        }

        if (options.TryGetValue("indent_size", out var value) && int.TryParse(value, out var size) && size > 0) {
            return new string(' ', size);
        }

        return "    ";
    }

    private static string GetNewline(SourceText source, AnalyzerConfigOptions options) {
        if (options.TryGetValue("end_of_line", out var configured)) {
            if (string.Equals(configured, "crlf", StringComparison.OrdinalIgnoreCase)) {
                return "\r\n";
            }

            if (string.Equals(configured, "lf", StringComparison.OrdinalIgnoreCase)) {
                return "\n";
            }
        }

        foreach (var line in source.Lines) {
            var lineBreak = source.ToString(line.SpanIncludingLineBreak);
            if (lineBreak.EndsWith("\r\n", StringComparison.Ordinal)) {
                return "\r\n";
            }

            if (lineBreak.EndsWith("\n", StringComparison.Ordinal)) {
                return "\n";
            }
        }

        return "\n";
    }

    private sealed class ArgumentListFormattingRewriter(ImmutableHashSet<int> diagnosticStarts, AnalyzerConfigOptions options, SourceText source) : CSharpSyntaxRewriter {

        private bool _insideTarget;

        public override SyntaxNode? VisitArgumentList(ArgumentListSyntax node) {
            var wasInsideTarget = _insideTarget;
            _insideTarget |= diagnosticStarts.Contains(node.SpanStart);
            var format = _insideTarget;
            var rewritten = (ArgumentListSyntax)base.VisitArgumentList(node)!;
            _insideTarget = wasInsideTarget;
            if (!IsSupported(node) || !HasBoundaryLineBreaks(node, source) || (!format && ReferenceEquals(node, rewritten))) {
                return rewritten;
            }

            // 用修改前的边界判断换行意图，内层展开不会替外层作出换行决定。
            return Wrap(node, rewritten, options, source);
        }
    }

    private sealed class IndentationRewriter(int offset, int tabWidth, AnalyzerConfigOptions options) : CSharpSyntaxRewriter {

        private bool _atLineStart;

        public override SyntaxToken VisitToken(SyntaxToken token) {
            var leading = RewriteTrivia(token.LeadingTrivia);
            if (_atLineStart && !token.IsMissing) {
                AddIndentation(ref leading, 0);
            }

            // token 文本可能包含原始字符串或逐字字符串，保持原样以保留其值。
            _atLineStart = false;
            var trailing = RewriteTrivia(token.TrailingTrivia);
            return token.WithLeadingTrivia(leading).WithTrailingTrivia(trailing);
        }

        private SyntaxTriviaList RewriteTrivia(SyntaxTriviaList original) {
            var result = new SyntaxTriviaList();
            var directiveEnd = GetLastDirectiveIndex(original);
            for (var index = 0; index < original.Count; index++) {
                var trivia = original[index];
                if (index <= directiveEnd) {
                    result = result.Add(trivia);
                    _atLineStart = EndsWithNewline(trivia);
                    continue;
                }

                if (_atLineStart && trivia.IsKind(SyntaxKind.WhitespaceTrivia)) {
                    AddIndentation(ref result, GetIndentationWidth(trivia.ToString(), tabWidth));
                    continue;
                }

                if (_atLineStart && !trivia.IsKind(SyntaxKind.EndOfLineTrivia) && !trivia.IsDirective) {
                    AddIndentation(ref result, 0);
                }

                result = result.Add(trivia);
                _atLineStart = trivia.IsKind(SyntaxKind.EndOfLineTrivia) || trivia.IsDirective;
            }

            return result;
        }

        private void AddIndentation(ref SyntaxTriviaList result, int originalWidth) {
            var width = Math.Max(0, originalWidth + offset);
            var useTabs = options.TryGetValue("indent_style", out var style) && string.Equals(style, "tab", StringComparison.OrdinalIgnoreCase);
            var indentation = useTabs ? new string('\t', width / tabWidth) + new string(' ', width % tabWidth) : new string(' ', width);
            if (indentation.Length > 0) {
                result = result.Add(SyntaxFactory.Whitespace(indentation));
            }

            _atLineStart = false;
        }
    }
}
