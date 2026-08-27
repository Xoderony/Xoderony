namespace Xoderony.Serialization;

// T 默认按原始内存布局序列化；需要固定字段协议时覆盖委托。
public static class ValueSerializer<T> where T : unmanaged {

    public delegate void SerializeDelegate(ref BufferWriter writer, in T value);

    public static SerializeDelegate Serialize = static (ref writer, in value) => writer.WriteUnmanaged(value);
}
