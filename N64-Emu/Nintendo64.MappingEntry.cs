namespace N64Emu
{
    public partial class Nintendo64
    {
        public struct MappingEntry
        {
            #region Fields
            public delegate T Read<T>(MappingEntry entry, ulong address) where T : struct;
            public delegate void Write<T>(MappingEntry entry, ulong address, T value) where T : struct;
            #endregion

            #region Properties
            public uint StartAddress { get; set; }

            public uint EndAddress { get; set; }

            public Read<uint> ReadWord { get; set; }

            public Write<uint> WriteWord { get; set; }
            #endregion

            #region Methods
            public bool Contains(ulong address) => (uint)address >= StartAddress && (uint)address <= EndAddress;
            #endregion
        }
    }
}
