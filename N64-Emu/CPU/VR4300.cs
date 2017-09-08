using System;
using System.Collections.Generic;
using System.Linq;

namespace N64Emu.CPU
{
    using static Helpers.BitHelper;

    // Reference: http://datasheets.chipdb.org/NEC/Vr-Series/Vr43xx/U10504EJ7V0UMJ1.pdf
    public partial class VR4300
    {
        #region Fields
        private readonly Nintendo64 nintendo64;
        private readonly IReadOnlyDictionary<OpCode, Action<Instruction>> operations;
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

        public SystemControlUnit CP0 { get; } = new SystemControlUnit();
        #endregion

        #region Constructors
        public VR4300(Nintendo64 nintendo64)
        {
            this.nintendo64 = nintendo64;
            operations = new Dictionary<OpCode, Action<Instruction>>
            {
                [OpCode.LUI] = i => GPRegisters[i.RT] = (ulong)(i.Immediate << 16),
                [OpCode.MTC0] = i => CP0.Registers[i.RD] = GPRegisters[i.RT],
                [OpCode.ORI] = i => GPRegisters[i.RT] = GPRegisters[i.RS] | i.Immediate,
                [OpCode.LW] = i => GPRegisters[i.RT] = SignExtend(ReadWord(SignExtend(i.Immediate) + GPRegisters[i.RS])),
                [OpCode.ANDI] = i => GPRegisters[i.RT] = (ulong)(i.Immediate & (ushort)GPRegisters[i.RS]),
                [OpCode.BEQL] = i => BranchLikely(i, (rs, rt) => rs == rt),
                [OpCode.ADDIU] = i => GPRegisters[i.RT] = GPRegisters[i.RS] + SignExtend(i.Immediate),
                [OpCode.SW] = i => WriteWord(SignExtend(i.Immediate) + GPRegisters[i.RS], (uint)GPRegisters[i.RT]),
                [OpCode.BNEL] = i => BranchLikely(i, (rs, rt) => rs != rt),
                [OpCode.BNE] = i => Branch(i, (rs, rt) => rs != rt),
                [OpCode.ADD] = i => GPRegisters[i.RD] = GPRegisters[i.RS] + GPRegisters[i.RT], // Should we discard the upper word and extend the lower one ?
                [OpCode.BEQ] = i => Branch(i, (rs, rt) => rs == rt)
            };
        }
        #endregion

        #region Methods
        public void PowerOnReset()
        {
            ProgramCounter = 0xFFFFFFFFBFC00000;

            CP0.PowerOnReset();
        }

        public void Run(Instruction instruction)
        {
            if (operations.TryGetValue(instruction.OP, out var operation))
                operation(instruction);
            else
                throw new Exception($"Unknown opcode (0b{Convert.ToString((byte)instruction.OP, 2)}) from instruction 0x{(uint)instruction:X}.");
        }

        public void Step()
        {
            var instruction = ReadWord(ProgramCounter);
            ProgramCounter += sizeof(uint);

            Run(instruction);
        }

        private void Branch(Instruction instruction, Func<ulong, ulong, bool> condition)
        {
            if (condition(GPRegisters[instruction.RS], GPRegisters[instruction.RT]))
                ProgramCounter += (ulong)((long)(short)instruction.Immediate & ~((1 << 18) - 1) | (long)instruction.Immediate << 2);
        }

        private void BranchLikely(Instruction instruction, Func<ulong, ulong, bool> condition)
        {
            var delaySlotInstruction = ReadWord(ProgramCounter);

            if (condition(GPRegisters[instruction.RS], GPRegisters[instruction.RT]))
            {
                ProgramCounter += (ulong)((long)(short)instruction.Immediate & ~((1 << 18) - 1) | (long)instruction.Immediate << 2);

                Run(delaySlotInstruction);
            }
            else
                ProgramCounter += sizeof(uint);
        }

        private uint ReadWord(ulong virtualAddress)
        {
            var physicalAddress = MapMemory(virtualAddress);
            var entry = nintendo64.MemoryMaps.FirstOrDefault(e => e.Contains(physicalAddress));

            if (entry.Contains(physicalAddress))
                return entry.ReadWord(physicalAddress);

            throw new Exception($"Unknown physical address: 0x{(uint)physicalAddress:X}.");
        }

        private void WriteWord(ulong virtualAddress, uint value)
        {
            var physicalAddress = MapMemory(virtualAddress);
            var entry = nintendo64.MemoryMaps.FirstOrDefault(e => e.Contains(physicalAddress));

            if (!entry.Contains(physicalAddress))
                throw new Exception($"Unknown physical address: 0x{(uint)physicalAddress:X}.");

            entry.WriteWord(physicalAddress, value);
        }

        /// <summary>
        /// Translates a virtual address into a physical address.
        /// See: datasheet#5.2.4 Table 5-3.
        /// </summary>
        /// <param name="address">The virtual address.</param>
        /// <returns>The physical address.</returns>
        public ulong MapMemory(ulong address) // TODO: move to an MMU, and CP0 relations ?
        {
            switch (address >> 29 & 0b111)
            {
                case 0b100: // kseg0.
                    return address - 0xFFFFFFFF80000000;
                case 0b101: // kseg1.
                    return address - 0xFFFFFFFFA0000000;
                default:
                    throw new Exception($"Unknown memory map segment for location 0x{address:X}.");
            }
        }
        #endregion
    }
}
