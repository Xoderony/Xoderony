using Microsoft.Extensions.DependencyInjection;
using System;
using Xoderony;

namespace Xoderony.Localization.Editor;

internal static class DependencyInjectionExtensions {

    public static IServiceCollection AddDelegateChannel<TDelegate>(this IServiceCollection services) where TDelegate : Delegate {
        services.AddSingleton<DelegateChannel<TDelegate>>();
        services.AddSingleton<IDelegateSubscriber<TDelegate>>(static provider => provider.GetRequiredService<DelegateChannel<TDelegate>>());
        services.AddSingleton<IDelegateDispatcher<TDelegate>>(static provider => provider.GetRequiredService<DelegateChannel<TDelegate>>());
        return services;
    }
}
