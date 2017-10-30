namespace DotN64.PI
{
    public partial class PeripheralInterface
    {
        [System.Flags]
        public enum StatusRegister
        {
            DMABusy = 1 << 0,
            IOBusy = 1 << 1,
            Error = 1 << 2
        }
    }
}
