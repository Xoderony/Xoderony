using System;

namespace Xoderony.DependencyInjection;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class InjectKeyAttribute : Attribute {
    public int Key { get; }

    public InjectKeyAttribute(int key) {
        Key = key;
    }

    /// <summary>仅接受以 int 为底层类型的枚举；object 参数用于特性参数传递，不表示支持任意对象键。枚举类型不参与键比较。</summary>
    public InjectKeyAttribute(object key) {
        if (key is not Enum enumKey || Enum.GetUnderlyingType(enumKey.GetType()) != typeof(int)) {
            throw new ArgumentException("Service keys require an Int32 or an enum with Int32 as its underlying type.", nameof(key));
        }
        Key = Convert.ToInt32(enumKey);
    }
}
