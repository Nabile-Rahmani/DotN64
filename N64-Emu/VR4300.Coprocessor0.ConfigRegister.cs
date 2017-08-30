namespace N64Emu
{
    public partial class VR4300
    {
        public partial class Coprocessor0
        {
            public class ConfigRegister
            {
                #region Fields
                private readonly Coprocessor0 cp0;

                private const int EPShift = 24, EPSize = 4;
                private const int BEShift = 15, BESize = 1;
                #endregion

                #region Properties
                public EP ConfigEP
                {
                    get => (EP)(cp0.Registers[(int)Register.Config] >> EPShift & EPSize);
                    set
                    {
                        cp0.Registers[(int)Register.Config] &= ~(ulong)(EPSize << EPShift);
                        cp0.Registers[(int)Register.Config] |= ((ulong)value & EPSize) << EPShift;
                    }
                }

                public BE ConfigBE
                {
                    get => (BE)(cp0.Registers[(int)Register.Config] >> BEShift & BESize);
                    set
                    {
                        cp0.Registers[(int)Register.Config] &= ~(ulong)(BESize << BEShift);
                        cp0.Registers[(int)Register.Config] |= ((ulong)value & BESize) << BEShift;
                    }
                }
                #endregion

                #region Constructors
                public ConfigRegister(Coprocessor0 cp0)
                {
                    this.cp0 = cp0;
                }
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
