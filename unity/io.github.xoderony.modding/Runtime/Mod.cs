using System.Diagnostics;
using System.Threading.Tasks;

namespace Xoderony.Modding;

public abstract class Mod {

    protected Mod(ModManifest manifest, string rootDirectory) {
        Debug.Assert(manifest is not null);
        Debug.Assert(!string.IsNullOrWhiteSpace(rootDirectory));
        Manifest = manifest;
        RootDirectory = rootDirectory;
    }

    public ModManifest Manifest { get; }

    public string RootDirectory { get; }

    public virtual ValueTask Load() {
        return default;
    }

    public virtual ValueTask Unload() {
        return default;
    }
}
