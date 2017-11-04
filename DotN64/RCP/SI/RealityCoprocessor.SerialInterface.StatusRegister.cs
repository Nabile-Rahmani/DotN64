namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class SerialInterface
        {
            [System.Flags]
            public enum StatusRegister
            {
                DMABusy = 1 << 0,
                IOReadBusy = 1 << 1,
                DMAError = 1 << 3,
                Interrupt = 1 << 12
            }
        }
    }
}
