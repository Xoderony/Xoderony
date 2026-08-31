using System;
using System.Runtime.CompilerServices;

namespace Xoderony.Logging;

public static class UnityLoggingExtensions {
    extension(object context) {
        public void LogDebug(scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            var logger = default(UnityDebugLogger);
            logger.LogDebug(context, message, filePath, memberName);
        }

        public void LogDebug(ref UnityDebugLogInterpolatedStringHandler message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            var logger = default(UnityDebugLogger);
            logger.Log(LogLevel.Debug, taggedMessage, context);
        }

        public void Log(scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            var logger = default(UnityDebugLogger);
            logger.Log(context, message, filePath, memberName);
        }

        public void Log(ref UnityLogInterpolatedStringHandler message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            var logger = default(UnityDebugLogger);
            logger.Log(LogLevel.Information, taggedMessage, context);
        }

        public void LogWarning(scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            var logger = default(UnityDebugLogger);
            logger.LogWarning(context, message, filePath, memberName);
        }

        public void LogWarning(ref UnityWarningLogInterpolatedStringHandler message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            var logger = default(UnityDebugLogger);
            logger.Log(LogLevel.Warning, taggedMessage, context);
        }

        public void LogError(scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            var logger = default(UnityDebugLogger);
            logger.LogError(context, message, filePath, memberName);
        }

        public void LogError(ref UnityErrorLogInterpolatedStringHandler message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            var logger = default(UnityDebugLogger);
            logger.Log(LogLevel.Error, taggedMessage, context);
        }

        public void LogCritical(scoped ReadOnlySpan<char> message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
            var logger = default(UnityDebugLogger);
            logger.LogCritical(context, message, filePath, memberName);
        }

        public void LogCritical(ref UnityCriticalLogInterpolatedStringHandler message) {
            if (!message.IsEnabled) {
                return;
            }

            var taggedMessage = message.GetFormattedText();
            var logger = default(UnityDebugLogger);
            logger.Log(LogLevel.Critical, taggedMessage, context);
        }

        public void LogException(Exception exception, LogLevel level = LogLevel.Error) {
            var logger = default(UnityDebugLogger);
            logger.LogException(level, exception, context);
        }
    }
}
