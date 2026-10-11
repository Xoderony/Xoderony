using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Xunit;

namespace Xoderony.DependencyInjection.Tests;

public sealed class CollectibleInjectionTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SharedInjectionCacheDoesNotRetainCollectibleTypes(bool closedGeneric) {
        using var root = new ContainerBuilder().Build();
        var type = InjectCollectibleType(root, closedGeneric);
        for (var attempt = 0; attempt < 20 && type.IsAlive; attempt++) {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        Assert.False(type.IsAlive);
        GC.KeepAlive(root);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference InjectCollectibleType(Container root, bool closedGeneric) {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Injection_" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.RunAndCollect);
        var builder = assembly.DefineDynamicModule("Fixture").DefineType("Target", TypeAttributes.Public);
        var attribute = new CustomAttributeBuilder(typeof(InjectAttribute).GetConstructor(Type.EmptyTypes)!, []);
        var field = builder.DefineField("Resolver", typeof(IResolver), FieldAttributes.Public);
        field.SetCustomAttribute(attribute);
        var methodField = builder.DefineField("MethodResolver", typeof(IResolver), FieldAttributes.Public);
        var method = builder.DefineMethod("Initialize", MethodAttributes.Public, typeof(void), [typeof(IResolver)]);
        method.SetCustomAttribute(attribute);
        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, methodField);
        il.Emit(OpCodes.Ret);
        var type = builder.CreateType()!;
        if (closedGeneric) {
            type = typeof(GenericTarget<>).MakeGenericType(type);
        }
        Assert.True(type.IsCollectible);
        var target = Activator.CreateInstance(type)!;
        root.Inject(target);
        Assert.Same(root, type.GetField("Resolver")!.GetValue(target));
        Assert.Same(root, type.GetField("MethodResolver")!.GetValue(target));
        using (var child = root.CreateChild()) {
            child.Inject(target);
            Assert.Same(child, type.GetField("Resolver")!.GetValue(target));
            Assert.Same(child, type.GetField("MethodResolver")!.GetValue(target));
        }
        return new WeakReference(type);
    }

    public sealed class GenericTarget<T> {
        [Inject]
        public IResolver? Resolver = null;
        public IResolver? MethodResolver;

        [Inject]
        public void Initialize(IResolver resolver) {
            MethodResolver = resolver;
        }
    }
}
