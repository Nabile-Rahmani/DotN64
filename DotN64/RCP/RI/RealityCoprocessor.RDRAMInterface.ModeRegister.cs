using System;
using System.Collections.Specialized;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class RDRAMInterface
        {
            public struct ModeRegister
            {
                #region Fields
                private BitVector32 bits;

                private static readonly BitVector32.Section operatingMode = BitVector32.CreateSection((1 << 2) - 1),
                stopTActive = BitVector32.CreateSection(1, operatingMode),
                stopRActive = BitVector32.CreateSection(1, stopTActive);
                #endregion

                #region Properties
                public byte OperatingMode
                {
                    get => (byte)bits[operatingMode];
                    set => bits[operatingMode] = value;
                }

                public bool StopTActive
                {
                    get => Convert.ToBoolean(bits[stopTActive]);
                    set => bits[stopTActive] = Convert.ToInt32(value);
                }

                public bool StopRActive
                {
                    get => Convert.ToBoolean(bits[stopRActive]);
                    set => bits[stopRActive] = Convert.ToInt32(value);
                }
                #endregion

                #region Operators
                public static implicit operator ModeRegister(uint data) => new ModeRegister { bits = new BitVector32((int)data) };

                public static implicit operator uint(ModeRegister register) => (uint)register.bits.Data;
                #endregion
            }
        }
    }
}
