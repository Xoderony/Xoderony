using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;

namespace Xoderony.DependencyInjection.Internal;

internal sealed class MemberInjection {
    internal required List<(FieldInfo Field, Dependency Dependency)> Fields { get; init; }
    internal required List<(MethodInvoker Setter, Dependency Dependency)> Properties { get; init; }
    internal required List<(MethodInvoker Method, Dependency[] Dependencies)> Methods { get; init; }
    internal required int MaxParameterCount { get; init; }

    internal void Inject(object instance, IResolver resolver) {
        foreach (var (field, dependency) in Fields) {
            var value = resolver.Resolve(dependency.Type, dependency.Key);
            field.SetValue(instance, value);
        }
        foreach (var (setter, dependency) in Properties) {
            var value = resolver.Resolve(dependency.Type, dependency.Key);
            setter.Invoke(instance, value);
        }
        if (Methods.Count == 0) {
            return;
        }
        var buffer = ArrayPool<object?>.Shared.Rent(MaxParameterCount);
        try {
            foreach (var (method, dependencies) in Methods) {
                var arguments = buffer.AsSpan(0, dependencies.Length);
                for (var i = 0; i < dependencies.Length; i++) {
                    var dependency = dependencies[i];
                    arguments[i] = resolver.Resolve(dependency.Type, dependency.Key);
                }
                method.Invoke(instance, arguments);
            }
        } finally {
            ArrayPool<object?>.Shared.Return(buffer, clearArray: true);
        }
    }
}
