# Xoderony Text

提供基于调用方缓冲区的 UTF-16 与 UTF-8 文本构造、格式化工具和字符串扩展，不依赖 `UnityEngine`。

## 程序集

- `Xoderony.Text`

根命名空间是 `Xoderony.Text`。`Utf16SpanWriter` 写入 `Span<char>`，`Utf8SpanWriter` 写入 `Span<byte>`；两者均由调用方提供固定容量缓冲区。

## 源码

权威源码直接位于本包的 `Runtime/`；仓库中的 `src/Xoderony.Text/Xoderony.Text.csproj` 通过链接编译项引用同一份源码。

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.text
```

## 兼容性

- Unity 7（技术版本 `7000.0`）或更高版本

.NET 项目同时生成 `net10.0` 和 `netstandard2.1`。Unity 6000.7 使用 `netstandard2.1` DLL；UPM 源码安装仍面向 Unity 7。

| 能力 | net10.0 | netstandard2.1 |
| --- | --- | --- |
| 基础写入、可空布尔值、换行、缓冲区视图与链式 ref 返回 | 支持 | 支持，相同 API |
| UTF-8 / UTF-16 编码转换与无效序列替换 | 支持 | 支持，相同 API |
| 泛型 `Write<T>` 与可空值格式化 | 支持 | 不提供 |
| 字符串判空 | 扩展属性 | 扩展方法，适配 Unity C# 9 调用方 |

兼容版的格式化由调用方显式完成后写入，不隐式调用 `ToString()`。所有版本均要求调用方提供足够容量的缓冲区。

## 字符串扩展

Xoderony.Extensions.StringExtensions 提供 IsNullOrEmpty 和 IsNullOrWhiteSpace 扩展属性，从 Xoderony.Core 移入本程序集，命名空间保持不变。使用这些扩展的调用方需改为引用 Xoderony.Text 并重新编译。

`netstandard2.1` 调用形式为 `value.IsNullOrEmpty()` 和 `value.IsNullOrWhiteSpace()`。
