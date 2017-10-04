namespace N64Emu.RCP
{
    public partial class RealityCoprocessor
    {
        #region Properties
        public SignalProcessor SP { get; } = new SignalProcessor();

        public DisplayProcessor DP { get; } = new DisplayProcessor();
        #endregion
    }
}
