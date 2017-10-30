using System.Collections.Specialized;

namespace DotN64.PI
{
    public partial class PeripheralInterface
    {
        public struct StatusRegister
        {
            #region Fields
            private BitVector32 bits;

            private static readonly int dmaBusy = BitVector32.CreateMask(),
            ioBusy = BitVector32.CreateMask(dmaBusy),
            error = BitVector32.CreateMask(ioBusy);
            public static readonly int ResetControllerMask = BitVector32.CreateMask(),
            ClearInterruptMask = BitVector32.CreateMask(ResetControllerMask);
            #endregion

            #region Properties
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

            #region Operators
            public static implicit operator StatusRegister(uint data) => new StatusRegister { bits = new BitVector32((int)data) };

            public static implicit operator uint(StatusRegister register) => (uint)register.bits.Data;
            #endregion
        }
    }
}
