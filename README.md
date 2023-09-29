# Info
Since C# 7.3 it is now easier to map (reinterpret, copy) memory buffer to a struct using [`unmanaged` constraint](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters#unmanaged-constraint), `Span<T>` and `MemoryMarshal` classes.

```csharp
static class ReinterpretCast
{
    public static ref T AsRef<T>(Span<byte> buffer) where T : struct
        => ref MemoryMarshal.AsRef<T>(buffer); // <- reinterpret but gets a ref

    public static unsafe void As<T>(Span<byte> buffer, out T result) where T : unmanaged // <- preallocated
    { 
        fixed (T* resultPtr = &result) 
        { 
            buffer.Slice(0, sizeof(T)).CopyTo(new Span<byte>(resultPtr, sizeof(T))); // <- copy
        };
    }

    public static unsafe T As<T>(Span<byte> buffer) where T : unmanaged
    {
        T result = new T(); // <- allocation
        buffer.Slice(0, sizeof(T)).CopyTo(new Span<byte>(&result, sizeof(T))); // <- copy
        return result;
    }
}

ReinterpretCast.As<BpbStruct>(buf, out var bpb);
var bpb = ReinterpretCast.AsRef<BpbStruct>(buf);
ref var bpb2 = ref ReinterpretCast.AsRef<BpbStruct>(buf);

Console.WriteLine("Sector size: {0}", bpbRef.LogicalSectorSize);
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
|                            Method |        Mean |        Gen 0 |      Gen 1 |        Allocated |
|---------------------------------- |------------:|-------------:|-----------:|-----------------:|
|              DeserializeUsingOld1 | 18,644.7 ms | 4412000.0000 | 19000.0000 | 55,365,696,056 B |
|              DeserializeUsingOld2 | 14,235.2 ms | 4412000.0000 | 30000.0000 | 55,365,695,784 B |
|              DeserializeUnaligned |  1,209.1 ms |            - |          - |            384 B |
|      DeserializeUnalignedOutParam |    693.7 ms |            - |          - |            384 B |
|                DeserializeUsing73 |  2,374.4 ms |            - |          - |            384 B |
|         DeserializeUsing73AndSpan |  2,141.1 ms |            - |          - |            384 B |
| DeserializeUsing73AndSpanOutParam |  1,117.7 ms |            - |          - |            384 B |
|    DeserializeUsing73AndSpanAsRef |          NA |            - |          - |                - |
```
Note: `DeserializeUsing73AndSpanAsRef` was not possible to be captured becase the mean value was too close to zero.
