namespace N64Emu.Interfaces.Peripheral
{
    public partial class PeripheralInterface
    {
        public class StatusRegister
        {
            #region Properties
            private byte data;
            public byte Data
            {
                get => data;
                set
                {
                    if ((value & 1) != 0)
                        ResetController();

                    if ((value >> 1 & 1) != 0)
                        ClearInterrupt();
                }
            }

            public bool DMABusy
            {
                get => Get(0);
                set => Set(0, value);
            }

            public bool IOBusy
            {
                get => Get(1);
                set => Set(1, value);
            }

            public bool Error
            {
                get => Get(2);
                set => Set(2, value);
            }
            #endregion

            #region Methods
            private bool Get(int shift) => (Data >> shift & 1) != 0;

            private void Set(int shift, bool value)
            {
                data &= (byte)~((1 << shift) - 1);
                data |= (byte)((value ? 1 : 0) << shift);
            }

            private void ResetController() { }

            private void ClearInterrupt() { }
            #endregion
        }
    }
}
