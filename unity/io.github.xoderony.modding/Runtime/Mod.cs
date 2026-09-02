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

    /// <remarks>
    /// 加载失败时，管理器仍会调用一次 <see cref="Unload"/>；派生实现应将业务资源清理集中在卸载方法中。
    /// </remarks>
    public virtual ValueTask Load() {
        return default;
    }

    /// <remarks>
    /// 必须支持部分初始化状态，并释放已创建的业务资源。
    /// 实现必须自行处理并记录清理错误，不得向外传播异常。
    /// 由管理器调度时，每个实例至多调用一次；加载失败后的重试使用新实例。
    /// </remarks>
    public virtual ValueTask Unload() {
        return default;
    }
}
