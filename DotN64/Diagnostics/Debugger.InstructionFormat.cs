namespace DotN64.Diagnostics
{
    using CPU;

    public partial class Debugger
    {
        // TODO: Double check for possibly missing exceptions in formatting according to the CPU's documentation, as I started with help from a MIPS I (?) general documentation (https://en.wikibooks.org/wiki/MIPS_Assembly/Instruction_Formats).
        // NOTE: Perhaps this should be moved beside the CPU. It would probably be nice to make it overridable from a more general MIPS format class, as well as make it instantiable for changing display variables (register prefix, etc.).
        // NOTE: If assembling input has to be implemented, make an enum Type, and move the currently used methods inside a Disassembler class, and the new ones into Assembler. The caller would look for the type associated with a given instruction and choose from here.
        /// <summary>
        /// Formatting helpers for disassembling instructions.
        /// </summary>
        private static class InstructionFormat
        {
            #region Fields
            private const string Separator = ", ", RegisterPrefix = "$";
            #endregion

            #region Methods
            private static string Format(VR4300.Instruction instruction, params object[] values) => $"{FormatOpCode(instruction)} {string.Join(Separator, values)}";

            private static string FormatOpCode(VR4300.Instruction instruction)
            {
                // I was originally going to use the CPU in all cases and casting the COP[instruction.COPz] to a debuggable coprocessor to get the opcode, but it just makes the argument list larger because I have to show the register contents.
                // You could argue this should be made instantiable but hey.
                // Besides, coprocessors are already hard-coded here (FormatCP0Register). I'll have to clean things up some day.
                if (instruction.COPz.HasValue)
                {
                    var opCode = "?";

                    switch (instruction.OP)
                    {
                        case VR4300.OpCode.COP0:
                            switch ((VR4300.SystemControlUnit.OpCode)instruction.RS)
                            {
                                case VR4300.SystemControlUnit.OpCode.CO:
                                    opCode = ((VR4300.SystemControlUnit.FunctOpCode)instruction.Funct).ToString();
                                    break;
                                default:
                                    opCode = ((VR4300.SystemControlUnit.OpCode)instruction.RS).ToString();
                                    break;
                            }
                            break;
                        case VR4300.OpCode.COP1:
                            opCode = ((VR4300.FloatingPointUnit.OpCode)instruction.RS).ToString();
                            break;
                    }

                    return $"{instruction}.{opCode}";
                }

                return instruction.ToString();
            }

            private static string FormatRegister(int index, VR4300 cpu) => RegisterPrefix + (VR4300.GPRIndex)index + (cpu != null ? FormatRegisterContents(cpu.GPR[index]) : string.Empty);

            private static string FormatCP0Register(int index, VR4300 cpu) => RegisterPrefix + (VR4300.SystemControlUnit.RegisterIndex)index + (cpu != null ? FormatRegisterContents(cpu.CP0.Registers[index]) : string.Empty);

            private static string FormatRegisterContents(ulong value) => $"[0x{value:X}]";

            /// <summary>
            /// Immediate type.
            /// </summary>
            public static string I(VR4300.Instruction instruction, VR4300 cpu)
            {
                switch (instruction.RegImm)
                {
                    case VR4300.RegImmOpCode.BGEZAL:
                    case VR4300.RegImmOpCode.BGEZL:
                    case VR4300.RegImmOpCode.BLTZ:
                    case VR4300.RegImmOpCode.BGEZ:
                        return Format(instruction, FormatRegister(instruction.RS, cpu), (short)instruction.Immediate);
                }

                switch (instruction.OP)
                {
                    case VR4300.OpCode.BEQ:
                    case VR4300.OpCode.BNE:
                    case VR4300.OpCode.BEQL:
                    case VR4300.OpCode.BNEL:
                        return Format(instruction, FormatRegister(instruction.RS, cpu), FormatRegister(instruction.RT, cpu), (short)instruction.Immediate);
                    case VR4300.OpCode.BLEZL:
                    case VR4300.OpCode.BLEZ:
                    case VR4300.OpCode.BGTZ:
                        return Format(instruction, FormatRegister(instruction.RS, cpu), (short)instruction.Immediate);
                    default:
                        return Format(instruction, FormatRegister(instruction.RT, cpu), FormatRegister(instruction.RS, cpu), (short)instruction.Immediate);
                }
            }

            /// <summary>
            /// Register type.
            /// </summary>
            public static string R(VR4300.Instruction instruction, VR4300 cpu)
            {
                switch (instruction.Special)
                {
                    case VR4300.SpecialOpCode.JALR:
                        return Format(instruction, FormatRegister(instruction.RD, cpu), FormatRegister(instruction.RS, cpu));
                    case VR4300.SpecialOpCode.JR:
                    case VR4300.SpecialOpCode.MTLO:
                    case VR4300.SpecialOpCode.MTHI:
                        return Format(instruction, FormatRegister(instruction.RS, cpu));
                    case VR4300.SpecialOpCode.MFHI:
                    case VR4300.SpecialOpCode.MFLO:
                        return Format(instruction, FormatRegister(instruction.RD, cpu));
                    case VR4300.SpecialOpCode.MULTU:
                    case VR4300.SpecialOpCode.DMULTU:
                    case VR4300.SpecialOpCode.DDIVU:
                        return Format(instruction, FormatRegister(instruction.RS, cpu), FormatRegister(instruction.RT, cpu));
                    case VR4300.SpecialOpCode.SLLV:
                    case VR4300.SpecialOpCode.SRLV:
                        return Format(instruction, FormatRegister(instruction.RD, cpu), FormatRegister(instruction.RT, cpu), FormatRegister(instruction.RS, cpu));
                    case VR4300.SpecialOpCode.SLL:
                    case VR4300.SpecialOpCode.SRL:
                    case VR4300.SpecialOpCode.DSLL32:
                    case VR4300.SpecialOpCode.DSRA32:
                    case VR4300.SpecialOpCode.SRA:
                        return Format(instruction, FormatRegister(instruction.RD, cpu), FormatRegister(instruction.RT, cpu), (sbyte)instruction.SA);
                }

                return Format(instruction, FormatRegister(instruction.RD, cpu), FormatRegister(instruction.RS, cpu), FormatRegister(instruction.RT, cpu));
            }

            /// <summary>
            /// Jump type.
            /// </summary>
            public static string J(VR4300.Instruction instruction, VR4300 cpu) => Format(instruction, $"0x{((cpu != null ? (cpu.DelaySlot ?? cpu.PC) : 0) & ~(ulong)((1 << 28) - 1)) | (instruction.Target << 2):X8}");

            public static string CP0(VR4300.Instruction instruction, VR4300 cpu)
            {
                switch ((VR4300.SystemControlUnit.OpCode)instruction.RS)
                {
                    case VR4300.SystemControlUnit.OpCode.MT:
                    case VR4300.SystemControlUnit.OpCode.MF:
                        return Format(instruction, FormatRegister(instruction.RT, cpu), FormatCP0Register(instruction.RD, cpu));
                    default:
                        return FormatOpCode(instruction);
                }
            }

            public static string Unknown(VR4300.Instruction instruction) => FormatOpCode(instruction);
            #endregion
        }
    }
}
