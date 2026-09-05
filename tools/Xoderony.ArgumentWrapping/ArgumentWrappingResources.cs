using System.Resources;
using Microsoft.CodeAnalysis;

namespace Xoderony.ArgumentWrapping;

internal static class ArgumentWrappingResources {

    private static readonly ResourceManager ResourceManager = new("Xoderony.ArgumentWrapping.Resources", typeof(ArgumentWrappingResources).Assembly);

    public static readonly LocalizableString Title = new LocalizableResourceString(nameof(Title), ResourceManager, typeof(ArgumentWrappingResources));
    public static readonly LocalizableString Message = new LocalizableResourceString(nameof(Message), ResourceManager, typeof(ArgumentWrappingResources));
    public static readonly LocalizableString Description = new LocalizableResourceString(nameof(Description), ResourceManager, typeof(ArgumentWrappingResources));

    public static string CodeFixTitle => Title.ToString();
}
