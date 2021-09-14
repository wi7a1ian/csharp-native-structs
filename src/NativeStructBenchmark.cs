﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Win32.SafeHandles;
using System;

namespace ReadStructFromFs
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1)]
    public class NativeStructBenchmark
    {
        private SafeFileHandle hDrive;
        private byte[] buf;
        private BpbStruct bpb;
        private const int sectorSize = 512;

        [Params(1 * 1024 * 1024)]
        public int IterationCnt { get; set; }

        [Params(@"\\.\C:")]
        public string DrivePath { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            hDrive = NativeMethods.CreateFile(DrivePath, NativeMethods.GENERIC_READ, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (hDrive.IsInvalid)
            {
                throw new Exception($"Failed opening {DrivePath}");
            }

            if (NativeMethods.SetFilePointerEx(hDrive, 0, out var moveToHigh, NativeMethods.EMoveMethod.Begin) == 0)
            {
                throw new Exception($"Failed seeking 0 offset in {DrivePath}");
            }

            buf = new byte[sectorSize];
            if ((NativeMethods.ReadFile(hDrive, buf, sectorSize, out int read, IntPtr.Zero) == 0) || (read != sectorSize))
            {
                throw new Exception($"Failed reading {DrivePath}");
            }
        }

        [IterationCleanup]
        public void IterationCheck()
        {
            if(bpb.LogicalSectorSize != sectorSize)
            {
                throw new Exception("Failed reading BPB info");
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            NativeMethods.CloseHandle(hDrive);
        }

        [Benchmark]
        public void DeserializeUsingOld1()
        {
            for (var i = 0; i < IterationCnt; ++i)
            {
                bpb = NativeStructSerializer.Deserialize<BpbStruct>(buf);
            }
        }

        [Benchmark]
        public void DeserializeUsingOld2()
        {
            for (var i = 0; i < IterationCnt; ++i)
            {
                bpb = NativeStructSerializer.Deserialize2<BpbStruct>(buf);
            }
        }

        [Benchmark]
        public void DeserializeUsing73()
        {
            for (var i = 0; i < IterationCnt; ++i)
            {
                bpb = NativeStructSerializer73.Deserialize<BpbStruct>(buf);
            }
        }

        [Benchmark(Baseline = true)]
        public void DeserializeUsing73AndSpan()
        {
            for(var i = 0; i < IterationCnt; ++i)
            {
                bpb = NativeStructSerializer73.Deserialize<BpbStruct>(buf.AsSpan());
            }
        }

        [Benchmark]
        public void DeserializeUsing73AndSpanOutParam()
        {
            for (var i = 0; i < IterationCnt; ++i)
            {
                NativeStructSerializer73.Deserialize(buf.AsSpan(), out bpb);
            }
        }
    }
}