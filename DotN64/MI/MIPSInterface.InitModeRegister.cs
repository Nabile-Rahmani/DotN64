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
            ClearEBusTestModeSection = BitVector32.CreateSection(1, SetInitModeSection),
            SetEBusTestModeSection = BitVector32.CreateSection(1, ClearEBusTestModeSection),
            ClearDPInterruptSection = BitVector32.CreateSection(1, SetEBusTestModeSection),
            ClearRDRAMRegSection = BitVector32.CreateSection(1, ClearDPInterruptSection),
            SetRDRAMRegModeSection = BitVector32.CreateSection(1, ClearRDRAMRegSection);
            private static readonly BitVector32.Section initModeSection = BitVector32.CreateSection(1, InitLengthSection),
            eBusTestModeSection = BitVector32.CreateSection(1, initModeSection),
            rdramRegModeSection = BitVector32.CreateSection(1, eBusTestModeSection);
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

            public bool EBusTestMode
            {
                get => Convert.ToBoolean(bits[eBusTestModeSection]);
                set => bits[eBusTestModeSection] = Convert.ToInt32(value);
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
