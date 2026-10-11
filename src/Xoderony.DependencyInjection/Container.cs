using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Xoderony.DependencyInjection.Internal;

namespace Xoderony.DependencyInjection;

/// <summary>由调用方保证串行使用；子作用域必须先于父容器释放。</summary>
public sealed class Container : IResolver, IServiceProvider, IDisposable {
    private static readonly MethodInfo AsReadOnlyMethod = typeof(Array).GetMethod(nameof(Array.AsReadOnly))!;
    private readonly Dictionary<(Type ServiceType, int Key), Lazy<object>> _registrations = [];
    private readonly List<IDisposable> _disposables = [];
    private readonly ConditionalWeakTable<Type, MemberInjection> _memberInjections;

    internal Container(List<RegistrationBuilder> registrations, Container? parent) {
        _memberInjections = parent?._memberInjections ?? [];
        var declarations = new HashSet<(Type Type, int Key)>();
        var groups = new Dictionary<(Type ServiceType, int Key), List<Lazy<object>>>();
        var instancesToActivate = new List<Lazy<object>>();
        foreach (var registration in registrations) {
            if (!declarations.Add((registration.ImplementationType, registration.Key))) {
                throw new ArgumentException($"Type '{registration.ImplementationType}' (key: {registration.Key}) is declared more than once in the same container.", nameof(registrations));
            }
            Lazy<object> instance;
            if (registration.Instance is { } existing) {
                instance = new Lazy<object>(existing);
            } else if (registration.Factory is { } factory) {
                instance = new Lazy<object>(() => CreateInstance(factory), LazyThreadSafetyMode.None);
            } else {
                var implementationType = registration.ImplementationType;
                instance = new Lazy<object>(() => CreateInstance(implementationType), LazyThreadSafetyMode.None);
            }
            if (registration.IsAutoActivationEnabled) {
                instancesToActivate.Add(instance);
            }
            foreach (var type in registration.GetServiceTypes()) {
                ref var group = ref CollectionsMarshal.GetValueRefOrAddDefault(groups, (type, registration.Key), out _);
                group ??= [];
                group.Add(instance);
            }
        }
        // 本层容器、解析器及其集合都必须指向本层，不继承父层入口。
        var currentContainer = new Lazy<object>(this);
        groups[(typeof(Container), 0)] = [currentContainer];
        groups[(typeof(IResolver), 0)] = [currentContainer];
        foreach (var ((type, key), entries) in groups) {
            if (entries.Count == 1) {
                _registrations.Add((type, key), entries[0]);
            } else {
                _registrations.Add((type, key), new Lazy<object>(() => throw new InvalidOperationException($"Multiple registrations for {type} (key: {key}). Resolve a collection or use a distinct key."), LazyThreadSafetyMode.None));
            }
            var collection = new Lazy<object>(() => CreateCollection(type, entries), LazyThreadSafetyMode.None);
            _registrations.Add((typeof(IReadOnlyList<>).MakeGenericType(type), key), collection);
            _registrations.Add((typeof(IEnumerable<>).MakeGenericType(type), key), collection);
            _registrations.Add((type.MakeArrayType(), key), collection);
        }
        if (parent is not null) {
            // 本层组的四个入口已完整建立，继承不会残留同组的旧集合。
            foreach (var (key, value) in parent._registrations) {
                _registrations.TryAdd(key, value);
            }
        }

        // 完整映射建立后按原始注册激活，不受别名歧义影响，也不遍历父层注册。
        foreach (var instance in instancesToActivate) {
            _ = instance.Value;
        }

        static object CreateCollection(Type elementType, List<Lazy<object>> entries) {
            var items = Array.CreateInstance(elementType, entries.Count);
            for (var i = 0; i < entries.Count; i++) {
                items.SetValue(entries[i].Value, i);
            }
            var method = AsReadOnlyMethod.MakeGenericMethod(elementType);
            var invoker = MethodInvoker.Create(method);
            return invoker.Invoke(null, items)!;
        }
    }

    /// <inheritdoc />
    public T Resolve<T>(int key = 0) where T : class {
        return (T)Resolve(typeof(T), key);
    }

    /// <inheritdoc />
    public object Resolve(Type serviceType, int key = 0) {
        return ResolveCore(serviceType, key) ?? throw new InvalidOperationException($"No registration for {serviceType} (key: {key}).");
    }

    /// <inheritdoc />
    public bool TryResolve<T>([NotNullWhen(true)] out T? instance, int key = 0) where T : class {
        instance = (T?)ResolveCore(typeof(T), key);
        return instance is not null;
    }

    /// <inheritdoc />
    public bool IsRegistered<T>(int key = 0) where T : class {
        return _registrations.ContainsKey((typeof(T), key));
    }

    /// <inheritdoc />
    public void Inject(object instance) {
        GetMemberInjection(instance.GetType()).Inject(instance, this);
    }

    /// <summary>创建继承当前注册的子容器；configure 同步配置子层注册，同服务与键的组整组覆盖父层。</summary>
    /// <remarks>继承的实例仍由父层创建与拥有；调用方必须先释放子容器，父容器不会代为释放。</remarks>
    public Container CreateChild(Action<ContainerBuilder>? configure = null) {
        var builder = new ContainerBuilder();
        configure?.Invoke(builder);
        return builder.Build(this);
    }

    object? IServiceProvider.GetService(Type serviceType) {
        return ResolveCore(serviceType, 0);
    }

    /// <summary>释放本层拥有的服务；不释放子作用域或外部来源对象。</summary>
    public void Dispose() {
        for (var i = _disposables.Count - 1; i >= 0; i--) {
            _disposables[i].Dispose();
        }
        _disposables.Clear();
        _registrations.Clear();
    }

    private object CreateInstance(Type implementationType) {
        var injection = GetMemberInjection(implementationType);
        var (constructor, dependencies) = InjectionAnalyzer.AnalyzeConstructor(implementationType);
        var buffer = ArrayPool<object?>.Shared.Rent(dependencies.Length);
        object instance;
        try {
            var arguments = buffer.AsSpan(0, dependencies.Length);
            for (var i = 0; i < dependencies.Length; i++) {
                var dependency = dependencies[i];
                arguments[i] = Resolve(dependency.Type, dependency.Key);
            }
            instance = constructor.Invoke(arguments);
        } finally {
            ArrayPool<object?>.Shared.Return(buffer, clearArray: true);
        }
        injection.Inject(instance, this);
        if (instance is IDisposable disposable) {
            _disposables.Add(disposable);
        }
        return instance;
    }

    private object CreateInstance(Func<IResolver, object> factory) {
        var instance = factory(this) ?? throw new InvalidOperationException("The service factory returned null.");
        if (instance is IDisposable disposable) {
            _disposables.Add(disposable);
        }
        return instance;
    }

    private MemberInjection GetMemberInjection(Type type) {
        return _memberInjections.GetValue(type, InjectionAnalyzer.AnalyzeMembers);
    }

    private object? ResolveCore(Type serviceType, int key) {
        // 非法服务类型无法注册，统一按未注册处理；不在解析热路径重复校验。
        if (!_registrations.TryGetValue((serviceType, key), out var instance)) {
            return null;
        }
        var result = instance.Value;
        if (serviceType.IsSZArray) {
            var items = (ICollection)result;
            var copy = Array.CreateInstance(serviceType.GetElementType()!, items.Count);
            items.CopyTo(copy, 0);
            return copy;
        }
        return result;
    }
}
