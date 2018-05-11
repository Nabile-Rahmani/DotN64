namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        public partial class DisplayProcessor
        {
            [System.Flags]
            public enum Statuses : ushort
            {
                XBusDMemDMA = 1 << 0,
                Freeze = 1 << 1,
                Flush = 1 << 2,
                StartGClk = 1 << 3,
                TMemBusy = 1 << 4,
                PipeBusy = 1 << 5,
                CmdBusy = 1 << 6,
                CBufReady = 1 << 7,
                DMABusy = 1 << 8,
                EndValid = 1 << 9,
                StartValid = 1 << 10
            }
        }
    }
}
