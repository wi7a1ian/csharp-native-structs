# Info
Since C# 7.3 it is now easier to map (copy) memory directly to a struct with no heap allocation.

```csharp
public static unsafe T Deserialize<T>(Span<byte> buffer) where T : unmanaged
{
    T result = new T();

    buffer.Slice(0, sizeof(T)).CopyTo(new Span<byte>(&result, sizeof(T)));

    return result;
}

var bpb = NativeStructSerializer.Deserialize<BpbStruct>(buf);

unsafe
{
    Console.WriteLine("Sector size: {0}", bpbRef.LogicalSectorSize);
}
```

# Test
1. Ensure you have NTFS volume mounted as `C:`
1. `cd src`
1. `dotnet run` (*note: you may need elevated privileges to access `C:`*)
1. Expected output:
    ```
    NTFS
    Sector size: 512
    End signature: 0xaa55 (0xAA55)
    ```

# Benchmark
```
|                            Method | IterationCnt | DrivePath |      Mean |    Median | Ratio|      Gen 0 |     Allocated |
|---------------------------------- |------------- |---------- |----------:|----------:|------|-----------:|--------------:|
|              DeserializeUsingOld1 |      1048576 |    \\.\C: | 233.53 ms | 234.33 ms | 10.36| 88000.0000 | 553,650,232 B |
|              DeserializeUsingOld2 |      1048576 |    \\.\C: | 160.80 ms | 162.56 ms |  7.52| 88000.0000 | 553,648,128 B |
|                DeserializeUsing73 |      1048576 |    \\.\C: |  23.09 ms |  21.20 ms |  1.05|          - |             - |
|         DeserializeUsing73AndSpan |      1048576 |    \\.\C: |  22.62 ms |  20.84 ms |  1.00|          - |             - |
| DeserializeUsing73AndSpanOutParam |      1048576 |    \\.\C: |  12.58 ms |  11.50 ms |  0.57|          - |             - |
```