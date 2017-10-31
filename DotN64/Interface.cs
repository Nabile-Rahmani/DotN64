using System.Collections.Generic;

namespace DotN64
{
    using Extensions;

    public abstract class Interface
    {
        #region Properties
        protected abstract IReadOnlyList<MappingEntry> MemoryMaps { get; }
        #endregion

        #region Methods
        public uint ReadWord(ulong address) => MemoryMaps.GetEntry(address).ReadWord(address);

        public void WriteWord(ulong address, uint value) => MemoryMaps.GetEntry(address).WriteWord(address, value);
        #endregion
    }
}
