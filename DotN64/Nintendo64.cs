namespace DotN64
{
    using CPU;
    using Extensions;
    using RCP;

    public class Nintendo64
    {
        #region Properties
        public VR4300 CPU { get; }

        public RealityCoprocessor RCP { get; }

        public RDRAM RAM { get; } = new RDRAM(new byte[0x00400000]); // The base system has 4 MB of RAM installed.

        public PeripheralInterface PIF { get; }

        public Cartridge Cartridge { get; set; }
        #endregion

        #region Constructors
        public Nintendo64()
        {
            PIF = new PeripheralInterface(this);
            RCP = new RealityCoprocessor(this);
            CPU = new VR4300
            {
                DivMode = 0b01, // Assuming this value as the CPU is clocked at 93.75 MHz, and the RCP would be clocked at 93.75 / 3 * 2 = 62.5 MHz.
                ReadSysAD = RCP.MemoryMaps.ReadWord,
                WriteSysAD = RCP.MemoryMaps.WriteWord
            };
        }
        #endregion

        #region Methods
        public void PowerOn()
        {
            CPU.Reset();
            PIF.Reset();
        }

        public void Run()
        {
            while (true)
            {
                CPU.Step();
            }
        }
        #endregion
    }
}
