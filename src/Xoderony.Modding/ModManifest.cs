using System.Collections.Generic;

namespace Xoderony.Modding;

public sealed class ModManifest {

    public string Id { get; set; } = "";

    public string Version { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Name { get; set; } = "";

    public string Author { get; set; } = "";

    public string Description { get; set; } = "";

    public Dictionary<string, string> Dependencies { get; set; } = [];
}
