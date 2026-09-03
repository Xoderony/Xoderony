# Xoderony Numerics

提供 Q16 定点数和泛型数值扩展。不依赖 UnityEngine 或其他 Xoderony 包。

## 程序集与 API

- 程序集：`Xoderony.Numerics`。
- `Xoderony.Numerics.Q16`：16.16 定点数，底层编码由 RawValue 表达。
- `Xoderony.Extensions.NumberExtensions`：Abs、Clamp、Clamp01、LerpTo、LerpToUnclamped 和 MoveTowards。

Q16 的乘除和转换对无法表示的小数部分向零截断，默认不检测溢出；格式化输出精确的 RawValue/Scale 分数。文本格式化直接使用 BCL 接口，不需要引用 Xoderony.Text。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.numerics
```

支持 Unity 7（7000.0）和 .NET 10。权威源码位于 Runtime/，src/Xoderony.Numerics/Xoderony.Numerics.csproj 链接同一份源码。

## Unity Inspector

需要以小数编辑 Q16 时，可另外安装 [Numerics Unity 适配包](../io.github.xoderony.numerics.unity/README.md)。适配包只提供 Editor 程序集，本包继续保持无 Unity 依赖。

## 从 Core 迁移

这些类型从 Xoderony.Core 移入本程序集，类型名和命名空间保持不变。调用方改为引用 Xoderony.Numerics 并重新编译。
