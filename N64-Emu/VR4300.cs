using System;
using System.Net;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong SignExtend(ushort value) => (ulong)(short)value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong SignExtend(uint value) => (ulong)(int)value;

        public void Run(Instruction instruction)
        {
            switch (instruction.OP)
            {
                case OpCode.LUI:
                    GPRegisters[instruction.RT] = (ulong)(instruction.Immediate << 16);
                    break;
                case OpCode.MTC0:
                    CP0.Registers[instruction.RD] = GPRegisters[instruction.RT];
                    break;
                case OpCode.ORI:
                    GPRegisters[instruction.RT] = GPRegisters[instruction.RS] | instruction.Immediate;
                    break;
                case OpCode.LW: // 'offset' is Immediate, 'base' is RS.
                    var vAddr = SignExtend(instruction.Immediate) + GPRegisters[instruction.RS];
                    GPRegisters[instruction.RT] = SignExtend(ReadWord(new UIntPtr(vAddr)));
                    break;
                default:
                    throw new Exception($"Unknown opcode (0b{Convert.ToString((byte)instruction.OP, 2)}) from instruction 0x{(uint)instruction:x}.");
            }
        }

        public void Step()
        {
            Run(ReadWord(new UIntPtr(ProgramCounter)));

            ProgramCounter += sizeof(uint);
        }

        private uint ReadWord(UIntPtr virtualAddress)
        {
            var physicalAddress = MapMemory(virtualAddress);

            if ((uint)physicalAddress >= 0x1FC00000 && (uint)physicalAddress <= 0x1FC007BF) // TODO: define consts.
                throw new NotImplementedException("PIF ROM access is not supported.");

            return (uint)IPAddress.HostToNetworkOrder(BitConverter.ToInt32(nintendo64.RAM, (int)physicalAddress)); // Use a binary stream extension package for byte swapping (with runtime endianness check) ?
        }

        /// <summary>
        /// Translates a virtual address into a physical address.
        /// See: datasheet#5.2.4 Table 5-3.
        /// </summary>
        /// <param name="address">The virtual address.</param>
        /// <returns>The physical address.</returns>
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
