using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Xoderony.Modding.Tests;

public sealed class RootDirectoryTests {
    [Fact]
    public async Task RootDirectoryUsesScannedFolderRatherThanModIdAndRefreshRemovesMissingEntries() {
        var root = Path.Combine(Path.GetTempPath(), "Xoderony-ModDirectory-" + Guid.NewGuid().ToString("N"));
        var directory = Directory.CreateDirectory(Path.Combine(root, "unrelated-folder")).FullName;
        try {
            var manifest = Path.Combine(directory, ModManager.ManifestFileName);
            File.WriteAllText(manifest, "{\"id\":\"mod-a\",\"version\":\"1.0\"}");
            var manager = new TestManager(root);
            Assert.False(manager.TryGetRootDirectory("mod-a", out var absent));
            Assert.Null(absent);

            await manager.Refresh();

            Assert.False(manager.IsLoaded("mod-a"));
            Assert.True(manager.TryGetRootDirectory("mod-a", out var found));
            Assert.Equal(directory, found);
            Assert.False(manager.TryGetRootDirectory("unrelated-folder", out _));
            File.Delete(manifest);
            await manager.Refresh();
            Assert.False(manager.TryGetRootDirectory("mod-a", out absent));
            Assert.Null(absent);
        } finally {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class TestManager(string directory) : ModManager(directory) {
        protected override Mod CreateMod(ModManifest manifest, string rootDirectory) => throw new NotSupportedException();
        protected override void ReleaseMod(Mod mod) => throw new NotSupportedException();
    }
}
