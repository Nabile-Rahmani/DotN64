using System;
using System.Collections.Specialized;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class RDRAMInterface
        {
            public struct RefreshRegister
            {
                #region Fields
                private BitVector32 bits;

                private static readonly BitVector32.Section cleanRefreshDelaySection = BitVector32.CreateSection((1 << 8) - 1),
                dirtyRefreshDelaySection = BitVector32.CreateSection((1 << 8) - 1, cleanRefreshDelaySection),
                refreshBankSection = BitVector32.CreateSection(1, dirtyRefreshDelaySection),
                refreshEnableSection = BitVector32.CreateSection(1, refreshBankSection),
                refreshOptimiseSection = BitVector32.CreateSection(1, refreshEnableSection);
                #endregion

                #region Properties
                public byte CleanRefreshDelay
                {
                    get => (byte)bits[cleanRefreshDelaySection];
                    set => bits[cleanRefreshDelaySection] = value;
                }

                public byte DirtyRefreshDelay
                {
                    get => (byte)bits[dirtyRefreshDelaySection];
                    set => bits[dirtyRefreshDelaySection] = value;
                }

                public bool RefreshBank
                {
                    get => Convert.ToBoolean(bits[refreshBankSection]);
                    set => bits[refreshBankSection] = Convert.ToInt32(value);
                }

                public bool RefreshEnable
                {
                    get => Convert.ToBoolean(bits[refreshEnableSection]);
                    set => bits[refreshEnableSection] = Convert.ToInt32(value);
                }

                public bool RefreshOptimise
                {
                    get => Convert.ToBoolean(bits[refreshOptimiseSection]);
                    set => bits[refreshOptimiseSection] = Convert.ToInt32(value);
                }
                #endregion

                #region Operators
                public static implicit operator RefreshRegister(uint data) => new RefreshRegister { bits = new BitVector32((int)data) };

                public static implicit operator uint(RefreshRegister register) => (uint)register.bits.Data;
                #endregion
            }
        }
    }
}
