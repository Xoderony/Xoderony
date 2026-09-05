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
public class Vector3ReadBenchmarks {

    private const byte Sentinel = 0xA5;

    private byte[] _source = [];
    private Vector3[] _destination = [];
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

        _destination = new Vector3[Count + 2];
        _expected = new byte[_destination.Length * Vector3Codec.ByteCount];
        Array.Fill(_expected, Sentinel);
        var payload = _expected.AsSpan(Vector3Codec.ByteCount, Count * Vector3Codec.ByteCount);
        var random = new Random(42);
        for (var i = 0; i < payload.Length; i += sizeof(float)) {
            BinaryPrimitives.WriteInt32LittleEndian(payload[i..], (int)random.NextInt64(int.MinValue, (long)int.MaxValue + 1));
        }
        BinaryPrimitives.WriteInt32LittleEndian(payload, int.MinValue);
        BinaryPrimitives.WriteInt32LittleEndian(payload[4..], 0x7F800000);
        BinaryPrimitives.WriteInt32LittleEndian(payload[8..], 0x7FC01234);

        _source = new byte[Offset + payload.Length];
        Array.Fill(_source, Sentinel);
        payload.CopyTo(_source.AsSpan(Offset));

        Verify(Fields);
        Verify(Codec);
        Verify(Unmanaged);
        Verify(FixedOffsets);
        Verify(BulkCopy);
    }

    [Benchmark(Baseline = true)]
    public int Fields() {
        var reader = new SpanReader(_source) { Position = Offset };
        var destination = _destination.AsSpan(1, Count);
        for (var i = 0; i < destination.Length; i++) {
            destination[i] = ReadFields(ref reader);
        }
        return reader.Position;
    }

    [Benchmark]
    public int Codec() {
        var reader = new SpanReader(_source) { Position = Offset };
        var destination = _destination.AsSpan(1, Count);
        for (var i = 0; i < destination.Length; i++) {
            destination[i] = Vector3Codec.Decode(ref reader);
        }
        return reader.Position;
    }

    [Benchmark]
    public int Unmanaged() {
        var reader = new SpanReader(_source) { Position = Offset };
        var destination = _destination.AsSpan(1, Count);
        for (var i = 0; i < destination.Length; i++) {
            destination[i] = reader.ReadUnmanaged<Vector3>();
        }
        return reader.Position;
    }

    [Benchmark]
    public int FixedOffsets() {
        var reader = new SpanReader(_source) { Position = Offset };
        var destination = _destination.AsSpan(1, Count);
        for (var i = 0; i < destination.Length; i++) {
            destination[i] = ReadFixedOffsets(ref reader);
        }
        return reader.Position;
    }

    [Benchmark]
    public int BulkCopy() {
        var reader = new SpanReader(_source) { Position = Offset };
        var destination = _destination.AsSpan(1, Count);
        var bytes = MemoryMarshal.AsBytes(destination);
        reader.Source.Slice(reader.Position, bytes.Length).CopyTo(bytes);
        reader.Position += bytes.Length;
        return reader.Position;
    }

    [GlobalCleanup]
    public void Cleanup() {
        if (!MemoryMarshal.AsBytes(_destination.AsSpan()).SequenceEqual(_expected)) {
            throw new InvalidOperationException("Benchmark output changed during measurement.");
        }
    }

    private static Vector3 ReadFields(ref SpanReader reader) {
        return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    private static Vector3 ReadFixedOffsets(ref SpanReader reader) {
        var source = reader.Source.Slice(reader.Position, Vector3Codec.ByteCount);
        var value = new Vector3(BinaryPrimitives.ReadSingleLittleEndian(source), BinaryPrimitives.ReadSingleLittleEndian(source[4..]), BinaryPrimitives.ReadSingleLittleEndian(source[8..]));
        reader.Position += Vector3Codec.ByteCount;
        return value;
    }

    private void Verify(Func<int> read) {
        MemoryMarshal.AsBytes(_destination.AsSpan()).Fill(Sentinel);
        var position = read();
        if (position != _source.Length || !MemoryMarshal.AsBytes(_destination.AsSpan()).SequenceEqual(_expected)) {
            throw new InvalidOperationException($"{read.Method.Name} produced different values, changed guard bytes, or advanced to the wrong position.");
        }
    }
}
