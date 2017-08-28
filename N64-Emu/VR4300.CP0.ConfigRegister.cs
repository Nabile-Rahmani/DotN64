namespace N64Emu
{
    public partial class VR4300
    {
        public partial class CP0
        {
            public class ConfigRegister
            {
                #region Fields
                private readonly ulong[] registers;

                private const int EPShift = 24, EPSize = 4;
                private const int BEShift = 15, BESize = 1;
                #endregion

                #region Properties
                public EP ConfigEP
                {
                    get => (EP)(registers[(int)Register.Config] >> EPShift & EPSize);
                    set => registers[(int)Register.Config] |= ((ulong)value & EPSize) << EPShift;
                }

                public BE ConfigBE
                {
                    get => (BE)(registers[(int)Register.Config] >> BEShift & BESize);
                    set => registers[(int)Register.Config] |= ((ulong)value & BESize) << BEShift;
                }
                #endregion

                #region Constructors
                public ConfigRegister(ulong[] registers)
                {
                    this.registers = registers;
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
