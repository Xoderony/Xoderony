using System;
using System.Collections.Generic;
using Xoderony.DependencyInjection.Internal;

namespace Xoderony.DependencyInjection;

public sealed class RegistrationBuilder {
    private readonly Type _implementationType;
    private readonly object? _instance;
    private readonly Func<IResolver, object>? _factory;
    private readonly List<Type> _serviceTypes = [];
    private int _key;
    private bool _autoActivationEnabled;

    internal Type ImplementationType => _implementationType;
    internal object? Instance => _instance;
    internal Func<IResolver, object>? Factory => _factory;
    internal int Key => _key;
    internal bool IsAutoActivationEnabled => _autoActivationEnabled;

    internal RegistrationBuilder(Type implementationType, object? instance, Func<IResolver, object>? factory) {
        ValidateServiceType(implementationType);
        _implementationType = implementationType;
        _instance = instance;
        _factory = factory;
    }

    /// <summary>追加服务别名；首次显式指定时取消隐式自身暴露，重复别名忽略。</summary>
    public RegistrationBuilder As<TService>() where TService : class {
        return As(typeof(TService));
    }

    /// <summary>追加兼容的完整闭合服务类型；首次指定时取消隐式自身暴露。容器保留的集合类型不可指定。</summary>
    public RegistrationBuilder As(Type serviceType) {
        ValidateServiceType(serviceType);
        if (!serviceType.IsAssignableFrom(_implementationType)) {
            throw new ArgumentException($"{_implementationType} cannot provide {serviceType}.", nameof(serviceType));
        }
        if (!_serviceTypes.Contains(serviceType)) {
            _serviceTypes.Add(serviceType);
        }
        return this;
    }

    /// <summary>显式追加注册声明类型，保证后续添加其他别名时仍能按自身解析。</summary>
    public RegistrationBuilder AsSelf() {
        return As(_implementationType);
    }

    /// <summary>设置整条注册的键，覆盖之前的键；全部别名及自动集合共用，0 为默认键。</summary>
    public RegistrationBuilder WithKey(int key) {
        _key = key;
        return this;
    }

    /// <summary>枚举键仅按数值区分，必须以 int 为底层类型；0 表示默认键。</summary>
    public RegistrationBuilder WithKey<TKey>(TKey key) where TKey : struct, Enum {
        return WithKey(EnumKey<TKey>.ToInt32(key));
    }

    /// <summary>启用此注册的自动激活，在本层注册就绪后、构建回调前执行；重复调用无额外作用。</summary>
    /// <remarks>沿用原有创建、注入、缓存和所有权规则，不要求暴露自身类型；已有实例不额外注入或接管释放。</remarks>
    public RegistrationBuilder AutoActivate() {
        _autoActivationEnabled = true;
        return this;
    }

    internal Type[] GetServiceTypes() {
        return _serviceTypes.Count == 0 ? [_implementationType] : [.. _serviceTypes];
    }

    private static void ValidateServiceType(Type type) {
        if (type.ContainsGenericParameters) {
            throw new ArgumentException($"Registration requires a fully closed type: '{type}'.", nameof(type));
        }
        if (type.IsSZArray) {
            throw new ArgumentException($"Collection service '{type}' is reserved for automatic registration.", nameof(type));
        }
        if (type.IsGenericType) {
            var definition = type.GetGenericTypeDefinition();
            if (definition == typeof(IEnumerable<>) || definition == typeof(IReadOnlyList<>)) {
                throw new ArgumentException($"Collection service '{type}' is reserved for automatic registration.", nameof(type));
            }
        }
    }
}
