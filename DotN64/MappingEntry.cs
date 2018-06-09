using System;
using System.Runtime.CompilerServices;

namespace DotN64
{
    public struct MappingEntry
    {
        #region Properties
        public uint StartAddress { get; set; }

        public uint EndAddress { get; set; }

        public bool OffsetAddress { get; set; }

        public Func<uint, uint> Read { get; set; }

        public Action<uint, uint> Write { get; set; }
        #endregion

        #region Constructors
        public MappingEntry(uint startAddress, uint endAddress, bool offsetAddress = true)
            : this()
        {
            StartAddress = startAddress;
            EndAddress = endAddress;
            OffsetAddress = offsetAddress;
        }
        #endregion

        #region Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(uint address) => address >= StartAddress && address <= EndAddress;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadWord(uint address) => Read(OffsetAddress ? address - StartAddress : address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWord(uint address, uint data) => Write(OffsetAddress ? address - StartAddress : address, data);

        public override string ToString() => $"0x{StartAddress:X8} .. 0x{EndAddress:X8} ({(Read != null ? "R" : string.Empty)}{(Write != null ? "W" : string.Empty)})";
        #endregion
    }
}
