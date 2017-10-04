using System;
using System.Collections.Generic;

namespace N64Emu.CPU
{
    // Reference: http://datasheets.chipdb.org/NEC/Vr-Series/Vr43xx/U10504EJ7V0UMJ1.pdf
    public partial class VR4300
    {
        #region Fields
        private readonly IReadOnlyDictionary<OpCode, Action<Instruction>> operations;
        private readonly IReadOnlyDictionary<SpecialOpCode, Action<Instruction>> specialOperations;
        private readonly IReadOnlyDictionary<RegImmOpCode, Action<Instruction>> regImmOperations;

        private ulong? delaySlot;
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

        public SystemControlUnit CP0 { get; }
        #endregion

        #region Constructors
        public VR4300(IReadOnlyList<MappingEntry> memoryMaps)
        {
            CP0 = new SystemControlUnit(memoryMaps);
            operations = new Dictionary<OpCode, Action<Instruction>>
            {
                [OpCode.SPECIAL] = i =>
                {
                    if (specialOperations.TryGetValue(i.SpecialOP, out var operation))
                        operation(i);
                    else
                        throw new Exception($"Unknown special opcode (0b{Convert.ToString((byte)i.SpecialOP, 2)}) from instruction 0x{(uint)i:X}.");
                },
                [OpCode.REGIMM] = i =>
                {
                    if (regImmOperations.TryGetValue(i.RegImmOP, out var operation))
                        operation(i);
                    else
                        throw new Exception($"Unknown reg imm opcode (0b{Convert.ToString((byte)i.RegImmOP, 2)}) from instruction 0x{(uint)i:X}.");
                },
                [OpCode.LUI] = i => GPRegisters[i.RT] = (ulong)(i.Immediate << 16),
                [OpCode.MTC0] = i => CP0.Registers[i.RD] = GPRegisters[i.RT],
                [OpCode.ORI] = i => GPRegisters[i.RT] = GPRegisters[i.RS] | i.Immediate,
                [OpCode.LW] = i => GPRegisters[i.RT] = (ulong)(int)(ReadWord((ulong)(short)i.Immediate + GPRegisters[i.RS])),
                [OpCode.ANDI] = i => GPRegisters[i.RT] = GPRegisters[i.RS] & i.Immediate,
                [OpCode.BEQL] = i => BranchLikely(i, (rs, rt) => rs == rt),
                [OpCode.ADDIU] = i => GPRegisters[i.RT] = GPRegisters[i.RS] + (ulong)(short)i.Immediate,
                [OpCode.SW] = i => WriteWord((ulong)(short)i.Immediate + GPRegisters[i.RS], (uint)GPRegisters[i.RT]),
                [OpCode.BNEL] = i => BranchLikely(i, (rs, rt) => rs != rt),
                [OpCode.BNE] = i => Branch(i, (rs, rt) => rs != rt),
                [OpCode.BEQ] = i => Branch(i, (rs, rt) => rs == rt),
                [OpCode.ADDI] = i => GPRegisters[i.RT] = GPRegisters[i.RS] + (ulong)(short)i.Immediate
            };
            specialOperations = new Dictionary<SpecialOpCode, Action<Instruction>>
            {
                [SpecialOpCode.ADD] = i => GPRegisters[i.RD] = GPRegisters[i.RS] + GPRegisters[i.RT], // Should we discard the upper word and extend the lower one ?
                [SpecialOpCode.JR] = i =>
                {
                    delaySlot = ProgramCounter;
                    ProgramCounter = GPRegisters[i.RS];
                },
                [SpecialOpCode.SRL] = i => GPRegisters[i.RD] = (ulong)(int)(GPRegisters[i.RT] >> i.SA),
                [SpecialOpCode.OR] = i => GPRegisters[i.RD] = GPRegisters[i.RS] | GPRegisters[i.RT],
                [SpecialOpCode.MULTU] = i =>
                {
                    var result = (uint)GPRegisters[i.RS] * (uint)GPRegisters[i.RT];
                    LORegister = (ulong)(int)result;
                    HIRegister = (ulong)(int)(result >> 32);
                },
                [SpecialOpCode.MFLO] = i => GPRegisters[i.RD] = LORegister,
                [SpecialOpCode.SLL] = i => GPRegisters[i.RD] = (ulong)(int)(GPRegisters[i.RT] << i.SA),
                [SpecialOpCode.SUBU] = i => GPRegisters[i.RD] = (ulong)(int)(GPRegisters[i.RS] - GPRegisters[i.RT]),
                [SpecialOpCode.XOR] = i => GPRegisters[i.RD] = GPRegisters[i.RS] ^ GPRegisters[i.RT],
                [SpecialOpCode.MFHI] = i => GPRegisters[i.RD] = HIRegister,
                [SpecialOpCode.ADDU] = i => GPRegisters[i.RD] = (ulong)(int)(GPRegisters[i.RS] + GPRegisters[i.RT]),
                [SpecialOpCode.SLTU] = i => GPRegisters[i.RD] = (ulong)(GPRegisters[i.RS] < GPRegisters[i.RT] ? 1 : 0),
                [SpecialOpCode.SLLV] = i => GPRegisters[i.RD] = (ulong)(int)(GPRegisters[i.RT] << (int)(GPRegisters[i.RS] & ((1 << 5) - 1))),
                [SpecialOpCode.SRLV] = i => GPRegisters[i.RD] = (ulong)(int)(GPRegisters[i.RT] >> (int)(GPRegisters[i.RS] & ((1 << 5) - 1))),
                [SpecialOpCode.AND] = i => GPRegisters[i.RD] = GPRegisters[i.RS] & GPRegisters[i.RT]
            };
            regImmOperations = new Dictionary<RegImmOpCode, Action<Instruction>>
            {
                [RegImmOpCode.BGEZAL] = i => Branch(i, (rs, rt) => rs >= 0, true)
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
            Instruction instruction;

            if (!delaySlot.HasValue)
            {
                instruction = ReadWord(ProgramCounter);
                ProgramCounter += Instruction.Size;
            }
            else
            {
                instruction = ReadWord(delaySlot.Value);
                delaySlot = null;
            }

            Run(instruction);
        }

        private bool Branch(Instruction instruction, Func<ulong, ulong, bool> condition, bool storeLink = false)
        {
            var result = condition(GPRegisters[instruction.RS], GPRegisters[instruction.RT]);

            if (storeLink)
                GPRegisters[31] = ProgramCounter + Instruction.Size;

            if (result)
            {
                delaySlot = ProgramCounter;
                ProgramCounter += (ulong)(short)instruction.Immediate << 2;
            }

            return result;
        }

        private void BranchLikely(Instruction instruction, Func<ulong, ulong, bool> condition)
        {
            if (!Branch(instruction, condition))
                ProgramCounter += Instruction.Size;
        }

        private uint ReadWord(ulong address) => CP0.Map(ref address).ReadWord(address);

        private void WriteWord(ulong address, uint value) => CP0.Map(ref address).WriteWord(address, value);
        #endregion
    }
}
