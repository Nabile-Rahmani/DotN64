namespace DotN64.CPU
{
    public partial class VR4300
    {
        public enum RegImmOpCode : byte
        {
            /// <summary>Branch On Greater Than Or Equal To Zero And Link.</summary>
            BGEZAL = 0b10001
        }
    }
}
