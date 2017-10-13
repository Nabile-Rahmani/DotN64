using System.Collections.Generic;

namespace DotN64.MI
{
    using Extensions;

    public class MIPSInterface
    {
        #region Fields
        private readonly IReadOnlyList<MappingEntry> memoryMaps;
        #endregion

        #region Constructors
        public MIPSInterface()
        {
            memoryMaps = new[]
            {
                new MappingEntry(0x04300004, 0x04300007) // MI version.
                {
                    Write = (o, v) => { }
                }
            };
        }
        #endregion

        #region Methods
        public uint ReadWord(ulong address) => memoryMaps.GetEntry(address).ReadWord(address);

        public void WriteWord(ulong address, uint value) => memoryMaps.GetEntry(address).WriteWord(address, value);
        #endregion
    }
}
