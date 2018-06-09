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
        private bool branchDelay;
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
        public uint FCR0 { get; set; }

        /// <summary>
        /// 32-bit floating-point Control/Status register.
        /// </summary>
        public uint FCR31 { get; set; }

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

        public FloatingPointUnit CP1 => COP[1] as FloatingPointUnit;

        public ulong? DelaySlot { get; private set; }
        #endregion

        #region Constructors
        public VR4300()
        {
            COP[0] = new SystemControlUnit(this);
            COP[1] = new FloatingPointUnit(this);
            operations = new Dictionary<Instruction, Action<Instruction>>
            {
                [Instruction.FromOpCode(OpCode.LUI)] = i => GPR[i.RT] = (ulong)(i.Immediate << 16),
                [Instruction.FromOpCode(OpCode.ORI)] = i => GPR[i.RT] = GPR[i.RS] | i.Immediate,
                [Instruction.FromOpCode(OpCode.LW)] = i => Load(i, AccessSize.Word),
                [Instruction.FromOpCode(OpCode.ANDI)] = i => GPR[i.RT] = i.Immediate & GPR[i.RS],
                [Instruction.FromOpCode(OpCode.BEQL)] = i => BranchLikely(i, (rs, rt) => rs == rt),
                [Instruction.FromOpCode(OpCode.ADDIU)] = i => GPR[i.RT] = (ulong)(int)(GPR[i.RS] + (ulong)(short)i.Immediate),
                [Instruction.FromOpCode(OpCode.SW)] = i => Store(i, AccessSize.Word),
                [Instruction.FromOpCode(OpCode.BNEL)] = i => BranchLikely(i, (rs, rt) => rs != rt),
                [Instruction.FromOpCode(OpCode.BNE)] = i => Branch(i, (rs, rt) => rs != rt),
                [Instruction.FromOpCode(OpCode.BEQ)] = i => Branch(i, (rs, rt) => rs == rt),
                [Instruction.FromOpCode(OpCode.ADDI)] = i => GPR[i.RT] = (ulong)(int)(GPR[i.RS] + (ulong)(short)i.Immediate),
                [Instruction.FromOpCode(OpCode.CACHE)] = i => { /* TODO: Implement and compare the performance if it's a concern. */ },
                [Instruction.FromOpCode(OpCode.JAL)] = i =>
                {
                    StoreLink();
                    Jump((PC & ~((ulong)(1 << 28) - 1)) | (i.Target << 2));
                },
                [Instruction.FromOpCode(OpCode.SLTI)] = i => GPR[i.RT] = (long)GPR[i.RS] < (long)(short)i.Immediate ? (ulong)1 : 0,
                [Instruction.FromOpCode(OpCode.XORI)] = i => GPR[i.RT] = GPR[i.RS] ^ i.Immediate,
                [Instruction.FromOpCode(OpCode.BLEZL)] = i => BranchLikely(i, (rs, rt) => rs <= 0),
                [Instruction.FromOpCode(OpCode.SB)] = i => Store(i, AccessSize.Byte),
                [Instruction.FromOpCode(OpCode.LBU)] = i => LoadUnsigned(i, AccessSize.Byte),
                [Instruction.FromOpCode(OpCode.COP3)] = i => ExceptionProcessing.ReservedInstruction(this, i), // CP3 access throws a reserved instruction for this CPU.
                [Instruction.FromOpCode(OpCode.BLEZ)] = i => Branch(i, (rs, rt) => rs <= 0),
                [Instruction.FromOpCode(OpCode.LD)] = i => Load(i, AccessSize.DoubleWord),
                [Instruction.FromOpCode(OpCode.SLTIU)] = i => GPR[i.RT] = GPR[i.RS] < (ulong)(short)i.Immediate ? (ulong)1 : 0,
                [Instruction.FromOpCode(OpCode.SH)] = i => Store(i, AccessSize.HalfWord),
                [Instruction.FromOpCode(OpCode.LHU)] = i => LoadUnsigned(i, AccessSize.HalfWord),
                [Instruction.FromOpCode(OpCode.J)] = i => Jump((PC & ~((ulong)(1 << 28) - 1)) | (i.Target << 2)),
                [Instruction.FromOpCode(OpCode.LB)] = i => Load(i, AccessSize.Byte),
                [Instruction.FromOpCode(SpecialOpCode.ADD)] = i => GPR[i.RD] = (ulong)((int)GPR[i.RS] + (int)GPR[i.RT]),
                [Instruction.FromOpCode(SpecialOpCode.JR)] = i => Jump(GPR[i.RS]),
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
                [Instruction.FromOpCode(SpecialOpCode.SLT)] = i => GPR[i.RD] = (long)GPR[i.RS] < (long)GPR[i.RT] ? (ulong)1 : 0,
                [Instruction.FromOpCode(SpecialOpCode.DMULTU)] = i =>
                {
                    ulong rsHi = (uint)(GPR[i.RS] >> 32), rtHi = (uint)(GPR[i.RT] >> 32);
                    ulong rsLo = (uint)GPR[i.RS], rtLo = (uint)GPR[i.RT];
                    ulong midProducts = rsHi * rtLo + rsLo * rtHi, loProduct = rsLo * rtLo, hiProduct = rsHi * rtHi;
                    LO = (uint)loProduct + (midProducts << 32);
                    HI = hiProduct + (midProducts >> 32);
                },
                [Instruction.FromOpCode(SpecialOpCode.DSLL32)] = i => GPR[i.RD] = GPR[i.RT] << (i.SA + 32),
                [Instruction.FromOpCode(SpecialOpCode.DSRA32)] = i => GPR[i.RD] = (ulong)((long)GPR[i.RT] >> (i.SA + 32)),
                [Instruction.FromOpCode(SpecialOpCode.DDIVU)] = i =>
                {
                    ulong rs = GPR[i.RS], rt = GPR[i.RT];

                    if (rt == 0)
                        return;

                    LO = rs / rt;
                    HI = rs % rt;
                },
                [Instruction.FromOpCode(SpecialOpCode.SRA)] = i => GPR[i.RD] = (ulong)((int)GPR[i.RT] >> i.SA),
                [Instruction.FromOpCode(SpecialOpCode.MTLO)] = i => LO = GPR[i.RS],
                [Instruction.FromOpCode(SpecialOpCode.MTHI)] = i => HI = GPR[i.RS],
                [Instruction.FromOpCode(SpecialOpCode.JALR)] = i =>
                {
                    StoreLink((GPRIndex)i.RD);
                    Jump(GPR[i.RS]);
                },
                [Instruction.FromOpCode(RegImmOpCode.BGEZAL)] = i =>
                {
                    StoreLink();
                    Branch(i, (rs, rt) => rs >= 0);
                },
                [Instruction.FromOpCode(RegImmOpCode.BGEZL)] = i => BranchLikely(i, (rs, rt) => rs >= 0),
                [Instruction.FromOpCode(RegImmOpCode.BLTZ)] = i => Branch(i, (rs, rt) => rs < 0),
                [Instruction.FromOpCode(RegImmOpCode.BGEZ)] = i => Branch(i, (rs, rt) => rs >= 0)
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

                if (CP0.IsCoprocessorUsable(unit))
                    COP[unit].Run(instruction);
                else
                    ExceptionProcessing.CoprocessorUnusable(this, unit);
            }
            else
                ExceptionProcessing.ReservedInstruction(this, instruction);

            GPR[(int)GPRIndex.Zero] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Step()
        {
            Instruction instruction;
            branchDelay = DelaySlot.HasValue;

            if (!branchDelay)
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
            CP0.IncrementCounter();

            if (CP0.HasPendingInterrupt)
            {
                ExceptionProcessing.Interrupt(this);
                return;
            }

            Step();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Branch(Instruction instruction, BranchCondition condition)
        {
            var result = condition(GPR[instruction.RS], GPR[instruction.RT]);

            if (result)
                Jump(PC + ((ulong)(short)instruction.Immediate << 2));

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BranchLikely(Instruction instruction, BranchCondition condition)
        {
            if (!Branch(instruction, condition))
                PC += Instruction.Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Jump(ulong address)
        {
            DelaySlot = PC;
            PC = address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StoreLink(GPRIndex index = GPRIndex.RA) => GPR[(int)index] = PC + Instruction.Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadWord(ulong address) => (uint)Read(address, AccessSize.Word);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong Read(ulong address, AccessSize size)
        {
            var physicalAddress = CP0.Translate(address);

            switch (size)
            {
                case AccessSize.Byte:
                    return (byte)ReadSysAD(physicalAddress);
                case AccessSize.HalfWord:
                    return (ushort)ReadSysAD(physicalAddress);
                case AccessSize.Word:
                    return ReadSysAD(physicalAddress);
                case AccessSize.DoubleWord:
                    return ReadSysAD(physicalAddress) << 32 | ReadSysAD(physicalAddress + sizeof(uint));
                default:
                    throw new ArgumentException("Invalid system bus access size.", nameof(size));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Write(ulong address, ulong data, AccessSize size)
        {
            var physicalAddress = CP0.Translate(address);

            switch (size)
            {
                case AccessSize.Byte:
                    WriteSysAD(physicalAddress, (ReadWord(address) & ~((uint)(1 << 8) - 1)) | (byte)data);
                    break;
                case AccessSize.HalfWord:
                    WriteSysAD(physicalAddress, (ReadWord(address) & ~((uint)(1 << 16) - 1)) | (ushort)data);
                    break;
                case AccessSize.Word:
                    WriteSysAD(physicalAddress, (uint)data);
                    break;
                case AccessSize.DoubleWord:
                    WriteSysAD(physicalAddress, (uint)(data >> 32));
                    WriteSysAD(physicalAddress + sizeof(uint), (uint)data);
                    break;
                default:
                    throw new ArgumentException("Invalid system bus access size.", nameof(size));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Load(Instruction instruction, AccessSize size, bool signExtend = true)
        {
            var value = Read((ulong)(short)instruction.Immediate + GPR[instruction.RS], size);

            if (signExtend)
            {
                switch (size)
                {
                    case AccessSize.Byte:
                        value = (ulong)(sbyte)value;
                        break;
                    case AccessSize.HalfWord:
                        value = (ulong)(short)value;
                        break;
                    case AccessSize.Word:
                        value = (ulong)(int)value;
                        break;
                }
            }

            GPR[instruction.RT] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LoadUnsigned(Instruction instruction, AccessSize size) => Load(instruction, size, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Store(Instruction instruction, AccessSize size) => Write((ulong)(short)instruction.Immediate + GPR[instruction.RS], GPR[instruction.RT], size);
        #endregion
    }
}
