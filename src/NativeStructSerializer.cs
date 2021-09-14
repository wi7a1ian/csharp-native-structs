using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace ReadStructFromFs
{
    public class NativeStructSerializer73
    {

        public static unsafe void Deserialize<T>(Span<byte> buffer, out T result) where T : unmanaged
        { 
            fixed (T* resultPtr = &result) 
            { 
                buffer.Slice(0, sizeof(T)).CopyTo(new Span<byte>(resultPtr, sizeof(T))); 
            };
        }

        public static unsafe T Deserialize<T>(Span<byte> buffer) where T : unmanaged
        {
            T result = new T();

            buffer.Slice(0, sizeof(T)).CopyTo(new Span<byte>(&result, sizeof(T)));

            return result;
        }

        public static unsafe T Deserialize<T>(byte[] buffer) where T : unmanaged
        {
            T result = new T();

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(bufferPtr, &result, sizeof(T), sizeof(T));
            }

            return result;
        }

        public static unsafe byte[] Serialize<T>(T value) where T : unmanaged
        {
            byte[] buffer = new byte[sizeof(T)];

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(&value, bufferPtr, sizeof(T), sizeof(T));
            }

            return buffer;
        }

        public static unsafe void Serialize<T>(T value, byte[] to) where T : unmanaged
           => new Span<byte>(&value, sizeof(T)).CopyTo(to);

        public static unsafe void Serialize<T>(T value, Span<byte> to) where T : unmanaged
            => new Span<byte>(&value, sizeof(T)).CopyTo(to);
    }

    public class NativeStructSerializer
    { 
        public static T Deserialize2<T>(byte[] buffer) where T : struct
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

        public static T Deserialize<T>(byte[] buffer) where T : struct
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

        public static byte[] Serialize<T>(T value) where T : struct
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