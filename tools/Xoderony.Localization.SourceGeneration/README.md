# Xoderony.Localization.SourceGeneration

`Xoderony.Localization.SourceGeneration` 是面向标准 .NET/MSBuild 项目的可选增量源生成器。它从显式标记的 `keys.json` 生成嵌套的 `public const string` 键类型；JSON 中对象表示键组，`null` 表示词条。

```xml
<ItemGroup>
  <PackageReference Include="Xoderony.Localization.SourceGeneration" Version="0.1.0" PrivateAssets="all" />
  <AdditionalFiles Include="Localization\keys.json">
    <XoderonyLocalizationGenerate>true</XoderonyLocalizationGenerate>
    <XoderonyLocalizationNamespace>Example.Localization</XoderonyLocalizationNamespace>
    <XoderonyLocalizationTypeName>StringTableKeys</XoderonyLocalizationTypeName>
  </AdditionalFiles>
</ItemGroup>
```

省略 `XoderonyLocalizationNamespace` 时使用项目的 `RootNamespace`（无此值时为 `Xoderony.Localization`）；省略 `XoderonyLocalizationTypeName` 时使用 `StringTableKeys`。

```json
{
  "main_menu": {
    "start": null
  }
}
```

生成后可直接使用 `StringTableKeys.MainMenu.Start`，其值为 `"main_menu.start"`。键结构、命名空间或类型名的改变都是 API 变更；生成器不会重写调用方源码。

此阶段仅支持标准 .NET/MSBuild 的 analyzer 分发，不支持 Unity 的 RoslynAnalyzer DLL 或 `.additionalfile` 工作流。
