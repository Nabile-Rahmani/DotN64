namespace N64Emu.Interfaces.Video
{
    public partial class VideoInterface
    {
        public struct HorizontalVideoRegister
        {
            #region Fields
            private uint data;

            private const int ActiveVideoEndShift = 0, ActiveVideoEndSize = (1 << 10) - 1;
            private const int ActiveVideoStartShift = 16, ActiveVideoStartSize = (1 << 10) - 1;
            #endregion

            #region Properties
            /// <summary>
            /// End of active video in screen pixels.
            /// </summary>
            public ushort ActiveVideoEnd
            {
                get => (ushort)Get(ActiveVideoEndShift, ActiveVideoEndSize);
                set => Set(ActiveVideoEndShift, ActiveVideoEndSize, value);
            }

            /// <summary>
            /// Start of active video in screen pixels.
            /// </summary>
            public ushort ActiveVideoStart
            {
                get => (ushort)Get(ActiveVideoStartShift, ActiveVideoStartSize);
                set => Set(ActiveVideoStartShift, ActiveVideoStartSize, value);
            }
            #endregion

            #region Methods
            private uint Get(int shift, uint size) => data >> shift & size;

            private void Set(int shift, uint size, uint value)
            {
                data &= ~(size << shift);
                data |= (value & size) << shift;
            }
            #endregion

            #region Operators
            public static implicit operator HorizontalVideoRegister(uint value) => new HorizontalVideoRegister { data = value };

            public static implicit operator uint(HorizontalVideoRegister register) => register.data;
            #endregion
        }
    }
}
