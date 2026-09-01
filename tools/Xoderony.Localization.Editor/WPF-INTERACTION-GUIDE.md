# Xoderony Localization Editor 的 WPF 界面与代码交互

本文面向第一次接触 WPF 的开发者，以当前编辑器源码为准，说明界面如何创建、XAML 如何连接 C#、表格如何绑定数据，以及一次编辑操作如何最终写入 JSON。

相关文件：

- [`Xoderony.Localization.Editor.csproj`](./Xoderony.Localization.Editor.csproj)：WPF 项目配置与数据层引用。
- [`App.xaml`](./App.xaml)：应用入口、启动窗口和全局资源。
- [`App.xaml.cs`](./App.xaml.cs)：`Application` 的代码后置类。
- [`MainWindow.xaml`](./MainWindow.xaml)：主窗口控件树、布局、模板和事件绑定。
- [`MainWindow.xaml.cs`](./MainWindow.xaml.cs)：主窗口状态与全部交互逻辑。
- [`EditorLocalizer.cs`](./EditorLocalizer.cs)：编辑器界面语言与核心本地化包之间的适配层。
- [`EditorSettings.cs`](./EditorSettings.cs)：在程序根目录的 `settings.json` 中保存主题、界面语言、窗口尺寸和上次打开的语言表目录。
- [`Localization`](./Localization)：编辑器自身使用的 JSON 界面语言表。
- [`Styles/VisualStudio.xaml`](./Styles/VisualStudio.xaml)：字体和浅暗主题共用的控件样式。
- [`Styles/VisualStudioLight.xaml`](./Styles/VisualStudioLight.xaml)：浅色主题颜色和画刷。
- [`Styles/VisualStudioDark.xaml`](./Styles/VisualStudioDark.xaml)：深色主题颜色和画刷。
- [`../../unity/io.github.xoderony.localization/Runtime/Json/JsonLocaleTableCollection.cs`](../../unity/io.github.xoderony.localization/Runtime/Json/JsonLocaleTableCollection.cs)：JSON 语言表数据层。

## 1. 先建立整体认识

当前程序可以粗略分为三层：

```text
编辑器 UI JSON ─→ EditorLocalizer ─→ XAML Binding / C# 动态文案

用户输入
  │
  ▼
MainWindow.xaml
控件、布局、样式、DataTemplate
  │  RoutedEvent / Binding
  ▼
MainWindow.xaml.cs
窗口状态、导航、搜索、编辑提交、命令启用状态
  │  调用公开 API
  ▼
用户 JsonLocaleTableCollection
加载、修改、复制、移动、删除、保存用户语言表
```

这里没有使用大型 MVVM 框架。主窗口采用 WPF 原生的 **XAML + code-behind（代码后置）** 模式：XAML 声明界面，C# 事件处理器执行操作。

这种结构对于一个窗口、状态边界明确的内部工具比较直接。它不代表所有 WPF 项目都必须这样写；大型应用常把状态和命令进一步拆到 ViewModel，但当前编辑器没有这项复杂度需求。

## 2. WPF 项目是怎样启动的

项目文件中的关键配置是：

```xml
<OutputType>WinExe</OutputType>
<TargetFramework>net10.0-windows</TargetFramework>
<UseWPF>true</UseWPF>
```

- `WinExe` 表示生成 Windows 图形程序，不显示控制台窗口。
- `net10.0-windows` 表示使用面向 Windows 的 .NET 10。
- `UseWPF` 会启用 WPF 的 XAML 编译和相关程序集引用。

`App.xaml` 相当于应用级入口：

```xml
<Application ... StartupUri="MainWindow.xaml">
```

`StartupUri` 告诉 WPF 启动后创建并显示 `MainWindow`。`App.xaml.cs` 中的：

```csharp
public partial class App : Application {
}
```

与 `App.xaml` 编译生成的另一部分代码共同组成 `App` 类。这就是 `partial` 的用途：一部分由我们编写，另一部分由 XAML 编译器生成。

主窗口也是同样的关系：

```xml
<Window x:Class="Xoderony.Localization.Editor.MainWindow">
```

对应：

```csharp
public partial class MainWindow : Window {
}
```

构造函数中的 `InitializeComponent()` 会加载编译后的 XAML、创建控件树、应用资源，并把带有 `x:Name` 的控件连接到生成字段。必须先调用它，后续代码才能访问 `TableGrid`、`SearchTextBox`、`SaveButton` 等控件。

## 3. XAML 可以理解成“声明式对象创建”

XAML 不是图片或 HTML。它会被编译成创建 .NET 对象的代码。例如：

```xml
<Button x:Name="SaveButton"
        Click="Save"
        ToolTip="{Binding [toolbar.save_tooltip]}" />
```

可以近似理解为：

```csharp
var saveButton = new Button();
saveButton.Click += Save;
```

其中：

- `Button` 是 `System.Windows.Controls.Button`。
- `x:Name="SaveButton"` 让代码后置可以通过 `SaveButton` 访问该实例。
- `Click="Save"` 把按钮点击事件连接到 `MainWindow.xaml.cs` 的 `Save` 方法。
- `ToolTip`、`Margin`、`Padding`、`Visibility` 等都是控件属性。示例中的 ToolTip 通过绑定从 `EditorLocalizer` 读取当前界面语言。

### 当前主窗口的布局

主 `Grid` 有四行：

```text
Auto  顶部工具栏
Auto  面包屑路径栏和搜索框
*     DataGrid 主内容区，占用全部剩余空间
Auto  底部状态栏
```

WPF 常见布局容器的职责不同：

- `Grid`：按行列分配空间，适合页面骨架。
- `StackPanel`：按水平或垂直方向连续排列子元素。
- `WrapPanel`：空间不足时自动换行，当前用于自适应工具栏。
- `Border`：提供背景、边框和内边距，也常用作区域容器。
- `ScrollViewer`：内容超出时滚动，当前用于较长的面包屑路径栏。

`Width="*"` 表示获取剩余空间；`Width="Auto"` 表示按内容需要占用空间。`3*` 与 `2*` 表示按 3:2 分配剩余空间。

## 4. 样式和资源如何生效

`App.xaml` 合并了：

```xml
<ResourceDictionary Source="Styles/VisualStudioLight.xaml" />
<ResourceDictionary Source="Styles/VisualStudio.xaml" />
```

第一项提供默认浅色调色板，第二项提供共用字体与控件模板；深色主题启用时会覆盖同名颜色资源。因此这些资源对整个应用可见。例如：

```xml
Background="{StaticResource VsToolbarBrush}"
Style="{StaticResource ToolbarButtonStyle}"
```

`StaticResource` 会按资源键查找对象。当前资源字典集中保存：

- VS 浅色主题的颜色和画刷。
- 普通字体与图标字体。
- `Button`、`TextBox`、`DataGrid` 等控件的默认样式。
- 工具栏、危险按钮和面包屑按钮的具名样式。

没有 `x:Key`、只有 `TargetType` 的样式是该类型的隐式默认样式。例如所有 `TextBox` 默认使用同一套字体继承、边框和聚焦颜色。带 `x:Key` 的样式需要显式引用。

样式中的 `ControlTemplate` 决定控件真正怎样绘制；`Trigger` 根据 `IsMouseOver`、`IsPressed`、`IsEnabled` 等状态改变颜色。也就是说，按钮悬停和禁用效果不是事件处理器手动完成的，而是 WPF 样式系统完成的。

## 5. 事件：XAML 怎样调用 C#

当前编辑器大量使用 WPF 的 **路由事件**。例如：

```xml
<TextBox TextChanged="SearchChanged" />
<DataGrid BeginningEdit="BeginCellEdit"
          CellEditEnding="EndCellEdit"
          MouseDoubleClick="HandleCellDoubleClick" />
```

事件发生时，WPF 调用对应的 C# 方法。事件处理器通常接收：

```csharp
private void SomeHandler(object sender, RoutedEventArgs e)
```

- `sender` 是触发事件的控件。
- `e` 保存事件的额外信息。
- 设置 `e.Handled = true` 表示该事件已经处理，不希望它继续向其他控件传播。

WPF 路由事件主要有两种传播方向：

- `Preview...` 事件从窗口向实际控件传播，称为隧道路由。
- 普通事件从实际控件向父容器传播，称为冒泡路由。

窗口使用 `PreviewKeyDown="OnPreviewKeyDown"`，因此可以统一处理 `Ctrl+S`、`Ctrl+C/X/V`、F2、Delete、Backspace 等快捷键。方法先判断焦点是否位于 `TextBox`；如果正在输入文字，就不抢占 Backspace 与剪贴板快捷键。

## 6. 数据绑定和 DataContext

WPF 的绑定表达式通常长这样：

```xml
Text="{Binding DisplayName}"
```

它的含义不是绑定到 `MainWindow.DisplayName`，而是绑定到控件当前的 `DataContext.DisplayName`。

主窗口本身的 `DataContext` 是 `EditorLocalizer`，所以工具栏中的：

```xml
Text="{Binding [toolbar.open]}"
```

会调用本地化适配层的字符串索引器。切换界面语言后，`EditorLocalizer` 通过 `INotifyPropertyChanged` 通知这些绑定重新读取文本。

对于 `DataGrid`，每行的 `DataContext` 会覆盖窗口级 DataContext，自动设置为 `ItemsSource` 中对应的对象。当前 `ItemsSource` 是 `List<LocalizationRow>`，所以名称模板中的：

```xml
Text="{Binding DisplayName}"
ToolTip="{Binding Node.FullKey}"
```

实际读取的是当前 `LocalizationRow` 的属性。

`LocalizationRow` 是一个专门给界面使用的行模型：

```text
Node         JSON 节点及完整键
DisplayName  普通浏览时为键段，搜索时为相对路径
CultureNameToTranslation       culture 名称到翻译字符串的字典
IsGroup      是否为键组，并驱动键组与词条的图标样式
```

它没有写回 JSON 的职责。真正的数据修改仍通过 `JsonLocaleTableCollection` 完成。

## 7. 为什么 Locale 列必须动态创建

XAML 编写时不知道用户目录中有 `zh-CN`、`en-US` 还是其他 Locale，因此不能在 XAML 中固定写死列。`DataGrid` 设置了：

```xml
AutoGenerateColumns="False"
```

打开目录后，`RebuildColumns()` 在 C# 中执行：

1. 清空旧列。
2. 创建固定的“名称”列。
3. 遍历 `_tables.GetCultures()`。
4. 为每个 culture 创建一个 `DataGridTextColumn`。

每个翻译列绑定到类似下面的路径：

```csharp
CultureNameToTranslation[zh-CN]
CultureNameToTranslation[en-US]
```

`_columnByCultureName` 保存 culture 与列实例的对应关系，用于刷新列表后恢复当前列。

### 两种列类型

名称列使用只读的 `DataGridTemplateColumn`，因为它需要组合节点类型图标和显示名称。

Locale 列使用 `DataGridTextColumn`，因为它只需显示和编辑字符串，标准列实现更简单。

## 8. DataTemplate：名称单元格怎样显示

名称列使用 `NameCellTemplate` 组合图标和 `DisplayName`。它设置为只读，不参与 DataGrid 的单元格编辑事务。

搜索结果可能显示 `menu / settings / title`，但名称列始终只负责显示；重命名由独立对话框读取和提交新名称。

## 9. 一行数据是怎样显示出来的

`RefreshRows()` 是界面列表的主要重建入口：

1. 通过 `_currentGroupKey` 找到当前键组节点。
2. 没有搜索文本时，只枚举当前键组的直接子项。
3. 有搜索文本时，递归枚举当前键组及全部后代。
4. 为每个结果创建 `LocalizationRow`。
5. 对词条读取每个 Locale 的值，填入 `row.CultureNameToTranslation`。
6. 把列表赋给 `TableGrid.ItemsSource`。
7. 恢复选择项、当前列和滚动位置。

键组行的 Locale 值保持为空，而且 `BeginCellEdit()` 会禁止编辑键组行的 Locale 单元格。

需要注意：给 `ItemsSource` 赋一个新列表会让 DataGrid 重新生成行容器。这是结构变化、导航和搜索时允许做的事情，但不能在单元格提交事务中随意执行。

## 10. 翻译编辑的完整流程

以编辑 `zh-CN` 单元格并按 Enter 为例：

```text
用户双击翻译单元格
  ↓
HandleCellDoubleClick
  ↓ TableGrid.BeginEdit()
BeginCellEdit
  ↓ 允许字符串行的 Locale 列
TextBox 进入编辑状态
  ↓ 用户按 Enter、Tab 或点击其他单元格
EndCellEdit
  ↓
TryApplyValue
  ├─ _tables.SetTranslation(culture, fullKey, translation)
  ├─ row.CultureNameToTranslation[culture.Name] = value
  └─ UpdateStatus()
```

这里刻意 **不调用 `RefreshRows()`**。原因是 `CellEditEnding` 发生在 DataGrid 自己的提交生命周期中；如果此时清空 `Columns` 或替换 `ItemsSource`，DataGrid 内部正在使用的行和单元格会突然失效，可能在 `UpdateRowEditing` 中抛出异常。

普通翻译修改只更新：

- JSON 数据层中的值。
- 当前 `LocalizationRow.CultureNameToTranslation` 中对应的值。
- 保存状态和底部摘要。

这样 Enter、Tab、鼠标切换单元格和编辑状态下 `Ctrl+S` 都不会重建表格。

### 为什么使用 Explicit

Locale 列的绑定使用：

```csharp
Mode = BindingMode.TwoWay,
UpdateSourceTrigger = UpdateSourceTrigger.Explicit
```

这表示 TextBox 输入期间不自动把每个字符写回字典。最终值由 `EndCellEdit()` 明确读取并提交，JSON 修改边界集中在 `TryApplyValue()`。

## 11. 重命名为什么使用独立对话框

名称列保持只读。F2、工具栏和右键菜单调用 `Rename()` 后执行：

1. 提交当前翻译单元格。
2. 读取所选节点及其当前名称。
3. 打开普通输入对话框。
4. 用户确认后，通过 `ChangeStructure()` 调用 `_tables.Rename(...)`。
5. 重建行并恢复到重命名后的节点。

重命名不再发生在 `CellEditEnding` 内，因此不需要重命名许可标记、延迟刷新字段或 `Dispatcher` 调度。双击键组仍然只负责进入该键组。

## 12. 导航和面包屑

`_currentGroupKey` 保存当前浏览路径：

```text
""                 根键组
"menu"             menu
"menu.settings"    menu/settings
```

`NavigateToGroup()` 负责统一执行：

1. 提交当前编辑。
2. 更新 `_currentGroupKey`。
3. 重建面包屑。
4. 清除搜索。
5. 刷新当前键组的直接子项。

`RefreshBreadcrumb()` 根据键段在 `BreadcrumbPanel` 中动态创建 Button。每个按钮的 `Tag` 保存该层级的完整键组路径，点击后由 `NavigateBreadcrumb()` 取出并交给 `NavigateToGroup()`。

Backspace 由窗口的 `OnPreviewKeyDown()` 捕获，并通过 `GetParentKey()` 返回父级。焦点位于 TextBox 时不会拦截，因此仍可正常删除文字。

## 13. 搜索为什么不需要树形控件

输入搜索文本会触发 `SearchChanged()`，它调用 `RefreshRows()`。

普通浏览和搜索只是两种不同的行来源：

```text
普通浏览：当前键组.LocalKeyToChild
搜索模式：递归遍历当前键组的全部后代，然后筛选
```

`MatchesSearch()` 检查完整键和每个 Locale 的翻译值。搜索结果平铺到同一个 DataGrid，`DisplayName` 改为相对路径，让相同键段仍可区分。

清空搜索框后，`RefreshRows()` 再次走普通浏览分支，不需要保存或恢复树节点的展开状态。

## 14. 工具栏、右键菜单和快捷键为什么会调用同一方法

例如重命名有三个入口：

- 工具栏按钮 `Click="Rename"`。
- 右键菜单 `Click="Rename"`。
- F2 在 `OnPreviewKeyDown()` 中调用 `Rename(...)`。

它们最终进入同一个方法，避免不同入口产生不同规则。新增、删除、移动和复制也采用相同思路。

`SelectRowOnRightClick()` 会在右键菜单弹出前选中鼠标所在行，否则用户可能右键 A 行，操作却作用在之前选中的 B 行。

`UpdateCommandState()` 集中设置按钮与菜单的 `IsEnabled`，例如：

- 没有打开语言表时禁用保存、增加和搜索。
- 没有选择行时禁用重命名、移动、复制和删除。

## 15. 复制、剪切、粘贴与移动

**剪贴板（Ctrl+C / X / V）**：快照写入 Windows 剪贴板；剪切为 `Set` 后立刻 `Remove`；粘贴到**当前键组**，同名时用 `AllocateLocalKey`。

**复制到 / 移动到**：目标键组对话框 → `Copy` / `Move`（不经剪贴板）。复制允许同父；移动禁止同父。

当 TextBox 正在编辑时，窗口不拦截 Ctrl+C/X/V。

## 16. 结构修改的统一入口

新增、复制、移动和删除都会改变节点结构，因此统一通过 `ChangeStructure()`：

1. 提交正在编辑的单元格。
2. 捕获当前选择、列和滚动位置。
3. 执行传入的数据层操作。
4. 必要时重建动态列。
5. 刷新行并恢复界面状态。
6. 把数据层参数错误显示为消息框。

这与 `TryApplyValue()` 的局部更新形成明确区别：

```text
仅翻译文字变化  → 不重建 DataGrid
节点结构变化    → 结束编辑后重建行
Locale 数量变化 → 结束编辑后重建列和行
```

## 17. 打开、保存和未保存提示

`OpenDirectory()` 使用 `OpenFolderDialog` 选择目录，再调用：

```csharp
JsonLocaleTableCollection.LoadDirectory(dialog.FolderName)
```

打开成功后会：

- 保存目录路径。
- 返回根键组。
- 重建面包屑、Locale 列和行。

数据修改后，数据层的 `IsDirty` 变为 `true`。`UpdateStatus()` 据此更新：

- 窗口标题后的 `*`。
- 保存按钮启用状态。
- 底部“有未保存更改”状态。
- Locale、键和空值摘要。

保存或关闭窗口前会调用 `CommitCurrentEdit()`，确保当前 TextBox 中尚未离开的内容先进入数据层。关闭窗口通过 `Closing="OnClosing"` 询问保存、放弃或取消关闭。

## 18. Visual Tree：为什么有 FindVisualParent/Child

WPF 控件会被模板展开为更细的可视元素。例如鼠标实际点击到的可能是单元格内部的 `TextBlock`，而不是 `DataGridCell` 本身。

`FindVisualParent<DataGridCell>()` 从点击对象沿视觉树向上查找所属单元格，用于双击和右键选择。

`FindVisualChild<TextBox>()` 从编辑元素向下查找实际 TextBox，因为不同 DataGridColumn 和模板产生的编辑元素层级可能不同。

这类代码依赖的是 WPF 的 **Visual Tree（视觉树）**，与业务数据中的 JSON 节点树不是同一个概念。

## 19. 当前实现中最值得记住的 WPF 基础

1. **XAML 会创建真实的 .NET 控件对象。**
2. **`x:Name` 把 XAML 控件暴露给 code-behind。**
3. **事件负责“用户做了什么”，绑定负责“控件显示什么”。**
4. **DataContext 决定 `{Binding ...}` 从哪个对象取值。**
5. **DataTemplate 决定数据对象如何显示，ControlTemplate 决定控件如何绘制。**
6. **动态数据结构不一定适合全部写在 XAML 中。** Locale 列由运行时数据决定，所以在 C# 中创建。
7. **DataGrid 编辑有自己的事务和生命周期。** 提交过程中不要替换列或 `ItemsSource`。
8. **WPF 控件一般只能由 UI 线程操作。** 当前程序所有加载和编辑都是同步执行，因此没有跨线程问题。
9. **code-behind 并非错误。** 是否引入 MVVM 应由窗口规模、复用和测试需求决定，而不是为了形式统一。

## 20. 想修改某项功能时从哪里开始

| 需求 | 主要入口 |
|---|---|
| 调整字体、控件模板、悬停和选择行为 | `Styles/VisualStudio.xaml` |
| 调整浅色或深色配色 | `Styles/VisualStudioLight.xaml`、`Styles/VisualStudioDark.xaml` |
| 调整工具栏、路径栏、搜索框或状态栏布局 | `MainWindow.xaml` |
| 修改快捷键 | `OnPreviewKeyDown()` |
| 修改双击和右键行为 | `HandleCellDoubleClick()`、`SelectRowOnRightClick()` |
| 修改键组导航 | `NavigateToGroup()`、`RefreshBreadcrumb()` |
| 修改搜索范围或匹配规则 | `RefreshRows()`、`AddSearchRows()`、`MatchesSearch()` |
| 修改 DataGrid 固定列 | `CreateNameColumn()` |
| 修改动态 Locale 列 | `RebuildColumns()`、`CreateValueColumn()` |
| 修改翻译提交行为 | `BeginCellEdit()`、`EndCellEdit()`、`TryApplyValue()` |
| 修改重命名行为 | `Rename()`、`Prompt()`、`ChangeStructure()` |
| 修改增删复制移动 | 对应事件方法与 `ChangeStructure()` |
| 修改 JSON 契约或保存语义 | `JsonLocaleTableCollection`，而不是 XAML |

## 21. 推荐的源码阅读顺序

第一次阅读时不必从上到下逐行看。可以按以下顺序：

1. `App.xaml`：找到程序入口和全局样式。
2. `MainWindow.xaml`：只看四行总体布局和控件名称。
3. `MainWindow.xaml.cs` 字段：理解窗口保存了哪些状态。
4. `OpenDirectory()`、`RebuildColumns()`、`RefreshRows()`：理解数据如何首次出现在表格中。
5. `HandleCellDoubleClick()`、`BeginCellEdit()`、`EndCellEdit()`：理解编辑生命周期。
6. `TryApplyValue()` 与 `Rename()`：理解 UI 如何写回数据层。
7. `NavigateToGroup()`、`RefreshBreadcrumb()`、`AddSearchRows()`：理解文件夹管理器式浏览。
8. `ChangeStructure()`：理解为什么结构操作和普通翻译编辑走不同刷新路径。
9. 最后阅读 `VisualStudio.xaml` 与两个主题调色板：理解 WPF 样式、模板和动态颜色资源如何共同塑造视觉效果。

按这个顺序，可以先建立数据流，再补充具体控件和样式细节。

## 22. 编辑器为什么维护两套 JSON 数据

编辑器现在同时使用两套相互独立的语言表：

```text
Localization/*.json
  → 编辑器自己的按钮、菜单、对话框和状态文字

用户选择的目录/*.json
  → DataGrid 中正在浏览和编辑的项目语言表
```

内置语言表在窗口创建时由 `EditorLocalizer.Load()` 加载，再通过 `StringLocalizerBuilder` 构造核心 `IStringLocalizer`。默认选择系统 UI culture；没有完全匹配时尝试同语言 culture，最后回退到 `zh-CN`。

构建目标语言时会先加入中文非空字符串，再加入目标语言非空字符串。JSON 数据层会把缺失值补成空字符串，因此适配层必须跳过空字符串，避免它覆盖中文 fallback。

界面语言 ComboBox 的内容来自内置语言表的 `Cultures`。切换后：

1. `EditorLocalizer` 重新构建目标 `StringLocalizer`。
2. `Item[]` 属性变更通知让 XAML 索引器绑定自动更新。
3. `MainWindow.UpdateLocalizedText()` 更新代码动态创建的名称列表头、窗口标题和状态文字。
4. 用户 DataGrid 的行、列和值不会因为界面语言变化而重建。

这条路径让 WPF 编辑器在实际运行中同时消费 `Xoderony.Localization.Json` 与 `Xoderony.Localization`，但不会把编辑器界面文案混入用户正在编辑和保存的文件。
