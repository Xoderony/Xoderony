# Xoderony Numerics

提供 Q16 定点数和泛型数值扩展。不依赖其他 Xoderony 包。

## 程序集与 API

- 程序集：`Xoderony.Numerics`。
- `Xoderony.Numerics.Q16`：16.16 定点数，底层编码由 RawValue 表达。
- `Xoderony.Extensions.NumberExtensions`：Abs、Clamp、Clamp01、LerpTo、LerpToUnclamped 和 MoveTowards。

Q16 的乘除和转换对无法表示的小数部分向零截断，默认不检测溢出；格式化输出精确的 RawValue/Scale 分数。文本格式化直接使用 BCL 接口，不需要引用 Xoderony.Text。

## 安装

通过 NuGet 引用 `Xoderony.Numerics`；本地开发可直接引用同目录的项目文件。

目标框架为 `net10.0`。

## 从 Core 迁移

这些类型从 Xoderony.Core 移入本程序集，类型名和命名空间保持不变。调用方改为引用 Xoderony.Numerics 并重新编译。
