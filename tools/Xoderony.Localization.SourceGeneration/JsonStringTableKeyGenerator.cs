using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xoderony.Localization.Tooling;

namespace Xoderony.Localization.SourceGeneration;

[Generator]
public sealed class JsonStringTableKeyGenerator : IIncrementalGenerator {

    private const string GenerateMetadataName = "build_metadata.AdditionalFiles.XoderonyLocalizationGenerate";
    private const string NamespaceMetadataName = "build_metadata.AdditionalFiles.XoderonyLocalizationNamespace";
    private const string TypeNameMetadataName = "build_metadata.AdditionalFiles.XoderonyLocalizationTypeName";
    private const string DefaultNamespace = "Xoderony.Localization";
    private const string DefaultTypeName = "StringTableKeys";

    private static readonly DiagnosticDescriptor InvalidConfiguration = new(
        "XLG001",
        "Invalid Xoderony localization generator configuration",
        "The Xoderony localization generator configuration is invalid: {0}",
        "Xoderony.Localization.SourceGeneration",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor InvalidJson = new(
        "XLG002",
        "Invalid localization keys JSON",
        "The keys JSON is invalid",
        "Xoderony.Localization.SourceGeneration",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor InvalidKeysShape = new(
        "XLG003",
        "Invalid localization keys JSON shape",
        "The keys JSON shape is invalid: {0}",
        "Xoderony.Localization.SourceGeneration",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor InvalidKeys = new(
        "XLG004",
        "Localization keys cannot generate C# members",
        "The keys JSON cannot generate C# members: {0}",
        "Xoderony.Localization.SourceGeneration",
        DiagnosticSeverity.Error,
        true);

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var keyFiles = context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Where(static pair => IsEnabled(pair.Left, pair.Right))
            .Select(static (pair, cancellationToken) => CreateKeyFile(pair.Left, pair.Right, cancellationToken))
            .Collect();

        context.RegisterSourceOutput(keyFiles, static (productionContext, files) => Generate(productionContext, files));
    }

    private static bool IsEnabled(AdditionalText file, AnalyzerConfigOptionsProvider optionsProvider) {
        var metadataNameToValue = optionsProvider.GetOptions(file);
        return metadataNameToValue.TryGetValue(GenerateMetadataName, out var enabled) && string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase);
    }

    private static KeyFile CreateKeyFile(AdditionalText file, AnalyzerConfigOptionsProvider optionsProvider, System.Threading.CancellationToken cancellationToken) {
        var metadataNameToValue = optionsProvider.GetOptions(file);
        return new KeyFile(
            file.Path,
            GetMetadata(metadataNameToValue, NamespaceMetadataName, DefaultNamespace),
            GetMetadata(metadataNameToValue, TypeNameMetadataName, DefaultTypeName),
            file.GetText(cancellationToken));
    }

    private static string GetMetadata(AnalyzerConfigOptions metadataNameToValue, string name, string defaultValue) {
        return metadataNameToValue.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value) ? value : defaultValue;
    }

    private static void Generate(SourceProductionContext context, ImmutableArray<KeyFile> files) {
        var orderedFiles = new List<KeyFile>(files);
        orderedFiles.Sort(static (left, right) => StringComparer.Ordinal.Compare(left.Path, right.Path));

        var duplicateTypes = new HashSet<string>(StringComparer.Ordinal);
        for (var index = 0; index < orderedFiles.Count; index++) {
            var file = orderedFiles[index];
            var typeIdentity = file.NamespaceName + "." + file.TypeName;
            for (var otherIndex = index + 1; otherIndex < orderedFiles.Count; otherIndex++) {
                var other = orderedFiles[otherIndex];
                if (string.Equals(typeIdentity, other.NamespaceName + "." + other.TypeName, StringComparison.Ordinal)) {
                    duplicateTypes.Add(typeIdentity);
                }
            }
        }

        for (var index = 0; index < orderedFiles.Count; index++) {
            var file = orderedFiles[index];
            var location = CreateLocation(file.Path);
            var typeIdentity = file.NamespaceName + "." + file.TypeName;
            if (duplicateTypes.Contains(typeIdentity)) {
                context.ReportDiagnostic(Diagnostic.Create(InvalidConfiguration, location, $"Multiple AdditionalFiles generate '{typeIdentity}'."));
                continue;
            }

            if (!TryValidateConfiguration(file, location, context)) {
                continue;
            }

            if (file.Text is null) {
                context.ReportDiagnostic(Diagnostic.Create(InvalidJson, location));
                continue;
            }

            try {
                using var document = JsonDocument.Parse(file.Text.ToString(), new JsonDocumentOptions {
                    AllowTrailingCommas = false,
                    CommentHandling = JsonCommentHandling.Disallow
                });
                if (document.RootElement.ValueKind != JsonValueKind.Object) {
                    context.ReportDiagnostic(Diagnostic.Create(InvalidKeysShape, location, "the root must be an object"));
                    continue;
                }

                var keys = new List<string>();
                if (!TryCollectKeys(document.RootElement, string.Empty, keys, out var invalidValueKey)) {
                    context.ReportDiagnostic(Diagnostic.Create(InvalidKeysShape, location, $"the value at '{invalidValueKey}' must be null or an object"));
                    continue;
                }

                try {
                    var source = StringTableKeyGenerator.Generate(keys, file.NamespaceName, file.TypeName);
                    context.AddSource($"Xoderony.Localization.{index}.g.cs", SourceText.From(source, System.Text.Encoding.UTF8));
                } catch (ArgumentException exception) {
                    context.ReportDiagnostic(Diagnostic.Create(InvalidKeys, location, exception.Message));
                }
            } catch (JsonException) {
                context.ReportDiagnostic(Diagnostic.Create(InvalidJson, location));
            }
        }
    }

    private static bool TryValidateConfiguration(KeyFile file, Location location, SourceProductionContext context) {
        try {
            _ = StringTableKeyGenerator.Generate(["key.value"], file.NamespaceName, file.TypeName);
            return true;
        } catch (ArgumentException exception) {
            context.ReportDiagnostic(Diagnostic.Create(InvalidConfiguration, location, exception.Message));
            return false;
        }
    }

    private static bool TryCollectKeys(JsonElement group, string parentKey, List<string> keys, out string invalidValueKey) {
        foreach (var property in group.EnumerateObject()) {
            var key = parentKey.Length == 0 ? property.Name : parentKey + "." + property.Name;
            if (property.Value.ValueKind == JsonValueKind.Null) {
                keys.Add(key);
                continue;
            }

            if (property.Value.ValueKind == JsonValueKind.Object) {
                if (!TryCollectKeys(property.Value, key, keys, out invalidValueKey)) {
                    return false;
                }

                continue;
            }

            invalidValueKey = key;
            return false;
        }

        invalidValueKey = string.Empty;
        return true;
    }

    private static Location CreateLocation(string path) {
        var position = new LinePosition(0, 0);
        return Location.Create(path, new TextSpan(0, 0), new LinePositionSpan(position, position));
    }

    private sealed class KeyFile(string path, string namespaceName, string typeName, SourceText? text) {

        public string Path { get; } = path;

        public string NamespaceName { get; } = namespaceName;

        public string TypeName { get; } = typeName;

        public SourceText? Text { get; } = text;
    }
}
