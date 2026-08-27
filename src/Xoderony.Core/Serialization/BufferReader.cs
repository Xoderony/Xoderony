using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xoderony.Serialization;

public ref struct BufferReader {

    public ReadOnlySpan<byte> Buffer;

    public int Position;

    public readonly int Remaining => Buffer.Length - Position;

    // 尚未读取的数据。
    public readonly ReadOnlySpan<byte> Unread => Buffer[Position..];

    public BufferReader(ReadOnlySpan<byte> buffer) {
        Buffer = buffer;
        Position = 0;
    }

    // 剩余检查：还能再读 count 字节；读取前调用一次，之后可连续 Read* 而不逐次比较。
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool CanRead(int count) {
        return Position + count <= Buffer.Length;
    }

    // 按本机布局读取；跨端多字节字段请用 Deserializer<T> 覆盖。
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadUnmanaged<T>() where T : unmanaged {
        var value = MemoryMarshal.Read<T>(Buffer[Position..]);
        Position += Unsafe.SizeOf<T>();
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte() {
        return Buffer[Position++];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte() {
        return (sbyte)Buffer[Position++];
    }

    // 非 0 视为 true。
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool() {
        return Buffer[Position++] != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUShort() {
        var value = BinaryPrimitives.ReadUInt16LittleEndian(Buffer[Position..]);
        Position += 2;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadShort() {
        var value = BinaryPrimitives.ReadInt16LittleEndian(Buffer[Position..]);
        Position += 2;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char ReadChar() {
        var value = BinaryPrimitives.ReadUInt16LittleEndian(Buffer[Position..]);
        Position += 2;
        return (char)value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt() {
        var value = BinaryPrimitives.ReadInt32LittleEndian(Buffer[Position..]);
        Position += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt() {
        var value = BinaryPrimitives.ReadUInt32LittleEndian(Buffer[Position..]);
        Position += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadULong() {
        var value = BinaryPrimitives.ReadUInt64LittleEndian(Buffer[Position..]);
        Position += 8;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadLong() {
        var value = BinaryPrimitives.ReadInt64LittleEndian(Buffer[Position..]);
        Position += 8;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat() {
        var value = BinaryPrimitives.ReadSingleLittleEndian(Buffer[Position..]);
        Position += 4;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble() {
        var value = BinaryPrimitives.ReadDoubleLittleEndian(Buffer[Position..]);
        Position += 8;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBytes(Span<byte> destination) {
        Buffer.Slice(Position, destination.Length).CopyTo(destination);
        Position += destination.Length;
    }
}
