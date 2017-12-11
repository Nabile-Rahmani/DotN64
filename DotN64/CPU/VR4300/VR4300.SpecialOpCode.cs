namespace DotN64.CPU
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
            OR = 0b100101,
            /// <summary>Multiply Unsigned.</summary>
            MULTU = 0b011001,
            /// <summary>Move From LO.</summary>
            MFLO = 0b010010,
            /// <summary>Shift Left Logical.</summary>
            SLL = 0b000000,
            /// <summary>Subtract Unsigned.</summary>
            SUBU = 0b100011,
            /// <summary>Exclusive Or.</summary>
            XOR = 0b100110,
            /// <summary>Move From HI.</summary>
            MFHI = 0b010000,
            /// <summary>Add Unsigned.</summary>
            ADDU = 0b100001,
            /// <summary>Set On Less Than Unsigned.</summary>
            SLTU = 0b101011,
            /// <summary>Shift Left Logical Variable.</summary>
            SLLV = 0b000100,
            /// <summary>Shift Right Logical Variable.</summary>
            SRLV = 0b000110,
            /// <summary>And.</summary>
            AND = 0b100100,
            /// <summary>Set On Less Than.</summary>
            SLT = 0b101010
        }
    }
}
