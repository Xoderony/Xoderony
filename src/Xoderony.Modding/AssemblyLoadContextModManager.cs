using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Xoderony.Modding;

public class AssemblyLoadContextModManager : ModManager {

    private static readonly Assembly ContractAssembly = typeof(Mod).Assembly;
    private static readonly AssemblyName ContractAssemblyName = ContractAssembly.GetName();

    private readonly Action<string, Exception> _onUnloadError;
    private readonly Dictionary<string, AssemblyLoadContext> _idToContext = new(StringComparer.Ordinal);

    /// <remarks><paramref name="onUnloadError"/> 接收 Mod ID 和上下文卸载异常；回调不得向外传播异常。</remarks>
    public AssemblyLoadContextModManager(string modsDirectory, Action<string, Exception> onUnloadError) : base(modsDirectory) {
        Debug.Assert(onUnloadError is not null);
        _onUnloadError = onUnloadError;
    }

    protected override Mod CreateMod(ModManifest manifest, string rootDirectory) {
        var context = new ModLoadContext(manifest.Id);
        try {
            foreach (var path in Directory.EnumerateFiles(rootDirectory, "*.dll", SearchOption.TopDirectoryOnly)) {
                try {
                    if (AssemblyName.ReferenceMatchesDefinition(AssemblyName.GetAssemblyName(path), ContractAssemblyName)) {
                        continue;
                    }
                    context.LoadFromAssemblyPath(Path.GetFullPath(path));
                } catch (BadImageFormatException) {
                }
            }

            Type? entryType = null;
            foreach (var assembly in context.Assemblies) {
                foreach (var type in assembly.GetExportedTypes()) {
                    if (type.IsAbstract || !type.IsSubclassOf(typeof(Mod))) {
                        continue;
                    }
                    if (entryType is not null) {
                        throw new InvalidDataException($"Multiple Mod entry types were found in '{rootDirectory}'.");
                    }
                    entryType = type;
                }
            }
            if (entryType is null) {
                throw new InvalidDataException($"No Mod entry type was found in '{rootDirectory}'.");
            }

            if (Activator.CreateInstance(entryType, manifest, rootDirectory) is not Mod instance) {
                throw new InvalidDataException($"Failed to create Mod entry '{entryType}' in '{rootDirectory}'.");
            }
            _idToContext.Add(manifest.Id, context);
            return instance;
        } catch {
            UnloadContext(manifest.Id, context);
            throw;
        }
    }

    protected override void ReleaseMod(Mod mod) {
        if (_idToContext.Remove(mod.Manifest.Id, out var context)) {
            UnloadContext(mod.Manifest.Id, context);
        }
    }

    private void UnloadContext(string modId, AssemblyLoadContext context) {
        try {
            context.Unload();
        } catch (Exception exception) {
            _onUnloadError(modId, exception);
        }
    }

    /// <summary>
    /// 复用宿主契约程序集，保持 Mod 入口及构造参数在加载上下文之间的类型一致。
    /// </summary>
    private sealed class ModLoadContext(string name) : AssemblyLoadContext(name, isCollectible: true) {

        protected override Assembly? Load(AssemblyName assemblyName) {
            if (AssemblyName.ReferenceMatchesDefinition(assemblyName, ContractAssemblyName)) {
                return ContractAssembly;
            }
            return null;
        }
    }
}
