namespace DotN64.RCP
{
    using static Helpers.BitHelper;

    public partial class RealityCoprocessor
    {
        public partial class VideoInterface
        {
            public struct VerticalBurstRegister
            {
                #region Fields
                private uint data;

                private const ushort ColorBurstEnableEndShift = 0, ColorBurstEnableEndSize = (1 << 10) - 1;
                private const ushort ColorBurstEnableStartShift = 16, ColorBurstEnableStartSize = (1 << 10) - 1;
                #endregion

                #region Properties
                /// <summary>
                /// End of color burst enable in half-lines.
                /// </summary>
                public ushort ColorBurstEnableEnd
                {
                    get => (ushort)Get(data, ColorBurstEnableEndShift, ColorBurstEnableEndSize);
                    set => Set(ref data, ColorBurstEnableEndShift, ColorBurstEnableEndSize, value);
                }

                /// <summary>
                /// Start of color burst enable in half-lines.
                /// </summary>
                public ushort ColorBurstEnableStart
                {
                    get => (ushort)Get(data, ColorBurstEnableStartShift, ColorBurstEnableStartSize);
                    set => Set(ref data, ColorBurstEnableStartShift, ColorBurstEnableStartSize, value);
                }
                #endregion

                #region Operators
                public static implicit operator VerticalBurstRegister(uint data) => new VerticalBurstRegister { data = data };

                public static implicit operator uint(VerticalBurstRegister register) => register.data;
                #endregion
            }
        }
    }
}
