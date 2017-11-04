namespace DotN64.RCP
{
    public partial class RealityCoprocessor
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
}
