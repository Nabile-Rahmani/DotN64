﻿namespace DotN64.RCP
{
    using static Helpers.BitHelper;

    public partial class RealityCoprocessor
    {
        public partial class VideoInterface
        {
            public struct HorizontalVideoRegister
            {
                #region Fields
                private uint data;

                private const ushort ActiveVideoEndShift = 0, ActiveVideoEndSize = (1 << 10) - 1;
                private const ushort ActiveVideoStartShift = 16, ActiveVideoStartSize = (1 << 10) - 1;
                #endregion

                #region Properties
                /// <summary>
                /// End of active video in screen pixels.
                /// </summary>
                public ushort ActiveVideoEnd
                {
                    get => (ushort)Get(data, ActiveVideoEndShift, ActiveVideoEndSize);
                    set => Set(ref data, ActiveVideoEndShift, ActiveVideoEndSize, value);
                }

                /// <summary>
                /// Start of active video in screen pixels.
                /// </summary>
                public ushort ActiveVideoStart
                {
                    get => (ushort)Get(data, ActiveVideoStartShift, ActiveVideoStartSize);
                    set => Set(ref data, ActiveVideoStartShift, ActiveVideoStartSize, value);
                }
                #endregion

                #region Operators
                public static implicit operator HorizontalVideoRegister(uint data) => new HorizontalVideoRegister { data = data };

                public static implicit operator uint(HorizontalVideoRegister register) => register.data;
                #endregion
            }
        }
    }
}
