using System.Collections.Generic;

namespace DotN64.RCP
{
    using Extensions;

    public partial class RealityCoprocessor
    {
        public class DisplayProcessor
        {
            #region Fields
            private readonly IReadOnlyList<MappingEntry> memoryMaps;
            #endregion

            #region Constructors
            public DisplayProcessor()
            {
                memoryMaps = new[]
                {
                    new MappingEntry(0x0410000C, 0x0410000F) // DP CMD status.
                    {
                        Read = o => 0
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
}
