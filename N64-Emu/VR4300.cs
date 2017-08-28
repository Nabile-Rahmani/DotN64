namespace N64Emu
{
    // Reference: http://datasheets.chipdb.org/NEC/Vr-Series/Vr43xx/U10504EJ7V0UMJ1.pdf
    public partial class VR4300
    {
        #region Properties
        /// <summary>
        /// General purpose registers.
        /// </summary>
        public ulong[] GPRegisters { get; } = new ulong[32];

        /// <summary>
        /// Floating-point operation registers.
        /// </summary>
        public double[] FPRegisters { get; } = new double[32];

        public ulong ProgramCounter { get; set; }

        /// <summary>
        /// Integer multiply and divide high-order double word result.
        /// </summary>
        public ulong HIRegister { get; set; }

        /// <summary>
        /// Integer multiply and divide low-order double word result.
        /// </summary>
        public ulong LORegister { get; set; }

        /// <summary>
        /// Load/Link 1-bit register.
        /// </summary>
        public bool LLBitRegister { get; set; }

        /// <summary>
        /// Implementation/Revision register.
        /// </summary>
        /// <value>The FCR.</value>
        public float FCR0 { get; set; }

        /// <summary>
        /// Control/Status register.
        /// </summary>
        public float FCR31 { get; set; }

        public Coprocessor0 CP0 { get; } = new Coprocessor0();
        #endregion

        #region Methods
        public void PowerOnReset()
        {
            CP0.PowerOnReset();
        }
        #endregion
    }
}
