# Xoderony.Stride.UI.SourceGeneration

从 Game Studio 保存的 `.sduipage` / `.sduilib` 生成强类型 UI 实例绑定。生成器以 `netstandard2.0` NuGet analyzer 分发，通过 YamlDotNet 读取资产，不在编译器宿主中加载 Stride。生成代码面向 .NET 10 与 Stride UI，不需要 Xoderony 运行时包。

## 配置

```xml
<ItemGroup>
  <PackageReference Include="Xoderony.Stride.UI.SourceGeneration"
                    Version="0.1.0-dev.YOUR_VERSION" PrivateAssets="all" />
</ItemGroup>
```

引用包后，随包分发的 MSBuild 文件自动将项目 `Assets/**/*.sduipage` 和 `Assets/**/*.sduilib` 加入 `AdditionalFiles`，无需手写 Include。已有条目按完整路径去重并保留元数据；生成器本身不扫描磁盘。收集发生在项目求值阶段，普通编译和设计时编译均可获取输入。新增、删除或移动资产后，下次项目重新求值与编译会更新生成结果；IDE 中的即时刷新取决于项目系统的文件监视与设计时编译。

默认命名空间为 `RootNamespace` 加相对 `Assets` 的目录，忽略 `Assets` 本身；未配置 `RootNamespace` 时沿用 `StrideUIBindings`。页面类型名固定为文件名加 `View`，库类型名固定为文件名加 `Library`，即使文件名已经包含对应后缀也仍会追加。例如 `RootNamespace=Expriverse` 时：

| 资产 | 生成类型 |
| --- | --- |
| `Assets/UI/LanguageSelection.sduipage` | `Expriverse.UI.LanguageSelectionView` |
| `Assets/UI/Menu/Page.sduipage` | `Expriverse.UI.Menu.PageView` |
| `Assets/UI/Widgets.sduilib` | `Expriverse.UI.WidgetsLibrary` |

目录、文件和节点名称均保留大小写，不自动 PascalCase、不替换非法字符、不追加数字。非法目录段或文件名推导结果报告 `XSUI003`；合法 C# 关键字使用 `@` 转义。最终完整类型名冲突会报告 `XSUI001`。移动目录或重命名文件会改变生成类型，旧业务代码通过编译错误发现变化。生成器不提供按资产覆盖命名空间或类型名的配置入口。

资产中的 `Name` 原样成为属性名，C# 关键字自动转义。不改变大小写、不清理非法字符、不追加数字。新增节点会新增属性；删除或重命名节点会删除旧属性；节点类型变化会改变属性类型。调用方不再维护节点名称字符串。可赋值兼容的类型变化不一定导致编译错误。

## 页面

对于根类型为 `Grid` 的 `LanguageSelection.sduipage`：

```csharp
public sealed class MenuController : StartupScript {
    public LanguageSelectionView View { get; set; }

    public override void Start() {
        View.ConfirmButton.IsEnabled = false;
    }
}
```

页面资产生成可挂载到 Entity 的 `StartupScript`。它带有 `RequireComponentAttribute`，但该特性不会自动添加或验证组件：同一 Entity 必须配置 `UIComponent`，且它的 `Page.RootElement` 必须与资产根类型相符。View 在 `Start()` 中按资产声明的 `Children[index]` 与 `Content` 逻辑路径定位节点，校验路径上的类型及资产中命名节点的名称，然后缓存根与所有命名节点；组件、页面、根、路径节点缺失或不兼容，以及名称不匹配，都会抛出包含逻辑路径的 `InvalidOperationException`。内部视觉节点（包括 ScrollViewer 创建的滚动条）不参与绑定。在 `Start()` 成功前访问属性同样会抛出该异常。

生成 View 的默认 `Priority` 是 `-10000`；数值较小的 Stride 脚本先启动。若业务脚本在自己的 `Start()` 中使用 View，业务脚本的优先级必须大于该值。场景中显式配置的优先级会覆盖该默认值。绑定后修改视觉树或更换 `UIComponent.Page` 不会重新绑定，缓存会继续引用启动时的页面实例。View 不持有订阅，也不销毁页面或节点。

## UI 库与克隆

`.sduilib` 生成带 `DataContract` 的 `readonly struct` Library。作为脚本或组件的属性时，该值始终存在，不需要写 `= new()`；其中的 `UILibrary` 引用由 Game Studio 或调用方配置，生成代码不会加载或卸载 Content。`Widgets.sduilib` 中每个命名根生成一个 `Create{RootName}()` 方法和对应的嵌套 Library Instance View，例如根 `LanguageButton` 生成 `WidgetsLibrary.CreateLanguageButton()` 与 `WidgetsLibrary.LanguageButtonView`：

```csharp
public WidgetsLibrary Widgets { get; set; }

// 仅在代码中配置资源时，替换整个值。
Widgets = new WidgetsLibrary { Library = library };
var ui = Widgets.CreateLanguageButton();
parent.Children.Add(ui.Root);
ui.Label.Text = "English";
```

`Library` 是 `init` 属性；代码不能在已取得的 Library 值上单独改写它，需要用新的完整值替换。`default(WidgetsLibrary)` 的 `Library` 仍为 `null`，未在 Game Studio 或代码中配置资源时，调用创建方法会抛出 `InvalidOperationException`。Stride 的 `InlinePropertyAttribute` 不允许标在结构体类型上；如需让属性网格强制内联展开，应在调用方属性上添加 `[InlineProperty(Expand = ExpandRule.Always)]`。

创建方法以根名称作为 Stride 库导出键，使用 `InstantiateElement<T>()` 创建独立克隆，并立即按资产的 `Children[index]` 与 `Content` 逻辑路径校验和缓存该克隆树。每次调用都会返回独立的 Library Instance View；其构造器不公开，调用方不能绕过创建与绑定流程。运行时名称只用于验证已按路径定位的资产节点，不用于全树搜索，因此内部视觉节点及不同子树中的同名节点不参与绑定。未配置库、导出根缺失、逻辑路径不可达、类型不兼容或名称不匹配会抛出 `InvalidOperationException`。页面内嵌模板暂不单独生成子树绑定；需要独立强类型克隆绑定时，将模板保存为库根。

## 支持边界与诊断

解析 `Hierarchy.RootParts`、`Hierarchy.Parts[*].UIElement`、GUID `ref!!`、带条目 ID 的 `Children` 集合与 `Content` 关系；正常 YAML 引号、块文本和嵌套映射由 YAML 解析器处理，`DependencyProperties` 等非结构数据不参与节点提取。要求输入包含完整展开的节点树，不解析外部基资产或执行 Stride 的资产继承合并。无法通过 `Children` / `Content` 到达的 Parts、悬空引用、循环及共享子节点均报错。

支持可从调用方编译符号解析的 Stride `Controls` / `Panels` / `UI` 短类型标签，以及公开、非泛型 UIElement 派生类的完整 CLR 类型名标签。自定义 DataContract 别名、程序集限定标签及自定义子节点存储尚不支持。生成器不改变 Game Studio 的序列化行为。库根必须命名；空库生成空容器类型。

| 诊断 | 含义 |
| --- | --- |
| XSUI001 | 配置非法，或生成类型与其他资产/源码类型冲突 |
| XSUI002 | 文件不可读、YAML 无效、结构或引用不受支持 |
| XSUI003 | 非法、重复或保留名称；库根缺少有效名称 |
| XSUI004 | 类型标签不可解析、存在歧义或不是公开 UIElement 类型 |

`Root`、当前绑定类名、`object` 成员以及 Page View 从 `StartupScript` 继承的成员名为保留名称。错误定位到输入资产；失败资产不输出部分绑定。

仓库内部测试通过普通 `ProjectReference` 调用生成器。若通过 `OutputItemType="Analyzer"` 在其他仓库内项目加载它，还必须将 `YamlDotNet.dll` 作为 analyzer 输入。外部项目使用版本化 NuGet 包；包内生成器和 YAML 依赖位于同一 `analyzers/dotnet/cs` 目录。

Stride 的页面/库实例约定参见官方 [UI libraries](https://doc.stride3d.net/latest/en/manual/ui/ui-libraries.html)。测试项目使用 Stride 4.4.0-beta6，覆盖生成代码编译、实际节点缓存和库实例隔离。
