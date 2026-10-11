using System;

namespace Xoderony.DependencyInjection;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class InjectAttribute : Attribute;
