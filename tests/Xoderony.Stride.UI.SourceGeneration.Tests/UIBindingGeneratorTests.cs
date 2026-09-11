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
using System.Runtime.Loader;
using Xunit;

namespace Xoderony.Stride.UI.SourceGeneration.Tests;

public sealed class UIBindingGeneratorTests {
    private const string Page = """
        !UIPageAsset
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
                  ba8fd686e89bd6700e6c469217984485: !Button ref!! 69e74524-67f2-499a-a708-321f04eb4207
            - UIElement: !Button
                Id: 69e74524-67f2-499a-a708-321f04eb4207
                Name: "Confirm"
                DependencyProperties:
                  51921a3c75fc47a899342ca91616600f~GridBase.RowPropertyKey: 2
                Content: !TextBlock ref!! a7f470d6-28d9-40cc-83d4-bea6ddcc3d90
            - UIElement: !TextBlock
                Id: a7f470d6-28d9-40cc-83d4-bea6ddcc3d90
                Name: 'Label'
                Text: |
                  Name: NotANode
                  !Button ref!! is text here
        """;

    private const string TwoButtonsPage = """
        !UIPageAsset
        Hierarchy:
          RootParts:
            - !StackPanel ref!! 00000000-0000-0000-0000-000000000001
          Parts:
            - UIElement: !StackPanel
                Id: 00000000-0000-0000-0000-000000000001
                Name: Menu
                Children:
                  first: !Button ref!! 00000000-0000-0000-0000-000000000002
                  second: !Button ref!! 00000000-0000-0000-0000-000000000003
            - UIElement: !Button
                Id: 00000000-0000-0000-0000-000000000002
                Name: First
            - UIElement: !Button
                Id: 00000000-0000-0000-0000-000000000003
                Name: Second
        """;

    private const string ScrollViewersPage = """
        !UIPageAsset
        Hierarchy:
          RootParts:
            - !StackPanel ref!! 00000000-0000-0000-0000-000000000001
          Parts:
            - UIElement: !StackPanel
                Id: 00000000-0000-0000-0000-000000000001
                Name: Menu
                Children:
                  first: !ScrollViewer ref!! 00000000-0000-0000-0000-000000000002
                  second: !ScrollViewer ref!! 00000000-0000-0000-0000-000000000003
            - UIElement: !ScrollViewer
                Id: 00000000-0000-0000-0000-000000000002
                Name: FirstScroller
                Content: !Button ref!! 00000000-0000-0000-0000-000000000004
            - UIElement: !ScrollViewer
                Id: 00000000-0000-0000-0000-000000000003
                Name: SecondScroller
                Content: !Button ref!! 00000000-0000-0000-0000-000000000005
            - UIElement: !Button
                Id: 00000000-0000-0000-0000-000000000004
                Name: FirstButton
            - UIElement: !Button
                Id: 00000000-0000-0000-0000-000000000005
                Name: SecondButton
        """;

    [Fact]
    public void PageViewBindsActualStrideInstancesAndCachesThem() {
        var (_, output) = Run(new TextFile("Menu.sduipage", Page));
        var type = Load(output).GetType("StrideUIBindings.MenuView", true)!;
        var root = new StackPanel { Name = "Menu" };
        var label = new TextBlock { Name = "Label" };
        var button = new Button { Name = "Confirm", Content = label };
        root.Children.Add(button);
        var component = new UIComponent { Page = new UIPage { RootElement = root } };
        var binding = AttachPageView(type, component);
        Assert.Same(button, type.GetProperty("Confirm")!.GetValue(binding));
        Assert.Same(label, type.GetProperty("Label")!.GetValue(binding));
        Assert.Same(root, type.GetProperty("Menu")!.GetValue(binding));
        root.Children.Remove(button);
        Assert.Same(button, type.GetProperty("Confirm")!.GetValue(binding));
        component.Page = new UIPage { RootElement = new StackPanel { Name = "Menu" } };
        Assert.Same(root, type.GetProperty("Root")!.GetValue(binding));
    }

    [Fact]
    public void PageViewIgnoresScrollViewerVisualChildren() {
        var (driver, output) = Run(new TextFile("Menu.sduipage", ScrollViewersPage));
        var type = Load(output).GetType("StrideUIBindings.MenuView", true)!;
        var firstButton = new Button { Name = "FirstButton" };
        var secondButton = new Button { Name = "SecondButton" };
        var root = new StackPanel { Name = "Menu" };
        root.Children.Add(new ScrollViewer { Name = "FirstScroller", Content = firstButton });
        root.Children.Add(new ScrollViewer { Name = "SecondScroller", Content = secondButton });
        var binding = AttachPageView(type, new UIComponent { Page = new UIPage { RootElement = root } });
        Assert.Same(firstButton, type.GetProperty("FirstButton")!.GetValue(binding));
        Assert.Same(secondButton, type.GetProperty("SecondButton")!.GetValue(binding));
        Assert.All(driver.GetRunResult().GeneratedTrees, tree => Assert.DoesNotContain("VisualChildren", tree.ToString()));
    }

    [Fact]
    public void LibraryCreatesAndBindsClones() {
        var library = Page.Replace("!UIPageAsset", "!UILibraryAsset", StringComparison.Ordinal);
        var (_, output) = Run(new TextFile("Widgets.sduilib", library));
        var assembly = Load(output);
        var type = assembly.GetType("StrideUIBindings.WidgetsLibrary", true)!;
        var instanceType = assembly.GetType("StrideUIBindings.WidgetsLibrary+MenuView", true)!;
        var template = new StackPanel { Name = "Menu" };
        var button = new Button { Name = "Confirm", Content = new TextBlock { Name = "Label" } };
        template.Children.Add(button);
        var libraryAsset = new UILibrary();
        libraryAsset.UIElements.Add("Menu", template);
        var view = Activator.CreateInstance(type)!;
        type.GetProperty("Library")!.SetValue(view, libraryAsset);
        var binding = type.GetMethod("CreateMenu")!.Invoke(view, null)!;
        var anotherBinding = type.GetMethod("CreateMenu")!.Invoke(view, null)!;
        Assert.IsType(instanceType, binding);
        Assert.NotSame(button, instanceType.GetProperty("Confirm")!.GetValue(binding));
        Assert.NotSame(instanceType.GetProperty("Root")!.GetValue(binding), instanceType.GetProperty("Root")!.GetValue(anotherBinding));
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
        var assembly = Load(output);
        Assert.NotNull(assembly.GetType("StrideUIBindings.WidgetsLibrary+AcceptView")!.GetProperty("Label"));
        Assert.NotNull(assembly.GetType("StrideUIBindings.WidgetsLibrary+CancelView")!.GetProperty("Label"));
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("wrongType")]
    public void IncompatibleRuntimeTreesFailDuringBinding(string failure) {
        var (_, output) = Run(new TextFile("Menu.sduipage", Page));
        var type = Load(output).GetType("StrideUIBindings.MenuView", true)!;
        var root = new StackPanel { Name = "Menu" };
        if (failure == "wrongType") {
            root.Children.Add(new TextBlock { Name = "Confirm" });
        }

        var entity = new Entity();
        entity.Components.Add(new UIComponent { Page = new UIPage { RootElement = root } });
        var binding = (EntityComponent)Activator.CreateInstance(type)!;
        entity.Components.Add(binding);
        var error = Assert.Throws<TargetInvocationException>(() => type.GetMethod("Start")!.Invoke(binding, null));
        var exception = Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("Root.Children[0]", exception.Message);
    }

    [Fact]
    public void SameTypeSiblingOrderMismatchFailsByName() {
        var (_, output) = Run(new TextFile("Menu.sduipage", TwoButtonsPage));
        var type = Load(output).GetType("StrideUIBindings.MenuView", true)!;
        var root = new StackPanel { Name = "Menu" };
        root.Children.Add(new Button { Name = "Second" });
        root.Children.Add(new Button { Name = "First" });
        var entity = new Entity();
        entity.Components.Add(new UIComponent { Page = new UIPage { RootElement = root } });
        var binding = (EntityComponent)Activator.CreateInstance(type)!;
        entity.Components.Add(binding);
        var error = Assert.Throws<TargetInvocationException>(() => type.GetMethod("Start")!.Invoke(binding, null));
        var exception = Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("Root.Children[0]", exception.Message);
        Assert.Contains("expected 'First'", exception.Message);
        Assert.Contains("actual 'Second'", exception.Message);
    }

    [Fact]
    public void MissingContentFailsByLogicalPath() {
        var (_, output) = Run(new TextFile("Menu.sduipage", Page));
        var type = Load(output).GetType("StrideUIBindings.MenuView", true)!;
        var root = new StackPanel { Name = "Menu" };
        root.Children.Add(new Button { Name = "Confirm" });
        var entity = new Entity();
        entity.Components.Add(new UIComponent { Page = new UIPage { RootElement = root } });
        var binding = (EntityComponent)Activator.CreateInstance(type)!;
        entity.Components.Add(binding);
        var error = Assert.Throws<TargetInvocationException>(() => type.GetMethod("Start")!.Invoke(binding, null));
        var exception = Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("Root.Children[0].Content", exception.Message);
    }

    [Fact]
    public void UnsupportedAssetRelationHasTypeDiagnostic() {
        var (driver, _) = Run(new TextFile("Menu.sduipage", Page.Replace("!StackPanel", "!Button", StringComparison.Ordinal)));
        Assert.Equal("XSUI004", Assert.Single(driver.GetRunResult().Diagnostics).Id);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
    }

    [Fact]
    public void PageViewDeclaresItsComponentRequirementAndPriority() {
        var (_, output) = Run(new TextFile("Menu.sduipage", Page));
        var type = Load(output).GetType("StrideUIBindings.MenuView", true)!;
        var requirement = type.GetCustomAttribute<RequireComponentAttribute>();
        var view = (StartupScript)Activator.CreateInstance(type)!;
        Assert.Equal(typeof(UIComponent), requirement!.Type);
        Assert.Equal(-10000, view.Priority);
        var error = Assert.Throws<TargetInvocationException>(() => type.GetProperty("Root")!.GetValue(view));
        Assert.IsType<InvalidOperationException>(error.InnerException);
    }

    [Theory]
    [InlineData("component")]
    [InlineData("page")]
    [InlineData("root")]
    public void PageViewRejectsMissingOrIncompatiblePageConfiguration(string failure) {
        var (_, output) = Run(new TextFile("Menu.sduipage", Page));
        var type = Load(output).GetType("StrideUIBindings.MenuView", true)!;
        var entity = new Entity();
        if (failure == "page") {
            entity.Components.Add(new UIComponent());
        } else if (failure == "root") {
            entity.Components.Add(new UIComponent { Page = new UIPage { RootElement = new TextBlock { Name = "Menu" } } });
        }

        var view = (EntityComponent)Activator.CreateInstance(type)!;
        entity.Components.Add(view);
        var error = Assert.Throws<TargetInvocationException>(() => type.GetMethod("Start")!.Invoke(view, null));
        Assert.IsType<InvalidOperationException>(error.InnerException);
    }

    [Fact]
    public void LibraryIsSerializableAndRequiresAConfiguredResource() {
        var library = Page.Replace("!UIPageAsset", "!UILibraryAsset", StringComparison.Ordinal);
        var (_, output) = Run(new TextFile("Widgets.sduilib", library));
        var type = Load(output).GetType("StrideUIBindings.WidgetsLibrary", true)!;
        var instanceType = type.GetNestedType("MenuView")!;
        var view = Activator.CreateInstance(type)!;
        Assert.NotNull(type.GetCustomAttribute<DataContractAttribute>());
        Assert.True(type.IsValueType);
        Assert.NotNull(type.GetCustomAttribute<System.Runtime.CompilerServices.IsReadOnlyAttribute>());
        var libraryProperty = type.GetProperty("Library")!;
        Assert.NotNull(libraryProperty.GetCustomAttribute<DataMemberAttribute>());
        Assert.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit), libraryProperty.SetMethod!.ReturnParameter.GetRequiredCustomModifiers());
        Assert.Empty(instanceType.GetConstructors());
        var consumer = output.AddSyntaxTrees(CSharpSyntaxTree.ParseText("class Consumer { public StrideUIBindings.WidgetsLibrary Widgets { get; set; } }"));
        Assert.DoesNotContain(consumer.GetDiagnostics(), diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        var error = Assert.Throws<TargetInvocationException>(() => type.GetMethod("CreateMenu")!.Invoke(view, null));
        var exception = Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("WidgetsLibrary", exception.Message);
    }

    [Fact]
    public void LibraryRejectsUnavailableRoot() {
        var library = Page.Replace("!UIPageAsset", "!UILibraryAsset", StringComparison.Ordinal);
        var (_, output) = Run(new TextFile("Widgets.sduilib", library));
        var type = Load(output).GetType("StrideUIBindings.WidgetsLibrary", true)!;
        var view = Activator.CreateInstance(type)!;
        type.GetProperty("Library")!.SetValue(view, new UILibrary());
        var error = Assert.Throws<TargetInvocationException>(() => type.GetMethod("CreateMenu")!.Invoke(view, null));
        var exception = Assert.IsType<InvalidOperationException>(error.InnerException);
        Assert.Contains("Menu", exception.Message);
        Assert.Contains(typeof(StackPanel).FullName!, exception.Message);
    }

    [Theory]
    [InlineData("Menu", "XSUI003")]
    [InlineData("1Button", "XSUI003")]
    [InlineData("Root", "XSUI003")]
    [InlineData("MenuView", "XSUI003")]
    public void InvalidNamesHaveAssetDiagnostics(string name, string diagnostic) {
        var (driver, _) = Run(new TextFile("Menu.sduipage", Page.Replace("\"Confirm\"", "\"" + name + "\"", StringComparison.Ordinal)));
        var error = Assert.Single(driver.GetRunResult().Diagnostics);
        Assert.Equal(diagnostic, error.Id);
        Assert.Equal("Menu.sduipage", error.Location.GetLineSpan().Path);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
    }

    [Theory]
    [InlineData("!Button", "!Unknown", "XSUI004")]
    [InlineData("ref!! 69e74524-67f2-499a-a708-321f04eb4207", "ref!! 00000000-0000-0000-0000-000000000000", "XSUI002")]
    [InlineData("Content: !TextBlock ref!! a7f470d6-28d9-40cc-83d4-bea6ddcc3d90", "Content: !StackPanel ref!! 77191962-223e-43c2-8ab9-6e592d2275f1", "XSUI002")]
    [InlineData("Hierarchy:", "Hierarchy: [", "XSUI002")]
    public void InvalidAssetsDoNotEmitPartialBindings(string before, string after, string diagnostic) {
        var (driver, _) = Run(new TextFile("Menu.sduipage", Page.Replace(before, after, StringComparison.Ordinal)));
        Assert.Equal(diagnostic, Assert.Single(driver.GetRunResult().Diagnostics).Id);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
    }

    [Fact]
    public void KeywordsAreEscaped() {
        var (_, output) = Run(new TextFile("Menu.sduipage", Page.Replace("\"Confirm\"", "class", StringComparison.Ordinal)));
        Assert.DoesNotContain(output.GetDiagnostics(), diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        Assert.NotNull(output.GetTypeByMetadataName("StrideUIBindings.MenuView")!.GetMembers("class").Single());
    }

    [Fact]
    public void SameFilenameInDifferentFoldersReportsCollision() {
        var (driver, _) = Run(new TextFile("A/Menu.sduipage", Page), new TextFile("B/Menu.sduipage", Page));
        Assert.Equal(2, driver.GetRunResult().Diagnostics.Length);
        Assert.All(driver.GetRunResult().Diagnostics, diagnostic => Assert.Equal("XSUI001", diagnostic.Id));
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
    }

    [Theory]
    [InlineData("Assets/UI/LanguageSelection.sduipage", "Expriverse.UI.LanguageSelectionView")]
    [InlineData("Assets/UI/Menu/Page.sduipage", "Expriverse.UI.Menu.PageView")]
    [InlineData("Assets/ui/class/page.sduipage", "Expriverse.ui.class.pageView")]
    [InlineData("Assets/Page.sduipage", "Expriverse.PageView")]
    [InlineData("Assets/UI/Widgets.sduilib", "Expriverse.UI.WidgetsLibrary")]
    [InlineData("Assets/UI/SettingsView.sduipage", "Expriverse.UI.SettingsViewView")]
    [InlineData("Assets/UI/WidgetsLibrary.sduilib", "Expriverse.UI.WidgetsLibraryLibrary")]
    public void DefaultNamesPreserveAssetDirectories(string path, string identity) {
        var text = path.EndsWith(".sduilib", StringComparison.OrdinalIgnoreCase) ? Page.Replace("!UIPageAsset", "!UILibraryAsset", StringComparison.Ordinal) : Page;
        var file = new TextFile(Path.Combine(Path.GetTempPath(), "UIConsumer", path), text);
        var (_, output) = RunWithOptions(new OptionsProvider(), file);
        Assert.NotNull(output.GetTypeByMetadataName(identity));
        Assert.DoesNotContain(output.GetDiagnostics(), diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Theory]
    [InlineData("Assets/UI/bad-name/Page.sduipage")]
    [InlineData("Assets/UI/bad.name/Page.sduipage")]
    [InlineData("Assets/UI/1Menu/Page.sduipage")]
    [InlineData("Assets/UI/bad-name.sduipage")]
    [InlineData("Assets/UI/1Page.sduipage")]
    public void InvalidDerivedIdentifiersAreDiagnosed(string path) {
        var (driver, _) = RunWithOptions(new OptionsProvider(), new TextFile(path, Page));
        Assert.Equal("XSUI003", Assert.Single(driver.GetRunResult().Diagnostics).Id);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
    }

    [Fact]
    public void DifferentAssetDirectoriesDisambiguateFilenames() {
        var (driver, output) = RunWithOptions(new OptionsProvider(), new TextFile("Assets/UI/Page.sduipage", Page), new TextFile("Assets/UI/Menu/Page.sduipage", Page));
        Assert.Empty(driver.GetRunResult().Diagnostics);
        Assert.NotNull(output.GetTypeByMetadataName("Expriverse.UI.PageView"));
        Assert.NotNull(output.GetTypeByMetadataName("Expriverse.UI.Menu.PageView"));
    }

    [Fact]
    public void PageAndLibraryWithSameBasenameHaveDistinctTypes() {
        var (driver, output) = RunWithOptions(new OptionsProvider(), new TextFile("Assets/UI/Page.sduipage", Page),
            new TextFile("Assets/UI/Page.sduilib", Page.Replace("!UIPageAsset", "!UILibraryAsset", StringComparison.Ordinal)));
        Assert.Empty(driver.GetRunResult().Diagnostics);
        Assert.NotNull(output.GetTypeByMetadataName("Expriverse.UI.PageView"));
        Assert.NotNull(output.GetTypeByMetadataName("Expriverse.UI.PageLibrary"));
    }

    [Theory]
    [InlineData("Assets/UI/Menu/Page.sduipage")]
    [InlineData("Assets/UI/Renamed.sduipage")]
    public void MovingOrRenamingAssetsInvalidatesOldTypeReferences(string newPath) {
        var original = new TextFile("Assets/UI/Page.sduipage", Page);
        var compilation = Compilation("class Consumer { void Use(Expriverse.UI.PageView ui) { } }");
        GeneratorDriver driver = CSharpGeneratorDriver.Create([new UIBindingGenerator().AsSourceGenerator()], [original], optionsProvider: new OptionsProvider());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var initial, out _);
        Assert.DoesNotContain(initial.GetDiagnostics(), diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        driver = driver.ReplaceAdditionalText(original, new TextFile(newPath, Page)).RunGeneratorsAndUpdateCompilation(compilation, out var changed, out _);
        Assert.Null(changed.GetTypeByMetadataName("Expriverse.UI.PageView"));
        Assert.Contains(changed.GetDiagnostics(), diagnostic => diagnostic.Id == "CS0234");
    }

    [Fact]
    public void AssetRenameAndRemovalInvalidateOldBusinessCode() {
        var file = new TextFile("Menu.sduipage", Page);
        var compilation = Compilation("class Consumer { public StrideUIBindings.MenuView View { get; set; } void Use() { _ = View.Confirm; } }");
        GeneratorDriver driver = CSharpGeneratorDriver.Create([new UIBindingGenerator().AsSourceGenerator()], [file]);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var initial, out _);
        Assert.DoesNotContain(initial.GetDiagnostics(), diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        var updated = new TextFile(file.Path, Page.Replace("\"Confirm\"", "\"Accept\"", StringComparison.Ordinal));
        driver = driver.ReplaceAdditionalText(file, updated).RunGeneratorsAndUpdateCompilation(compilation, out var renamed, out _);
        Assert.Contains(renamed.GetDiagnostics(), diagnostic => diagnostic.Id == "CS1061");
        driver = driver.RemoveAdditionalTexts([updated]).RunGeneratorsAndUpdateCompilation(compilation, out var removed, out _);
        Assert.Empty(driver.GetRunResult().GeneratedTrees);
        Assert.Contains(removed.GetDiagnostics(), diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void TypeChangesInvalidateIncompatibleAssignments() {
        var (_, output) = Run(new TextFile("Menu.sduipage", Page.Replace("!Button", "!ContentControl", StringComparison.Ordinal)));
        var property = (IPropertySymbol)output.GetTypeByMetadataName("StrideUIBindings.MenuView")!.GetMembers("Confirm").Single();
        Assert.Equal("Stride.UI.Controls.ContentControl", property.Type.ToDisplayString());
        var consumer = output.AddSyntaxTrees(CSharpSyntaxTree.ParseText("class Consumer { Stride.UI.Controls.Button Use(StrideUIBindings.MenuView ui) => ui.Confirm; }"));
        Assert.Contains(consumer.GetDiagnostics(), diagnostic => diagnostic.Id == "CS0266");
    }

    private static (GeneratorDriver Driver, Microsoft.CodeAnalysis.Compilation Output) Run(params AdditionalText[] files) {
        GeneratorDriver driver = CSharpGeneratorDriver.Create([new UIBindingGenerator().AsSourceGenerator()], files);
        driver = driver.RunGeneratorsAndUpdateCompilation(Compilation(), out var output, out _);
        return (driver, output);
    }

    private static (GeneratorDriver Driver, Microsoft.CodeAnalysis.Compilation Output) RunWithOptions(AnalyzerConfigOptionsProvider provider, params AdditionalText[] files) {
        GeneratorDriver driver = CSharpGeneratorDriver.Create([new UIBindingGenerator().AsSourceGenerator()], files, optionsProvider: provider);
        driver = driver.RunGeneratorsAndUpdateCompilation(Compilation(), out var output, out _);
        return (driver, output);
    }

    private static CSharpCompilation Compilation(string source = "") {
        var paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        var references = paths.Append(typeof(UIElement).Assembly.Location).Append(typeof(StartupScript).Assembly.Location).Append(typeof(DataContractAttribute).Assembly.Location)
            .Distinct(StringComparer.OrdinalIgnoreCase).Select(path => MetadataReference.CreateFromFile(path));
        return CSharpCompilation.Create("UIBindingTests" + Guid.NewGuid().ToString("N"), [CSharpSyntaxTree.ParseText(source)], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static object AttachPageView(Type type, UIComponent component) {
        var entity = new Entity();
        entity.Components.Add(component);
        var view = (EntityComponent)Activator.CreateInstance(type)!;
        entity.Components.Add(view);
        type.GetMethod("Start")!.Invoke(view, null);
        return view;
    }

    private static Assembly Load(Microsoft.CodeAnalysis.Compilation compilation) {
        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        Assert.True(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
        stream.Position = 0;
        return AssemblyLoadContext.Default.LoadFromStream(stream);
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
