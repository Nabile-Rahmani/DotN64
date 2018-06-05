using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit : ICoprocessor
        {
            #region Fields
            private readonly VR4300 cpu;
            private readonly IReadOnlyDictionary<OpCode, Action<Instruction>> operations;
            private readonly IReadOnlyDictionary<FunctOpCode, Action<Instruction>> functOperations;
            #endregion

            #region Properties
            public ulong[] Registers { get; } = new ulong[32];

            public ConfigRegister Config { get; }

            public StatusRegister Status { get; }

            public CauseRegister Cause { get; }

            public bool HasPendingInterrupt => Status.IE && !(Status.EXL | Status.ERL) && (Status.IM & Cause.IP) != 0;
            #endregion

            #region Constructors
            public SystemControlUnit(VR4300 cpu)
            {
                this.cpu = cpu;
                Config = new ConfigRegister(this);
                Status = new StatusRegister(this);
                Cause = new CauseRegister(this);
                operations = new Dictionary<OpCode, Action<Instruction>>
                {
                    [OpCode.MT] = i => Registers[i.RD] = cpu.GPR[i.RT],
                    [OpCode.MF] = i => cpu.GPR[i.RT] = (ulong)(int)Registers[i.RD],
                    [OpCode.CO] = i =>
                    {
                        if (functOperations.TryGetValue((FunctOpCode)i.Funct, out var operation))
                            operation(i);
                        else //if (i.Funct == 0b010000) // TODO: Uncomment once the operations are implemented.
                            ExceptionProcessing.ReservedInstruction(cpu, i);
                    }
                };
                functOperations = new Dictionary<FunctOpCode, Action<Instruction>>
                {
                    [FunctOpCode.TLBWI] = i => { /* TODO. */ },
                    [FunctOpCode.ERET] = i =>
                    {
                        if (Status.ERL)
                        {
                            cpu.PC = Registers[(int)RegisterIndex.ErrorEPC];
                            Status.ERL = false;
                        }
                        else
                        {
                            cpu.PC = Registers[(int)RegisterIndex.EPC];
                            Status.EXL = false;
                        }

                        cpu.LLBit = false;
                        cpu.DelaySlot = null;
                    }
                };
            }
            #endregion

            #region Methods
            /// <summary>
            /// Increments the Count register (must be called on every odd PClock cycle), and updates the timer interrupt.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void IncrementCounter()
            {
                if ((uint)++Registers[(int)RegisterIndex.Count] == (uint)Registers[(int)RegisterIndex.Compare])
                {
                    var ip = Cause.IP;
                    ip.TimerInterrupt = true;
                    Cause.IP = ip;
                }
            }

            /// <summary>
            /// Translates a virtual address into a physical address.
            /// See: datasheet#5.2.4 Table 5-3.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint Translate(ulong address)
            {
                switch (address >> 29 & 0b111)
                {
                    case 0b100: // kseg0.
                        return (uint)(address - 0xFFFFFFFF80000000);
                    case 0b101: // kseg1.
                        return (uint)(address - 0xFFFFFFFFA0000000);
                    default:
                        throw new Exception($"Unknown memory map segment for location 0x{address:X16}.");
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsCoprocessorUsable(byte unit) => ((byte)Status.CU & 1 << unit) != 0 || (unit == 0 && Status.KSU == StatusRegister.Mode.Kernel);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Run(Instruction instruction)
            {
                if (operations.TryGetValue((OpCode)instruction.RS, out var operation))
                    operation(instruction);
                else
                    ExceptionProcessing.ReservedInstruction(cpu, instruction);
            }
            #endregion
        }
    }
}
