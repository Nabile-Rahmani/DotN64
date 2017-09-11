namespace N64Emu.CPU
{
    public partial class VR4300
    {
        public enum SpecialOpCode : byte
        {
            /// <summary>Add.</summary>
            ADD = 0b100000,
            /// <summary>Jump Register.</summary>
            JR = 0b001000,
            /// <summary>Shift Right Logical.</summary>
            SRL = 0b000010,
            /// <summary>Or.</summary>
            OR = 0b100101
        }
    }
}
