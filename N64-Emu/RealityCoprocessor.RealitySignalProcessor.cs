namespace N64Emu
{
    public partial class RealityCoprocessor
    {
        public class RealitySignalProcessor
        {
            #region Properties
            public uint StatusRegister { get; set; } = 1;

            public uint DMABusyRegister { get; set; }
            #endregion
        }
    }
}
