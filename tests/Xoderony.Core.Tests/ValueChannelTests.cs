using Xunit;

namespace Xoderony.Tests;

public class ValueChannelTests {

    [Fact]
    public void ReaderAndWriter_ShareSameStorage() {
        var channel = new ValueChannel<int>();
        IValueWriter<int> writer = channel;
        IValueReader<int> reader = channel;

        ref var write = ref writer.Value;
        write = 7;

        Assert.Equal(7, reader.Value);
        Assert.Equal(7, channel.Value);
    }

    [Fact]
    public void Map_GetOrAdd_SharesInstanceByKey() {
        var map = new ValueChannelMap<float>();
        var first = map.GetOrAdd(1);
        var second = map.GetOrAdd(1);
        var other = map.GetOrAdd(2);

        Assert.Same(first, second);
        Assert.NotSame(first, other);

        ref var value = ref first.Value;
        value = 1.5f;
        Assert.Equal(1.5f, second.Value);
    }
}
