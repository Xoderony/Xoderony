using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Stride.Core;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using Xunit;

namespace Xoderony.Stride.UI.SourceGeneration.Tests;

public sealed class UILibraryGeneratorTests {
    private const string Library = """
        !UILibraryAsset
        Id: b85d39a7-3218-4a70-949a-f4d4a624821c
        SerializedVersion: {Stride: 2.1.0.1}
        Hierarchy:
          RootParts:
            - !StackPanel ref!! 77191962-223e-43c2-8ab9-6e592d2275f1
          Parts:
            - UIElement: !StackPanel
                Id: 77191962-223e-43c2-8ab9-6e592d2275f1
                Name: Menu
                Children:
                  button: !Button ref!! 69e74524-67f2-499a-a708-321f04eb4207
            - UIElement: !Button
                Id: 69e74524-67f2-499a-a708-321f04eb4207
                Name: Confirm
                Content: !TextBlock ref!! a7f470d6-28d9-40cc-83d4-bea6ddcc3d90
            - UIElement: !TextBlock
                Id: a7f470d6-28d9-40cc-83d4-bea6ddcc3d90
                Name: Label
        """;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CreateReturnsFullyBoundIndependentOrdinaryViews(bool debug) {
        var (_, output) = Run(debug, new TextFile("Widgets.sduilib", Library));
        Assert.DoesNotContain(output.GetDiagnostics(), diagnostic => diagnostic.Severity >= DiagnosticSeverity.Warning);
        var assembly = Load(output);
        var libraryType = assembly.GetType("StrideUIBindings.WidgetsLibrary", true)!;
        var viewType = assembly.GetType("StrideUIBindings.WidgetsLibrary+MenuView", true)!;
        var createMethod = libraryType.GetMethod("CreateMenu")!;
        var viewConstructor = viewType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
        var templateLabel = new TextBlock { Name = "Label", Text = "Template" };
        var templateButton = new Button { Name = "Confirm", Content = templateLabel };
        var templateRoot = new StackPanel { Name = "Menu" };
        templateRoot.Children.Add(templateButton);
        var libraryAsset = new UILibrary();
        libraryAsset.UIElements.Add("Menu", templateRoot);
        var library = Activator.CreateInstance(libraryType)!;
        libraryType.GetProperty("Library")!.SetValue(library, libraryAsset);

        var first = createMethod.Invoke(library, null)!;
        var second = createMethod.Invoke(library, null)!;
        var firstRoot = Assert.IsType<StackPanel>(viewType.GetProperty("Root")!.GetValue(first));
        var firstButton = Assert.IsType<Button>(viewType.GetProperty("Confirm")!.GetValue(first));
        var firstLabel = Assert.IsType<TextBlock>(viewType.GetProperty("Label")!.GetValue(first));
        var secondRoot = Assert.IsType<StackPanel>(viewType.GetProperty("Root")!.GetValue(second));
        var secondButton = Assert.IsType<Button>(viewType.GetProperty("Confirm")!.GetValue(second));
        var secondLabel = Assert.IsType<TextBlock>(viewType.GetProperty("Label")!.GetValue(second));

        Assert.Equal(typeof(object), viewType.BaseType);
        Assert.False(typeof(EntityComponent).IsAssignableFrom(viewType));
        Assert.False(typeof(IDisposable).IsAssignableFrom(viewType));
        Assert.Null(viewType.GetMethod("Start"));
        Assert.Null(viewType.GetMethod("Cancel"));
        Assert.Empty(viewType.GetConstructors());
        Assert.Equal(debug, CallsDebugAssert(createMethod));
        Assert.Equal(debug, CallsDebugAssert(viewConstructor));
        Assert.All(new[] { "Root", "Menu", "Confirm", "Label" }, name => Assert.False(viewType.GetProperty(name)!.CanWrite));
        Assert.Same(firstRoot, viewType.GetProperty("Menu")!.GetValue(first));
        Assert.Same(firstButton, firstRoot.Children[0]);
        Assert.Same(firstLabel, firstButton.Content);
        Assert.NotSame(templateRoot, firstRoot);
        Assert.NotSame(templateButton, firstButton);
        Assert.NotSame(templateLabel, firstLabel);
        Assert.NotSame(firstRoot, secondRoot);
        Assert.NotSame(firstButton, secondButton);
        Assert.NotSame(firstLabel, secondLabel);

        firstLabel.Text = "Changed";
        firstButton.IsEnabled = false;
        Assert.Equal("Template", templateLabel.Text);
        Assert.True(templateButton.IsEnabled);
        Assert.Equal("Template", secondLabel.Text);
        Assert.True(secondButton.IsEnabled);

        var parent = new Grid();
        parent.Children.Add(firstRoot);
        Assert.Same(firstRoot, parent.Children[0]);
    }

    [Fact]
    public void LibraryIsSerializableAndKeepsItsConfiguredResourceContract() {
        var (_, output) = Run(new TextFile("Widgets.sduilib", Library));
        var type = Load(output).GetType("StrideUIBindings.WidgetsLibrary", true)!;
        var viewType = type.GetNestedType("MenuView")!;
        Assert.NotNull(type.GetCustomAttribute<DataContractAttribute>());
        Assert.True(type.IsValueType);
        Assert.NotNull(type.GetCustomAttribute<System.Runtime.CompilerServices.IsReadOnlyAttribute>());
        var property = type.GetProperty("Library")!;
        Assert.NotNull(property.GetCustomAttribute<DataMemberAttribute>());
        Assert.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit), property.SetMethod!.ReturnParameter.GetRequiredCustomModifiers());
        Assert.Empty(viewType.GetConstructors());
    }

    [Fact]
    public void RuntimeAssemblyChecksAreAssertionsWithoutExplicitThrows() {
        var (driver, _) = Run(new TextFile("Widgets.sduilib", Library));
        var source = Assert.Single(driver.GetRunResult().GeneratedTrees).ToString();
        Assert.Contains("global::System.Diagnostics.Debug.Assert(library is not null", source);
        Assert.Contains("global::System.Diagnostics.Debug.Assert(root is not null", source);
        Assert.Contains("global::System.Diagnostics.Debug.Assert(root.Children.Count > 0", source);
        Assert.Contains("global::System.Diagnostics.Debug.Assert(root.Children[0] is global::Stride.UI.Controls.Button", source);
        Assert.Contains("global::System.Diagnostics.Debug.Assert(global::System.String.Equals(", source);
        Assert.Contains("\"WidgetsLibrary requires its configured UILibrary.\"", source);
        Assert.Contains("\"WidgetsLibrary: export Menu must exist as global::Stride.UI.Panels.StackPanel.\"", source);
        Assert.Contains("\"Menu: Root.Children[0] must exist.\"", source);
        Assert.Contains("\"Menu: Root.Children[0] must be global::Stride.UI.Controls.Button.\"", source);
        Assert.Contains("\"Menu: Root.Children[0] must be named Confirm.\"", source);
        Assert.Contains("var node0 = (global::Stride.UI.Controls.Button)root.Children[0];", source);
        Assert.DoesNotContain("throw new global::System.InvalidOperationException", source);
        Assert.DoesNotContain("?? throw", source);
    }

    [Fact]
    public void PageAssetsAreIgnored() {
        var page = Library.Replace("!UILibraryAsset", "!UIPageAsset", StringComparison.Ordinal);
        var (driver, output) = Run(new TextFile("Menu.sduipage", page));
        Assert.Empty(driver.GetRunResult().Diagnostics);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
        Assert.Null(output.GetTypeByMetadataName("StrideUIBindings.MenuView"));
    }

    [Theory]
    [InlineData("Menu", "XSUI003")]
    [InlineData("1Button", "XSUI003")]
    [InlineData("Root", "XSUI003")]
    [InlineData("MenuView", "XSUI003")]
    public void InvalidNamesHaveAssetDiagnostics(string name, string diagnostic) {
        var (driver, _) = Run(new TextFile("Widgets.sduilib", Library.Replace("Name: Confirm", "Name: " + name, StringComparison.Ordinal)));
        var error = Assert.Single(driver.GetRunResult().Diagnostics);
        Assert.Equal(diagnostic, error.Id);
        Assert.Equal("Widgets.sduilib", error.Location.GetLineSpan().Path);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
    }

    [Theory]
    [InlineData("!Button", "!Unknown", "XSUI004")]
    [InlineData("ref!! 69e74524-67f2-499a-a708-321f04eb4207", "ref!! 00000000-0000-0000-0000-000000000000", "XSUI002")]
    [InlineData("Content: !TextBlock ref!! a7f470d6-28d9-40cc-83d4-bea6ddcc3d90", "Content: !StackPanel ref!! 77191962-223e-43c2-8ab9-6e592d2275f1", "XSUI002")]
    [InlineData("Hierarchy:", "Hierarchy: [", "XSUI002")]
    public void InvalidAssetsDoNotEmitPartialBindings(string before, string after, string diagnostic) {
        var (driver, _) = Run(new TextFile("Widgets.sduilib", Library.Replace(before, after, StringComparison.Ordinal)));
        Assert.Equal(diagnostic, Assert.Single(driver.GetRunResult().Diagnostics).Id);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
    }

    [Fact]
    public void LibraryRootsHaveIndependentNameScopes() {
        const string library = """
            !UILibraryAsset
            Hierarchy:
              RootParts:
                - !Button ref!! 00000000-0000-0000-0000-000000000001
                - !Button ref!! 00000000-0000-0000-0000-000000000002
              Parts:
                - UIElement: !Button
                    Id: 00000000-0000-0000-0000-000000000001
                    Name: Accept
                    Content: !TextBlock ref!! 00000000-0000-0000-0000-000000000003
                - UIElement: !Button
                    Id: 00000000-0000-0000-0000-000000000002
                    Name: Cancel
                    Content: !TextBlock ref!! 00000000-0000-0000-0000-000000000004
                - UIElement: !TextBlock
                    Id: 00000000-0000-0000-0000-000000000003
                    Name: Label
                - UIElement: !TextBlock
                    Id: 00000000-0000-0000-0000-000000000004
                    Name: Label
            """;
        var (driver, output) = Run(new TextFile("Widgets.sduilib", library));
        Assert.Empty(driver.GetRunResult().Diagnostics);
        var type = output.GetTypeByMetadataName("StrideUIBindings.WidgetsLibrary")!;
        Assert.NotNull(type.GetTypeMembers("AcceptView").Single().GetMembers("Label").Single());
        Assert.NotNull(type.GetTypeMembers("CancelView").Single().GetMembers("Label").Single());
    }

    [Fact]
    public void NamingAndTypeChangesRemainCompileTimeContracts() {
        var file = new TextFile(Path.Combine(Path.GetTempPath(), "UIConsumer", "Assets", "UI", "Widgets.sduilib"), Library);
        var (_, output) = RunWithOptions(new OptionsProvider(), file);
        var libraryType = output.GetTypeByMetadataName("Expriverse.UI.WidgetsLibrary")!;
        var viewType = libraryType.GetTypeMembers("MenuView").Single();
        Assert.Equal("Stride.UI.Controls.Button", ((IPropertySymbol)viewType.GetMembers("Confirm").Single()).Type.ToDisplayString());

        var renamed = Library.Replace("Name: Confirm", "Name: Accept", StringComparison.Ordinal);
        var compilation = Compilation("class Consumer { void Use(Expriverse.UI.WidgetsLibrary ui) { _ = ui.CreateMenu().Confirm; } }");
        GeneratorDriver driver = CSharpGeneratorDriver.Create([new UIBindingGenerator().AsSourceGenerator()], [file], optionsProvider: new OptionsProvider());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var initial, out _);
        Assert.DoesNotContain(initial.GetDiagnostics(), diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        driver = driver.ReplaceAdditionalText(file, new TextFile(file.Path, renamed)).RunGeneratorsAndUpdateCompilation(compilation, out var changed, out _);
        Assert.Contains(changed.GetDiagnostics(), diagnostic => diagnostic.Id == "CS1061");
    }

    [Theory]
    [InlineData("Assets/UI/bad-name/Widgets.sduilib")]
    [InlineData("Assets/UI/bad.name/Widgets.sduilib")]
    [InlineData("Assets/UI/1Menu/Widgets.sduilib")]
    [InlineData("Assets/UI/bad-name.sduilib")]
    public void InvalidDerivedIdentifiersAreDiagnosed(string path) {
        var file = new TextFile(Path.Combine(Path.GetTempPath(), "UIConsumer", path), Library);
        var (driver, _) = RunWithOptions(new OptionsProvider(), file);
        Assert.Equal("XSUI003", Assert.Single(driver.GetRunResult().Diagnostics).Id);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
    }

    private static (GeneratorDriver Driver, Microsoft.CodeAnalysis.Compilation Output) Run(params AdditionalText[] files) {
        return Run(false, files);
    }

    private static (GeneratorDriver Driver, Microsoft.CodeAnalysis.Compilation Output) Run(bool debug, params AdditionalText[] files) {
        var parseOptions = ParseOptions(debug);
        GeneratorDriver driver = CSharpGeneratorDriver.Create([new UIBindingGenerator().AsSourceGenerator()], files, parseOptions: parseOptions);
        driver = driver.RunGeneratorsAndUpdateCompilation(Compilation(parseOptions: parseOptions), out var output, out _);
        return (driver, output);
    }

    private static (GeneratorDriver Driver, Microsoft.CodeAnalysis.Compilation Output) RunWithOptions(AnalyzerConfigOptionsProvider provider, params AdditionalText[] files) {
        var parseOptions = ParseOptions(false);
        GeneratorDriver driver = CSharpGeneratorDriver.Create([new UIBindingGenerator().AsSourceGenerator()], files, parseOptions: parseOptions, optionsProvider: provider);
        driver = driver.RunGeneratorsAndUpdateCompilation(Compilation(parseOptions: parseOptions), out var output, out _);
        return (driver, output);
    }

    private static CSharpCompilation Compilation(string source = "", CSharpParseOptions? parseOptions = null) {
        parseOptions ??= ParseOptions(false);
        var paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        var references = paths.Append(typeof(UIElement).Assembly.Location).Append(typeof(UILibrary).Assembly.Location).Append(typeof(DataContractAttribute).Assembly.Location)
            .Distinct(StringComparer.OrdinalIgnoreCase).Select(path => MetadataReference.CreateFromFile(path));
        return CSharpCompilation.Create("UIBindingTests" + Guid.NewGuid().ToString("N"), [CSharpSyntaxTree.ParseText(source, parseOptions)], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static CSharpParseOptions ParseOptions(bool debug) {
        return debug ? CSharpParseOptions.Default.WithPreprocessorSymbols("DEBUG") : CSharpParseOptions.Default;
    }

    private static Assembly Load(Microsoft.CodeAnalysis.Compilation compilation) {
        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        Assert.True(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
        stream.Position = 0;
        return AssemblyLoadContext.Default.LoadFromStream(stream);
    }

    private static bool CallsDebugAssert(MethodBase method) {
        var body = method.GetMethodBody();
        var il = body?.GetILAsByteArray() ?? [];
        var opCodes = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => (OpCode)field.GetValue(null)!)
            .ToDictionary(opCode => unchecked((ushort)opCode.Value));
        var offset = 0;
        while (offset < il.Length) {
            ushort value = il[offset++];
            if (value == 0xfe) {
                value = (ushort)(0xfe00 | il[offset++]);
            }

            var opCode = opCodes[value];
            var operandOffset = offset;
            if (opCode.OperandType == OperandType.InlineMethod) {
                var token = BitConverter.ToInt32(il, operandOffset);
                var calledMethod = method.Module.ResolveMethod(token);
                if (calledMethod?.DeclaringType == typeof(System.Diagnostics.Debug) && calledMethod.Name == nameof(System.Diagnostics.Debug.Assert)) {
                    return true;
                }
            }

            offset += OperandSize(opCode.OperandType, il, operandOffset);
        }

        return false;
    }

    private static int OperandSize(OperandType operandType, byte[] il, int operandOffset) {
        return operandType switch {
            OperandType.InlineNone => 0,
            OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or OperandType.ShortInlineVar => 1,
            OperandType.InlineVar => 2,
            OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or OperandType.InlineMethod or OperandType.InlineSig
                or OperandType.InlineString or OperandType.InlineTok or OperandType.InlineType or OperandType.ShortInlineR => 4,
            OperandType.InlineI8 or OperandType.InlineR => 8,
            OperandType.InlineSwitch => 4 + BitConverter.ToInt32(il, operandOffset) * 4,
            _ => throw new InvalidOperationException($"Unsupported IL operand type: {operandType}.")
        };
    }

    private sealed class TextFile(string path, string text) : AdditionalText {
        public override string Path { get; } = path;
        public override SourceText GetText(System.Threading.CancellationToken cancellationToken = default) => SourceText.From(text);
    }

    private sealed class OptionsProvider : AnalyzerConfigOptionsProvider {
        public override AnalyzerConfigOptions GlobalOptions { get; } = new Options(new Dictionary<string, string> {
            ["build_property.RootNamespace"] = "Expriverse",
            ["build_property.MSBuildProjectDirectory"] = Path.Combine(Path.GetTempPath(), "UIConsumer")
        });
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new Options([]);
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => new Options([]);
    }

    private sealed class Options(Dictionary<string, string> values) : AnalyzerConfigOptions {
        public override bool TryGetValue(string key, out string value) => values.TryGetValue(key, out value!);
    }
}
