# Xoderony

面向 Unity 7 和 .NET 10 的基础库，提供无 Unity 依赖的通用能力和 Unity 专用扩展。

## 包

- `Xoderony.Core` / `io.github.xoderony.core`：集合、序列化缓冲、委托与值通道、通用扩展和对象池，不依赖 `UnityEngine`。
- `Xoderony.Text` / `io.github.xoderony.text`：基于 Span 的 UTF-16 与 UTF-8 文本构造和格式化工具，不依赖 `UnityEngine`。
- `Xoderony.Logging` / `io.github.xoderony.logging`：无 Unity 依赖的通用日志核心，以及自动携带调用位置与 Unity 对象上下文的 Unity 适配。
- `Xoderony.Localization` / `io.github.xoderony.localization`：无 Unity 依赖的本地化核心、强类型字符串键生成工具，以及可选的 Hjson Unity 编辑器数据层。

## 安装

在 Unity Package Manager 中选择 **Add package from git URL**，输入所需包的地址：

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.core
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.logging
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.localization
```

## 兼容性

- Unity 7（`7000.0`）或更高版本
- .NET 10

## 许可证

[MIT](LICENSE)
