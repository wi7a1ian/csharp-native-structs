using System;
using System.Runtime.InteropServices;

namespace ReadStructFromFs
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 96, Pack = 1)]
    unsafe public struct BpbStruct
    {
        [FieldOffset(0x000)]
        public fixed byte  m_jumpCode[3];				// 0x000(0x003) - jump instruction
        
        [FieldOffset(0x003)]
        public fixed sbyte  m_oemLabel[8];				// 0x003(0x008) - OEM label = "NTFS    "
        
        [FieldOffset(0x00B)]
        public UInt16 m_logicalSectorSize;		// 0x00B(0x002) - logical sector size in bytes (typically matches physical sector size)
        
        [FieldOffset(0x00D)]
        public byte  m_sectorsPerCluster;		// 0x00D(0x001) - sectors per cluster
        
        [FieldOffset(0x00E)]
        public UInt16 m_reservedRecords;			// 0x00E(0x002) - always zero on NTFS (reserved records)
        
        [FieldOffset(0x010)]
        public byte  m_numFats;					// 0x010(0x001) - always zero on NTFS
        
        [FieldOffset(0x011)]
        public UInt16 m_rootDirEntries;			// 0x011(0x002) - always zero on NTFS
        
        [FieldOffset(0x013)]
        public UInt16 m_volumeRecordsFat;		// 0x013(0x002) - always zero on NTFS (used on FAT12/16 < 32MB)
        
        [FieldOffset(0x015)]
        public byte  m_mediaType;				// 0x015(0x001) - media descriptor (0xF8 on NTFS)
        
        [FieldOffset(0x016)]
        public UInt16 m_fatRecords;				// 0x016(0x002) - always zero on NTFS
        
        [FieldOffset(0x018)]
        public UInt16 m_sectorsPerTrack;			// 0x018(0x002) - unused on NTFS (sectors/track)

        [FieldOffset(0x01A)]
        public UInt16 m_heads;					// 0x01A(0x002) - unused on NTFS (tracks/cylinder)

        [FieldOffset(0x01C)]
        public UInt32 m_relativeRecordsFatExt;	// 0x01C(0x004) - unused on NTFS (offset from MBR)

        [FieldOffset(0x020)]
        public UInt32 m_volumeRecords2;			// 0x020(0x004) - always zero on NTFS (used on FAT16 > 32mb)

        [FieldOffset(0x024)]
        public UInt32 m_unused24;				// 0x024(0x004) - unused on NTFS

        [FieldOffset(0x028)]
        public UInt64 m_volumeSectorCount;		// 0x028(0x008) - total sector count on volume

        [FieldOffset(0x030)]
        public UInt64 m_mftCluster;				// 0x030(0x008) - first cluster of MFT

        [FieldOffset(0x038)]
        public UInt64 m_mftMirrorCluster;		// 0x038(0x008) - first cluster of MFTMirr

        [FieldOffset(0x040)]
        public sbyte   m_mftEntrySize;			// 0x040(0x001) - MFT entry size (positive value - clusters; negative value - bytes computed as 2 at power of absolute value)

        [FieldOffset(0x041)]
        public fixed byte  m_unused41[3];				// 0x041(0x003) - unused on NTFS

        [FieldOffset(0x044)]
        public sbyte   m_indexEntrySize;			// 0x044(0x001) - index entry size (positive value - clusters; negative value - bytes computed as 2 at power of absolute value)

        [FieldOffset(0x045)]
        public fixed byte  m_unused45[3];				// 0x045(0x003) - unused on NTFS

        [FieldOffset(0x048)]
        public UInt64 m_serialNumber;			// 0x048(0x008) - volume serial number

        [FieldOffset(0x050)]
        public UInt32 m_checkSum;				// 0x050(0x004) - unused on NTFS (BPB checksum)

        [FieldOffset(0x054)]
        public fixed byte  m_bootCode[0x1AA];			// 0x054(0x1AA) - boot code

        [FieldOffset(0x1FE)]
        public UInt16 m_endSignature;			// 0x1FE(0x002) - sector signature = 0xAA55 (seems to be at the same position no matter the sector size)
    }
}