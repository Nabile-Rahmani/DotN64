using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace N64Emu
{
    using CPU;
    using RCP;
    using PI;
    using SI;
    using AI;
    using VI;

    public class Nintendo64
    {
        #region Fields
        private readonly IReadOnlyList<MappingEntry> memoryMaps;
        #endregion

        #region Properties
        public VR4300 CPU { get; }

        public RealityCoprocessor RCP { get; } = new RealityCoprocessor();

        public PeripheralInterface PI { get; } = new PeripheralInterface();

        public SerialInterface SI { get; } = new SerialInterface();

        public AudioInterface AI { get; } = new AudioInterface();

        public VideoInterface VI { get; } = new VideoInterface();

        public byte[] RAM { get; } = new byte[4 * 1024 * 1024]; // 4 MB of base memory (excludes the expansion pack).

        public Cartridge Cartridge { get; set; }
        #endregion

        #region Constructors
        public Nintendo64()
        {
            memoryMaps = new[]
            {
                new MappingEntry(0x1FC00000, 0x1FC007BF) // PIF Boot ROM.
                {
                    Read = o => (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(PI.BootROM, (int)o))
                },
                new MappingEntry(0x04001000, 0x04001FFF) // SP_IMEM read/write.
                {
                    Read = o => BitConverter.ToUInt32(RCP.SP.IMEM, (int)o),
                    Write = (o, v) => Array.Copy(BitConverter.GetBytes(v), 0, RCP.SP.IMEM, (int)o, sizeof(uint))
                },
                new MappingEntry(0x1FC007C0, 0x1FC007FF) // PIF (JoyChannel) RAM.
                {
                    Read = o => BitConverter.ToUInt32(PI.RAM, (int)o) // Somehow it expects something in particular from the very last bytes to be correctly set to continue execution.
                },
                new MappingEntry(0x04040010, 0x04040013) // SP status.
                {
                    Read = o => RCP.SP.StatusRegister,
                    Write = (o, v) => RCP.SP.StatusRegister = v
                },
                new MappingEntry(0x04040018, 0x0404001B) // SP DMA busy.
                {
                    Read = o => RCP.SP.DMABusyRegister
                },
                new MappingEntry(0x04600010, 0x04600013) // PI status.
                {
                    Write = (o, v) => PI.Status.Data = (byte)v
                },
                new MappingEntry(0x0440000C, 0x0440000F) // VI vertical intr.
                {
                    Write = (o, v) => VI.VerticalInterrupt = (ushort)v
                },
                new MappingEntry(0x04400024, 0x04400027) // VI horizontal video.
                {
                    Write = (o, v) => VI.HorizontalVideo = v
                },
                new MappingEntry(0x04400010, 0x04400013) // VI current vertical line.
                {
                    Write = (o, v) => VI.CurrentVerticalLine = (ushort)v
                },
                new MappingEntry(0x04500000, 0x04500003) // AI DRAM address.
                {
                    Write = (o, v) => AI.DRAMAddress = v
                },
                new MappingEntry(0x04500004, 0x04500007) // AI length.
                {
                    Write = (o, v) => AI.TransferLength = v
                },
                new MappingEntry(0x04000000, 0x04000FFF) // SP_DMEM read/write.
                {
                    Read = o => BitConverter.ToUInt32(RCP.SP.DMEM, (int)o),
                    Write = (o, v) => Array.Copy(BitConverter.GetBytes(v), 0, RCP.SP.DMEM, (int)o, sizeof(uint))
                },
                new MappingEntry(0x04300004, 0x04300007) // MI version.
                {
                    Write = (o, v) => { }
                },
                new MappingEntry(0x04800018, 0x0480001B) // SI status.
                {
                    Read = o => SI.Status.Data
                }
            };
            CPU = new VR4300(memoryMaps);
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

            uint versionAddress = 0x04300004;

            memoryMaps.First(e => e.Contains(versionAddress)).WriteWord(versionAddress, 0x01010101);

            for (int i = 0; i < 0x1000; i += sizeof(uint))
            {
                var dmemAddress = (ulong)(0x04000000 + i);

                memoryMaps.First(e => e.Contains(dmemAddress)).WriteWord(dmemAddress, BitConverter.ToUInt32(Cartridge.ROM, i));
            }

            CPU.ProgramCounter = 0xA4000040;
        }

        public void Initialise()
        {
            CPU.PowerOnReset();

            if (PI.BootROM == null)
                RunPIF();

            while (true)
            {
                CPU.Step();
            }
        }
        #endregion
    }
}
