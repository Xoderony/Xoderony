#if NET10_0_OR_GREATER
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Xoderony.Logging;

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct DebugLogInterpolatedStringHandler<TLogger> where TLogger : ILogger
{
    private LogInterpolatedStringHandlerCore _handler;

    public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, TLogger logger, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        IsEnabled = logger.IsEnabled(LogLevel.Debug);
        shouldAppend = IsEnabled;
        _handler = IsEnabled ? new LogInterpolatedStringHandlerCore(literalLength, formattedCount, filePath, memberName) : default;
    }

    internal bool IsEnabled { get; }

    public void AppendLiteral(string value)
    {
        _handler.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        _handler.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        _handler.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(string? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(object? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct LogInterpolatedStringHandler<TLogger> where TLogger : ILogger
{
    private LogInterpolatedStringHandlerCore _handler;

    public LogInterpolatedStringHandler(int literalLength, int formattedCount, TLogger logger, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        IsEnabled = logger.IsEnabled(LogLevel.Information);
        shouldAppend = IsEnabled;
        _handler = IsEnabled ? new LogInterpolatedStringHandlerCore(literalLength, formattedCount, filePath, memberName) : default;
    }

    internal bool IsEnabled { get; }

    public void AppendLiteral(string value)
    {
        _handler.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        _handler.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        _handler.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(string? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(object? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct WarningLogInterpolatedStringHandler<TLogger> where TLogger : ILogger
{
    private LogInterpolatedStringHandlerCore _handler;

    public WarningLogInterpolatedStringHandler(int literalLength, int formattedCount, TLogger logger, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        IsEnabled = logger.IsEnabled(LogLevel.Warning);
        shouldAppend = IsEnabled;
        _handler = IsEnabled ? new LogInterpolatedStringHandlerCore(literalLength, formattedCount, filePath, memberName) : default;
    }

    internal bool IsEnabled { get; }

    public void AppendLiteral(string value)
    {
        _handler.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        _handler.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        _handler.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(string? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(object? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct ErrorLogInterpolatedStringHandler<TLogger> where TLogger : ILogger
{
    private LogInterpolatedStringHandlerCore _handler;

    public ErrorLogInterpolatedStringHandler(int literalLength, int formattedCount, TLogger logger, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        IsEnabled = logger.IsEnabled(LogLevel.Error);
        shouldAppend = IsEnabled;
        _handler = IsEnabled ? new LogInterpolatedStringHandlerCore(literalLength, formattedCount, filePath, memberName) : default;
    }

    internal bool IsEnabled { get; }

    public void AppendLiteral(string value)
    {
        _handler.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        _handler.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        _handler.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(string? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(object? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct CriticalLogInterpolatedStringHandler<TLogger> where TLogger : ILogger
{
    private LogInterpolatedStringHandlerCore _handler;

    public CriticalLogInterpolatedStringHandler(int literalLength, int formattedCount, TLogger logger, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        IsEnabled = logger.IsEnabled(LogLevel.Critical);
        shouldAppend = IsEnabled;
        _handler = IsEnabled ? new LogInterpolatedStringHandlerCore(literalLength, formattedCount, filePath, memberName) : default;
    }

    internal bool IsEnabled { get; }

    public void AppendLiteral(string value)
    {
        _handler.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        _handler.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        _handler.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(string? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(object? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}

internal ref struct LogInterpolatedStringHandlerCore
{
    private DefaultInterpolatedStringHandler _handler;

    public LogInterpolatedStringHandlerCore(int literalLength, int formattedCount, string filePath, string memberName)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath.AsSpan());
        _handler = new DefaultInterpolatedStringHandler(literalLength + fileName.Length + memberName.Length + 4, formattedCount);
        _handler.AppendLiteral("[");
        _handler.AppendFormatted(fileName);
        _handler.AppendLiteral(".");
        _handler.AppendLiteral(memberName);
        _handler.AppendLiteral("] ");
    }

    public void AppendLiteral(string value)
    {
        _handler.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        _handler.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        _handler.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        _handler.AppendFormatted(value);
    }

    public void AppendFormatted(string? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(object? value, int alignment = 0, string? format = null)
    {
        _handler.AppendFormatted(value, alignment, format);
    }

    public string GetFormattedText()
    {
        return _handler.ToStringAndClear();
    }
}
#else
using System;
using System.IO;
using System.Text;

namespace Xoderony.Logging;

internal ref struct LogInterpolatedStringHandlerCore {
    private StringBuilder _builder;

    public LogInterpolatedStringHandlerCore(int literalLength, int formattedCount, string filePath, string memberName) {
        var fileName = Path.GetFileNameWithoutExtension(filePath.AsSpan());
        _builder = new StringBuilder(literalLength + fileName.Length + memberName.Length + 4);
        _builder.Append('[').Append(fileName).Append('.').Append(memberName).Append("] ");
    }

    public void AppendFormatted(scoped ReadOnlySpan<char> value) {
        _builder.Append(value);
    }

    public string GetFormattedText() {
        return _builder.ToString();
    }
}
#endif
