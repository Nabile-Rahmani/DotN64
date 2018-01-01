using System;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class MIPSInterface
        {
            [Flags]
            public enum Interrupts : byte
            {
                SP = 1 << 0,
                SI = 1 << 1,
                AI = 1 << 2,
                VI = 1 << 3,
                PI = 1 << 4,
                DP = 1 << 5
            }

            [Flags]
            private enum InterruptMaskWrites : ushort
            {
                ClearSP = 1 << 0,
                SetSP = 1 << 1,
                ClearSI = 1 << 2,
                SetSI = 1 << 3,
                ClearAI = 1 << 4,
                SetAI = 1 << 5,
                ClearVI = 1 << 6,
                SetVI = 1 << 7,
                ClearPI = 1 << 8,
                SetPI = 1 << 9,
                ClearDP = 1 << 10,
                SetDP = 1 << 11
            }
        }
    }
}
