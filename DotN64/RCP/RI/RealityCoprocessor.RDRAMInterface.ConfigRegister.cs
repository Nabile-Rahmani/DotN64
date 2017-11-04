using System;
using System.Collections.Specialized;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class RDRAMInterface
        {
            public struct ConfigRegister
            {
                #region Fields
                private BitVector32 bits;

                private static readonly BitVector32.Section currentControlInput = BitVector32.CreateSection((1 << 6) - 1),
                currentControlEnable = BitVector32.CreateSection(1, currentControlInput);
                #endregion

                #region Properties
                public byte CurrentControlInput
                {
                    get => (byte)bits[currentControlInput];
                    set => bits[currentControlInput] = value;
                }

                public bool CurrentControlEnable
                {
                    get => Convert.ToBoolean(bits[currentControlEnable]);
                    set => bits[currentControlEnable] = Convert.ToInt32(value);
                }
                #endregion

                #region Operators
                public static implicit operator ConfigRegister(uint data) => new ConfigRegister { bits = new BitVector32((int)data) };

                public static implicit operator uint(ConfigRegister register) => (uint)register.bits.Data;
                #endregion
            }
        }
    }
}
