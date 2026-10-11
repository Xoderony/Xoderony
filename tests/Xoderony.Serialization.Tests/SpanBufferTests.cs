using System;
using Xunit;

namespace Xoderony.Serialization.Tests;

public sealed class SpanBufferTests {
    [Fact]
    public void OversizedPayloadsAreRejectedBeforeWritingTruncatedLengths() {
        var destination = new byte[140000];
        Array.Fill(destination, (byte)123);
        Assert.Throws<ArgumentOutOfRangeException>(() => {
            var writer = new SpanWriter(destination);
            writer.WriteBytes(new byte[65536]);
        });
        Assert.Throws<ArgumentOutOfRangeException>(() => {
            var writer = new SpanWriter(destination);
            writer.WriteChars(new char[65536]);
        });
        Assert.All(destination, value => Assert.Equal((byte)123, value));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void InvalidDestinationDoesNotWritePartialPayload(int position) {
        var destination = new byte[] { 123, 123, 123, 123 };
        Assert.Throws<ArgumentOutOfRangeException>(() => {
            var writer = new SpanWriter(destination) { Position = position };
            try {
                writer.WriteBytes(new byte[] { 1, 2 });
            } finally {
                Assert.Equal(position, writer.Position);
            }
        });
        Assert.Equal(new byte[] { 123, 123, 123, 123 }, destination);
    }

    [Fact]
    public void TruncatedPayloadFailsWithoutAdvancingReader() {
        Assert.Throws<ArgumentOutOfRangeException>(() => {
            var reader = new SpanReader(new byte[] { 2, 0, 1 });
            try {
                reader.ReadBytes();
            } finally {
                Assert.Equal(0, reader.Position);
            }
        });
    }

    [Fact]
    public void MaximumLengthsRoundTrip() {
        var bytes = new byte[ushort.MaxValue];
        bytes[^1] = 123;
        var chars = new string('x', ushort.MaxValue);
        var writer = new SpanWriter(new byte[200000]);
        writer.WriteBytes(bytes);
        writer.WriteChars(chars);
        var reader = new SpanReader(writer.WrittenSpan);
        Assert.True(reader.ReadBytes().SequenceEqual(bytes));
        Assert.Equal(chars, reader.ReadChars().ToString());
        Assert.Equal(writer.Position, reader.Position);
    }
}
