namespace DotN64.RCP
{
    using static Helpers.BitHelper;

    public partial class RealityCoprocessor
    {
        public partial class VideoInterface
        {
            public struct HorizontalSyncRegister
            {
                #region Fields
                private uint data;

                private const ushort TotalLineDurationShift = 0, TotalLineDurationSize = (1 << 12) - 1;
                private const byte LeapPatternShift = 16, LeapPatternSize = (1 << 5) - 1;
                #endregion

                #region Properties
                /// <summary>
                /// Total duration of a line in 1/4 pixel.
                /// </summary>
                public ushort TotalLineDuration
                {
                    get => (ushort)Get(data, TotalLineDurationShift, TotalLineDurationSize);
                    set => Set(ref data, TotalLineDurationShift, TotalLineDurationSize, value);
                }

                /// <summary>
                /// A 5-bit leap pattern used for PAL only (h_sync_period).
                /// </summary>
                public byte LeapPattern
                {
                    get => (byte)Get(data, LeapPatternShift, LeapPatternSize);
                    set => Set(ref data, LeapPatternShift, LeapPatternSize, value);
                }
                #endregion

                #region Operators
                public static implicit operator HorizontalSyncRegister(uint data) => new HorizontalSyncRegister { data = data };

                public static implicit operator uint(HorizontalSyncRegister register) => register.data;
                #endregion
            }
        }
    }
}
