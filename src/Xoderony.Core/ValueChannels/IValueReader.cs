namespace Xoderony;

public interface IValueReader<T> {

    ref readonly T Value { get; }
    
}
