using System.Globalization;

namespace Xoderony.Localization;

public interface IStringLocalizer {
    CultureInfo Culture { get; }

    string this[string key] { get; }

    string this[string key, params object?[] arguments] { get; }
}
