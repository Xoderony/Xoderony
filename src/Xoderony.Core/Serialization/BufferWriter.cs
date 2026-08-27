using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xoderony.Serialization;

public ref struct BufferWriter {

    public Span<byte> Buffer;

    public int Position;

    public readonly int Remaining => Buffer.Length - Position;

    // 已写入的数据。
    public readonly ReadOnlySpan<byte> Written => Buffer[..Position];

    public BufferWriter(Span<byte> buffer) {
        Buffer = buffer;
        Position = 0;
    }

    // 容量检查：能否再写 count 字节；写入前调用一次，之后可连续 Write* 而不逐次比较。
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool CanWrite(int count) {
        return Position + count <= Buffer.Length;
    }

    // 按本机布局写入；跨端多字节字段请用 Serializer<T> 覆盖。
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUnmanaged<T>(in T value) where T : unmanaged {
        MemoryMarshal.Write(Buffer[Position..], value);
        Position += Unsafe.SizeOf<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte value) {
        Buffer[Position++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSByte(sbyte value) {
        Buffer[Position++] = (byte)value;
    }

    // bool 按 1 字节 0/1 写入。
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBool(bool value) {
        Buffer[Position++] = (byte)(value ? 1 : 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUShort(ushort value) {
        BinaryPrimitives.WriteUInt16LittleEndian(Buffer[Position..], value);
        Position += 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteShort(short value) {
        BinaryPrimitives.WriteInt16LittleEndian(Buffer[Position..], value);
        Position += 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteChar(char value) {
        BinaryPrimitives.WriteUInt16LittleEndian(Buffer[Position..], value);
        Position += 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt(int value) {
        BinaryPrimitives.WriteInt32LittleEndian(Buffer[Position..], value);
        Position += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt(uint value) {
        BinaryPrimitives.WriteUInt32LittleEndian(Buffer[Position..], value);
        Position += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteULong(ulong value) {
        BinaryPrimitives.WriteUInt64LittleEndian(Buffer[Position..], value);
        Position += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLong(long value) {
        BinaryPrimitives.WriteInt64LittleEndian(Buffer[Position..], value);
        Position += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFloat(float value) {
        BinaryPrimitives.WriteSingleLittleEndian(Buffer[Position..], value);
        Position += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDouble(double value) {
        BinaryPrimitives.WriteDoubleLittleEndian(Buffer[Position..], value);
        Position += 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(ReadOnlySpan<byte> data) {
        data.CopyTo(Buffer[Position..]);
        Position += data.Length;
    }
}
