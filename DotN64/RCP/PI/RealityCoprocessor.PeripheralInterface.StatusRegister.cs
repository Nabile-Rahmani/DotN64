using System;

namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class PeripheralInterface
        {
            [Flags]
            public enum StatusRegister
            {
                DMABusy = 1 << 0,
                IOBusy = 1 << 1,
                Error = 1 << 2
            }

            [Flags]
            private enum WriteStatusRegister : byte
            {
                ResetController = 1 << 0,
                ClearInterrupt = 1 << 1
            }
        }
    }
}
