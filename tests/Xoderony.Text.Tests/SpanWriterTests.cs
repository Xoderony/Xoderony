using System;
using Xunit;

namespace Xoderony.Text.Tests;

public sealed class SpanWriterTests {
    [Fact]
    public void Utf8FormattingFailureDoesNotSucceedOrAdvancePosition() {
        Assert.Throws<ArgumentException>(() => {
            var writer = new Utf8SpanWriter(new byte[1]);
            try {
                writer.Write(12345);
            } finally {
                Assert.Equal(0, writer.Position);
            }
        });
    }

    [Fact]
    public void Utf16FormattingFailureDoesNotSucceedOrAdvancePosition() {
        Assert.Throws<ArgumentException>(() => {
            var writer = new Utf16SpanWriter(new char[1]);
            try {
                writer.Write(12345);
            } finally {
                Assert.Equal(0, writer.Position);
            }
        });
    }

    [Fact]
    public void FormattingSuccessPreservesOutputAndPosition() {
        var utf8 = new Utf8SpanWriter(new byte[8]);
        utf8.Write(12345);
        Assert.True(utf8.WrittenSpan.SequenceEqual("12345"u8));
        var utf16 = new Utf16SpanWriter(new char[8]);
        utf16.Write(12345);
        Assert.Equal("12345", utf16.WrittenSpan.ToString());
    }
}
