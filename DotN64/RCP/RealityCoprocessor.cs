namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        #region Properties
        public SignalProcessor SP { get; } = new SignalProcessor();

        public DisplayProcessor DP { get; } = new DisplayProcessor();

        public PeripheralInterface PI { get; } = new PeripheralInterface();

        public SerialInterface SI { get; } = new SerialInterface();

        public AudioInterface AI { get; } = new AudioInterface();

        public VideoInterface VI { get; } = new VideoInterface();

        public MIPSInterface MI { get; } = new MIPSInterface();

        public RDRAMInterface RI { get; } = new RDRAMInterface();
        #endregion
    }
}
