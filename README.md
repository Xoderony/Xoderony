# Xoderony

面向 .NET 的模块化基础库，各模块独立打包为 NuGet。源码位于 `src/`，测试位于 `tests/`，开发工具位于 `tools/`。

## 包

- `Xoderony.Channels`
- `Xoderony.Collections`
- `Xoderony.Localization`
- `Xoderony.Localization.Json`
- `Xoderony.Localization.CodeGeneration`
- `Xoderony.Logging`
- `Xoderony.Modding`
- `Xoderony.Numerics`
- `Xoderony.ObjectPool`
- `Xoderony.Serialization`
- `Xoderony.Text`
- `Xoderony.Localization.CodeGeneration`：本地化代码生成的共享构建期工具。
- `Xoderony.Localization.SourceGeneration`：可选的本地化键源生成器，以 NuGet analyzer 分发。
- `Xoderony.Stride.UI.SourceGeneration`：从 Stride UI 资产生成强类型实例绑定，详见[使用说明](tools/Xoderony.Stride.UI.SourceGeneration/README.md)。

## 本地化组件边界

- `Xoderony.Localization` 是运行时本地化契约与字符串查询实现，不依赖具体存储格式。
- `Xoderony.Localization.Json` 负责 JSON 语言表的读写、结构编辑与校验，不承担字符串查询。
- `Xoderony.Localization.CodeGeneration` 是不依赖 Roslyn 的 C# 键源码构造器，供编辑器和源生成器复用。
- `Xoderony.Localization.SourceGeneration` 是 Roslyn 适配层，将 `AdditionalFiles` 转换为键后调用 CodeGeneration；NuGet 包会携带 CodeGeneration 作为 analyzer 依赖。
- `Xoderony.Localization.Editor` 是 WPF 应用，组合 Localization、Json 和 CodeGeneration，并仅以 analyzer 方式使用 SourceGeneration 生成自身的界面字符串键。

依赖方向固定为：

```text
Xoderony.Localization.Editor
├─ Xoderony.Localization
├─ Xoderony.Localization.Json
├─ Xoderony.Localization.CodeGeneration
└─ analyzer: Xoderony.Localization.SourceGeneration
              └─ Xoderony.Localization.CodeGeneration
```

`Xoderony.Localization.Json` 与 `Xoderony.Localization` 保持平行，由应用层组合；SourceGeneration 不引用 .NET 10 运行时库。

## 使用与打包

仓库内部通过 `ProjectReference` 组织项目；外部调用方只通过 `PackageReference` 使用版本化包。运行时库使用 .NET 10，Roslyn 扩展及其共享构建期工具使用宿主要求的 `netstandard2.0`。

生成供本地调用方使用的唯一预发布版本：

```powershell
.\pack-local.ps1
```

包会写入 `artifacts/packages`，版本格式为 `0.1.0-dev.<UTC 时间戳>`。将该目录配置为本地 NuGet 包源并使用 `0.1.0-dev.*` 即可获取最新构建，无需公开发布。`Xoderony.Localization.Editor` 是独立 WPF 应用；ArgumentWrapping 的分析器、CodeFix 和 VSIX 是开发工具，不作为游戏运行时库引用。

## 开发位置

后续开发在 `E:\Projects\Xoderony` 进行。Expriverse 通过本地 NuGet 包使用 Xoderony，不直接引用其项目或源码。

## 许可证

[MIT](LICENSE)
