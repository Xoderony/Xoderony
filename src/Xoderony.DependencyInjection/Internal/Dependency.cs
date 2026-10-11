using System;

namespace Xoderony.DependencyInjection.Internal;

internal readonly record struct Dependency(Type Type, int Key);
