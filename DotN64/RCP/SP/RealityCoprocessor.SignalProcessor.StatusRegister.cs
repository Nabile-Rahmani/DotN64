using System;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class SignalProcessor
        {
            [Flags]
            public enum StatusRegister : ushort
            {
                Halt = 1 << 0,
                Broke = 1 << 1,
                DMABusy = 1 << 2,
                DMAFull = 1 << 3,
                IOFull = 1 << 4,
                SingleStep = 1 << 5,
                InterruptOnBreak = 1 << 6,
                Signal0 = 1 << 7,
                Signal1 = 1 << 8,
                Signal2 = 1 << 9,
                Signal3 = 1 << 10,
                Signal4 = 1 << 11,
                Signal5 = 1 << 12,
                Signal6 = 1 << 13,
                Signal7 = 1 << 14
            }

            [Flags]
            private enum WriteStatusRegister
            {
                ClearHalt = 1 << 0,
                SetHalt = 1 << 1,
                ClearBroke = 1 << 2,
                ClearInterrupt = 1 << 3,
                SetInterrupt = 1 << 4,
                ClearSingleStep = 1 << 5,
                SetSingleStep = 1 << 6,
                ClearInterruptOnBreak = 1 << 7,
                SetInterruptOnBreak = 1 << 8,
                ClearSignal0 = 1 << 9,
                SetSignal0 = 1 << 10,
                ClearSignal1 = 1 << 11,
                SetSignal1 = 1 << 12,
                ClearSignal2 = 1 << 13,
                SetSignal2 = 1 << 14,
                ClearSignal3 = 1 << 15,
                SetSignal3 = 1 << 16,
                ClearSignal4 = 1 << 17,
                SetSignal4 = 1 << 18,
                ClearSignal5 = 1 << 19,
                SetSignal5 = 1 << 20,
                ClearSignal6 = 1 << 21,
                SetSignal6 = 1 << 22,
                ClearSignal7 = 1 << 23,
                SetSignal7 = 1 << 24,
            }
        }
    }
}
