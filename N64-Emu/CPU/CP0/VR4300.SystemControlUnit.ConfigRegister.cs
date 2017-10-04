using System;
using System.Collections.Specialized;

namespace N64Emu.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            public class ConfigRegister : Register
            {
                #region Fields
                private static readonly BitVector32.Section k0 = BitVector32.CreateSection((1 << 3) - 1),
                cu = BitVector32.CreateSection(1, k0),
                constant1 = BitVector32.CreateSection((1 << 11) - 1, cu),
                be = BitVector32.CreateSection(1, constant1),
                constant2 = BitVector32.CreateSection((1 << 8) - 1, be),
                ep = BitVector32.CreateSection((1 << 4) - 1, constant2),
                ec = BitVector32.CreateSection((1 << 3) - 1, ep),
                constant3 = BitVector32.CreateSection(1, ec);
                #endregion

                #region Properties
                protected override RegisterIndex Index => RegisterIndex.Config;

                /// <summary>
                /// Sets coherency algorithm of kseg0.
                /// </summary>
                public CoherencyAlgorithm K0
                {
                    get => (CoherencyAlgorithm)this[k0];
                    set => this[k0] = (int)value;
                }

                /// <summary>
                /// RFU. However, can be read or written by software.
                /// </summary>
                public bool CU
                {
                    get => Convert.ToBoolean(this[cu]);
                    set => this[cu] = Convert.ToInt32(value);
                }

                /// <summary>
                /// Sets BigEndianMem (endianness).
                /// </summary>
                public Endianness BE
                {
                    get => (Endianness)this[be];
                    set => this[be] = (int)value;
                }

                /// <summary>
                /// Sets transfer data pattern (single/block write request).
                /// </summary>
                public TransferDataPattern EP
                {
                    get => (TransferDataPattern)this[ep];
                    set => this[ep] = (int)value;
                }

                /// <summary>
                /// Operating frequency ratio (read-only). The value displayed corresponds to the frequency ratio set by the DivMode pins on power application.
                /// </summary>
                public byte EC
                {
                    get => (byte)this[ec];
                    set => this[ec] = value;
                }
                #endregion

                #region Constructors
                public ConfigRegister(SystemControlUnit cp0)
                    : base(cp0)
                {
                    this[constant1] = 0b11001000110;
                    this[constant2] = 0b00000110;
                    this[constant3] = 0;
                }
                #endregion

                #region Enumerations
                public enum CoherencyAlgorithm : byte
                {
                    CacheUnused = 0b010
                }

                public enum Endianness : byte
                {
                    LittleEndian,
                    BigEndian
                }

                public enum TransferDataPattern : byte
                {
                    D = 0,
                    DxxDxx = 6
                }
                #endregion
            }
        }
    }
}
