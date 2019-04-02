using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace ReadStructFromFs
{
    public class NativeStructSerializer
    {
        
        public static unsafe T DeserializeSinceCv7_3<T>(byte[] buffer) where T : unmanaged
        {
            T result = new T();

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(bufferPtr, &result, sizeof(T), sizeof(T));
            }

            return result;
        }

        public static unsafe byte[] SerializeSinceCv7_3<T>(T value) where T : unmanaged
        {
            byte[] buffer = new byte[sizeof(T)];

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(&value, bufferPtr, sizeof(T), sizeof(T));
            }

            return buffer;
        }

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