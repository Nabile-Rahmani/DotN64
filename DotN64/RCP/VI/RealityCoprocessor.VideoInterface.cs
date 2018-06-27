namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class VideoInterface : Interface
        {
            #region Properties
            /// <summary>
            /// Interrupt when current half-line = V_INTR.
            /// </summary>
            public ushort VerticalInterrupt { get; set; }

            public HorizontalVideoRegister HorizontalVideo { get; set; }

            /// <summary>
            /// Current half line, sampled once per line (the lsb of V_CURRENT is constant within a field, and in interlaced modes gives the field number - which is constant for non-interlaced modes).
            /// </summary>
            private ushort currentVerticalLine;
            public ushort CurrentVerticalLine
            {
                get => currentVerticalLine;
                set
                {
                    if ((currentVerticalLine = value) == VerticalInterrupt)
                        rcp.MI.Interrupt |= MIPSInterface.Interrupts.VI;
                }
            }

            public ControlRegister Control { get; set; }

            /// <summary>
            /// Frame buffer origin in bytes.
            /// </summary>
            public uint DRAMAddress { get; set; }

            /// <summary>
            /// Frame buffer line width in pixels.
            /// </summary>
            public ushort Width { get; set; }

            public TimingRegister Timing { get; set; }

            /// <summary>
            /// Number of half-lines per field.
            /// </summary>
            public ushort VerticalSync { get; set; }

            public HorizontalSyncRegister HorizontalSync { get; set; }

            /// <summary>
            /// [11:0] identical to h_sync_period.
            /// [27:16] identical to h_sync_period.
            /// </summary>
            public uint HorizontalSyncLeap { get; set; }

            public VerticalVideoRegister VerticalVideo { get; set; }

            public VerticalBurstRegister VerticalBurst { get; set; }

            public ScaleRegister HorizontalScale { get; set; }

            public ScaleRegister VerticalScale { get; set; }
            #endregion

            #region Constructors
            public VideoInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x04400000, 0x04400003) // VI status/control.
                    {
                        Write = (o, d) => Control = (ushort)d
                    },
                    new MappingEntry(0x04400004, 0x04400007) // VI origin.
                    {
                        Write = (o, d) => DRAMAddress = d & ((1 << 24) - 1)
                    },
                    new MappingEntry(0x04400008, 0x0440000B) // VI width.
                    {
                        Write = (o, d) => Width = (ushort)(d & ((1 << 12) - 1))
                    },
                    new MappingEntry(0x0440000C, 0x0440000F) // VI vertical intr.
                    {
                        Write = (o, d) => VerticalInterrupt = (ushort)(d & ((1 << 10) - 1))
                    },
                    new MappingEntry(0x04400010, 0x04400013) // VI current vertical line.
                    {
                        Read = o => CurrentVerticalLine,
                        Write = (o, d) =>
                        {
                            CurrentVerticalLine = (ushort)(d & ((1 << 10) - 1));
                            rcp.MI.Interrupt &= ~MIPSInterface.Interrupts.VI;
                        }
                    },
                    new MappingEntry(0x04400014, 0x04400017) // VI video timing.
                    {
                        Write = (o, d) => Timing = d
                    },
                    new MappingEntry(0x04400018, 0x0440001B) // VI vertical sync.
                    {
                        Write = (o, d) => VerticalSync = (ushort)(d & ((1 << 10) - 1))
                    },
                    new MappingEntry(0x0440001C, 0x0440001F) // VI horizontal sync.
                    {
                        Write = (o, d) => HorizontalSync = d
                    },
                    new MappingEntry(0x04400020, 0x04400023) // VI horizontal sync leap.
                    {
                        Write = (o, d) => HorizontalSyncLeap = d
                    },
                    new MappingEntry(0x04400024, 0x04400027) // VI horizontal video.
                    {
                        Write = (o, d) => HorizontalVideo = d
                    },
                    new MappingEntry(0x04400028, 0x0440002B) // VI vertical video.
                    {
                        Write = (o, d) => VerticalVideo = d
                    },
                    new MappingEntry(0x0440002C, 0x0440002F) // VI vertical burst.
                    {
                        Write = (o, d) => VerticalBurst = d
                    },
                    new MappingEntry(0x04400030, 0x04400033) // VI x-scale.
                    {
                        Write = (o, d) => HorizontalScale = d
                    },
                    new MappingEntry(0x04400034, 0x04400037) // VI y-scale.
                    {
                        Write = (o, d) => VerticalScale = d
                    }
                };
            }
            #endregion
        }
    }
}
