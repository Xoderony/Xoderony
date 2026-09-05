# Xoderony.ArgumentWrapping

这是供 Visual Studio 使用的 C# Roslyn 分析器与 CodeFix。它检查方法调用与构造函数调用（`new Type(...)`、`new(...)`）的实参列表；方法声明不受影响。项目不属于 Unity 运行时程序集或 UPM 包。

## 行为

由用户决定实参列表是否换行。`XoderonyArgumentLayout` 只规范已经在列表边界换行的调用，不根据实参数量、表达式复杂度或行长度自动展开，也不把多行列表收拢为单行。

列表边界包括左括号与首个实参之间、实参与逗号之间、逗号与下一个实参之间，以及最后一个实参与右括号之间。例如以下写法会触发修复：

```csharp
Call(first,
second);
```

修复后实参逐项换行，右括号另起一行；已经逐项换行但缩进不符合配置时也会报告：

```csharp
Call(
    first,
    second
);
```

实参内部的 lambda、字符串或嵌套调用所包含的换行，不作为外层列表的换行意图。例如以下代码只检查 `Inner` 的布局，不自动展开 `Outer`：

```csharp
Outer(Inner(
    first,
    second
), third);
```

单行调用保持原样。方法声明及构造函数的 `base(...)`、`this(...)` 初始化器不在本规则范围内。

## 配置

规则默认启用，无需专用配置项。诊断严重程度使用 Visual Studio 的标准 EditorConfig 机制，默认为 `warning`；可以按需设置为 `error`、`warning`、`suggestion`、`silent` 或 `none`，其中 `none` 禁用诊断：

```editorconfig
[*.cs]
dotnet_diagnostic.XoderonyArgumentLayout.severity = warning
```

修复会从当前文件的有效 `AnalyzerConfigOptions` 读取 `indent_style`、`indent_size`、`tab_width` 和 `end_of_line`。多行布局以调用起始行的行首缩进为基准：实参增加一级，右括号与基准对齐。移动多行实参时保持其内部代码的相对缩进，不修改字符串 token 的内容。

快捷修复 **规范调用实参布局 / Format call arguments** 与 Fix All 共用布局处理逻辑，按各层原有的换行意图处理嵌套调用。已有的注释与指令会保留；已符合布局的调用不会再次报告。

诊断标题、消息、说明以及修复操作标题随 Visual Studio 的界面语言选择：简体中文（`zh-CN`）使用中文，其他未提供翻译的语言回退到英文。诊断 ID 和 EditorConfig 配置键不随语言变化，无需配置语言选项。

## 构建与安装

构建 `tools/Xoderony.ArgumentWrapping.Vsix/Xoderony.ArgumentWrapping.Vsix.csproj` 会产出 VSIX。双击生成的 `.vsix` 安装它，然后重启 Visual Studio。VSIX 将分析器作为 IDE 扩展资产加载，因此对 Visual Studio 打开的 Unity C# 源码工程生效，无需在 Unity 生成的 `.csproj` 中添加 NuGet 包或修改 UPM 运行时程序集。

安装后打开 Unity 项目生成的解决方案即可使用。需要通过 **Code Cleanup** 配置中的 **Fix all warnings and errors set in EditorConfig** 修复时，在项目根目录的 `.editorconfig` 中显式将 `XoderonyArgumentLayout` 设置为 `warning` 或 `error`；`suggestion` 与 `silent` 不保证由该入口执行。此扩展不替换 Visual Studio 的格式化文档命令。

VSIX 将 `Xoderony.ArgumentWrapping.dll` 注册为 `Microsoft.VisualStudio.Analyzer`，将 `Xoderony.ArgumentWrapping.CodeFixes.dll` 注册为 `Microsoft.VisualStudio.MefComponent`。分析器只依赖编译器 API，CodeFix 单独依赖 Workspaces，两个程序集一同打包。清单使用 Visual Studio 17.0 起的开放上界安装目标，适用于 VS2026 的兼容模型；尚未在本机 Visual Studio 中执行安装或 Code Cleanup 集成验证。
