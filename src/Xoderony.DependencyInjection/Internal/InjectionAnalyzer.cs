using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xoderony.DependencyInjection.Internal;

internal static class InjectionAnalyzer {
    private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    internal static (ConstructorInvoker Constructor, Dependency[] Dependencies) AnalyzeConstructor(Type type) {
        var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        ConstructorInfo? selected = null;
        ConstructorInfo? publicConstructor = null;
        var publicCount = 0;
        foreach (var constructor in constructors) {
            if (constructor.IsPublic) {
                publicConstructor = constructor;
                publicCount++;
            }
            if (constructor.IsDefined(typeof(InjectAttribute), false)) {
                if (selected is not null) {
                    throw new ArgumentException($"Multiple constructors marked with [Inject] on {type}.");
                }
                selected = constructor;
            }
        }
        if (selected is null) {
            if (publicConstructor is null) {
                throw new ArgumentException($"No public constructor or constructor marked with [Inject] on {type}.");
            }
            if (publicCount > 1) {
                throw new ArgumentException($"Multiple public constructors on {type}; mark one constructor with [Inject].");
            }
            selected = publicConstructor;
        }
        return (ConstructorInvoker.Create(selected), AnalyzeParameters(selected.GetParameters()));
    }

    internal static MemberInjection AnalyzeMembers(Type type) {
        List<(FieldInfo Field, Dependency Dependency)> fields = [];
        List<(MethodInvoker Setter, Dependency Dependency)> properties = [];
        List<(MethodInvoker Method, Dependency[] Dependencies)> methods = [];
        var maxParameterCount = 0;
        var hierarchy = new Stack<Type>();
        for (var current = type; current is not null && current != typeof(object); current = current.BaseType) {
            hierarchy.Push(current);
        }
        while (hierarchy.TryPop(out var level)) {
            foreach (var field in level.GetFields(Members)) {
                if (!field.IsDefined(typeof(InjectAttribute), false)) {
                    continue;
                }
                if (field.IsInitOnly) {
                    throw new ArgumentException($"Injected field must be writable: {field}.");
                }
                var dependency = new Dependency(field.FieldType, field.GetCustomAttribute<InjectKeyAttribute>()?.Key ?? 0);
                fields.Add((field, dependency));
            }
            foreach (var property in level.GetProperties(Members)) {
                if (!property.IsDefined(typeof(InjectAttribute), false)) {
                    continue;
                }
                var setter = property.GetSetMethod(true) ?? throw new ArgumentException($"Injected property requires a setter: {property}.");
                if (property.GetIndexParameters().Length != 0) {
                    throw new ArgumentException($"Property injection does not support indexers: {property}.");
                }
                if (Array.IndexOf(setter.ReturnParameter.GetRequiredCustomModifiers(), typeof(IsExternalInit)) >= 0) {
                    throw new ArgumentException($"Property injection does not support init-only setters: {property}.");
                }
                var dependency = new Dependency(property.PropertyType, property.GetCustomAttribute<InjectKeyAttribute>()?.Key ?? 0);
                properties.Add((MethodInvoker.Create(setter), dependency));
            }
            foreach (var method in level.GetMethods(Members)) {
                if (!method.IsDefined(typeof(InjectAttribute), false)) {
                    continue;
                }
                if (method.IsGenericMethod || method.IsVirtual || method.ReturnType != typeof(void)) {
                    throw new ArgumentException($"Injected method must be non-generic, non-virtual and return void: {method}.");
                }
                var dependencies = AnalyzeParameters(method.GetParameters());
                methods.Add((MethodInvoker.Create(method), dependencies));
                maxParameterCount = Math.Max(maxParameterCount, dependencies.Length);
            }
        }
        return new MemberInjection {
            Fields = fields,
            Properties = properties,
            Methods = methods,
            MaxParameterCount = maxParameterCount
        };
    }

    private static Dependency[] AnalyzeParameters(ParameterInfo[] parameters) {
        var dependencies = new Dependency[parameters.Length];
        for (var i = 0; i < parameters.Length; i++) {
            var parameter = parameters[i];
            if (parameter.ParameterType.IsByRef || parameter.ParameterType.IsPointer || parameter.ParameterType.IsByRefLike) {
                throw new ArgumentException($"Injected parameter must be passed by value and support object storage: {parameter}.");
            }
            dependencies[i] = new Dependency(parameter.ParameterType, parameter.GetCustomAttribute<InjectKeyAttribute>()?.Key ?? 0);
        }
        return dependencies;
    }
}
