namespace N64Emu.RCP
{
    public partial class RealityCoprocessor
    {
        #region properties
        public RealitySignalProcessor RSP { get; } = new RealitySignalProcessor();
        #endregion
    }
}
