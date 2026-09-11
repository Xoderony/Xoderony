# Xoderony Text

提供基于调用方缓冲区的 UTF-16 与 UTF-8 文本构造、格式化工具和字符串扩展，不依赖游戏引擎。

## 程序集

- `Xoderony.Text`

根命名空间是 `Xoderony.Text`。`Utf16SpanWriter` 写入 `Span<char>`，`Utf8SpanWriter` 写入 `Span<byte>`；两者均由调用方提供固定容量缓冲区。

## 源码

源码位于对应的 `src/Xoderony.Text` 项目中。

## 安装

通过 NuGet 引用 `Xoderony.Text`；本地开发可直接引用同目录的项目文件。

## 兼容性

目标框架为 `net10.0`。

## 字符串扩展

Xoderony.Extensions.StringExtensions 提供 IsNullOrEmpty 和 IsNullOrWhiteSpace 扩展属性，从 Xoderony.Core 移入本程序集，命名空间保持不变。使用这些扩展的调用方需改为引用 Xoderony.Text 并重新编译。
