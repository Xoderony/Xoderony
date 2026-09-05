using System;
using System.Runtime.CompilerServices;

namespace Xoderony.Logging;

public static class LoggingExtensions {
    extension<TLogger>(TLogger logger) where TLogger : ILogger {
        public void LogDebug(object? context, scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            if (!logger.IsEnabled(LogLevel.Debug)) {
                return;
            }

            var handler = new LogInterpolatedStringHandlerCore(message.Length, 0, filePath, memberName);
            handler.AppendFormatted(message);
            var taggedMessage = handler.GetFormattedText();
            logger.Log(LogLevel.Debug, taggedMessage, context);
        }

#if NET10_0_OR_GREATER
        public void LogDebug(object? context, [InterpolatedStringHandlerArgument("logger")] ref DebugLogInterpolatedStringHandler<TLogger> message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            logger.Log(LogLevel.Debug, taggedMessage, context);
        }
#endif

        public void Log(object? context, scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            if (!logger.IsEnabled(LogLevel.Information)) {
                return;
            }

            var handler = new LogInterpolatedStringHandlerCore(message.Length, 0, filePath, memberName);
            handler.AppendFormatted(message);
            var taggedMessage = handler.GetFormattedText();
            logger.Log(LogLevel.Information, taggedMessage, context);
        }

#if NET10_0_OR_GREATER
        public void Log(object? context, [InterpolatedStringHandlerArgument("logger")] ref LogInterpolatedStringHandler<TLogger> message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            logger.Log(LogLevel.Information, taggedMessage, context);
        }
#endif

        public void LogWarning(object? context, scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            if (!logger.IsEnabled(LogLevel.Warning)) {
                return;
            }

            var handler = new LogInterpolatedStringHandlerCore(message.Length, 0, filePath, memberName);
            handler.AppendFormatted(message);
            var taggedMessage = handler.GetFormattedText();
            logger.Log(LogLevel.Warning, taggedMessage, context);
        }

#if NET10_0_OR_GREATER
        public void LogWarning(object? context, [InterpolatedStringHandlerArgument("logger")] ref WarningLogInterpolatedStringHandler<TLogger> message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            logger.Log(LogLevel.Warning, taggedMessage, context);
        }
#endif

        public void LogError(object? context, scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            if (!logger.IsEnabled(LogLevel.Error)) {
                return;
            }

            var handler = new LogInterpolatedStringHandlerCore(message.Length, 0, filePath, memberName);
            handler.AppendFormatted(message);
            var taggedMessage = handler.GetFormattedText();
            logger.Log(LogLevel.Error, taggedMessage, context);
        }

#if NET10_0_OR_GREATER
        public void LogError(object? context, [InterpolatedStringHandlerArgument("logger")] ref ErrorLogInterpolatedStringHandler<TLogger> message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            logger.Log(LogLevel.Error, taggedMessage, context);
        }
#endif

        public void LogCritical(object? context, scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            if (!logger.IsEnabled(LogLevel.Critical)) {
                return;
            }

            var handler = new LogInterpolatedStringHandlerCore(message.Length, 0, filePath, memberName);
            handler.AppendFormatted(message);
            var taggedMessage = handler.GetFormattedText();
            logger.Log(LogLevel.Critical, taggedMessage, context);
        }

#if NET10_0_OR_GREATER
        public void LogCritical(object? context, [InterpolatedStringHandlerArgument("logger")] ref CriticalLogInterpolatedStringHandler<TLogger> message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            logger.Log(LogLevel.Critical, taggedMessage, context);
        }
#endif

        public void LogException(object? context, Exception exception, LogLevel level = LogLevel.Error) {
            if (!logger.IsEnabled(level)) {
                return;
            }

            logger.LogException(level, exception, context);
        }
    }
}
