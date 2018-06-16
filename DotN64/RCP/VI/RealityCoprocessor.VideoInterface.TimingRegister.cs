using System.Collections.Specialized;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class VideoInterface
        {
            public struct TimingRegister
            {
                #region Fields
                private BitVector32 bits;

                private static readonly BitVector32.Section horizontalSyncWidth = BitVector32.CreateSection((1 << 8) - 1),
                                                            colorBurstWidth = BitVector32.CreateSection((1 << 8) - 1, horizontalSyncWidth),
                                                            verticalSyncWidth = BitVector32.CreateSection((1 << 4) - 1, colorBurstWidth),
                                                            colorBurstStart = BitVector32.CreateSection((1 << 10) - 1, verticalSyncWidth);
                #endregion

                #region Properties
                /// <summary>
                /// Horizontal sync width in pixels.
                /// </summary>
                public byte HorizontalSyncWidth
                {
                    get => (byte)bits[horizontalSyncWidth];
                    set => bits[horizontalSyncWidth] = value;
                }

                /// <summary>
                /// Color burst width in pixels.
                /// </summary>
                public byte ColorBurstWidth
                {
                    get => (byte)bits[colorBurstWidth];
                    set => bits[colorBurstWidth] = value;
                }

                /// <summary>
                /// Vertical sync width in half lines.
                /// </summary>
                public byte VerticalSyncWidth
                {
                    get => (byte)bits[verticalSyncWidth];
                    set => bits[verticalSyncWidth] = value;
                }

                /// <summary>
                /// Start of color burst in pixels from h-sync.
                /// </summary>
                public ushort ColorBurstStart
                {
                    get => (ushort)bits[colorBurstStart];
                    set => bits[colorBurstStart] = value;
                }
                #endregion

                #region Operators
                public static implicit operator TimingRegister(uint data) => new TimingRegister { bits = new BitVector32((int)data) };

                public static implicit operator uint(TimingRegister timing) => (uint)timing.bits.Data;
                #endregion
            }
        }
    }
}
