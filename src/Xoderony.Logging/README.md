# Xoderony.Logging

通用日志契约、级别过滤、调用位置标签与插值处理器。由调用方提供日志后端。

## 自定义后端

实现实例日志契约：

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

## 目标框架

仅支持 `net10.0`。插值处理器在消息被过滤时不执行插值表达式。
