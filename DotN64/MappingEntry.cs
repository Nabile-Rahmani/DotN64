using System;
using System.Runtime.CompilerServices;

namespace DotN64
{
    public struct MappingEntry
    {
        #region Properties
        public ulong StartAddress { get; set; }

        public ulong EndAddress { get; set; }

        public bool OffsetAddress { get; set; }

        public Func<ulong, uint> Read { get; set; }

        public Action<ulong, uint> Write { get; set; }
        #endregion

        #region Constructors
        public MappingEntry(ulong startAddress, ulong endAddress, bool offsetAddress = true)
            : this()
        {
            StartAddress = startAddress;
            EndAddress = endAddress;
            OffsetAddress = offsetAddress;
        }
        #endregion

        #region Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(ulong address) => address >= StartAddress && address <= EndAddress;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadWord(ulong address) => Read(OffsetAddress ? address - StartAddress : address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWord(ulong address, uint value) => Write(OffsetAddress ? address - StartAddress : address, value);

        public override string ToString() => $"[0x{StartAddress:X16}..0x{EndAddress:X16}] - {(Read != null ? "R" : "-")}{(Write != null ? "W" : "-")}";
        #endregion
    }
}
