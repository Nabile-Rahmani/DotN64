namespace DotN64.RCP
{
    public partial class RealityCoprocessor
    {
        #region Properties
        public Nintendo64 Nintendo64 { get; }

        public SignalProcessor SP { get; } = new SignalProcessor();

        public DisplayProcessor DP { get; } = new DisplayProcessor();

        public ParallelInterface PI { get; }

        public SerialInterface SI { get; }

        public AudioInterface AI { get; }

        public VideoInterface VI { get; }

        public MIPSInterface MI { get; }

        public RDRAMInterface RI { get; }
        #endregion

        #region Constructors
        public RealityCoprocessor(Nintendo64 nintendo64)
        {
            Nintendo64 = nintendo64;
            PI = new ParallelInterface(this);
            SI = new SerialInterface(this);
            AI = new AudioInterface(this);
            VI = new VideoInterface(this);
            MI = new MIPSInterface(this);
            RI = new RDRAMInterface(this);
        }
        #endregion
    }
}
