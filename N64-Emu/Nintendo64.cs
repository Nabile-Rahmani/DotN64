using System;
using System.Collections.Generic;
using System.Net;

namespace N64Emu
{
    using CPU;
    using Interfaces.Audio;
    using Interfaces.Peripheral;
    using Interfaces.Video;
    using RCP;

    public partial class Nintendo64
    {
        #region Fields
        public readonly IReadOnlyList<MappingEntry> MemoryMaps;
        #endregion

        #region Properties
        public VR4300 CPU { get; }

        public RealityCoprocessor RCP { get; } = new RealityCoprocessor();

        public PeripheralInterface PI { get; } = new PeripheralInterface();

        public VideoInterface VI { get; } = new VideoInterface();

        public AudioInterface AI { get; } = new AudioInterface();

        public byte[] RAM { get; } = new byte[4 * 1024 * 1024]; // 4 MB of base memory (excludes the expansion pack).

        public byte[] PIFROM { get; set; }

        public Cartridge Cartridge { get; set; }
        #endregion

        #region Constructors
        public Nintendo64()
        {
            CPU = new VR4300(this);
            MemoryMaps = new[]
            {
                new MappingEntry // PIF Boot ROM.
                {
                    StartAddress = 0x1FC00000,
                    EndAddress = 0x1FC007BF,
                    ReadWord = (e, a) => (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(PIFROM, (int)((uint)a - e.StartAddress)))
                },
                new MappingEntry // SP_IMEM read/write.
                {
                    StartAddress = 0x04001000,
                    EndAddress = 0x04001FFF,
                    WriteWord = (e, a, v) => { } // What's that for ?
                },
                new MappingEntry // PIF (JoyChannel) RAM.
                {
                    StartAddress = 0x1FC007C0,
                    EndAddress = 0x1FC007FF,
                    ReadWord = (e, a) => 0 // Somehow it expects something in particular from the very last bytes to be correctly set to continue execution.
                },
                new MappingEntry // SP status.
                {
                    StartAddress = 0x04040010,
                    EndAddress = 0x04040013,
                    ReadWord = (e, a) => RCP.RSP.StatusRegister,
                    WriteWord = (e, a, v) => RCP.RSP.StatusRegister = v
                },
                new MappingEntry // SP DMA busy.
                {
                    StartAddress = 0x04040018,
                    EndAddress = 0x0404001B,
                    ReadWord = (e, a) => RCP.RSP.DMABusyRegister
                },
                new MappingEntry // PI status.
                {
                    StartAddress = 0x04600010,
                    EndAddress = 0x04600013,
                    WriteWord = (e, a, v) => PI.Status.Data = (byte)v
                },
                new MappingEntry // VI vertical intr.
                {
                    StartAddress = 0x0440000C,
                    EndAddress = 0x0440000F,
                    WriteWord = (e, a, v) => VI.VerticalInterrupt = (ushort)v
                },
                new MappingEntry // VI horizontal video.
                {
                    StartAddress = 0x04400024,
                    EndAddress = 0x04400027,
                    WriteWord = (e, a, v) => VI.HorizontalVideo = v
                },
                new MappingEntry // VI current vertical line.
                {
                    StartAddress = 0x04400010,
                    EndAddress = 0x04400010 + sizeof(ushort),
                    WriteWord = (e, a, v) => VI.CurrentVerticalLine = (ushort)v
                },
                new MappingEntry // AI DRAM address.
                {
                    StartAddress = 0x04500000,
                    EndAddress = 0x04500003,
                    WriteWord = (e, a, v) => AI.DRAMAddress = v
                },
                new MappingEntry // AI length.
                {
                    StartAddress = 0x04500004,
                    EndAddress = 0x04500007,
                    WriteWord = (e, a, v) => AI.TransferLength = v
                }
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Runs the PIF ROM.
        /// See: http://www.emulation64.com/ultra64/bootn64.html
        /// </summary>
        private void RunPIF()
        {
            CPU.GPRegisters[(int)VR4300.GPRegisterIndex.s4] = 0x1;
            CPU.GPRegisters[(int)VR4300.GPRegisterIndex.s6] = 0x3F;
            CPU.GPRegisters[(int)VR4300.GPRegisterIndex.sp] = 0xA4001FF0;
            CPU.CP0.Registers[(int)VR4300.SystemControlUnit.RegisterIndex.Random] = 0x0000001F;
            CPU.CP0.Registers[(int)VR4300.SystemControlUnit.RegisterIndex.Status] = 0x70400004;
            CPU.CP0.Registers[(int)VR4300.SystemControlUnit.RegisterIndex.PRId] = 0x00000B00;
            CPU.CP0.Registers[(int)VR4300.SystemControlUnit.RegisterIndex.Config] = 0x0006E463;

            Array.Copy(BitConverter.GetBytes(0x01010101), 0, RAM, (uint)CPU.MapMemory(0x04300004), sizeof(int));
            Array.Copy(Cartridge.ROM, 0, RAM, (uint)CPU.MapMemory(0xA4000000), 0x1000);

            CPU.ProgramCounter = CPU.MapMemory(0xA4000040);
        }

        public void Initialise()
        {
            CPU.PowerOnReset();

            if (PIFROM == null)
                RunPIF();

            while (true)
            {
                CPU.Step();
            }
        }
        #endregion
    }
}
