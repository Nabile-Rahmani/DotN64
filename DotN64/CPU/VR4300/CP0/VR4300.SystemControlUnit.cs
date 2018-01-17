using System;
using System.Collections.Generic;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit : ICoprocessor
        {
            #region Fields
            private readonly VR4300 cpu;
            private readonly IReadOnlyDictionary<OpCode, Action<Instruction>> operations;
            #endregion

            #region Properties
            public ulong[] Registers { get; } = new ulong[32];

            public ConfigRegister Config { get; }

            public StatusRegister Status { get; }

            public CauseRegister Cause { get; }
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
                    [OpCode.MF] = i => cpu.GPR[i.RT] = (ulong)(int)Registers[i.RD]
                };
            }
            #endregion

            #region Methods
            /// <summary>
            /// Translates a virtual address into a physical address.
            /// See: datasheet#5.2.4 Table 5-3.
            /// </summary>
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
