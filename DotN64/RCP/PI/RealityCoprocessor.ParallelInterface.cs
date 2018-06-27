using System;

namespace DotN64.RCP
{
    using Extensions;
    using Helpers;

    public partial class RealityCoprocessor
    {
        public partial class ParallelInterface : Interface
        {
            #region Fields
            private readonly MappingEntry[] memoryMaps;
            private readonly int cartridgeMapIndex;
            #endregion

            #region Properties
            private ref MappingEntry CartridgeMap => ref memoryMaps[cartridgeMapIndex];

            public Statuses Status { get; set; }

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
                MappingEntry cartridgeMap;
                MemoryMaps = memoryMaps = new[]
                {
                    new MappingEntry(0x04600010, 0x04600013) // PI status.
                    {
                        Read = o => (uint)Status,
                        Write = (o, d) =>
                        {
                            var status = (StatusWrites)d;

                            if ((status & StatusWrites.ResetController) != 0) { /* TODO. */ }

                            if ((status & StatusWrites.ClearInterrupt) != 0)
                                rcp.MI.Interrupt &= ~MIPSInterface.Interrupts.PI;
                        }
                    },
                    new MappingEntry(0x04600014, 0x04600017) // PI dom1 latency.
                    {
                        Write = (o, d) => Domains[0].Latency = (byte)d
                    },
                    new MappingEntry(0x04600018, 0x0460001B) // PI dom1 pulse width.
                    {
                        Write = (o, d) => Domains[0].PulseWidth = (byte)d
                    },
                    new MappingEntry(0x0460001C, 0x0460001F) // PI dom1 page size.
                    {
                        Write = (o, d) => Domains[0].PageSize = (byte)d
                    },
                    new MappingEntry(0x04600020, 0x04600023) // PI dom1 release.
                    {
                        Write = (o, d) => Domains[0].Release = (byte)d
                    },
                    new MappingEntry(0x04600000, 0x04600003) // PI DRAM address.
                    {
                        Write = (o, d) => DRAMAddress = d & ((1 << 24) - 1)
                    },
                    new MappingEntry(0x04600004, 0x04600007) // PI pbus (cartridge) address.
                    {
                        Write = (o, d) => PBusAddress = d
                    },
                    new MappingEntry(0x0460000C, 0x0460000F) // PI write length.
                    {
                        Write = (o, d) =>
                        {
                            WriteLength = d & ((1 << 24) - 1);
                            Status |= Statuses.DMABusy;
                            var maps = rcp.MemoryMaps;

                            for (uint i = 0; i < WriteLength + 1; i += sizeof(uint))
                            {
                                maps.WriteWord(DRAMAddress + i, maps.ReadWord(PBusAddress + i));
                            }

                            Status &= ~Statuses.DMABusy;
                            rcp.MI.Interrupt |= MIPSInterface.Interrupts.PI;
                        }
                    },
                    cartridgeMap = new MappingEntry(0x10000000, 0x1FBFFFFF, false) // Cartridge Domain 1 Address 2.
                    {
                        Read = BitHelper.ReadOpenBus
                    }
                };
                cartridgeMapIndex = Array.IndexOf(memoryMaps, cartridgeMap);
                rcp.nintendo64.CartridgeSwapped += (n, c) =>
                {
                    if (c != null)
                    {
                        var rom = c.ROM;
                        CartridgeMap.OffsetAddress = true;
                        CartridgeMap.Read = o => BitHelper.FromBigEndian(BitConverter.ToUInt32(rom, (int)o));
                    }
                    else
                    {
                        CartridgeMap.OffsetAddress = false;
                        CartridgeMap.Read = BitHelper.ReadOpenBus;
                    }
                };
            }
            #endregion
        }
    }
}
