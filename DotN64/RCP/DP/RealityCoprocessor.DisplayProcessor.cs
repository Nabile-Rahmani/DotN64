using System.Collections.Generic;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class DisplayProcessor
        {
            #region Properties
            public IReadOnlyList<MappingEntry> MemoryMaps { get; }

            public StatusRegister Status { get; set; }
            #endregion

            #region Constructors
            public DisplayProcessor()
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x0410000C, 0x0410000F) // DP CMD status.
                    {
                        Read = o => (uint)Status
                    }
                };
            }
            #endregion
        }
    }
}
