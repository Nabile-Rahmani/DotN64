namespace N64Emu
{
    public partial class VR4300
    {
        private enum OpCode : byte
        {
            /// <summary>
            /// Load Upper Immediate.
            /// The 16-bit immediate is shifted left 16 bits and combined to 16 bits of zeros.
            /// The result is placed into general purpose register rt.
            /// In 64-bit mode, the loaded word is sign-extended to 64 bits.
            /// </summary>
            LUI = 0b001111
        }
    }
}
