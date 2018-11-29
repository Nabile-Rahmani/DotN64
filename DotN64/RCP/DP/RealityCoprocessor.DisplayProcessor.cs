using System.Collections.Generic;

namespace DotN64.RCP
{
    using static RealityCoprocessor.DisplayProcessor.Angrylion;

    public partial class RealityCoprocessor
    {
        public partial class DisplayProcessor
        {
            #region Fields
            private readonly RealityCoprocessor rcp;
            #endregion

            #region Properties
            public IReadOnlyList<MappingEntry> MemoryMaps { get; }

            private uint status;
            public Statuses Status
            {
                get => (Statuses)status;
                set => status = (uint)value;
            }

            private uint startAddress;
            /// <summary>
            /// DMEM/RDRAM start address.
            /// </summary>
            public uint StartAddress
            {
                get => startAddress;
                set => startAddress = currentAddress = value;
            }

            private uint currentAddress;
            /// <summary>
            /// DMEM/RDRAM current address.
            /// </summary>
            public uint CurrentAddress
            {
                get => currentAddress;
                set => currentAddress = value;
            }

            private uint endAddress;
            /// <summary>
            /// DMEM/RDRAM end address.
            /// </summary>
            public uint EndAddress
            {
                get => endAddress;
                set
                {
                    endAddress = value;
                    ProcessCommands();
                }
            }
            #endregion

            #region Constructors
            public DisplayProcessor(RealityCoprocessor rcp)
            {
                this.rcp = rcp;
                MemoryMaps = new[]
                {
                    new MappingEntry(0x04100000, 0x04100003) // DP CMD DMA start.
                    {
                        Write = (o, d) => StartAddress = d & ((1 << 24) - 1)
                    },
                    new MappingEntry(0x04100004, 0x04100007) // DP CMD DMA end.
                    {
                        Write = (o, d) => EndAddress = d & ((1 << 24) - 1)
                    },
                    new MappingEntry(0x0410000C, 0x0410000F) // DP CMD status.
                    {
                        Read = o => (uint)Status
                    }
                };

                angrylion_rdp_init();
            }
            #endregion

            #region Methods
            private unsafe void ProcessCommands()
            {
                fixed (byte* dmem = &rcp.SP.DMEM[0])
                fixed (byte* rdram = &rcp.nintendo64.RAM.Memory[0])
                fixed (uint* startAddressPtr = &startAddress)
                fixed (uint* currentAddressPtr = &currentAddress)
                fixed (uint* endAddressPtr = &endAddress)
                fixed (uint* statusPtr = &status)
                {
                    var fullSynced = rdp_process_list(startAddressPtr, currentAddressPtr, endAddressPtr, statusPtr, (uint*)dmem, (uint*)rdram, out var crashed);

                    if (fullSynced != 0)
                        rcp.MI.Interrupt |= MIPSInterface.Interrupts.DP;
                }
            }
            #endregion
        }
    }
}
