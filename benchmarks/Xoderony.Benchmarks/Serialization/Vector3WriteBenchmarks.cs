using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using UnityEngine;
using Xoderony.Serialization;
using Xoderony.Serialization.Unity;
using Random = System.Random;

namespace Xoderony.Benchmarks.Serialization;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 3, exportCombinedDisassemblyReport: true)]
public class Vector3WriteBenchmarks {

    private const byte Sentinel = 0xA5;

    private Vector3[] _values = [];
    private byte[] _destination = [];
    private byte[] _expected = [];

    [Params(1, 1024, 65536)]
    public int Count { get; set; }

    [Params(0, 1)]
    public int Offset { get; set; }

    [GlobalSetup]
    public void Setup() {
        if (!BitConverter.IsLittleEndian || Unsafe.SizeOf<Vector3>() != Vector3Codec.ByteCount
            || Marshal.OffsetOf<Vector3>(nameof(Vector3.x)) != 0
            || Marshal.OffsetOf<Vector3>(nameof(Vector3.y)) != 4
            || Marshal.OffsetOf<Vector3>(nameof(Vector3.z)) != 8) {
            throw new PlatformNotSupportedException("This comparison requires little-endian Vector3 values with a 12-byte xyz layout.");
        }

        _values = new Vector3[Count];
        var random = new Random(42);
        for (var i = 0; i < _values.Length; i++) {
            _values[i] = new Vector3(NextSingle(random), NextSingle(random), NextSingle(random));
        }
        _values[0] = new Vector3(BitConverter.Int32BitsToSingle(int.MinValue), float.PositiveInfinity, BitConverter.Int32BitsToSingle(0x7FC01234));

        _destination = new byte[Offset + (Count * Vector3Codec.ByteCount) + 16];
        _expected = new byte[_destination.Length];
        Array.Fill(_expected, Sentinel);
        for (var i = 0; i < _values.Length; i++) {
            var destination = _expected.AsSpan(Offset + (i * Vector3Codec.ByteCount), Vector3Codec.ByteCount);
            var value = _values[i];
            BinaryPrimitives.WriteInt32LittleEndian(destination, BitConverter.SingleToInt32Bits(value.x));
            BinaryPrimitives.WriteInt32LittleEndian(destination[4..], BitConverter.SingleToInt32Bits(value.y));
            BinaryPrimitives.WriteInt32LittleEndian(destination[8..], BitConverter.SingleToInt32Bits(value.z));
        }

        Verify(Fields);
        Verify(Unmanaged);
        Verify(Codec);
        Verify(BulkCopy);
    }

    [Benchmark(Baseline = true)]
    public int Fields() {
        var writer = new SpanWriter(_destination) { Position = Offset };
        var values = _values;
        for (var i = 0; i < values.Length; i++) {
            WriteFields(ref writer, values[i]);
        }
        return writer.Position;
    }

    [Benchmark]
    public int Unmanaged() {
        var writer = new SpanWriter(_destination) { Position = Offset };
        var values = _values;
        for (var i = 0; i < values.Length; i++) {
            writer.WriteUnmanaged(values[i]);
        }
        return writer.Position;
    }

    [Benchmark]
    public int Codec() {
        var writer = new SpanWriter(_destination) { Position = Offset };
        var values = _values;
        for (var i = 0; i < values.Length; i++) {
            Vector3Codec.Encode(ref writer, values[i]);
        }
        return writer.Position;
    }

    [Benchmark]
    public int BulkCopy() {
        var writer = new SpanWriter(_destination) { Position = Offset };
        var bytes = MemoryMarshal.AsBytes(_values.AsSpan());
        // WriteBytes 会附加长度前缀；这里保持与其他写法相同的纯载荷格式。
        bytes.CopyTo(writer.Destination[writer.Position..]);
        writer.Position += bytes.Length;
        return writer.Position;
    }

    [GlobalCleanup]
    public void Cleanup() {
        if (!_destination.AsSpan().SequenceEqual(_expected)) {
            throw new InvalidOperationException("Benchmark output changed during measurement.");
        }
    }

    private static void WriteFields(ref SpanWriter writer, Vector3 value) {
        writer.WriteSingle(value.x);
        writer.WriteSingle(value.y);
        writer.WriteSingle(value.z);
    }

    private static float NextSingle(Random random) {
        return BitConverter.Int32BitsToSingle((int)random.NextInt64(int.MinValue, (long)int.MaxValue + 1));
    }

    private void Verify(Func<int> write) {
        Array.Fill(_destination, Sentinel);
        var position = write();
        if (position != Offset + (Count * Vector3Codec.ByteCount) || !_destination.AsSpan().SequenceEqual(_expected)) {
            throw new InvalidOperationException($"{write.Method.Name} produced different bytes, changed guard bytes, or advanced to the wrong position.");
        }
    }
}
