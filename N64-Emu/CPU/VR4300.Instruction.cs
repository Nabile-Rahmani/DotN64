namespace N64Emu.CPU
{
    public partial class VR4300
    {
        public struct Instruction
        {
            #region Fields
            private uint data;
            #endregion

            #region Properties
            /// <summary>
            /// 6-bit operation code.
            /// </summary>
            public OpCode OP => (OpCode)(data >> 26);

            public SpecialOpCode SpecialOP => (SpecialOpCode)(data & (1 << 6) - 1);

            /// <summary>
            /// 5-bit source register number.
            /// </summary>
            public byte RS => (byte)(data >> 21 & ((1 << 5) - 1));

            /// <summary>
            /// 5-bit target (source/destination) register number or branch condition.
            /// </summary>
            public byte RT => (byte)(data >> 16 & ((1 << 5) - 1));

            /// <summary>
            /// 16-bit immediate value, branch displacement or address displacement.
            /// </summary>
            public ushort Immediate => (ushort)(data & ((1 << 16) - 1));

            /// <summary>
            /// 26-bit unconditional branch target address.
            /// </summary>
            public uint Target => data & ((1 << 26) - 1);

            /// <summary>
            /// 5-bit destination register number.
            /// </summary>
            public byte RD => (byte)(data >> 11 & ((1 << 5) - 1));

            /// <summary>
            /// 5-bit shift amount.
            /// </summary>
            public byte SA => (byte)(data >> 6 & ((1 << 5) - 1));

            /// <summary>
            /// 6-bit function field.
            /// </summary>
            public byte Funct => (byte)(data & ((1 << 6) - 1));
            #endregion

            #region Operators
            public static implicit operator Instruction(uint value) => new Instruction { data = value };

            public static implicit operator uint(Instruction instruction) => instruction.data;
            #endregion
        }
    }
}
