using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xoderony.Modding;

public abstract class ModManager {

    public const string ManifestFileName = "manifest.json";

    private static readonly JsonSerializerOptions ManifestJsonOptions = new(JsonSerializerOptions.Web) {
        RespectNullableAnnotations = true
    };

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

    public bool TryGet(string modId, [NotNullWhen(true)] out Mod? mod) {
        return _idToMod.TryGetValue(modId, out mod);
    }

    /// <summary>
    /// 仅检查直接依赖的清单与版本，不表示依赖已经加载。
    /// </summary>
    public bool AreDependenciesSatisfied(string modId) {
        return _idToManifest.TryGetValue(modId, out var manifest) && AreDependenciesSatisfied(manifest);
    }

    /// <summary>
    /// 可解析的约束按最低版本比较，否则要求字符串相等；空约束接受任意版本。
    /// </summary>
    protected virtual bool IsVersionSatisfied(string version, string constraint) {
        if (constraint.Length == 0) {
            return true;
        }
        if (Version.TryParse(version, out var actualVersion) && Version.TryParse(constraint, out var requiredVersion)) {
            return actualVersion >= requiredVersion;
        }
        return version == constraint;
    }

    /// <summary>
    /// 扫描检查通过后，按加载顺序逆序卸载现有实例，并按新清单恢复此前已加载的 Mod。
    /// </summary>
    /// <remarks>异常不回滚已完成的变更。</remarks>
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
            if (!manifestIds.Add(manifest.Id)) {
                throw new InvalidDataException($"Duplicate mod id '{manifest.Id}'.");
            }
            manifests.Add((manifest, Path.GetFullPath(directory)));
        }

        ThrowIfCyclicDependencies(manifests);

        var loadedIds = new List<string>(_idToMod.Keys);
        for (var i = loadedIds.Count - 1; i >= 0; i--) {
            await Unload(loadedIds[i]);
        }

        _idToManifest.Clear();
        _idToRootDirectory.Clear();
        foreach (var (manifest, rootDirectory) in manifests) {
            _idToManifest.Add(manifest.Id, manifest);
            _idToRootDirectory.Add(manifest.Id, rootDirectory);
        }

        var exceptions = new List<Exception>();
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
                var manifest = JsonSerializer.Deserialize<ModManifest>(stream, ManifestJsonOptions) ?? throw new InvalidDataException($"The manifest '{path}' must contain a JSON object at '$'.");
                if (manifest.Id.Length == 0) {
                    throw new InvalidDataException($"The manifest '{path}' field 'id' must not be empty.");
                }
                if (manifest.Version.Length == 0) {
                    throw new InvalidDataException($"The manifest '{path}' field 'version' must not be empty.");
                }
                foreach (var (depId, constraint) in manifest.Dependencies) {
                    if (depId.Length == 0) {
                        throw new InvalidDataException($"The manifest '{path}' field 'dependencies' must not contain an empty mod id.");
                    }
                    if (constraint is null) {
                        throw new InvalidDataException($"The manifest '{path}' field 'dependencies[\"{depId}\"]' must be a string.");
                    }
                }
                return manifest;
            } catch (JsonException exception) {
                throw new InvalidDataException($"The manifest '{path}' contains invalid JSON at '{exception.Path ?? "$"}'.", exception);
            }
        }
    }

    /// <summary>
    /// 未知 ID、已加载或依赖不满足时直接返回，调用方可用 <see cref="IsLoaded"/> 或 <see cref="TryGet"/> 查询结果。
    /// </summary>
    /// <remarks>
    /// 实例加载失败时，先调用一次 <see cref="Mod.Unload"/>，再调用 <see cref="ReleaseMod"/>，然后重新抛出加载异常。
    /// 清理方法必须自行处理并记录错误，不得向外传播异常。
    /// </remarks>
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
            await mod.Unload();
            ReleaseMod(mod);
            throw;
        }
        _idToMod.Add(modId, mod);
    }

    /// <summary>
    /// 先卸载依赖当前 Mod 的实例；不会自动卸载当前 Mod 所依赖的其他 Mod。
    /// </summary>
    /// <remarks>
    /// 依次卸载实例、移除记录并释放实例；清理方法必须自行处理并记录错误，不得向外传播异常。
    /// </remarks>
    public async ValueTask Unload(string modId) {
        if (!_idToMod.TryGetValue(modId, out var mod)) {
            return;
        }
        foreach (var other in new List<Mod>(_idToMod.Values)) {
            if (other.Manifest.Dependencies.ContainsKey(modId)) {
                await Unload(other.Manifest.Id);
            }
        }
        await mod.Unload();
        _idToMod.Remove(modId);
        ReleaseMod(mod);
    }

    /// <remarks>
    /// 每次调用创建新实例；创建失败且未返回实例时，由实现负责清理已创建的资源。
    /// 清理错误应就地处理并记录，不得覆盖原始创建异常。
    /// </remarks>
    protected abstract Mod CreateMod(ModManifest manifest, string rootDirectory);

    /// <remarks>实现必须自行处理并记录底层释放错误，不得向外传播异常。</remarks>
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
