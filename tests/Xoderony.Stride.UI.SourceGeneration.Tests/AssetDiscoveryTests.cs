using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Xunit;

namespace Xoderony.Stride.UI.SourceGeneration.Tests;

public sealed class AssetDiscoveryTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EvaluationRefreshesAssetsAndPreservesExistingItems(bool designTimeBuild) {
        var directory = Path.Combine(Path.GetTempPath(), "XoderonyStrideUI-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(directory, "Assets", "UI"));
        try {
            var library = Path.Combine(directory, "Assets", "UI", "Widgets.sduilib");
            var ignoredPage = Path.Combine(directory, "Assets", "UI", "Page.sduipage");
            var added = Path.Combine(directory, "Assets", "UI", "Added.sduilib");
            File.WriteAllText(library, string.Empty);
            File.WriteAllText(ignoredPage, string.Empty);
            var imports = Path.Combine(AppContext.BaseDirectory, "BuildTransitive", "Xoderony.Stride.UI.SourceGeneration");
            var project = Path.Combine(directory, "Consumer.proj");
            new XDocument(new XElement("Project",
                new XElement("Import", new XAttribute("Project", imports + ".props")),
                new XElement("PropertyGroup", new XElement("RootNamespace", "Expriverse")),
                new XElement("ItemGroup",
                    new XElement("AdditionalFiles", new XAttribute("Include", "Assets/UI/Widgets.sduilib"),
                        new XElement("CustomMetadata", "Preserved"))),
                new XElement("Import", new XAttribute("Project", imports + ".targets")))).Save(project);

            using (var initial = Evaluate(project, designTimeBuild)) {
                var items = initial.RootElement.GetProperty("Items").GetProperty("AdditionalFiles").EnumerateArray().ToArray();
                Assert.Single(items);
                Assert.Equal("Preserved", items.Single(item => item.GetProperty("FullPath").GetString() == library).GetProperty("CustomMetadata").GetString());
                Assert.DoesNotContain(items, item => item.GetProperty("FullPath").GetString() == ignoredPage);
            }

            File.WriteAllText(added, string.Empty);
            using (var afterAddition = Evaluate(project, designTimeBuild)) {
                var items = afterAddition.RootElement.GetProperty("Items").GetProperty("AdditionalFiles");
                Assert.Equal(2, items.GetArrayLength());
                Assert.Contains(items.EnumerateArray(), item => item.GetProperty("FullPath").GetString() == added);
            }

            File.Delete(added);
            using var afterDeletion = Evaluate(project, designTimeBuild);
            Assert.Single(afterDeletion.RootElement.GetProperty("Items").GetProperty("AdditionalFiles").EnumerateArray());
        } finally {
            Directory.Delete(directory, true);
        }
    }

    private static JsonDocument Evaluate(string project, bool designTimeBuild) {
        var start = new ProcessStartInfo("dotnet") {
            WorkingDirectory = Path.GetDirectoryName(project)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        start.ArgumentList.Add("msbuild");
        start.ArgumentList.Add(project);
        start.ArgumentList.Add("-getItem:AdditionalFiles");
        start.ArgumentList.Add("-p:DesignTimeBuild=" + designTimeBuild);
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        Assert.True(process.ExitCode == 0, output + error);
        return JsonDocument.Parse(output);
    }
}
