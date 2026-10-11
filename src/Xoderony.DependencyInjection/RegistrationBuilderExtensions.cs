namespace Xoderony.DependencyInjection;

public static class RegistrationBuilderExtensions {
    /// <summary>将实现的接口逐个追加为服务别名；包含容器保留的集合接口时抛出异常，已追加的别名不回滚。</summary>
    public static RegistrationBuilder AsImplementedInterfaces(this RegistrationBuilder builder) {
        foreach (var interfaceType in builder.ImplementationType.GetInterfaces()) {
            builder.As(interfaceType);
        }
        return builder;
    }
}
