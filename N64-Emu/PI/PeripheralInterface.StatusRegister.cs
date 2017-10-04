using System.Collections.Specialized;

namespace N64Emu.PI
{
    public partial class PeripheralInterface
    {
        public class StatusRegister
        {
            #region Fields
            private static readonly int dmaBusy = BitVector32.CreateMask(),
            ioBusy = BitVector32.CreateMask(dmaBusy),
            error = BitVector32.CreateMask(ioBusy);
            public static readonly int ResetControllerMask = BitVector32.CreateMask(),
            ClearInterruptMask = BitVector32.CreateMask(ResetControllerMask);
            #endregion

            #region Properties
            private BitVector32 bits;
            public BitVector32 Bits => bits;

            public bool DMABusy
            {
                get => bits[dmaBusy];
                set => bits[dmaBusy] = value;
            }

            public bool IOBusy
            {
                get => bits[ioBusy];
                set => bits[ioBusy] = value;
            }

            public bool Error
            {
                get => bits[error];
                set => bits[error] = value;
            }
            #endregion
        }
    }
}
