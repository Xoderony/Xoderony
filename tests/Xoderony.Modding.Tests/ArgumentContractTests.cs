using System;
using Xunit;

namespace Xoderony.Modding.Tests;

public sealed class ArgumentContractTests {
    [Fact]
    public void InvalidConfigurationCannotCreateAModOrManager() {
        Assert.Throws<ArgumentNullException>(() => new TestMod(null!, "."));
        Assert.Throws<ArgumentException>(() => new TestMod(new ModManifest(), " "));
        Assert.Throws<ArgumentException>(() => new AssemblyLoadContextModManager(" ", static (_, _) => { }));
        Assert.Throws<ArgumentNullException>(() => new AssemblyLoadContextModManager(".", null!));
    }

    private sealed class TestMod(ModManifest manifest, string rootDirectory) : Mod(manifest, rootDirectory);
}
