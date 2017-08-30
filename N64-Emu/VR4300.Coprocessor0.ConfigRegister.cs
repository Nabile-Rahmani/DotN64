namespace N64Emu
{
    public partial class VR4300
    {
        public partial class Coprocessor0
        {
            public class ConfigRegister : Register
            {
                #region Fields
                private const int EPShift = 24, EPSize = 4;
                private const int BEShift = 15, BESize = 1;
                #endregion

                #region Properties
                protected override RegisterIndex Index => RegisterIndex.Config;

                public EP ConfigEP
                {
                    get => (EP)GetRegister(EPShift, EPSize);
                    set => SetRegister(EPShift, EPSize, (ulong)value);
                }

                public BE ConfigBE
                {
                    get => (BE)GetRegister(BEShift, BESize);
                    set => SetRegister(BEShift, BESize, (ulong)value);
                }
                #endregion

                #region Constructors
                public ConfigRegister(Coprocessor0 cp0)
                    : base(cp0) { }
                #endregion

                #region Enumerations
                public enum EP : byte
                {
                    D = 0,
                    DxxDxx = 6,
                    RFU
                }

                public enum BE : byte
                {
                    LittleEndian,
                    BigEndian
                }
                #endregion
            }
        }
    }
}
