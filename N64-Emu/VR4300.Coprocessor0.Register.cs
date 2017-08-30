namespace N64Emu
{
    public partial class VR4300
    {
        public partial class Coprocessor0
        {
            public abstract class Register
            {
                #region Fields
                private readonly Coprocessor0 cp0;
                #endregion

                #region Properties
                protected abstract RegisterIndex Index { get; }
                #endregion

                #region Constructors
                protected Register(Coprocessor0 cp0)
                {
                    this.cp0 = cp0;
                }
                #endregion

                #region Methods
                protected ulong GetRegister(int shift, ulong size) => cp0.Registers[(int)Index] >> shift & size;

                protected bool GetBoolean(int shift, ulong size) => GetRegister(shift, size) != 0;

                protected void SetRegister(int shift, ulong size, ulong value)
                {
                    cp0.Registers[(int)Index] &= ~(size << shift);
                    cp0.Registers[(int)Index] |= (value & size) << shift;
                }

                protected void SetRegister(int shift, ulong size, bool value) => SetRegister(shift, size, value ? size : 0);
                #endregion
            }
        }
    }
}
