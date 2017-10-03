namespace N64Emu.SI
{
    public partial class SerialInterface
    {
        public class StatusRegister
        {
            #region Properties
            private uint data;
            public uint Data
            {
                get => data;
                set => Interrupt = false;
            }

            public bool DMABusy
            {
                get => Get(0);
                set => Set(0, value);
            }

            public bool IOReadBusy
            {
                get => Get(1);
                set => Set(1, value);
            }

            public bool DMAError
            {
                get => Get(3);
                set => Set(3, value);
            }

            public bool Interrupt
            {
                get => Get(12);
                set => Set(12, value);
            }
            #endregion

            #region Methods
            private bool Get(int shift) => (Data >> shift & 1) != 0;

            private void Set(int shift, bool value)
            {
                data &= (byte)~((1 << shift) - 1);
                data |= (byte)((value ? 1 : 0) << shift);
            }
            #endregion
        }
    }
}
