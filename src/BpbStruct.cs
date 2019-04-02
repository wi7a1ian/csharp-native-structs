using System;
using System.Runtime.InteropServices;

namespace ReadStructFromFs
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 96, Pack = 1)]
    unsafe public struct BpbStruct
    {
        [FieldOffset(0x000)]
        public fixed byte  JumpCode[3];		    // 0x000(0x003) - jump instruction
        
        [FieldOffset(0x003)]
        public fixed sbyte  OemLabel[8];		// 0x003(0x008) - OEM label = "NTFS    "
        
        [FieldOffset(0x00B)]
        public UInt16 LogicalSectorSize;		// 0x00B(0x002) - logical sector size in bytes (typically matches physical sector size)
        
        [FieldOffset(0x00D)]
        public byte  SectorsPerCluster;		    // 0x00D(0x001) - sectors per cluster
        
        [FieldOffset(0x00E)]
        public UInt16 ReservedRecords;		    // 0x00E(0x002) - always zero on NTFS (reserved records)
        
        [FieldOffset(0x010)]
        public byte  NumFats;					// 0x010(0x001) - always zero on NTFS
        
        [FieldOffset(0x011)]
        public UInt16 RootDirEntries;			// 0x011(0x002) - always zero on NTFS
        
        [FieldOffset(0x013)]
        public UInt16 VolumeRecordsFat;		    // 0x013(0x002) - always zero on NTFS (used on FAT12/16 < 32MB)
        
        [FieldOffset(0x015)]
        public byte  MediaType;				    // 0x015(0x001) - media descriptor (0xF8 on NTFS)
        
        [FieldOffset(0x016)]
        public UInt16 FatRecords;				// 0x016(0x002) - always zero on NTFS
        
        [FieldOffset(0x018)]
        public UInt16 SectorsPerTrack;		    // 0x018(0x002) - unused on NTFS (sectors/track)

        [FieldOffset(0x01A)]
        public UInt16 Heads;					// 0x01A(0x002) - unused on NTFS (tracks/cylinder)

        [FieldOffset(0x01C)]
        public UInt32 RelativeRecordsFatExt;	// 0x01C(0x004) - unused on NTFS (offset from MBR)

        [FieldOffset(0x020)]
        public UInt32 VolumeRecords2;			// 0x020(0x004) - always zero on NTFS (used on FAT16 > 32mb)

        [FieldOffset(0x024)]
        public UInt32 Unused24;				    // 0x024(0x004) - unused on NTFS

        [FieldOffset(0x028)]
        public UInt64 VolumeSectorCount;		// 0x028(0x008) - total sector count on volume

        [FieldOffset(0x030)]
        public UInt64 MftCluster;				// 0x030(0x008) - first cluster of MFT

        [FieldOffset(0x038)]
        public UInt64 MftMirrorCluster;		    // 0x038(0x008) - first cluster of MFTMirr

        [FieldOffset(0x040)]
        public sbyte MftEntrySize;			    // 0x040(0x001) - MFT entry size (positive value - clusters; negative value - bytes computed as 2 at power of absolute value)

        [FieldOffset(0x041)]
        public fixed byte Unused41[3];		// 0x041(0x003) - unused on NTFS

        [FieldOffset(0x044)]
        public sbyte IndexEntrySize;		// 0x044(0x001) - index entry size (positive value - clusters; negative value - bytes computed as 2 at power of absolute value)

        [FieldOffset(0x045)]
        public fixed byte Unused45[3];		// 0x045(0x003) - unused on NTFS

        [FieldOffset(0x048)]
        public UInt64 SerialNumber;			// 0x048(0x008) - volume serial number

        [FieldOffset(0x050)]
        public UInt32 CheckSum;				// 0x050(0x004) - unused on NTFS (BPB checksum)

        [FieldOffset(0x054)]
        public fixed byte BootCode[0x1AA];	// 0x054(0x1AA) - boot code

        [FieldOffset(0x1FE)]
        public UInt16 EndSignature;			// 0x1FE(0x002) - sector signature = 0xAA55 (seems to be at the same position no matter the sector size)
    }
}