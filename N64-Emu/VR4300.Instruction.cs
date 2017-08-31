namespace N64Emu
{
    public partial class VR4300
    {
        public struct Instruction
        {
            #region Fields
            private uint data;
            #endregion

            #region Properties
            public OpCode OpCode => (OpCode)(data >> 26);

            public ushort Immediate => (ushort)(data & 0xFFFF);

            public byte RT => (byte)(data >> 16 & 0b11111);

            public byte RD => (byte)(data >> 11 & 0b11111);

            public byte RS => (byte)(data >> 21 & 0b11111);
            #endregion

            #region Operators
            public static implicit operator Instruction(uint value) => new Instruction { data = value };

            public static implicit operator uint(Instruction instruction) => instruction.data;
            #endregion
        }
    }
}
