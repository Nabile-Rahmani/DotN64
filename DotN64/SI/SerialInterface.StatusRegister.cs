using System;
using System.Collections.Specialized;

namespace DotN64.SI
{
    public partial class SerialInterface
    {
        public class StatusRegister
        {
            #region Fields
            private static readonly BitVector32.Section dmaBusy = BitVector32.CreateSection(1),
            ioReadBusy = BitVector32.CreateSection(1, dmaBusy),
            reserved = BitVector32.CreateSection(1, ioReadBusy),
            dmaError = BitVector32.CreateSection(1, reserved),
            unknown1 = BitVector32.CreateSection((1 << 8) - 1, dmaError),
            interrupt = BitVector32.CreateSection(1, unknown1);
            #endregion

            #region Properties
            private BitVector32 bits;
            public BitVector32 Bits => bits;

            public bool DMABusy
            {
                get => Convert.ToBoolean(bits[dmaBusy]);
                set => bits[dmaBusy] = Convert.ToInt32(value);
            }

            public bool IOReadBusy
            {
                get => Convert.ToBoolean(bits[ioReadBusy]);
                set => bits[ioReadBusy] = Convert.ToInt32(value);
            }

            public bool DMAError
            {
                get => Convert.ToBoolean(bits[dmaError]);
                set => bits[dmaError] = Convert.ToInt32(value);
            }

            public bool Interrupt
            {
                get => Convert.ToBoolean(bits[interrupt]);
                set => bits[interrupt] = Convert.ToInt32(value);
            }
            #endregion
        }
    }
}
