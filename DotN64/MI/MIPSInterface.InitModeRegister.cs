using System;
using System.Collections.Specialized;

namespace DotN64.MI
{
    public partial class MIPSInterface
    {
        public struct InitModeRegister
        {
            #region Fields
            private BitVector32 bits;

            public static readonly BitVector32.Section InitLengthSection = BitVector32.CreateSection((1 << 7) - 1),
            ClearInitModeSection = BitVector32.CreateSection(1, InitLengthSection),
            SetInitModeSection = BitVector32.CreateSection(1, ClearInitModeSection),
            ClearEbusTestModeSection = BitVector32.CreateSection(1, SetInitModeSection),
            SetEbusTestModeSection = BitVector32.CreateSection(1, ClearEbusTestModeSection),
            ClearDPInterruptSection = BitVector32.CreateSection(1, SetEbusTestModeSection),
            ClearRDRAMRegSection = BitVector32.CreateSection(1, ClearDPInterruptSection),
            SetRDRAMRegModeSection = BitVector32.CreateSection(1, ClearRDRAMRegSection);
            private static readonly BitVector32.Section initModeSection = BitVector32.CreateSection(1, InitLengthSection),
            ebusTestModeSection = BitVector32.CreateSection(1, initModeSection),
            rdramRegModeSection = BitVector32.CreateSection(1, ebusTestModeSection);
            #endregion

            #region Properties
            public byte InitLength
            {
                get => (byte)bits[InitLengthSection];
                set => bits[InitLengthSection] = value;
            }

            public bool InitMode
            {
                get => Convert.ToBoolean(bits[initModeSection]);
                set => bits[initModeSection] = Convert.ToInt32(value);
            }

            public bool EbusTestMode
            {
                get => Convert.ToBoolean(bits[ebusTestModeSection]);
                set => bits[ebusTestModeSection] = Convert.ToInt32(value);
            }

            public bool RDRAMRegMode
            {
                get => Convert.ToBoolean(bits[rdramRegModeSection]);
                set => bits[rdramRegModeSection] = Convert.ToInt32(value);
            }
            #endregion

            #region Operators
            public static implicit operator InitModeRegister(uint data) => new InitModeRegister { bits = new BitVector32((int)data) };

            public static implicit operator uint(InitModeRegister mode) => (uint)mode.bits.Data;
            #endregion
        }
    }
}
