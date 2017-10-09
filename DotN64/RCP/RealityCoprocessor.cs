namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        #region Properties
        public SignalProcessor SP { get; } = new SignalProcessor();

        public DisplayProcessor DP { get; } = new DisplayProcessor();
        #endregion
    }
}
