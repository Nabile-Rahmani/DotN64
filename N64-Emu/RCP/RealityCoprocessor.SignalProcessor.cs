namespace N64Emu.RCP
{
    public partial class RealityCoprocessor
    {
        public class SignalProcessor
        {
            #region Properties
            public uint StatusRegister { get; set; } = 1;

            public uint DMABusyRegister { get; set; }

            public byte[] IMEM { get; } = new byte[0x1000];

            public byte[] DMEM { get; } = new byte[0x1000];
            #endregion
        }
    }
}
