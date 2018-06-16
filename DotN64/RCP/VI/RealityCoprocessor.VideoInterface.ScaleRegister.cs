namespace DotN64.RCP
{
    using static Helpers.BitHelper;

    public partial class RealityCoprocessor
    {
        public partial class VideoInterface
        {
            public struct ScaleRegister
            {
                #region Fields
                private uint data;

                private const ushort ScaleUpFactorShift = 0, ScaleUpFactorSize = (1 << 12) - 1;
                private const ushort SubpixelOffsetShift = 16, SubpixelOffsetSize = (1 << 12) - 1;
                #endregion

                #region Properties
                /// <summary>
                /// 1/scale up factor (2.10 format).
                /// </summary>
                public ushort ScaleUpFactor
                {
                    get => (ushort)Get(data, ScaleUpFactorShift, ScaleUpFactorSize);
                    set => Set(ref data, ScaleUpFactorShift, ScaleUpFactorSize, value);
                }

                /// <summary>
                /// Subpixel offset (2.10 format).
                /// </summary>
                public ushort SubpixelOffset
                {
                    get => (ushort)Get(data, SubpixelOffsetShift, SubpixelOffsetSize);
                    set => Set(ref data, SubpixelOffsetShift, SubpixelOffsetSize, value);
                }
                #endregion

                #region Operators
                public static implicit operator ScaleRegister(uint data) => new ScaleRegister { data = data };

                public static implicit operator uint(ScaleRegister register) => register.data;
                #endregion
            }
        }
    }
}
