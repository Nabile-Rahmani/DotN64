namespace DotN64.CPU
{
    public partial class VR4300
    {
        public partial class SystemControlUnit
        {
            public enum FunctOpCode : byte
            {
                /// <summary>Write Indexed TLB Entry.</summary>
                TLBWI = 0b000010
            }
        }
    }
}
