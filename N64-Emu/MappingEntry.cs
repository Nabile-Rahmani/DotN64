using System;

namespace N64Emu
{
    public struct MappingEntry
    {
        #region Properties
        public uint StartAddress { get; set; }

        public uint EndAddress { get; set; }

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
        public bool Contains(ulong address) => (uint)address >= StartAddress && (uint)address <= EndAddress;

        public uint ReadWord(ulong address) => Read(OffsetAddress ? address - StartAddress : address);

        public void WriteWord(ulong address, uint value) => Write(OffsetAddress ? address - StartAddress : address, value);
        #endregion
    }
}
