namespace N64Emu.RCP
{
    public partial class RealityCoprocessor
    {
        #region properties
        public SignalProcessor SP { get; } = new SignalProcessor();
        #endregion
    }
}
