namespace DotN64.Diagnostics
{
    using CPU;

    public partial class Debugger
    {
        // TODO: Double check for possibly missing exceptions in formatting according to the CPU's documentation, as I started with help from a MIPS I (?) general documentation (https://en.wikibooks.org/wiki/MIPS_Assembly/Instruction_Formats).
        // NOTE: Perhaps this should be moved beside the CPU. It would probably be nice to make it overridable from a more general MIPS format class, as well as make it instanciable for changing display variables (register prefix, etc.).
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
            private static string Format(VR4300.Instruction instruction, params object[] values) => $"{instruction} {string.Join(Separator, values)}";

            private static string FormatRegister(int index, VR4300 cpu) => RegisterPrefix + (VR4300.GPRIndex)index + (cpu != null ? FormatRegisterContents(cpu.GPR[index]) : string.Empty);

            private static string FormatCP0Register(int index, VR4300 cpu) => RegisterPrefix + (VR4300.SystemControlUnit.RegisterIndex)index + (cpu != null ? FormatRegisterContents(cpu.CP0.Registers[index]) : string.Empty);

            private static string FormatRegisterContents(ulong value) => $"(0x{value:X})";

            /// <summary>
            /// Immediate type.
            /// </summary>
            public static string I(VR4300.Instruction instruction, VR4300 cpu)
            {
                switch (instruction.RegImm)
                {
                    case VR4300.RegImmOpCode.BGEZAL:
                    case VR4300.RegImmOpCode.BGEZL:
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
                    case VR4300.SpecialOpCode.JR:
                        return Format(instruction, FormatRegister(instruction.RS, cpu));
                    case VR4300.SpecialOpCode.SLLV:
                    case VR4300.SpecialOpCode.SRLV:
                        return Format(instruction, FormatRegister(instruction.RD, cpu), FormatRegister(instruction.RT, cpu), FormatRegister(instruction.RS, cpu));
                }

                switch (instruction.OP)
                {
                    case VR4300.OpCode.MTC0:
                        return Format(instruction, FormatRegister(instruction.RT, cpu), FormatCP0Register(instruction.RD, cpu));
                    default:
                        return Format(instruction, FormatRegister(instruction.RD, cpu), FormatRegister(instruction.RS, cpu), FormatRegister(instruction.RT, cpu));
                }
            }

            /// <summary>
            /// Jump type.
            /// </summary>
            public static string J(VR4300.Instruction instruction, VR4300 cpu) => Format(instruction, $"0x{instruction.Target:X8}");
            #endregion
        }
    }
}
