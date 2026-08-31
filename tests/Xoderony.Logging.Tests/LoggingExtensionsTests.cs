using System;
using Xunit;

namespace Xoderony.Logging.Tests;

public class LoggingExtensionsTests {

    [Fact]
    public void FilteredInterpolation_DoesNotEvaluateExpression() {
        var logger = new RecordingLogger { Enabled = false };
        var evaluationCount = 0;

        logger.Log(this, $"Value: {Increment(ref evaluationCount)}");

        Assert.Equal(0, evaluationCount);
        Assert.Null(logger.Message);
    }

    [Fact]
    public void FilteredSpanMessage_DoesNotCallSink() {
        var logger = new RecordingLogger { Enabled = false };

        logger.Log(this, "Ignored");

        Assert.Null(logger.Message);
    }

    [Fact]
    public void SpanMessage_AddsExplicitCallSiteTagAndContext() {
        var logger = new RecordingLogger();
        var context = new object();

        logger.Log(context, "Started", @"C:\Source\Player.cs", "Awake");

        Assert.Equal(LogLevel.Information, logger.Level);
        Assert.Equal("[Player.Awake] Started", logger.Message);
        Assert.Same(context, logger.Context);
    }

    [Theory]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void LevelExtensions_ForwardExpectedLevel(LogLevel level) {
        var logger = new RecordingLogger();
        var context = new object();

        switch (level) {
            case LogLevel.Debug:
                logger.LogDebug(context, "Message", "Source.cs", "Run");
                break;
            case LogLevel.Information:
                logger.Log(context, "Message", "Source.cs", "Run");
                break;
            case LogLevel.Warning:
                logger.LogWarning(context, "Message", "Source.cs", "Run");
                break;
            case LogLevel.Error:
                logger.LogError(context, "Message", "Source.cs", "Run");
                break;
            case LogLevel.Critical:
                logger.LogCritical(context, "Message", "Source.cs", "Run");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level));
        }

        Assert.Equal(level, logger.Level);
        Assert.Equal("[Source.Run] Message", logger.Message);
        Assert.Same(context, logger.Context);
    }

    [Fact]
    public void InterpolatedMessage_PreservesFormattingAndSpan() {
        var logger = new RecordingLogger();
        ReadOnlySpan<char> state = "Ready";

        logger.Log(this, $"Value: {42,4:X2}; State: {state}");

        Assert.Equal("[LoggingExtensionsTests.InterpolatedMessage_PreservesFormattingAndSpan] Value:   2A; State: Ready", logger.Message);
    }

    [Fact]
    public void LogException_UsesLevelFilteringAndPreservesValues() {
        var logger = new RecordingLogger { Enabled = false };
        var context = new object();
        var exception = new InvalidOperationException("Failed");

        logger.LogException(context, exception, LogLevel.Critical);
        Assert.Null(logger.Exception);

        logger.Enabled = true;
        logger.LogException(context, exception, LogLevel.Critical);

        Assert.Equal(LogLevel.Critical, logger.Level);
        Assert.Same(exception, logger.Exception);
        Assert.Same(context, logger.Context);
    }

    private static int Increment(ref int value) {
        value++;
        return value;
    }

    private sealed class RecordingLogger : ILogger {

        public bool Enabled { get; set; } = true;

        public LogLevel? Level { get; private set; }

        public string? Message { get; private set; }

        public Exception? Exception { get; private set; }

        public object? Context { get; private set; }

        public bool IsEnabled(LogLevel level) {
            return Enabled;
        }

        public void Log(LogLevel level, string message, object? context) {
            Level = level;
            Message = message;
            Context = context;
        }

        public void LogException(LogLevel level, Exception exception, object? context) {
            Level = level;
            Exception = exception;
            Context = context;
        }
    }
}
