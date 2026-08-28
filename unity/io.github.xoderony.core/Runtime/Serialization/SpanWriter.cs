using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xoderony.Serialization;

public ref struct SpanWriter(Span<byte> destination) {

    public Span<byte> Destination = destination;

    public int Position;

    public readonly int Remaining => Destination.Length - Position;

    public readonly ReadOnlySpan<byte> WrittenSpan => Destination[..Position];

    /// <summary>判断从当前位置起是否可再写入指定字节数。</summary>
    /// <param name="byteCount">要写入的字节数；必须为非负数。</param>
    /// <returns>当前位置和请求长度均有效时返回 true；否则返回 false。</returns>

    public readonly bool CanWrite(int byteCount) {
        return ((uint)Position <= (uint)Destination.Length) && ((uint)byteCount <= (uint)(Destination.Length - Position));
    }

    /// <summary>按本机内存布局写入非托管值。</summary>
    /// <remarks>直接写入 <typeparamref name="T"/> 的内存表示，包括可能存在的填充字节；调用方负责保证读取方使用相同的类型布局。</remarks>
    /// <typeparam name="T">要写入的非托管值类型。</typeparam>
    /// <param name="value">要写入的值。</param>

    public void WriteUnmanaged<T>(T value) where T : unmanaged {
        MemoryMarshal.Write(Destination[Position..], value);
        Position += Unsafe.SizeOf<T>();
    }

    public void WriteByte(byte value) {
        Destination[Position++] = value;
    }

    public void WriteSByte(sbyte value) {
        Destination[Position++] = (byte)value;
    }

    /// <summary>将布尔值按单字节 0 或 1 写入。</summary>
    /// <param name="value">要写入的布尔值。</param>

    public void WriteBoolean(bool value) {
        Destination[Position++] = (byte)(value ? 1 : 0);
    }

    public void WriteUInt16(ushort value) {
        BinaryPrimitives.WriteUInt16LittleEndian(Destination[Position..], value);
        Position += 2;
    }

    public void WriteInt16(short value) {
        BinaryPrimitives.WriteInt16LittleEndian(Destination[Position..], value);
        Position += 2;
    }

    public void WriteChar(char value) {
        BinaryPrimitives.WriteUInt16LittleEndian(Destination[Position..], value);
        Position += 2;
    }

    public void WriteInt32(int value) {
        BinaryPrimitives.WriteInt32LittleEndian(Destination[Position..], value);
        Position += 4;
    }

    public void WriteUInt32(uint value) {
        BinaryPrimitives.WriteUInt32LittleEndian(Destination[Position..], value);
        Position += 4;
    }

    public void WriteUInt64(ulong value) {
        BinaryPrimitives.WriteUInt64LittleEndian(Destination[Position..], value);
        Position += 8;
    }

    public void WriteInt64(long value) {
        BinaryPrimitives.WriteInt64LittleEndian(Destination[Position..], value);
        Position += 8;
    }

    public void WriteSingle(float value) {
        BinaryPrimitives.WriteSingleLittleEndian(Destination[Position..], value);
        Position += 4;
    }

    public void WriteDouble(double value) {
        BinaryPrimitives.WriteDoubleLittleEndian(Destination[Position..], value);
        Position += 8;
    }

    public void WriteBytes(ReadOnlySpan<byte> bytes) {
        Debug.Assert(bytes.Length <= ushort.MaxValue && CanWrite(sizeof(ushort) + bytes.Length));
        bytes.CopyTo(Destination[(Position + sizeof(ushort))..]);
        BinaryPrimitives.WriteUInt16LittleEndian(Destination[Position..], (ushort)bytes.Length);
        Position += sizeof(ushort) + bytes.Length;
    }

    public void WriteChars(ReadOnlySpan<char> chars) {
        var payloadByteCount = chars.Length * sizeof(char);
        Debug.Assert(chars.Length <= ushort.MaxValue && CanWrite(sizeof(ushort) + payloadByteCount));
        MemoryMarshal.Cast<char, byte>(chars).CopyTo(Destination[(Position + sizeof(ushort))..]);
        BinaryPrimitives.WriteUInt16LittleEndian(Destination[Position..], (ushort)chars.Length);
        Position += sizeof(ushort) + payloadByteCount;
    }
}
