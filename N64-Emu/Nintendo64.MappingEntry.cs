using System;

namespace N64Emu
{
    public partial class Nintendo64
    {
        public struct MappingEntry
        {
            #region Properties
            public uint StartAddress { get; set; }

            public uint EndAddress { get; set; }

            public Func<ulong, uint> Read { get; set; }

            public Action<ulong, uint> Write { get; set; }
            #endregion

            #region Constructors
            public MappingEntry(uint startAddress, uint endAddress)
                : this()
            {
                StartAddress = startAddress;
                EndAddress = endAddress;
            }
            #endregion

            #region Methods
            public bool Contains(ulong address) => (uint)address >= StartAddress && (uint)address <= EndAddress;

            public uint ReadWord(ulong address) => Read(address - StartAddress);

            public void WriteWord(ulong address, uint value) => Write(address - StartAddress, value);
            #endregion
        }
    }
}
