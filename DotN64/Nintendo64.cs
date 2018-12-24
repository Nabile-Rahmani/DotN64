using System;
using System.Threading;

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

        private Cartridge cartridge;
        public Cartridge Cartridge
        {
            get => cartridge;
            set
            {
                if (Cartridge == value)
                    return;

                cartridge = value;
                CartridgeSwapped?.Invoke(this, value);
            }
        }

        public IVideoOutput VideoOutput { get; set; }

        public Switch Power { get; private set; }

        public Diagnostics.Debugger Debugger { get; set; }
        #endregion

        #region Events
        public event Action<Nintendo64, Cartridge> CartridgeSwapped;
        #endregion

        #region Constructors
        public Nintendo64()
        {
            PIF = new PeripheralInterface(this);
            RCP = new RealityCoprocessor(this);
            CPU = new VR4300
            {
                DivMode = 0b01,
                MasterClock = 62.5 * Math.Pow(10, 6),
                ReadSysAD = RCP.MemoryMaps.ReadWord,
                WriteSysAD = RCP.MemoryMaps.WriteWord
            };
        }
        #endregion

        #region Methods
        public void PowerOn()
        {
            Power = Switch.On;

            CPU.Reset();
            PIF.Reset();
        }

        public void PowerOff() => Power = Switch.Off;

        public void Run()
        {
            var cpuThread = new Thread(() =>
            {
                if (Debugger == null)
                {
                    while (Power == Switch.On)
                    {
                        if (Debugger == null)
                            CPU.Cycle();
                        else
                        {
                            Debugger.Run(true);
                            Debugger = null;
                        }
                    }
                }
                else
                {
                    Debugger.Run(true);
                    PowerOff();
                }
            })
            { Name = nameof(VR4300) };
            cpuThread.Start();

            if (VideoOutput != null)
            {
                while (Power == Switch.On && VideoOutput != null)
                {
                    VideoOutput.Draw(new VideoFrame(RCP.VI), RCP.VI, RAM);
                }
            }

            cpuThread.Join();
        }
        #endregion
    }
}
