using System;
using System.Collections.Generic;

namespace DotN64.CPU
{
    // Reference: http://datasheets.chipdb.org/NEC/Vr-Series/Vr43xx/U10504EJ7V0UMJ1.pdf
    public partial class VR4300
    {
        #region Fields
        private readonly IReadOnlyDictionary<OpCode, Action<Instruction>> operations;
        private readonly IReadOnlyDictionary<SpecialOpCode, Action<Instruction>> specialOperations;
        private readonly IReadOnlyDictionary<RegImmOpCode, Action<Instruction>> regImmOperations;
        private ulong? delaySlot;

        private delegate bool BranchCondition(ulong rs, ulong rt);
        #endregion

        #region Properties
        /// <summary>
        /// 32 64-bit general purpose registers.
        /// </summary>
        public ulong[] GPR { get; } = new ulong[32];

        /// <summary>
        /// 32 64-bit floating-point operation registers.
        /// </summary>
        public double[] FPR { get; } = new double[32];

        /// <summary>
        /// 64-bit Program Counter.
        /// </summary>
        public ulong PC { get; set; }

        /// <summary>
        /// 64-bit HI register, containing the integer multiply and divide high-order doubleword result.
        /// </summary>
        public ulong HI { get; set; }

        /// <summary>
        /// 64-bit LO register, containing the integer multiply and divide low-order doubleword result.
        /// </summary>
        public ulong LO { get; set; }

        /// <summary>
        /// 1-bit Load/Link LLBit register.
        /// </summary>
        public bool LLBit { get; set; }

        /// <summary>
        /// 32-bit floating-point Implementation/Revision register.
        /// </summary>
        public float FCR0 { get; set; }

        /// <summary>
        /// 32-bit floating-point Control/Status register.
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
                    if (specialOperations.TryGetValue((SpecialOpCode)i.Funct, out var operation))
                        operation(i);
                    else
                        throw new Exception($"Unknown special opcode (0b{Convert.ToString(i.Funct, 2)}) from instruction 0x{(uint)i:X}.");
                },
                [OpCode.REGIMM] = i =>
                {
                    if (regImmOperations.TryGetValue((RegImmOpCode)i.RT, out var operation))
                        operation(i);
                    else
                        throw new Exception($"Unknown reg imm opcode (0b{Convert.ToString(i.RT, 2)}) from instruction 0x{(uint)i:X}.");
                },
                [OpCode.LUI] = i => GPR[i.RT] = (ulong)(i.Immediate << 16),
                [OpCode.MTC0] = i => CP0.Registers[i.RD] = GPR[i.RT],
                [OpCode.ORI] = i => GPR[i.RT] = GPR[i.RS] | i.Immediate,
                [OpCode.LW] = i => GPR[i.RT] = (ulong)(int)ReadWord((ulong)(short)i.Immediate + GPR[i.RS]),
                [OpCode.ANDI] = i => GPR[i.RT] = (ulong)(i.Immediate & (ushort)GPR[i.RS]),
                [OpCode.BEQL] = i => BranchLikely(i, (rs, rt) => rs == rt),
                [OpCode.ADDIU] = i => GPR[i.RT] = (ulong)(int)(GPR[i.RS] + (ulong)(short)i.Immediate),
                [OpCode.SW] = i => WriteWord((ulong)(short)i.Immediate + GPR[i.RS], (uint)GPR[i.RT]),
                [OpCode.BNEL] = i => BranchLikely(i, (rs, rt) => rs != rt),
                [OpCode.BNE] = i => Branch(i, (rs, rt) => rs != rt),
                [OpCode.BEQ] = i => Branch(i, (rs, rt) => rs == rt),
                [OpCode.ADDI] = i => GPR[i.RT] = (ulong)(int)(GPR[i.RS] + (ulong)(short)i.Immediate),
                [OpCode.CACHE] = i => { /* TODO: Implement and compare the performance if it's a concern. */ }
            };
            specialOperations = new Dictionary<SpecialOpCode, Action<Instruction>>
            {
                [SpecialOpCode.ADD] = i => GPR[i.RD] = (ulong)((int)GPR[i.RS] + (int)GPR[i.RT]),
                [SpecialOpCode.JR] = i =>
                {
                    delaySlot = PC;
                    PC = GPR[i.RS];
                },
                [SpecialOpCode.SRL] = i => GPR[i.RD] = (ulong)((int)GPR[i.RT] >> i.SA),
                [SpecialOpCode.OR] = i => GPR[i.RD] = GPR[i.RS] | GPR[i.RT],
                [SpecialOpCode.MULTU] = i =>
                {
                    var result = (ulong)(uint)GPR[i.RS] * (ulong)(uint)GPR[i.RT];
                    LO = (ulong)(int)result;
                    HI = (ulong)(int)(result >> 32);
                },
                [SpecialOpCode.MFLO] = i => GPR[i.RD] = LO,
                [SpecialOpCode.SLL] = i => GPR[i.RD] = (ulong)((int)GPR[i.RT] << i.SA),
                [SpecialOpCode.SUBU] = i => GPR[i.RD] = (ulong)(int)(GPR[i.RS] - GPR[i.RT]),
                [SpecialOpCode.XOR] = i => GPR[i.RD] = GPR[i.RS] ^ GPR[i.RT],
                [SpecialOpCode.MFHI] = i => GPR[i.RD] = HI,
                [SpecialOpCode.ADDU] = i => GPR[i.RD] = (ulong)((int)GPR[i.RS] + (int)GPR[i.RT]),
                [SpecialOpCode.SLTU] = i => GPR[i.RD] = (ulong)(GPR[i.RS] < GPR[i.RT] ? 1 : 0),
                [SpecialOpCode.SLLV] = i => GPR[i.RD] = (ulong)(int)(GPR[i.RT] << (int)(GPR[i.RS] & ((1 << 5) - 1))),
                [SpecialOpCode.SRLV] = i => GPR[i.RD] = (ulong)(int)(GPR[i.RT] >> (int)(GPR[i.RS] & ((1 << 5) - 1))),
                [SpecialOpCode.AND] = i => GPR[i.RD] = GPR[i.RS] & GPR[i.RT]
            };
            regImmOperations = new Dictionary<RegImmOpCode, Action<Instruction>>
            {
                [RegImmOpCode.BGEZAL] = i => Branch(i, (rs, rt) => rs >= 0, true)
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Cold reset.
        /// </summary>
        public void Reset()
        {
            PC = 0xFFFFFFFFBFC00000;

            CP0.Reset();
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
                instruction = ReadWord(PC);
                PC += Instruction.Size;
            }
            else
            {
                instruction = ReadWord(delaySlot.Value);
                delaySlot = null;
            }

            Run(instruction);
        }

        private bool Branch(Instruction instruction, BranchCondition condition, bool storeLink = false)
        {
            var result = condition(GPR[instruction.RS], GPR[instruction.RT]);

            if (storeLink)
                GPR[31] = PC + Instruction.Size;

            if (result)
            {
                delaySlot = PC;
                PC += (ulong)(short)instruction.Immediate << 2;
            }

            return result;
        }

        private void BranchLikely(Instruction instruction, BranchCondition condition)
        {
            if (!Branch(instruction, condition))
                PC += Instruction.Size;
        }

        private uint ReadWord(ulong address) => CP0.Map(ref address).ReadWord(address);

        private void WriteWord(ulong address, uint value) => CP0.Map(ref address).WriteWord(address, value);
        #endregion
    }
}
