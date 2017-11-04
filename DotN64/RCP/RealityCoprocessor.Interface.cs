using System.Collections.Generic;

namespace DotN64.RCP
{
    using Extensions;

    public partial class RealityCoprocessor
    {
        public abstract class Interface
        {
            #region Fields
            protected readonly RealityCoprocessor rcp;
            #endregion

            #region Properties
            protected abstract IReadOnlyList<MappingEntry> MemoryMaps { get; }
            #endregion

            #region Constructors
            protected Interface(RealityCoprocessor rcp)
            {
                this.rcp = rcp;
            }
            #endregion

            #region Methods
            public uint ReadWord(ulong address) => MemoryMaps.GetEntry(address).ReadWord(address);

            public void WriteWord(ulong address, uint value) => MemoryMaps.GetEntry(address).WriteWord(address, value);
            #endregion
        }
    }
}
