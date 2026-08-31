using System;
using UnityEngine;

namespace Xoderony.Logging;

public readonly struct UnityDebugLogger : ILogger {
    public bool IsEnabled(LogLevel level) {
        var logType = ToUnityLogType(level);
        return Debug.unityLogger.IsLogTypeAllowed(logType);
    }

    public void Log(LogLevel level, string message, object? context) {
        var logType = ToUnityLogType(level);
        if (!Debug.unityLogger.IsLogTypeAllowed(logType)) {
            return;
        }

        Debug.unityLogger.Log(logType, (object)message, context as UnityEngine.Object);
    }

    public void LogException(LogLevel level, Exception exception, object? context) {
        if (!IsEnabled(level)) {
            return;
        }

        Debug.unityLogger.LogException(exception, context as UnityEngine.Object);
    }

    private static LogType ToUnityLogType(LogLevel level) {
        return level switch {
            LogLevel.Debug => LogType.Log,
            LogLevel.Information => LogType.Log,
            LogLevel.Warning => LogType.Warning,
            LogLevel.Error => LogType.Error,
            LogLevel.Critical => LogType.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(level))
        };
    }
}
