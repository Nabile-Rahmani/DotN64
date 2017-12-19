using System;

namespace DotN64.RCP
{
    using Extensions;
    using Helpers;

    public partial class RealityCoprocessor
    {
        public partial class ParallelInterface : Interface
        {
            #region Properties
            public StatusRegister Status { get; set; }

            public Domain[] Domains { get; } = new[]
            {
                new Domain(),
                new Domain()
            };

            /// <summary>
            /// Starting RDRAM address.
            /// </summary>
            public uint DRAMAddress { get; set; }

            /// <summary>
            /// Starting AD16 address.
            /// </summary>
            public uint PBusAddress { get; set; }

            /// <summary>
            /// Write data length.
            /// </summary>
            public uint WriteLength { get; set; }
            #endregion

            #region Constructors
            public ParallelInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x04600010, 0x04600013) // PI status.
                    {
                        Read = o => (uint)Status,
                        Write = (o, v) =>
                        {
                            var status = (WriteStatusRegister)v;

                            if ((status & WriteStatusRegister.ResetController) != 0) { /* TODO. */ }

                            if ((status & WriteStatusRegister.ClearInterrupt) != 0) { /* TODO. */ }
                        }
                    },
                    new MappingEntry(0x04600014, 0x04600017) // PI dom1 latency.
                    {
                        Write = (o, v) => Domains[0].Latency = (byte)v
                    },
                    new MappingEntry(0x04600018, 0x0460001B) // PI dom1 pulse width.
                    {
                        Write = (o, v) => Domains[0].PulseWidth = (byte)v
                    },
                    new MappingEntry(0x0460001C, 0x0460001F) // PI dom1 page size.
                    {
                        Write = (o, v) => Domains[0].PageSize = (byte)v
                    },
                    new MappingEntry(0x04600020, 0x04600023) // PI dom1 release.
                    {
                        Write = (o, v) => Domains[0].Release = (byte)v
                    },
                    new MappingEntry(0x04600000, 0x04600003) // PI DRAM address.
                    {
                        Write = (o, v) => DRAMAddress = v & ((1 << 24) - 1)
                    },
                    new MappingEntry(0x04600004, 0x04600007) // PI pbus (cartridge) address.
                    {
                        Write = (o, v) => PBusAddress = v
                    },
                    new MappingEntry(0x0460000C, 0x0460000F) // PI write length.
                    {
                        Write = (o, v) =>
                        {
                            WriteLength = v & ((1 << 24) - 1);
                            Status |= StatusRegister.DMABusy;
                            var maps = rcp.Nintendo64.MemoryMaps;

                            for (uint i = 0; i < WriteLength + 1; i += sizeof(uint))
                            {
                                maps.WriteWord(DRAMAddress + i, maps.ReadWord(PBusAddress + i));
                            }

                            Status &= ~StatusRegister.DMABusy;
                            // TODO: Set interrupt.
                        }
                    },
                    new MappingEntry(0x10000000, 0x1FBFFFFF) // Cartridge Domain 1 Address 2.
                    {
                        Read = o => BitHelper.FromBigEndian(BitConverter.ToUInt32(rcp.Nintendo64.Cartridge.ROM, (int)o))
                    }
                };
            }
            #endregion
        }
    }
}
