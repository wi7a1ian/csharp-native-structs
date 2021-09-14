using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace ReadStructFromFs
{
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
