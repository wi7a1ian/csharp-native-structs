using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ReadStructFromFs
{
    class Program
    {
        static void Main(string[] args)
        {
             var drivePath = @"\\.\C:"; // @"\\.\PhysicalDrive1";
            ulong startReadByte = 0;
            int sectorSize = 512;

            var hDrive = NativeMethods.CreateFile(drivePath, NativeMethods.GENERIC_READ, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            
            if (hDrive.IsInvalid)
            {
                return;
            }

            if (NativeMethods.SetFilePointerEx(hDrive, startReadByte, out Int64 moveToHigh, NativeMethods.EMoveMethod.Begin) == 0)
            {
                return;
            }

            var buf = new byte[sectorSize];
            //var buf = stackalloc byte[numBytesToRead];
            if ((NativeMethods.ReadFile(hDrive, buf, sectorSize, out int read, IntPtr.Zero) == 0) || (read != sectorSize))
            {
                return;
            }

            var bpb = NativeStructSerializer.Deserialize<BpbStruct>(buf);
            bpb = NativeStructSerializer.Deserialize2<BpbStruct>(buf);
            bpb = NativeStructSerializer.DeserializeSinceCv7_3<BpbStruct>(buf);
            
            ref var bpbRef = ref bpb; // struct won't be copied that way

            unsafe
            {
                for(int i = 0; i < 8; ++i)
                {
                    Console.Write((char)bpbRef.OemLabel[i]);
                }

                Console.WriteLine();
                Console.WriteLine("Sector size: {0}", bpbRef.LogicalSectorSize);
                Console.WriteLine("End signature: 0x{0:x} (0xAA55)", bpbRef.EndSignature);
                //Console.WriteLine(Encoding.Default.GetString(bpbRef.m_oemLabel));}
            }

            NativeMethods.CloseHandle(hDrive);
            Console.ReadLine();
        }
    }

    internal static class NativeMethods
    {
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        internal const uint FILE_SHARE_READ = 0x01;
        internal const uint FILE_SHARE_WRITE = 0x02;
        internal const short INVALID_HANDLE_VALUE = -1;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const uint CREATE_NEW = 1;
        internal const uint CREATE_ALWAYS = 2;
        internal const uint OPEN_EXISTING = 3;

        const string KERNEL32 = "kernel32.dll";

        public enum EMoveMethod : uint
        {
            Begin = 0,
            Current = 1,
            End = 2
        }

        [DllImport(KERNEL32, SetLastError = true, CharSet = CharSet.Auto)]
        internal extern static uint SetFilePointerEx(
            [In] SafeFileHandle hFile,
            [In] ulong lDistanceToMove,
            [Out] out Int64 lpDistanceToMoveHigh,
            [In] EMoveMethod dwMoveMethod);

        [DllImport(KERNEL32, SetLastError = true, CharSet = CharSet.Unicode)]
        internal extern static SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
          uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
          uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport(KERNEL32, ExactSpelling = true, SetLastError = true)]
        internal extern static bool CloseHandle(SafeFileHandle handle);

        [DllImport(KERNEL32, SetLastError = true)]
        internal extern static int ReadFile(SafeFileHandle handle, byte[] bytes,
           int numBytesToRead, out int numBytesRead, IntPtr overlapped_MustBeZero);
    }
}
