using System;
using System.Collections.Generic;

namespace Xoderony.DependencyInjection;

public sealed class ContainerBuilder {
    private readonly List<RegistrationBuilder> _registrations = [];
    private Action<IResolver>? _callbacks;

    /// <summary>注册由容器创建并拥有的具体类，首次解析创建；默认以自身类型暴露。</summary>
    public RegistrationBuilder Register<T>() where T : class {
        return Register(typeof(T));
    }

    /// <summary>注册完整闭合的非抽象类，首次解析时创建；构造与注入规则在创建时分析。</summary>
    public RegistrationBuilder Register(Type implementationType) {
        return Add(implementationType, null);
    }

    /// <summary>按实例的实际类型注册已有对象；不自动注入，不接管释放责任。</summary>
    public RegistrationBuilder RegisterInstance(object instance) {
        return Add(instance.GetType(), instance);
    }

    /// <summary>首次解析时调用工厂并缓存结果；工厂负责完整装配，返回的非空对象由容器负责释放。</summary>
    /// <remarks>T 是注册声明类型，可为接口或抽象类；不自动成员注入。工厂使用注册所属容器解析依赖。</remarks>
    public RegistrationBuilder RegisterFactory<T>(Func<IResolver, T> factory) where T : class {
        ArgumentNullException.ThrowIfNull(factory);
        var registration = new RegistrationBuilder(typeof(T), null, factory);
        _registrations.Add(registration);
        return registration;
    }

    /// <summary>追加回调，在每次 Build 完成自动激活后、返回前按添加顺序同步执行；不由子容器继承。</summary>
    /// <remarks>回调异常直接传播并中止后续回调，不自动释放已创建服务或回滚副作用。</remarks>
    public void AddBuildCallback(Action<IResolver> callback) {
        ArgumentNullException.ThrowIfNull(callback);
        _callbacks += callback;
    }

    /// <summary>根据当前配置创建新的根容器；后续修改构建器不影响已构建容器。</summary>
    /// <remarks>全部注册就绪后按注册顺序激活 AutoActivate 注册，再执行构建回调；依赖可能先被创建。
    /// 同层声明类型与键重复时拒绝构建。激活或回调异常直接传播，中止构建，不自动释放已创建服务或回滚副作用。</remarks>
    public Container Build() {
        return Build(null);
    }

    internal Container Build(Container? parent) {
        var container = new Container(_registrations, parent);
        _callbacks?.Invoke(container);
        return container;
    }

    private RegistrationBuilder Add(Type implementationType, object? instance) {
        if (!implementationType.IsClass) {
            throw new ArgumentException($"Implementation type '{implementationType}' must be a class.", nameof(implementationType));
        }
        if (implementationType.IsAbstract) {
            throw new ArgumentException($"Implementation type '{implementationType}' is abstract and cannot be instantiated.", nameof(implementationType));
        }
        var registration = new RegistrationBuilder(implementationType, instance, null);
        _registrations.Add(registration);
        return registration;
    }
}
