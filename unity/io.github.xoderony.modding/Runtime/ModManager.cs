using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xoderony.Modding;

public abstract class ModManager {

    public const string ManifestFileName = "manifest.json";

    private readonly string _modsDirectory;
    private readonly OrderedDictionary<string, ModManifest> _idToManifest = new(StringComparer.Ordinal);
    private readonly OrderedDictionary<string, Mod> _idToMod = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _idToRootDirectory = new(StringComparer.Ordinal);

    protected ModManager(string modsDirectory) {
        Debug.Assert(!string.IsNullOrWhiteSpace(modsDirectory));
        _modsDirectory = Path.GetFullPath(modsDirectory);
    }

    public IReadOnlyList<ModManifest> Manifests => _idToManifest.Values;

    public IReadOnlyList<Mod> Loaded => _idToMod.Values;

    public bool IsLoaded(string modId) {
        return _idToMod.ContainsKey(modId);
    }

    public bool TryGet(string modId, out Mod? mod) {
        return _idToMod.TryGetValue(modId, out mod);
    }

    public bool AreDependenciesSatisfied(string modId) {
        return _idToManifest.TryGetValue(modId, out var manifest) && AreDependenciesSatisfied(manifest);
    }

    protected virtual bool IsVersionSatisfied(string version, string constraint) {
        if (constraint.Length == 0) {
            return true;
        }
        if (Version.TryParse(version, out var actualVersion) && Version.TryParse(constraint, out var requiredVersion)) {
            return actualVersion >= requiredVersion;
        }
        return version == constraint;
    }

    public async ValueTask Refresh() {
        Directory.CreateDirectory(_modsDirectory);
        var manifests = new List<(ModManifest Manifest, string RootDirectory)>();
        var manifestIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var directory in Directory.EnumerateDirectories(_modsDirectory)) {
            var manifestPath = Path.Combine(directory, ManifestFileName);
            if (!File.Exists(manifestPath)) {
                continue;
            }
            var manifest = ReadManifest(manifestPath);
            if (manifest.Id.Length == 0 || manifest.Version.Length == 0) {
                throw new InvalidDataException($"The manifest '{manifestPath}' must have id and version.");
            }
            if (!manifestIds.Add(manifest.Id)) {
                throw new InvalidDataException($"Duplicate mod id '{manifest.Id}'.");
            }
            manifests.Add((manifest, Path.GetFullPath(directory)));
        }

        ThrowIfCyclicDependencies(manifests);

        var loadedIds = new List<string>(_idToMod.Keys);
        var exceptions = new List<Exception>();
        foreach (var id in loadedIds) {
            try {
                await Unload(id);
            } catch (Exception exception) {
                exceptions.Add(exception);
            }
        }
        foreach (var id in new List<string>(_idToMod.Keys)) {
            try {
                await Unload(id);
            } catch (Exception exception) {
                exceptions.Add(exception);
            }
        }

        _idToManifest.Clear();
        _idToRootDirectory.Clear();
        foreach (var (manifest, rootDirectory) in manifests) {
            _idToManifest.Add(manifest.Id, manifest);
            _idToRootDirectory.Add(manifest.Id, rootDirectory);
        }

        foreach (var id in loadedIds) {
            if (!_idToManifest.ContainsKey(id)) {
                continue;
            }
            try {
                await Load(id);
            } catch (Exception exception) {
                exceptions.Add(exception);
            }
        }

        if (exceptions.Count > 0) {
            throw new AggregateException(exceptions);
        }

        static void ThrowIfCyclicDependencies(List<(ModManifest Manifest, string RootDirectory)> manifests) {
            var idToManifest = new Dictionary<string, ModManifest>(StringComparer.Ordinal);
            foreach (var (manifest, _) in manifests) {
                idToManifest.Add(manifest.Id, manifest);
            }

            var visitingIds = new HashSet<string>(StringComparer.Ordinal);
            var visitedIds = new HashSet<string>(StringComparer.Ordinal);
            var path = new List<string>();
            foreach (var id in idToManifest.Keys) {
                Visit(id);
            }

            void Visit(string id) {
                if (visitedIds.Contains(id)) {
                    return;
                }
                if (!visitingIds.Add(id)) {
                    var index = path.IndexOf(id);
                    throw new InvalidDataException($"Cyclic dependency: {string.Join(" -> ", path.GetRange(index, path.Count - index))} -> {id}.");
                }
                path.Add(id);
                foreach (var depId in idToManifest[id].Dependencies.Keys) {
                    if (idToManifest.ContainsKey(depId)) {
                        Visit(depId);
                    }
                }
                path.RemoveAt(path.Count - 1);
                visitingIds.Remove(id);
                visitedIds.Add(id);
            }
        }

        static ModManifest ReadManifest(string path) {
            try {
                using var stream = File.OpenRead(path);
                var manifest = JsonSerializer.Deserialize<ModManifest>(stream, JsonSerializerOptions.Web);
                Debug.Assert(manifest is not null);
                return manifest;
            } catch (JsonException exception) {
                throw new InvalidDataException($"The JSON file '{path}' is invalid.", exception);
            }
        }
    }

    public async ValueTask Load(string modId) {
        if (_idToMod.ContainsKey(modId) || !_idToManifest.TryGetValue(modId, out var manifest) || !AreDependenciesSatisfied(manifest)) {
            return;
        }
        foreach (var depId in manifest.Dependencies.Keys) {
            await Load(depId);
            if (!_idToMod.ContainsKey(depId)) {
                return;
            }
        }

        var rootDirectory = _idToRootDirectory[modId];
        var mod = CreateMod(manifest, rootDirectory);
        try {
            await mod.Load();
        } catch {
            ReleaseMod(mod);
            throw;
        }
        _idToMod.Add(modId, mod);
    }

    public async ValueTask Unload(string modId) {
        if (!_idToMod.TryGetValue(modId, out var mod)) {
            return;
        }
        foreach (var other in new List<Mod>(_idToMod.Values)) {
            if (other.Manifest.Dependencies.ContainsKey(modId)) {
                await Unload(other.Manifest.Id);
            }
        }
        try {
            await mod.Unload();
        } finally {
            _idToMod.Remove(modId);
            ReleaseMod(mod);
        }
    }

    protected abstract Mod CreateMod(ModManifest manifest, string rootDirectory);

    protected abstract void ReleaseMod(Mod mod);

    private bool AreDependenciesSatisfied(ModManifest manifest) {
        foreach (var (depId, constraint) in manifest.Dependencies) {
            if (!_idToManifest.TryGetValue(depId, out var dependency) || !IsVersionSatisfied(dependency.Version, constraint)) {
                return false;
            }
        }
        return true;
    }
}
