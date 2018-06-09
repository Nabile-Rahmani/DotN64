namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class RDRAMInterface : Interface
        {
            #region Properties
            public byte Select { get; set; }

            public ConfigRegister Config { get; set; }

            public ModeRegister Mode { get; set; }

            public RefreshRegister Refresh { get; set; }
            #endregion

            #region Constructors
            public RDRAMInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x0470000C, 0x0470000F) // RI select.
                    {
                        Read = o => Select,
                        Write = (o, d) => Select = (byte)(d & (1 << 3) - 1)
                    },
                    new MappingEntry(0x04700004, 0x04700007) // RI config.
                    {
                        Write = (o, d) => Config = d
                    },
                    new MappingEntry(0x04700008, 0x0470000B) // RI current load.
                    {
                        Write = (o, d) => { /* TODO: Any write updates current control register. */ }
                    },
                    new MappingEntry(0x04700000, 0x04700003) // RI mode.
                    {
                        Write = (o, d) => Mode = d
                    },
                    new MappingEntry(0x04700010, 0x04700013) // RI refresh.
                    {
                        Read = o => Refresh,
                        Write = (o, d) => Refresh = d
                    }
                };
            }
            #endregion
        }
    }
}
