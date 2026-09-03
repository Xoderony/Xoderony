using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xoderony.Serialization;

public ref struct SpanReader(ReadOnlySpan<byte> source) {

    public ReadOnlySpan<byte> Source = source;

    public int Position;

    public readonly int Remaining => Source.Length - Position;

    public readonly ReadOnlySpan<byte> UnreadSpan => Source[Position..];

    /// <summary>判断从当前位置起是否可再读取指定字节数。</summary>
    /// <param name="byteCount">要读取的字节数；必须为非负数。</param>
    /// <returns>当前位置和请求长度均有效时返回 true；否则返回 false。</returns>

    public readonly bool CanRead(int byteCount) {
        return ((uint)Position <= (uint)Source.Length) && ((uint)byteCount <= (uint)(Source.Length - Position));
    }

    /// <summary>按本机内存布局读取非托管值。</summary>
    /// <remarks>直接按 <typeparamref name="T"/> 的内存表示解释数据；调用方负责保证写入方与当前类型布局一致。</remarks>
    /// <typeparam name="T">要读取的非托管值类型。</typeparam>
    /// <returns>读取到的值。</returns>

    public T ReadUnmanaged<T>() where T : unmanaged {
        var value = MemoryMarshal.Read<T>(Source[Position..]);
        Position += Unsafe.SizeOf<T>();
        return value;
    }

    public byte ReadByte() {
        return Source[Position++];
    }

    public sbyte ReadSByte() {
        return (sbyte)Source[Position++];
    }

    /// <summary>读取一个字节，并将非零值解释为 true。</summary>
    /// <returns>读取到的布尔值。</returns>

    public bool ReadBoolean() {
        return Source[Position++] != 0;
    }

    public ushort ReadUInt16() {
        var value = BinaryPrimitives.ReadUInt16LittleEndian(Source[Position..]);
        Position += 2;
        return value;
    }

    public short ReadInt16() {
        var value = BinaryPrimitives.ReadInt16LittleEndian(Source[Position..]);
        Position += 2;
        return value;
    }

    public char ReadChar() {
        var value = BinaryPrimitives.ReadUInt16LittleEndian(Source[Position..]);
        Position += 2;
        return (char)value;
    }

    public int ReadInt32() {
        var value = BinaryPrimitives.ReadInt32LittleEndian(Source[Position..]);
        Position += 4;
        return value;
    }

    public uint ReadUInt32() {
        var value = BinaryPrimitives.ReadUInt32LittleEndian(Source[Position..]);
        Position += 4;
        return value;
    }

    public ulong ReadUInt64() {
        var value = BinaryPrimitives.ReadUInt64LittleEndian(Source[Position..]);
        Position += 8;
        return value;
    }

    public long ReadInt64() {
        var value = BinaryPrimitives.ReadInt64LittleEndian(Source[Position..]);
        Position += 8;
        return value;
    }

    public float ReadSingle() {
        var value = BinaryPrimitives.ReadSingleLittleEndian(Source[Position..]);
        Position += 4;
        return value;
    }

    public double ReadDouble() {
        var value = BinaryPrimitives.ReadDoubleLittleEndian(Source[Position..]);
        Position += 8;
        return value;
    }

    public ReadOnlySpan<byte> ReadBytes() {
        var payloadByteCount = BinaryPrimitives.ReadUInt16LittleEndian(Source[Position..]);
        Debug.Assert(CanRead(sizeof(ushort) + payloadByteCount));
        var bytes = Source.Slice(Position + sizeof(ushort), payloadByteCount);
        Position += sizeof(ushort) + payloadByteCount;
        return bytes;
    }

    public ReadOnlySpan<char> ReadChars() {
        var charCount = BinaryPrimitives.ReadUInt16LittleEndian(Source[Position..]);
        var payloadByteCount = charCount * sizeof(char);
        Debug.Assert(CanRead(sizeof(ushort) + payloadByteCount));
        var chars = MemoryMarshal.Cast<byte, char>(Source.Slice(Position + sizeof(ushort), payloadByteCount));
        Position += sizeof(ushort) + payloadByteCount;
        return chars;
    }
}
