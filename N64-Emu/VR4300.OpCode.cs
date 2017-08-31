namespace N64Emu
{
    public partial class VR4300
    {
        public enum OpCode : byte
        {
            /// <summary>
            /// Load Upper Immediate.
            /// The 16-bit immediate is shifted left 16 bits and combined to 16 bits of zeros.
            /// The result is placed into general purpose register rt.
            /// In 64-bit mode, the loaded word is sign-extended to 64 bits.
            /// </summary>
            LUI = 0b001111,
            /// <summary>
            /// Move To System Control Coprocessor.
            /// The contents of general purpose register rt are loaded into general purpose register rd of CP0.
            /// </summary>
            MTC0 = 0b010000,
            /// <summary>
            /// Or Immediate.
            /// A logical OR operation applied between 16-bit zero-extended immediate and the contents of general purpose register rs is executed in bit units.
            /// The result is stored in general purpose register rt.
            /// </summary>
            ORI = 0b001101
        }
    }
}
