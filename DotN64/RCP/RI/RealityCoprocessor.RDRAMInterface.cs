using System;
using System.Collections.Generic;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class RDRAMInterface : Interface
        {
            #region Fields
            private readonly IReadOnlyList<MappingEntry> memoryMaps;
            #endregion

            #region Properties
            protected override IReadOnlyList<MappingEntry> MemoryMaps => memoryMaps;

            public byte Select { get; set; } = 0x14;

            public ConfigRegister Config { get; set; } = 0x40;

            public ModeRegister Mode { get; set; } = 0x0E;

            public RefreshRegister Refresh { get; set; } = 0x00063634;

            public byte[] RAM { get; } = new byte[0x00400000]; // The base system has 4 MB of RAM installed.
            #endregion

            #region Constructors
            public RDRAMInterface()
            {
                memoryMaps = new[]
                {
                    new MappingEntry(0x0470000C, 0x0470000F) // RI select.
                    {
                        Read = o => Select,
                        Write = (o, v) => Select = (byte)(v & (1 << 3) - 1)
                    },
                    new MappingEntry(0x04700004, 0x04700007) // RI config.
                    {
                        Write = (o, v) => Config = v
                    },
                    new MappingEntry(0x04700008, 0x0470000B) // RI current load.
                    {
                        Write = (o, v) => { /* TODO: Any write updates current control register. */ }
                    },
                    new MappingEntry(0x04700000, 0x04700003) // RI mode.
                    {
                        Write = (o, v) => Mode = v
                    },
                    new MappingEntry(0x04700010, 0x04700013) // RI refresh.
                    {
                    },
                    new MappingEntry(0x00000000, 0x03EFFFFF) // RDRAM memory.
                    {
                        Read = o => BitConverter.ToUInt32(RAM, (int)o),
                        Write = (o, v) =>
                        {
                            unsafe
                            {
                                fixed (byte* data = &RAM[(int)o])
                                {
                                    *(uint*)data = v;
                                }
                            }
                        }
                    }
                };
            }
            #endregion
        }
    }
}
