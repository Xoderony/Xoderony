namespace Xoderony.Serialization;

// T 默认按原始内存布局反序列化；需要固定字段协议时覆盖委托。
public static class ValueDeserializer<T> where T : unmanaged {

    public delegate T DeserializeDelegate(ref BufferReader reader);

    public static DeserializeDelegate Deserialize = static (ref reader) => reader.ReadUnmanaged<T>();
}
