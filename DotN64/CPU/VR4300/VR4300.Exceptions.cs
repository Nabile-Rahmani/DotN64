using System;

namespace DotN64.CPU
{
    public partial class VR4300
    {
        public class UnimplementedOperationException : Exception
        {
            #region Properties
            public Instruction Instruction { get; }
            #endregion

            #region Constructors
            public UnimplementedOperationException(Instruction instruction)
                : base($"Unimplemented opcode ({GetOpCodeType(instruction).Name} {FormatOpCodeBits(instruction)}) from instruction 0x{(uint)instruction:X8}.")
            {
                Instruction = instruction;
            }
            #endregion

            #region Methods
            private static Type GetOpCodeType(Instruction instruction) => instruction.Special?.GetType() ?? instruction.RegImm?.GetType() ?? instruction.OP.GetType();

            private static string FormatOpCodeBits(Instruction instruction) => "0b" + Convert.ToString((byte?)instruction.Special ?? (byte?)instruction.RegImm ?? (byte)instruction.OP, 2);
            #endregion
        }
    }
}
