using System.Collections.Specialized;

namespace N64Emu.VI
{
    public partial class VideoInterface
    {
        public struct HorizontalVideoRegister
        {
            #region Fields
            private BitVector32 bits;

            private static readonly BitVector32.Section activeVideoEnd = BitVector32.CreateSection((1 << 10) - 1),
            unknown1 = BitVector32.CreateSection((1 << 6) - 1, activeVideoEnd),
            activeVideoStart = BitVector32.CreateSection((1 << 10) - 1, unknown1);
            #endregion

            #region Properties
            /// <summary>
            /// End of active video in screen pixels.
            /// </summary>
            public ushort ActiveVideoEnd
            {
                get => (ushort)bits[activeVideoEnd];
                set => bits[activeVideoEnd] = value;
            }

            /// <summary>
            /// Start of active video in screen pixels.
            /// </summary>
            public ushort ActiveVideoStart
            {
                get => (ushort)bits[activeVideoStart];
                set => bits[activeVideoStart] = value;
            }
            #endregion

            #region Operators
            public static implicit operator HorizontalVideoRegister(uint data) => new HorizontalVideoRegister { bits = new BitVector32((int)data) };

            public static implicit operator uint(HorizontalVideoRegister register) => (uint)register.bits.Data;
            #endregion
        }
    }
}
