using System;
using System.Runtime.InteropServices;

namespace Xoderony.Extensions;

public static class SpanExtensions {

    extension<T>(Span<T> span) where T : struct {

        public Span<byte> AsBytes() {
            return MemoryMarshal.AsBytes(span);
        }

        public Span<TTo> Cast<TTo>() where TTo : struct {
            return MemoryMarshal.Cast<T, TTo>(span);
        }

    }

    extension<T>(Span<T> span) {

        public ref T GetReference() {
            return ref MemoryMarshal.GetReference(span);
        }

    }

    extension<T>(ReadOnlySpan<T> span) where T : struct {

        public ReadOnlySpan<byte> AsBytes() {
            return MemoryMarshal.AsBytes(span);
        }

        public ReadOnlySpan<TTo> Cast<TTo>() where TTo : struct {
            return MemoryMarshal.Cast<T, TTo>(span);
        }

    }

    extension<T>(ReadOnlySpan<T> span) {

        public ref T GetReference() {
            return ref MemoryMarshal.GetReference(span);
        }

    }

    extension(Span<byte> span) {

        public void Write<T>(in T value) where T : struct {
            MemoryMarshal.Write(span, in value);
        }

        public bool TryWrite<T>(in T value) where T : struct {
            return MemoryMarshal.TryWrite(span, in value);
        }

        public ref T AsRef<T>() where T : struct {
            return ref MemoryMarshal.AsRef<T>(span);
        }

    }

    extension(ReadOnlySpan<byte> span) {

        public T Read<T>() where T : struct {
            return MemoryMarshal.Read<T>(span);
        }

        public bool TryRead<T>(out T value) where T : struct {
            return MemoryMarshal.TryRead(span, out value);
        }

        public ref readonly T AsRef<T>() where T : struct {
            return ref MemoryMarshal.AsRef<T>(span);
        }

    }
}
