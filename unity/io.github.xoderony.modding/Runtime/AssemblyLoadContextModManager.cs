using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Xoderony.Modding;

public class AssemblyLoadContextModManager(string modsDirectory) : ModManager(modsDirectory) {

    private readonly Dictionary<string, AssemblyLoadContext> _idToContext = new(StringComparer.Ordinal);

    protected override Mod CreateMod(ModManifest manifest, string rootDirectory) {
        var context = new AssemblyLoadContext(manifest.Id, isCollectible: true);
        try {
            foreach (var path in Directory.EnumerateFiles(rootDirectory, "*.dll", SearchOption.TopDirectoryOnly)) {
                try {
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
            context.Unload();
            throw;
        }
    }

    protected override void ReleaseMod(Mod mod) {
        if (_idToContext.Remove(mod.Manifest.Id, out var context)) {
            context.Unload();
        }
    }
}
