using System.Collections.Generic;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class VideoInterface : Interface
        {
            #region Fields
            private readonly IReadOnlyList<MappingEntry> memoryMaps;
            #endregion

            #region Properties
            protected override IReadOnlyList<MappingEntry> MemoryMaps => memoryMaps;

            private ushort verticalInterrupt;
            /// <summary>
            /// Interrupt when current half-line = V_INTR.
            /// </summary>
            public ushort VerticalInterrupt
            {
                get => verticalInterrupt;
                set => verticalInterrupt = (ushort)(value & ((1 << 10) - 1));
            }

            public HorizontalVideoRegister HorizontalVideo { get; set; }

            private ushort currentVerticalLine;
            /// <summary>
            /// Current half line, sampled once per line (the lsb of V_CURRENT is constant within a field, and in interlaced modes gives the field number - which is constant for non-interlaced modes).
            /// </summary>
            public ushort CurrentVerticalLine
            {
                get => currentVerticalLine;
                set
                {
                    currentVerticalLine = (ushort)(value & ((1 << 10) - 1));

                    // TODO: Clear interrupt line.
                }
            }
            #endregion

            #region Constructors
            public VideoInterface()
            {
                memoryMaps = new[]
                {
                    new MappingEntry(0x0440000C, 0x0440000F) // VI vertical intr.
                    {
                        Write = (o, v) => VerticalInterrupt = (ushort)v
                    },
                    new MappingEntry(0x04400024, 0x04400027) // VI horizontal video.
                    {
                        Write = (o, v) => HorizontalVideo = v
                    },
                    new MappingEntry(0x04400010, 0x04400013) // VI current vertical line.
                    {
                        Write = (o, v) => CurrentVerticalLine = (ushort)v
                    }
                };
            }
            #endregion
        }
    }
}
