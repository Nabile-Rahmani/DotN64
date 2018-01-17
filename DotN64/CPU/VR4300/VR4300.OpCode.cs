namespace DotN64.CPU
{
    public partial class VR4300
    {
        public enum OpCode : byte
        {
            SPECIAL = 0b000000,
            REGIMM = 0b000001,
            COP0 = 0b010000,
            COP1 = 0b010001,
            COP2 = 0b010010,
            COP3 = 0b010011,
            /// <summary>Load Upper Immediate.</summary>
            LUI = 0b001111,
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
            /// <summary>Branch On Equal.</summary>
            BEQ = 0b000100,
            /// <summary>Add Immediate.</summary>
            ADDI = 0b001000,
            /// <summary>Cache Operation.</summary>
            CACHE = 0b101111,
            /// <summary>Jump And Link.</summary>
            JAL = 0b000011,
            /// <summary>Set On Less Than Immediate.</summary>
            SLTI = 0b001010,
            /// <summary>Exclusive Or Immediate.</summary>
            XORI = 0b001110,
            /// <summary>Branch On Less Than Or Equal To Zero Likely.</summary>
            BLEZL = 0b010110,
            /// <summary>Store Byte.</summary>
            SB = 0b101000,
            /// <summary>Load Byte Unsigned.</summary>
            LBU = 0b100100
        }
    }
}
