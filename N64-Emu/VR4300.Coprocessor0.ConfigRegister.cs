namespace N64Emu
{
    public partial class VR4300
    {
        public partial class Coprocessor0
        {
            public class ConfigRegister : Register
            {
                #region Fields
                private const int K0Shift = 0, K0Size = (1 << 3) - 1;
                private const int CUShift = 3, CUSize = (1 << 1) - 1;
                private const int BEShift = 15, BESize = (1 << 1) - 1;
                private const int EPShift = 24, EPSize = (1 << 4) - 1;
                private const int ECShift = 28, ECSize = (1 << 3) - 1;
                #endregion

                #region Properties
                protected override RegisterIndex Index => RegisterIndex.Config;

                /// <summary>
                /// Sets coherency algorithm of kseg0.
                /// </summary>
                public K0 ConfigK0
                {
                    get => (K0)GetRegister(K0Shift, K0Size);
                    set => SetRegister(K0Shift, K0Size, (ulong)value);
                }

                /// <summary>
                /// RFU. However, can be read or written by software.
                /// </summary>
                public bool CU
                {
                    get => GetBoolean(CUShift, CUSize);
                    set => SetRegister(CUShift, CUSize, value);
                }

                /// <summary>
                /// Sets BigEndianMem (endianness).
                /// </summary>
                public BE ConfigBE
                {
                    get => (BE)GetRegister(BEShift, BESize);
                    set => SetRegister(BEShift, BESize, (ulong)value);
                }

                /// <summary>
                /// Sets transfer data pattern (single/block write request).
                /// </summary>
                public EP ConfigEP
                {
                    get => (EP)GetRegister(EPShift, EPSize);
                    set => SetRegister(EPShift, EPSize, (ulong)value);
                }

                /// <summary>
                /// Operating frequency ratio (read-only). The value displayed corresponds to the frequency ratio set by the DivMode pins on power application.
                /// </summary>
                public byte EC
                {
                    get => (byte)GetRegister(ECShift, ECSize);
                    set => SetRegister(ECShift, ECSize, value);
                }
                #endregion

                #region Constructors
                public ConfigRegister(Coprocessor0 cp0)
                    : base(cp0)
                {
                    SetRegister(4, (1 << 11) - 1, 0b11001000110);
                    SetRegister(16, (1 << 8) - 1, 0b00000110);
                    SetRegister(31, (1 << 1) - 1, 0);
                }
                #endregion

                #region Enumerations
                public enum K0 : byte
                {
                    UnusedCache = 0b010
                }

                public enum BE : byte
                {
                    LittleEndian,
                    BigEndian
                }

                public enum EP : byte
                {
                    D = 0,
                    DxxDxx = 6
                }
                #endregion
            }
        }
    }
}
