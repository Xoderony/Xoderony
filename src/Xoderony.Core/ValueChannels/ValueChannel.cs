namespace Xoderony;

public sealed class ValueChannel<T> : IValueReader<T>, IValueWriter<T> {

    private T _value = default!;
    
    ref readonly T IValueReader<T>.Value => ref _value;

    public ref T Value => ref _value;

}
