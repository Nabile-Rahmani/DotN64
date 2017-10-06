using System;
using System.Collections.Generic;
using System.Linq;

namespace N64Emu
{
    using AI;
    using CPU;
    using MI;
    using PI;
    using RCP;
    using SI;
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

        public MIPSInterface MI { get; } = new MIPSInterface();

        public byte[] RAM { get; } = new byte[4 * 1024 * 1024]; // 4 MB of base memory (excludes the expansion pack).

        public Cartridge Cartridge { get; set; }
        #endregion

        #region Constructors
        public Nintendo64()
        {
            memoryMaps = new[]
            {
                new MappingEntry(0x1FC00000, 0x1FC007BF, false) // PIF Boot ROM.
                {
                    Read = PI.ReadWord,
                    Write = PI.WriteWord
                },
                new MappingEntry(0x1FC007C0, 0x1FC007FF, false) // PIF (JoyChannel) RAM.
                {
                    Read = PI.ReadWord,
                    Write = PI.WriteWord
                },
                new MappingEntry(0x04600000, 0x046FFFFF, false) // Peripheral interface (PI) registers.
                {
                    Read = PI.ReadWord,
                    Write = PI.WriteWord
                },
                new MappingEntry(0x04000000, 0x040FFFFF, false) // SP registers.
                {
                    Read = RCP.SP.ReadWord,
                    Write = RCP.SP.WriteWord
                },
                new MappingEntry(0x04400000, 0x044FFFFF, false) // Video interface (VI) registers.
                {
                    Read = VI.ReadWord,
                    Write = VI.WriteWord
                },
                new MappingEntry(0x04500000, 0x045FFFFF, false) // Audio interface (AI) registers.
                {
                    Read = AI.ReadWord,
                    Write = AI.WriteWord
                },
                new MappingEntry(0x04300000, 0x043FFFFF, false) // MIPS interface (MI) registers.
                {
                    Read = MI.ReadWord,
                    Write = MI.WriteWord
                },
                new MappingEntry(0x04800000, 0x048FFFFF, false) // Serial interface (SI) registers.
                {
                    Read = SI.ReadWord,
                    Write = SI.WriteWord
                },
                new MappingEntry(0x10000000, 0x1FBFFFFF) // Cartridge Domain 1 Address 2.
                {
                    Read = o => BitConverter.ToUInt32(Cartridge.ROM, (int)o)
                },
                new MappingEntry(0x04100000, 0x041FFFFF, false) // DP command registers.
                {
                    Read = RCP.DP.ReadWord,
                    Write = RCP.DP.WriteWord
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
            CPU.GPR[(int)VR4300.GPRIndex.s4] = 0x1;
            CPU.GPR[(int)VR4300.GPRIndex.s6] = 0x3F;
            CPU.GPR[(int)VR4300.GPRIndex.sp] = 0xA4001FF0;
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

            CPU.PC = 0xA4000040;
        }

        public void PowerOn()
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
