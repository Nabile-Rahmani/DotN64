namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public class AudioInterface : Interface
        {
            #region Properties
            private uint dramAddress;
            /// <summary>
            /// Starting RDRAM address (8B-aligned).
            /// </summary>
            public uint DRAMAddress
            {
                get => dramAddress;
                set => dramAddress = value & ((1 << 24) - 1);
            }

            private uint transferLength;
            public uint TransferLength
            {
                get => transferLength;
                set => transferLength = (uint)(value & ((1 << 18) - 1) & ~((1 << 3) - 1)); // "v2.0".
            }
            #endregion

            #region Constructors
            public AudioInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x04500000, 0x04500003) // AI DRAM address.
                    {
                        Write = (o, v) => DRAMAddress = v
                    },
                    new MappingEntry(0x04500004, 0x04500007) // AI length.
                    {
                        Write = (o, v) => TransferLength = v
                    },
                    new MappingEntry(0x0450000C, 0x0450000F) // AI status.
                    {
                        Write = (o, v) => rcp.MI.Interrupt &= ~MIPSInterface.Interrupts.AI
                    }
                };
            }
            #endregion
        }
    }
}
