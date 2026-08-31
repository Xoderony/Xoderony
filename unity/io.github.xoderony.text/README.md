# Xoderony Text

提供基于调用方缓冲区的 UTF-16 与 UTF-8 文本构造和格式化工具，不依赖 `UnityEngine`。

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
