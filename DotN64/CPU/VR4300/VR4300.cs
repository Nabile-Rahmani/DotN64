using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DotN64.CPU
{
    /// <summary>
    /// The NEC VR4300 CPU, designed by MIPS Technologies.
    /// Datasheet: http://datasheets.chipdb.org/NEC/Vr-Series/Vr43xx/U10504EJ7V0UMJ1.pdf
    /// </summary>
    public partial class VR4300
    {
        #region Fields
        private readonly IReadOnlyDictionary<Instruction, Action<Instruction>> operations;
        private readonly IReadOnlyDictionary<byte, float> divModeMultipliers = new Dictionary<byte, float>
        {
            [0b01] = 1.5f,
            [0b10] = 2.0f,
            [0b11] = 3.0f
        };

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

        private byte divMode;
        /// <summary>
        /// Internal operating frequency mode.
        /// See: datasheet#2.2.2 Table 2-2.
        /// </summary>
        public byte DivMode
        {
            get => divMode;
            set => divMode = (byte)(value & ((1 << 2) - 1));
        }

        public double MasterClock { get; set; }

        /// <summary>
        /// Pipeline clock.
        /// </summary>
        private double PClock => MasterClock * divModeMultipliers[DivMode] * (CP0.Status.RP ? 0.25 : 1.0);

        /// <summary>
        /// System interface clock.
        /// </summary>
        private double SClock => PClock / divModeMultipliers[DivMode];

        /// <summary>
        /// Transmit/receive clock.
        /// </summary>
        public double TClock => SClock;

        private double TicksPerCycle => 1.0 / PClock * TimeSpan.TicksPerSecond;

        /// <summary>
        /// Interrupt request acknowledge.
        /// </summary>
        public byte Int
        {
            get => CP0.Cause.IP.ExternalNormalInterrupts;
            set
            {
                var ip = CP0.Cause.IP;
                ip.ExternalNormalInterrupts = value;
                CP0.Cause.IP = ip;
            }
        }

        /// <summary>
        /// System address/data bus.
        /// </summary>
        public Func<uint, uint> ReadSysAD { get; set; }

        /// <summary>
        /// System address/data bus.
        /// </summary>
        public Action<uint, uint> WriteSysAD { get; set; }

        public ICoprocessor[] COP { get; } = new ICoprocessor[4];

        public SystemControlUnit CP0 => COP[0] as SystemControlUnit;

        public ulong? DelaySlot { get; private set; }
        #endregion

        #region Constructors
        public VR4300()
        {
            COP[0] = new SystemControlUnit(this);
            operations = new Dictionary<Instruction, Action<Instruction>>
            {
                [Instruction.FromOpCode(OpCode.LUI)] = i => GPR[i.RT] = (ulong)(i.Immediate << 16),
                [Instruction.FromOpCode(OpCode.ORI)] = i => GPR[i.RT] = GPR[i.RS] | i.Immediate,
                [Instruction.FromOpCode(OpCode.LW)] = i => GPR[i.RT] = (ulong)(int)ReadWord((ulong)(short)i.Immediate + GPR[i.RS]),
                [Instruction.FromOpCode(OpCode.ANDI)] = i => GPR[i.RT] = i.Immediate & GPR[i.RS],
                [Instruction.FromOpCode(OpCode.BEQL)] = i => BranchLikely(i, (rs, rt) => rs == rt),
                [Instruction.FromOpCode(OpCode.ADDIU)] = i => GPR[i.RT] = (ulong)(int)(GPR[i.RS] + (ulong)(short)i.Immediate),
                [Instruction.FromOpCode(OpCode.SW)] = i => WriteWord((ulong)(short)i.Immediate + GPR[i.RS], (uint)GPR[i.RT]),
                [Instruction.FromOpCode(OpCode.BNEL)] = i => BranchLikely(i, (rs, rt) => rs != rt),
                [Instruction.FromOpCode(OpCode.BNE)] = i => Branch(i, (rs, rt) => rs != rt),
                [Instruction.FromOpCode(OpCode.BEQ)] = i => Branch(i, (rs, rt) => rs == rt),
                [Instruction.FromOpCode(OpCode.ADDI)] = i => GPR[i.RT] = (ulong)(int)(GPR[i.RS] + (ulong)(short)i.Immediate),
                [Instruction.FromOpCode(OpCode.CACHE)] = i => { /* TODO: Implement and compare the performance if it's a concern. */ },
                [Instruction.FromOpCode(OpCode.JAL)] = i =>
                {
                    GPR[31] = PC + Instruction.Size;
                    DelaySlot = PC;
                    PC = (PC & ~((ulong)(1 << 28) - 1)) | (i.Target << 2);
                },
                [Instruction.FromOpCode(OpCode.SLTI)] = i => GPR[i.RT] = GPR[i.RS] < (ulong)(short)i.Immediate ? (ulong)1 : 0,
                [Instruction.FromOpCode(OpCode.XORI)] = i => GPR[i.RT] = GPR[i.RS] ^ i.Immediate,
                [Instruction.FromOpCode(OpCode.BLEZL)] = i => BranchLikely(i, (rs, rt) => rs <= 0),
                [Instruction.FromOpCode(OpCode.SB)] = i =>
                {
                    var address = (ulong)(short)i.Immediate + GPR[i.RS];

                    WriteWord(address, (ReadWord(address) & ~((uint)(1 << 8) - 1)) | (byte)GPR[i.RT]);
                },
                [Instruction.FromOpCode(OpCode.LBU)] = i => GPR[i.RT] = (byte)ReadWord((ulong)(short)i.Immediate + GPR[i.RS]),
                [Instruction.FromOpCode(OpCode.COP3)] = i => ExceptionProcessing.ReservedInstruction(this, i), // CP3 access throws a reserved instruction for this CPU.
                [Instruction.FromOpCode(SpecialOpCode.ADD)] = i => GPR[i.RD] = (ulong)((int)GPR[i.RS] + (int)GPR[i.RT]),
                [Instruction.FromOpCode(SpecialOpCode.JR)] = i =>
                {
                    DelaySlot = PC;
                    PC = GPR[i.RS];
                },
                [Instruction.FromOpCode(SpecialOpCode.SRL)] = i => GPR[i.RD] = (ulong)((int)GPR[i.RT] >> i.SA),
                [Instruction.FromOpCode(SpecialOpCode.OR)] = i => GPR[i.RD] = GPR[i.RS] | GPR[i.RT],
                [Instruction.FromOpCode(SpecialOpCode.MULTU)] = i =>
                {
                    var result = (ulong)(uint)GPR[i.RS] * (ulong)(uint)GPR[i.RT];
                    LO = (ulong)(int)result;
                    HI = (ulong)(int)(result >> 32);
                },
                [Instruction.FromOpCode(SpecialOpCode.MFLO)] = i => GPR[i.RD] = LO,
                [Instruction.FromOpCode(SpecialOpCode.SLL)] = i => GPR[i.RD] = (ulong)((int)GPR[i.RT] << i.SA),
                [Instruction.FromOpCode(SpecialOpCode.SUBU)] = i => GPR[i.RD] = (ulong)(int)(GPR[i.RS] - GPR[i.RT]),
                [Instruction.FromOpCode(SpecialOpCode.XOR)] = i => GPR[i.RD] = GPR[i.RS] ^ GPR[i.RT],
                [Instruction.FromOpCode(SpecialOpCode.MFHI)] = i => GPR[i.RD] = HI,
                [Instruction.FromOpCode(SpecialOpCode.ADDU)] = i => GPR[i.RD] = (ulong)((int)GPR[i.RS] + (int)GPR[i.RT]),
                [Instruction.FromOpCode(SpecialOpCode.SLTU)] = i => GPR[i.RD] = (ulong)(GPR[i.RS] < GPR[i.RT] ? 1 : 0),
                [Instruction.FromOpCode(SpecialOpCode.SLLV)] = i => GPR[i.RD] = (ulong)(int)((uint)GPR[i.RT] << (int)(GPR[i.RS] & ((1 << 5) - 1))),
                [Instruction.FromOpCode(SpecialOpCode.SRLV)] = i => GPR[i.RD] = (ulong)(int)((uint)GPR[i.RT] >> (int)(GPR[i.RS] & ((1 << 5) - 1))),
                [Instruction.FromOpCode(SpecialOpCode.AND)] = i => GPR[i.RD] = GPR[i.RS] & GPR[i.RT],
                [Instruction.FromOpCode(SpecialOpCode.SLT)] = i => GPR[i.RD] = GPR[i.RS] < GPR[i.RT] ? (ulong)1 : 0,
                [Instruction.FromOpCode(RegImmOpCode.BGEZAL)] = i => Branch(i, (rs, rt) => rs >= 0, true),
                [Instruction.FromOpCode(RegImmOpCode.BGEZL)] = i => BranchLikely(i, (rs, rt) => rs >= 0)
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Cold reset.
        /// See: datasheet#6.4.4.
        /// </summary>
        public void Reset() => ExceptionProcessing.ColdReset(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run(Instruction instruction)
        {
            if (operations.TryGetValue(instruction.ToOpCode(), out var operation))
                operation(instruction);
            else if (instruction.COPz.HasValue)
            {
                var unit = instruction.COPz.Value;

                if (((byte)CP0.Status.CU & 1 << unit) != 0 || (unit == 0 && CP0.Status.KSU == SystemControlUnit.StatusRegister.Mode.Kernel))
                    COP[unit].Run(instruction);
                else
                    ExceptionProcessing.CoprocessorUnusable(this, unit);
            }
            else
                ExceptionProcessing.ReservedInstruction(this, instruction);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Step()
        {
            Instruction instruction;

            if (!DelaySlot.HasValue)
            {
                instruction = ReadWord(PC);
                PC += Instruction.Size;
            }
            else
            {
                instruction = ReadWord(DelaySlot.Value);
                DelaySlot = null;
            }

            Run(instruction);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cycle()
        {
            if ((uint)++CP0.Registers[(int)SystemControlUnit.RegisterIndex.Count] == (uint)CP0.Registers[(int)SystemControlUnit.RegisterIndex.Compare])
            {
                var ip = CP0.Cause.IP;
                ip.TimerInterrupt = true;
                CP0.Cause.IP = ip;
            }

            if (CP0.Status.IE && !CP0.Status.EXL && !CP0.Status.ERL && (CP0.Status.IM & CP0.Cause.IP) != 0)
            {
                ExceptionProcessing.Interrupt(this);
                return;
            }

            Step();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Branch(Instruction instruction, BranchCondition condition, bool storeLink = false)
        {
            var result = condition(GPR[instruction.RS], GPR[instruction.RT]);

            if (storeLink)
                GPR[31] = PC + Instruction.Size;

            if (result)
            {
                DelaySlot = PC;
                PC += (ulong)(short)instruction.Immediate << 2;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BranchLikely(Instruction instruction, BranchCondition condition)
        {
            if (!Branch(instruction, condition))
                PC += Instruction.Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadWord(ulong address) => ReadSysAD(CP0.Translate(address));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteWord(ulong address, uint value) => WriteSysAD(CP0.Translate(address), value);
        #endregion
    }
}
