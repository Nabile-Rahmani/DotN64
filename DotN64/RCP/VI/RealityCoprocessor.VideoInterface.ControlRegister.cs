using System;
using System.Collections.Specialized;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class VideoInterface
        {
            public struct ControlRegister
            {
                #region Fields
                private BitVector32 bits;

                private static readonly BitVector32.Section type = BitVector32.CreateSection((1 << 2) - 1),
                                                            gammaDitherEnable = BitVector32.CreateSection(1, type),
                                                            gammaEnable = BitVector32.CreateSection(1, gammaDitherEnable),
                                                            divotEnable = BitVector32.CreateSection(1, gammaEnable),
                                                            reserved1 = BitVector32.CreateSection(1, divotEnable), // Always off.
                                                            serrate = BitVector32.CreateSection(1, reserved1),
                                                            reserved2 = BitVector32.CreateSection(1, serrate), // Diagnostics only.
                                                            antiAliasMode = BitVector32.CreateSection((1 << 2) - 1, reserved2);
                #endregion

                #region Properties
                public PixelSize Type
                {
                    get => (PixelSize)bits[type];
                    set => bits[type] = (byte)value;
                }

                /// <summary>
                /// Normally on, unless "special effect".
                /// </summary>
                public bool GammaDitherEnable
                {
                    get => Convert.ToBoolean(bits[gammaDitherEnable]);
                    set => bits[gammaDitherEnable] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Normally on, unless MPEG/JPEG.
                /// </summary>
                public bool GammaEnable
                {
                    get => Convert.ToBoolean(bits[gammaEnable]);
                    set => bits[gammaEnable] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Normally on if antialiased, unless decal lines.
                /// </summary>
                public bool DivotEnable
                {
                    get => Convert.ToBoolean(bits[divotEnable]);
                    set => bits[divotEnable] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Always on if interlaced, off if not.
                /// </summary>
                public bool Serrate
                {
                    get => Convert.ToBoolean(bits[serrate]);
                    set => bits[serrate] = Convert.ToInt32(value);
                }

                public AntiAliasingMode AntiAliasMode
                {
                    get => (AntiAliasingMode)bits[antiAliasMode];
                    set => bits[antiAliasMode] = (byte)value;
                }
                #endregion

                #region Operators
                public static implicit operator ControlRegister(ushort data) => new ControlRegister { bits = new BitVector32(data) };

                public static implicit operator ushort(ControlRegister status) => (ushort)status.bits.Data;
                #endregion

                #region Enumerations
                public enum PixelSize : byte
                {
                    /// <summary>No data, no sync.</summary>
                    Blank = 0,
                    Reserved = 1,
                    /// <summary>"16" bit.</summary>
                    RGBA5553 = 2,
                    /// <summary>32 bit.</summary>
                    RGBA8888 = 3
                }

                public enum AntiAliasingMode : byte
                {
                    /// <summary>AA &amp; resamp (always fetch extra lines).</summary>
                    AntiAliasAndResampleAlways = 0,
                    /// <summary>AA &amp; resamp (fetch extra lines if needed).</summary>
                    AntiAliasAndResampleIfNeeded = 1,
                    /// <summary>Resamp only (treat as all fully covered).</summary>
                    ResampleOnly = 2,
                    /// <summary>Neither (replicate pixels, no interpolate).</summary>
                    Neither = 3
                }
                #endregion
            }
        }
    }
}
