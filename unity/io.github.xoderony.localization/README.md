# Xoderony Localization

为 Unity 和普通 .NET 项目提供无特殊依赖的本地化核心，以及独立的开发期工具。

## 程序集

- `Xoderony.Localization`
- `Xoderony.Localization.Tooling`：提供手动调用的强类型本地化键代码生成能力。
- `Xoderony.Localization.Json`：提供基于 `System.Text.Json` 的严格 JSON 语言表数据层。

当前最小骨架定义：

- `CultureInfo`：localizer 当前用于查询和格式化的标准 culture。
- `IStringLocalizer`：指定 culture 下返回最终字符串的普通查询与参数格式化查询契约。
- `StringLocalizer`：从最终字符串集合直接构造，或由 builder 构建，并以固定 culture 和不可变字符串集合提供只读查询与格式化。
- `StringLocalizerBuilder`：按添加顺序合并字符串层，后添加的层覆盖先添加的层，并构建不可变 localizer 快照。

所有 culture 必须能够由 .NET `CultureInfo` 表示，且不能使用 `InvariantCulture`。`StringLocalizer` 构造后不能更改 culture 或字符串集合；切换语言需要重新构建应用侧本地化对象。资源加载、fallback culture 顺序推导与 Unity 运行时组件尚未纳入当前契约。

已经完成合并的最终字符串集合可直接传入 `StringLocalizer` 的公开构造函数，便于依赖注入容器配置构造参数；需要按层覆盖字符串时使用 `StringLocalizerBuilder`。

## 源码

权威源码分别位于：

- `Runtime/`：通用核心源码。
- `Runtime/Tooling/`：编辑器与构建期工具源码；Unity 中仅编译到 Editor。
- `Runtime/Json/`：严格 JSON 语言表数据层源码。

仓库中的 `src/Xoderony.Localization`、`src/Xoderony.Localization.Tooling` 与 `src/Xoderony.Localization.Json` 项目通过链接编译项引用同一份源码。Windows 桌面程序位于 `tools/Xoderony.Localization.Editor/`，通过引用 JSON 类库编辑语言表，不依赖 Unity。

## JSON 数据层

一个语言表集合目录包含共享键文件与各 Locale 的译文文件，例如：

```text
UI/
  keys.json
  en-US.json
  ja-JP.json
  zh-CN.json
```

`keys.json` 是键权威：嵌套 object 表示键组，JSON `null` 表示词条。各 `{culture}.json` 只保存扁平的 `full.key` → 字符串译文，不再重复嵌套结构。公共结构由 `RootKeyGroup` 暴露，节点类型使用 `Group` 和 `Entry`；新增节点分别调用 `AddGroup(...)` 与 `AddEntry(...)`。结构变更只改 `keys.json`，并同步各 Locale 值表中的键；缺省译文在读取时视为空字符串。

空字符串表示尚未完成翻译。普通保存允许空字符串。`tools/Xoderony.Localization.Editor` 是用于打开、编辑和保存语言表目录的 WPF 程序；它显示树的可见节点列表、动态 Locale 列和每种语言的空值统计，并可调用 `StringTableKeyGenerator` 为当前语言表生成强类型 C# 键，不需要打开 Unity。

保存输出为 UTF-8 无 BOM、LF、缩进并以换行结尾的标准 JSON。Locale 值文件的属性按 ordinal 排序；JSON 不保留注释、空白或位置等源格式信息。

## 强类型本地化键生成

`StringTableKeyGenerator` 接收调用方已经解析并展平的本地化键，返回确定性的完整 C# 源码：

```csharp
var source = StringTableKeyGenerator.Generate(
    ["main_menu.start", "main_menu.quit.button"],
    "Example.Localization",
    "L10nKeys");
```

调用方自行决定如何读取 JSON 或其他数据源，以及何时将结果写入文件并通知 Unity 刷新资源。生成器不读取语言表、不写文件、不调用 Unity API，也不会监听文件变化自动生成。

键必须由点分隔的 `lower_snake_case` 段组成。点生成嵌套静态类，段名转换为 PascalCase，叶节点生成保留原始完整键的 `const string`。重复键、转换后的标识符冲突，以及同一路径同时作为完整键和键组前缀时会抛出 `ArgumentException`。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.localization
```

## 兼容性

- Unity 7（技术版本 `7000.0`）或更高版本
