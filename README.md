# Info
Since C# 7.3 it is now easier to map (copy) memory directly to a struct.

```csharp
public static unsafe T Deserialize<T>(byte[] buffer) where T : unmanaged
{
    T result = new T();

    fixed (byte* bufferPtr = buffer)
    {
        Buffer.MemoryCopy(bufferPtr, &result, sizeof(T), sizeof(T));
    }

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
1. `dotnet run`
1. Expected output:
    ```
    NTFS
    Sector size: 512
    End signature: 0xaa55 (0xAA55)
    ```
