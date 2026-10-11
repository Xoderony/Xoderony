using System;
using System.Diagnostics.CodeAnalysis;

namespace Xoderony.DependencyInjection;

public interface IResolver {
    /// <summary>解析指定服务；缺失、多个候选或创建失败时抛出异常。默认键为 0。</summary>
    T Resolve<T>(int key = 0) where T : class;
    /// <summary>按完整运行时类型解析服务；缺失、多个候选或创建失败时抛出异常。默认键为 0。</summary>
    object Resolve(Type serviceType, int key = 0);
    /// <summary>仅服务未注册时返回 false；多个候选或创建失败仍抛出异常。</summary>
    bool TryResolve<T>([NotNullWhen(true)] out T? instance, int key = 0) where T : class;
    /// <summary>检查解析入口是否存在，不创建对象；存在多个候选时也返回 true，不保证解析成功。</summary>
    bool IsRegistered<T>(int key = 0) where T : class;
    /// <summary>对已有对象执行成员注入，不接管所有权；每次调用都会重新注入并执行注入方法。</summary>
    void Inject(object instance);
}
