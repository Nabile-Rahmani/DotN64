using System;
using System.Diagnostics.Contracts;

namespace N64Emu
{
    // Reference: http://datasheets.chipdb.org/NEC/Vr-Series/Vr43xx/U10504EJ7V0UMJ1.pdf
    public partial class VR4300
    {
        #region Fields
        private readonly Nintendo64 nintendo64;
        #endregion

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

        #region Constructors
        public VR4300(Nintendo64 nintendo64)
        {
            this.nintendo64 = nintendo64;
        }
        #endregion

        #region Methods
        public void PowerOnReset()
        {
            ProgramCounter = 0xFFFFFFFFBFC00000;

            CP0.PowerOnReset();
        }

        public void Step()
        {
            var instruction = ReadWord(new UIntPtr(ProgramCounter));
            var opCode = (OpCode)(instruction >> 26);

            switch (opCode)
            {
                case OpCode.LUI:
                    var rt = instruction >> 16 & 0b11111;
                    var immediate = instruction & 0xFFFF;
                    GPRegisters[rt] = (ulong)(immediate << 16); // TODO: sign extend for 64-bit mode.
                    break;
                default:
                    throw new Exception($"Unknown opcode (0b{Convert.ToString((byte)opCode, 2)}) from instruction 0x{instruction:x}.");
            }

            ProgramCounter += sizeof(int);
        }

        private int ReadWord(UIntPtr virtualAddress)
        {
            var physicalAddress = MapMemory(virtualAddress);

            if ((uint)physicalAddress >= 0x1FC00000 && (uint)physicalAddress <= 0x1FC007BF) // TODO: define consts.
                throw new NotImplementedException("PIF ROM access is not supported.");

            return BitConverter.ToInt32(nintendo64.RAM, (int)physicalAddress);
        }

        /// <summary>
        /// Translates a virtual address into a physical address.
        /// See: datasheet#5.2.4 Table 5-3.
        /// </summary>
        public UIntPtr MapMemory(UIntPtr address) // TODO: move to an MMU, and CP0 relations ?
        {
            switch ((ulong)address >> 29 & 0b111)
            {
                case 0b100: // kseg0.
                    return new UIntPtr((ulong)address - 0xFFFFFFFF80000000);
                case 0b101: // kseg1.
                    return new UIntPtr((ulong)address - 0xFFFFFFFFA0000000);
                default:
                    throw new Exception($"Unknown memory map segment for location 0x{(ulong)address:x}.");
            }
        }
        #endregion
    }
}
