using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Stride.Core.Assets.Editor.Services;

namespace Xoderony.Numerics.Stride.Editor;

internal static class Module {
    [SuppressMessage("Usage", "CA2255", Justification = "Game Studio loads editor extensions through assembly module initialization.")]
    [ModuleInitializer]
    internal static void Initialize() {
        // 同一解决方案也由资产编译器加载；仅向 Game Studio 注册编辑器插件。
        if (Assembly.GetEntryAssembly()?.GetName().Name != "Stride.GameStudio") {
            return;
        }
        foreach (var plugin in AssetsPlugin.RegisteredPlugins) {
            if (plugin.GetType().FullName == typeof(Q16EditorPlugin).FullName) {
                return;
            }
        }
        AssetsPlugin.RegisterPlugin(typeof(Q16EditorPlugin));
    }
}
