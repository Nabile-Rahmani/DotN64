using System.Collections.Generic;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class DisplayProcessor
        {
            #region Fields
            private readonly RealityCoprocessor rcp;
            #endregion

            #region Properties
            public IReadOnlyList<MappingEntry> MemoryMaps { get; }

            public Statuses Status { get; set; }
            #endregion

            #region Constructors
            public DisplayProcessor(RealityCoprocessor rcp)
            {
                this.rcp = rcp;
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
