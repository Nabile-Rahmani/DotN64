namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class FloatingPointUnit
        {
            public enum OpCode : byte
            {
                /// <summary>Move Control Word From FPU.</summary>
                CF = 0b00010,
                /// <summary>Move Control Word To FPU.</summary>
                CT = 0b00110
            }
        }
    }
}
