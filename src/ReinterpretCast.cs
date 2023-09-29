using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace ReadStructFromFs
{
    public static class ReinterpretCastPost73
    {
        public static ref T AsRef<T>(Span<byte> buffer) where T : struct
            => ref MemoryMarshal.AsRef<T>(buffer);

        public static unsafe void As<T>(Span<byte> buffer, out T result) where T : unmanaged
        { 
            fixed (T* resultPtr = &result) 
            { 
                buffer.Slice(0, sizeof(T)).CopyTo(new Span<byte>(resultPtr, sizeof(T))); 
            };
        }

        public static unsafe T As<T>(Span<byte> buffer) where T : unmanaged
        {
            T result = new T();

            buffer.Slice(0, sizeof(T)).CopyTo(new Span<byte>(&result, sizeof(T)));

            return result;
        }

        public static unsafe T As<T>(byte[] buffer) where T : unmanaged
        {
            T result = new T();

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(bufferPtr, &result, sizeof(T), sizeof(T));
            }

            return result;
        }

        public static unsafe byte[] AsBytes<T>(T value) where T : unmanaged
        {
            byte[] buffer = new byte[sizeof(T)];

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(&value, bufferPtr, sizeof(T), sizeof(T));
            }

            return buffer;
        }

        public static unsafe void AsBytes<T>(T value, byte[] to) where T : unmanaged
           => new Span<byte>(&value, sizeof(T)).CopyTo(to);

        public static unsafe void AsBytes<T>(T value, Span<byte> to) where T : unmanaged
            => new Span<byte>(&value, sizeof(T)).CopyTo(to);
    }

    public static class ReinterpretCastPre73
    { 
        public static T As1<T>(byte[] buffer) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var ptr = Marshal.AllocHGlobal(size);
            try 
            {
                Marshal.Copy(buffer, 0, ptr, size);
                return (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            finally 
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static T As2<T>(byte[] buffer) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally { 
                handle.Free();
            }
        }

        public static unsafe void AsViaUnsafeUnaligned<T>(byte[] buffer, out T result)
        {
            fixed (byte* bufferPtr = buffer)
            {
                result = Unsafe.ReadUnaligned<T>(bufferPtr);
            }
        }

        public static unsafe T AsViaUnsafeUnaligned<T>(byte[] buffer)
        {
            fixed (byte* bufferPtr = buffer)
            {
                return Unsafe.ReadUnaligned<T>(bufferPtr);
            }
        }

        public static byte[] AsBytes<T>(T value) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var array = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            try
            { 
                Marshal.StructureToPtr(value, ptr, true);
                Marshal.Copy(ptr, array, 0, size);
            }
            finally
            { 
                Marshal.FreeHGlobal(ptr);
            }
            return array;
        }
    }
}