using System;

namespace DotN64
{
    public struct MappingEntry
    {
        #region Properties
        public ulong StartAddress { get; set; }

        public ulong EndAddress { get; set; }

        public Func<ulong, uint> Read { get; set; }

        public Action<ulong, uint> Write { get; set; }

        public bool OffsetAddress { get; set; }
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
        public bool Contains(ulong address) => address >= StartAddress && address <= EndAddress;

        public uint ReadWord(ulong address) => Read(OffsetAddress ? address - StartAddress : address);

        public void WriteWord(ulong address, uint value) => Write(OffsetAddress ? address - StartAddress : address, value);

        public override string ToString() => $"[0x{StartAddress:X16}..0x{EndAddress:X16}] - {(Read != null ? "R" : "-")}{(Write != null ? "W" : "-")}";
        #endregion
    }
}
