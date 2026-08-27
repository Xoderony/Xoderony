namespace Xoderony;

public interface IValueWriter<T> {

    ref T Value { get; }
}
