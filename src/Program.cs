using BenchmarkDotNet.Running;
using System;
using System.Diagnostics;

namespace ReadStructFromFs
{
    class Program
    {
        static void Main(string[] args)
        {
            #if RELEASE
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            #else
            Test();
            #endif
        }

        private static void Test()
        {
            var drivePath = @"\\.\C:"; // @"\\.\PhysicalDrive1";
            ulong startReadByte = 0;
            int sectorSize = 512;

            var hDrive = NativeMethods.CreateFile(drivePath, NativeMethods.GENERIC_READ, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (hDrive.IsInvalid)
            {
                Console.WriteLine("Invalid drive");
                return;
            }

            if (NativeMethods.SetFilePointerEx(hDrive, startReadByte, out Int64 moveToHigh, NativeMethods.EMoveMethod.Begin) == 0)
            {
                Console.WriteLine("Cannot set file pointer");
                return;
            }

            var buf = new byte[sectorSize];
            //var buf = stackalloc byte[sectorSize];
            if ((NativeMethods.ReadFile(hDrive, buf, sectorSize, out int read, IntPtr.Zero) == 0) || (read != sectorSize))
            {
                Console.WriteLine("Cannot read first sector");
                return;
            }

            var bpb = ReinterpretCastPre73.As1<BpbStruct>(buf);
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            bpb = ReinterpretCastPre73.As2<BpbStruct>(buf);
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            bpb = ReinterpretCastPre73.AsViaUnsafeUnaligned<BpbStruct>(buf);
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            ReinterpretCastPre73.AsViaUnsafeUnaligned(buf, out bpb);
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            bpb = ReinterpretCastPost73.As<BpbStruct>(buf);
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            bpb = ReinterpretCastPost73.As<BpbStruct>(buf.AsSpan());
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            ReinterpretCastPost73.As(buf.AsSpan(), out bpb);
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            Span<byte> localBuff = stackalloc byte[sectorSize];
            buf.CopyTo(localBuff);

            bpb = ReinterpretCastPost73.As<BpbStruct>(localBuff);
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            ReinterpretCastPost73.As(localBuff, out bpb);
            Debug.Assert(bpb.LogicalSectorSize == sectorSize);

            ref var bpbRef = ref bpb; // FYI struct won't be copied that way

            unsafe
            {
                for (int i = 0; i < 8; ++i)
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
}
