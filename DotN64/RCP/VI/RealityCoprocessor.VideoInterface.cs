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
            public ushort CurrentVerticalLine { get; set; }
            #endregion

            #region Constructors
            public VideoInterface(RealityCoprocessor rcp)
                : base(rcp)
            {
                MemoryMaps = new[]
                {
                    new MappingEntry(0x0440000C, 0x0440000F) // VI vertical intr.
                    {
                        Write = (o, d) => VerticalInterrupt = (ushort)(d & ((1 << 10) - 1))
                    },
                    new MappingEntry(0x04400024, 0x04400027) // VI horizontal video.
                    {
                        Write = (o, d) => HorizontalVideo = d
                    },
                    new MappingEntry(0x04400010, 0x04400013) // VI current vertical line.
                    {
                        Write = (o, d) =>
                        {
                            CurrentVerticalLine = (ushort)(d & ((1 << 10) - 1));
                            rcp.MI.Interrupt &= ~MIPSInterface.Interrupts.VI;
                        }
                    }
                };
            }
            #endregion
        }
    }
}
