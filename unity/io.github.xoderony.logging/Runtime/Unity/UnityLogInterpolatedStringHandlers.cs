#if NET10_0_OR_GREATER
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xoderony.Logging;

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct UnityDebugLogInterpolatedStringHandler
{
    private DebugLogInterpolatedStringHandler<UnityDebugLogger> _handler;

    public UnityDebugLogInterpolatedStringHandler(int literalLength, int formattedCount, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        var logger = default(UnityDebugLogger);
        _handler = new DebugLogInterpolatedStringHandler<UnityDebugLogger>(literalLength, formattedCount, logger, out shouldAppend, filePath, memberName);
        IsEnabled = shouldAppend;
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

    internal string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}
[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct UnityLogInterpolatedStringHandler
{
    private LogInterpolatedStringHandler<UnityDebugLogger> _handler;

    public UnityLogInterpolatedStringHandler(int literalLength, int formattedCount, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        var logger = default(UnityDebugLogger);
        _handler = new LogInterpolatedStringHandler<UnityDebugLogger>(literalLength, formattedCount, logger, out shouldAppend, filePath, memberName);
        IsEnabled = shouldAppend;
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

    internal string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct UnityWarningLogInterpolatedStringHandler
{
    private WarningLogInterpolatedStringHandler<UnityDebugLogger> _handler;

    public UnityWarningLogInterpolatedStringHandler(int literalLength, int formattedCount, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        var logger = default(UnityDebugLogger);
        _handler = new WarningLogInterpolatedStringHandler<UnityDebugLogger>(literalLength, formattedCount, logger, out shouldAppend, filePath, memberName);
        IsEnabled = shouldAppend;
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

    internal string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct UnityErrorLogInterpolatedStringHandler
{
    private ErrorLogInterpolatedStringHandler<UnityDebugLogger> _handler;

    public UnityErrorLogInterpolatedStringHandler(int literalLength, int formattedCount, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        var logger = default(UnityDebugLogger);
        _handler = new ErrorLogInterpolatedStringHandler<UnityDebugLogger>(literalLength, formattedCount, logger, out shouldAppend, filePath, memberName);
        IsEnabled = shouldAppend;
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

    internal string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
public ref struct UnityCriticalLogInterpolatedStringHandler
{
    private CriticalLogInterpolatedStringHandler<UnityDebugLogger> _handler;

    public UnityCriticalLogInterpolatedStringHandler(int literalLength, int formattedCount, out bool shouldAppend, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "")
    {
        var logger = default(UnityDebugLogger);
        _handler = new CriticalLogInterpolatedStringHandler<UnityDebugLogger>(literalLength, formattedCount, logger, out shouldAppend, filePath, memberName);
        IsEnabled = shouldAppend;
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

    internal string GetFormattedText()
    {
        return _handler.GetFormattedText();
    }
}
#endif
