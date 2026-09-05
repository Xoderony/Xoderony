# Xoderony Logging

提供无 Unity 依赖的通用日志核心，以及基于 `Debug.unityLogger` 的 Unity 默认适配。

## 程序集

- `Xoderony.Logging`：定义 `LogLevel`、`ILogger`、实例日志扩展和插值处理器，不引用 Unity。
- `Xoderony.Logging.Unity`：实现 Unity Debug 日志器，并提供无需显式持有日志器的默认扩展。

## Unity 默认用法

```csharp
using Xoderony.Logging;

this.Log("P2P runtime started.");
this.LogDebug($"Selected transport {transportName}.");
this.LogWarning($"Dropped send to unknown peer {peerId}.");
this.LogError($"Failed to load asset: {assetName}");
this.LogCritical("Required runtime service is unavailable.");
this.LogException(exception);
```

普通日志显示为 `[文件名.调用成员名] 消息`。普通消息和插值消息被当前日志级别过滤时，不会构造最终消息；插值消息也不会执行插值表达式。

调用对象是 `GameObject`、`Component`、`ScriptableObject` 等 Unity 对象时，Unity 适配器会自动关联 Console context。普通 C# 对象仍会获得调用位置标签，但没有 Unity context。异常保留 Unity 原生异常与堆栈格式，只自动传递 context。

## 自定义后端

实现不包含 Unity 类型的实例日志契约：

```csharp
public sealed class GameLogger : ILogger
{
    public bool IsEnabled(LogLevel level)
    {
        return true;
    }

    public void Log(LogLevel level, string message, object? context)
    {
        // 写入自定义日志目标。
    }

    public void LogException(LogLevel level, Exception exception, object? context)
    {
        // 写入自定义异常目标。
    }
}
```

日志器实例作为扩展接收器，日志源上下文作为第一个参数：

```csharp
var logger = new GameLogger();
logger.Log(this, $"Health: {health}");
logger.LogWarning(this, "Connection is unstable.");
logger.LogException(this, exception);
```

Unity 适配程序集公开提供无状态的 `UnityDebugLogger`，可直接用于依赖注入或需要显式日志器的调用位置：

```csharp
var logger = default(UnityDebugLogger);
logger.Log(this, $"Health: {health}");
```

## 源码

权威源码分别位于：

- `Runtime/Core/`
- `Runtime/Unity/`

仓库中的两个 .NET 项目通过链接编译项直接引用这些源码，同时参与解决方案构建，不需要额外同步。

本地开发时，在 `src/Xoderony.Logging.Unity/Xoderony.Logging.Unity.local.props` 中配置已安装 Unity Editor 的托管程序集目录；该机器专用文件不会提交到仓库：

```xml
<Project>
  <PropertyGroup>
    <UnityManagedPath>Unity安装目录/Editor/Data/Managed/UnityEngine</UnityManagedPath>
  </PropertyGroup>
</Project>
```

## 安装

```text
https://github.com/Xoderony/Xoderony.git?path=unity/io.github.xoderony.logging
```

## 兼容性

- Unity 7（技术版本 `7000.0`）或更高版本

## 许可证

[MIT](LICENSE.md)

## netstandard2.1 DLL 兼容版

两个程序集同时生成 `net10.0` 和 `netstandard2.1`；Unity 6000.7 使用兼容版 DLL。UPM 源码安装仍面向 Unity 7。

兼容版提供普通字符串/Span 日志、等级过滤、调用位置标签、context 与异常传递；不提供插值处理器。上文“过滤后不执行插值表达式”的保证仅适用于 .NET 10 处理器路径。Unity C# 9 的 `$"..."` 会在调用日志方法前求值；需要避免这项开销时，先检查 `logger.IsEnabled(level)`，再构造消息。

.NET 10 继续使用 `DefaultInterpolatedStringHandler`。兼容版在通过等级过滤后使用 `StringBuilder` 构造带标签的消息，分配特征与 .NET 10 不同。
