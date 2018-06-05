using System;
using System.Collections.Generic;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class FloatingPointUnit : ICoprocessor
        {
            #region Fields
            private readonly VR4300 cpu;
            private readonly IReadOnlyDictionary<OpCode, Action<Instruction>> operations;
            #endregion

            #region Properties
            public ImplementationRevisionRegister ImplementationRevision { get; }

            public ControlStatusRegister ControlStatus { get; }
            #endregion

            #region Constructors
            public FloatingPointUnit(VR4300 cpu)
            {
                this.cpu = cpu;
                ImplementationRevision = new ImplementationRevisionRegister(this);
                ControlStatus = new ControlStatusRegister(this);
                operations = new Dictionary<OpCode, Action<Instruction>>
                {
                    [OpCode.CF] = i =>
                    {
                        switch (i.RD)
                        {
                            case 0:
                                cpu.GPR[i.RT] = (ulong)(int)cpu.FCR0;
                                break;
                            case 31:
                                cpu.GPR[i.RT] = (ulong)(int)cpu.FCR31;
                                break;
                            default:
                                ExceptionProcessing.ReservedInstruction(cpu, i);
                                return;
                        }
                    },
                    [OpCode.CT] = i =>
                    {
                        switch (i.RD)
                        {
                            case 0:
                                return; // Read-only register.
                            case 31:
                                cpu.FCR31 = (uint)cpu.GPR[i.RT];

                                if ((ControlStatus.Cause & (ControlStatus.Enables | ControlStatusRegister.ExceptionFlags.E)) != 0)
                                {
                                    ExceptionProcessing.FloatingPoint(cpu);
                                    return;
                                }
                                break;
                            default:
                                ExceptionProcessing.ReservedInstruction(cpu, i);
                                return;
                        }
                    }
                };
            }
            #endregion

            #region Methods
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
