namespace DotN64.CPU
{
    public partial class VR4300
    {
        public enum RegImmOpCode : byte
        {
            /// <summary>Branch On Greater Than Or Equal To Zero And Link.</summary>
            BGEZAL = 0b10001,
            /// <summary>Branch On Greater Than Or Equal To Zero Likely.</summary>
            BGEZL = 0b00011,
            /// <summary>Branch On Less Than Zero.</summary>
            BLTZ = 0b00000,
            /// <summary>Branch On Greater Than Or Equal To Zero.</summary>
            BGEZ = 0b00001
        }
    }
}
