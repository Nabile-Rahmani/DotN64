namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            public enum OpCode : byte
            {
                /// <summary>Move To System Control Coprocessor.</summary>
                MT = 0b00100,
                /// <summary>Move From System Control Coprocessor.</summary>
                MF = 0b00000,
                /// <summary>Coprocessor function.</summary>
                CO = 0b10000 // TODO: Instruction set details show that this is only a single bit. Other bits could be overwritten by COP instruction structures.
            }
        }
    }
}
