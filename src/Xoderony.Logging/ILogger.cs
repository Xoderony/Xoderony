using System;

namespace Xoderony.Logging;

public interface ILogger {
    bool IsEnabled(LogLevel level);

    void Log(LogLevel level, string message, object? context);

    void LogException(LogLevel level, Exception exception, object? context);
}
