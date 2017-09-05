namespace N64Emu.CPU
{
    public partial class VR4300
    {
        public enum OpCode : byte
        {
            /// <summary>Load Upper Immediate.</summary>
            LUI = 0b001111,
            /// <summary>Move To System Control Coprocessor.</summary>
            MTC0 = 0b010000,
            /// <summary>Or Immediate.</summary>
            ORI = 0b001101,
            /// <summary>Load Word.</summary>
            LW = 0b100011,
            /// <summary>And Immediate.</summary>
            ANDI = 0b001100,
            /// <summary>Branch On Equal Likely.</summary>
            BEQL = 0b010100,
            /// <summary>Add Immediate Unsigned.</summary>
            ADDIU = 0b001001,
            /// <summary>Store Word.</summary>
            SW = 0b101011,
            /// <summary>Branch On Not Equal Likely.</summary>
            BNEL = 0b010101,
            /// <summary>Branch On Not Equal.</summary>
            BNE = 0b000101,
            /// <summary>Add.</summary>
            ADD = 0b000000,
            /// <summary>Branch On Equal.</summary>
            BEQ = 0b000100
        }
    }
}
