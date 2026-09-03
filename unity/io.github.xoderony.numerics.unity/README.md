# Xoderony Numerics Unity

Xoderony.Numerics 的可选 Unity 编辑器适配包，提供 Q16 的小数编辑界面。仅单向依赖 io.github.xoderony.numerics。

## 安装

先安装 Numerics，再安装本包。通过 Git URL 安装时，在 Unity Package Manager 中分别添加：

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.numerics
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.numerics.unity
```

使用本地仓库时，选择 **Add package from disk**，先选择 Numerics 包的 package.json，再选择本包的 package.json。

最低 Unity 版本为 7000.0（Unity 7）。如果项目已注册其他 Q16 PropertyDrawer，应移除旧 Drawer，避免重复注册。

## Q16 Inspector

序列化的 Q16 字段自动显示为单行小数编辑框，不需要额外属性标记。

- 支持小数和 Unity 数值表达式，例如 1.5、1/2、3/4。
- 文本在 Enter 或失去焦点时提交，按 Q16 当前契约向零截断到最近的可表示刻度。
- 提交后显示实际存储值。例如 1/3 存为 21845/65536，显示约 0.3333282470703125；不保存原始表达式。
- Tooltip 保留原字段说明，并显示实际值、精确的 RawValue/65536 分数和 RawValue。
- 非法表达式、NaN、Infinity 或超出 [-32768, 32767.9999847412109375] 的输入会被拒绝，保留原值并在 Console 提示。
- 通过 SerializedProperty 修改原 RawValue，支持多对象赋值和 Unity 的撤销操作；不改变序列化字段名或存储布局。

UI Toolkit 使用延迟提交的 DoubleField，支持数值标签拖动。IMGUI 使用延迟文本输入配合 double 求值，确保显示文本有完整往返精度；该入口不提供标签拖动。两种入口均使用 G17 显示格式，并仅在提交有效修改时写入序列化数据。

## 程序集与源码

Editor/Xoderony.Numerics.Unity.Editor.asmdef 仅在 Editor 平台编译，引用 Xoderony.Numerics；Drawer 不进入 Player。包内没有额外的运行时程序集。

编辑器权威源码位于 Editor/；src/Xoderony.Numerics.Unity.Editor/Xoderony.Numerics.Unity.Editor.csproj 链接同一份源码，供 .NET IDE 使用，不发布 NuGet 包。

该项目通过 UnityManagedPath 定位 Unity 编辑器程序集。可在同目录创建被 Git 忽略的 Xoderony.Numerics.Unity.Editor.local.props：

```xml
<Project>
  <PropertyGroup>
    <UnityManagedPath>你的 Unity 安装目录/Editor/Data/Managed/UnityEngine</UnityManagedPath>
  </PropertyGroup>
</Project>
```
