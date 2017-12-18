using System.Collections.Generic;

namespace DotN64
{
    using CPU;
    using Extensions;
    using RCP;

    public class Nintendo64
    {
        #region Properties
        public IReadOnlyList<MappingEntry> MemoryMaps { get; }

        public VR4300 CPU { get; }

        public RealityCoprocessor RCP { get; }

        public RDRAM RAM { get; } = new RDRAM(new byte[0x00400000]); // The base system has 4 MB of RAM installed.

        public PeripheralInterface PIF { get; }

        public Cartridge Cartridge { get; set; }
        #endregion

        #region Constructors
        public Nintendo64()
        {
            RCP = new RealityCoprocessor(this);
            PIF = new PeripheralInterface(this);
            MemoryMaps = new[]
            {
                new MappingEntry(0x1FC00000, 0x1FC007BF, false) // PIF Boot ROM.
                {
                    Read = PIF.MemoryMaps.ReadWord,
                    Write = PIF.MemoryMaps.WriteWord
                },
                new MappingEntry(0x1FC007C0, 0x1FC007FF, false) // PIF (JoyChannel) RAM.
                {
                    Read = PIF.MemoryMaps.ReadWord,
                    Write = PIF.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04600000, 0x046FFFFF, false) // Peripheral interface (PI) registers.
                {
                    Read = RCP.PI.MemoryMaps.ReadWord,
                    Write = RCP.PI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04000000, 0x040FFFFF, false) // SP registers.
                {
                    Read = RCP.SP.MemoryMaps.ReadWord,
                    Write = RCP.SP.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04400000, 0x044FFFFF, false) // Video interface (VI) registers.
                {
                    Read = RCP.VI.MemoryMaps.ReadWord,
                    Write = RCP.VI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04500000, 0x045FFFFF, false) // Audio interface (AI) registers.
                {
                    Read = RCP.AI.MemoryMaps.ReadWord,
                    Write = RCP.AI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04300000, 0x043FFFFF, false) // MIPS interface (MI) registers.
                {
                    Read = RCP.MI.MemoryMaps.ReadWord,
                    Write = RCP.MI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04800000, 0x048FFFFF, false) // Serial interface (SI) registers.
                {
                    Read = RCP.SI.MemoryMaps.ReadWord,
                    Write = RCP.SI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x10000000, 0x1FBFFFFF, false) // Cartridge Domain 1 Address 2.
                {
                    Read = RCP.PI.MemoryMaps.ReadWord
                },
                new MappingEntry(0x04100000, 0x041FFFFF, false) // DP command registers.
                {
                    Read = RCP.DP.MemoryMaps.ReadWord,
                    Write = RCP.DP.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04700000, 0x047FFFFF, false) // RDRAM interface (RI) registers.
                {
                    Read = RCP.RI.MemoryMaps.ReadWord,
                    Write = RCP.RI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x00000000, 0x03EFFFFF, false) // RDRAM memory.
                {
                    Read = RAM.MemoryMaps.ReadWord,
                    Write = RAM.MemoryMaps.WriteWord
                },
                new MappingEntry(0x03F00000, 0x03FFFFFF, false) // RDRAM registers.
                {
                    Read = RAM.MemoryMaps.ReadWord,
                    Write = RAM.MemoryMaps.WriteWord
                }
            };
            CPU = new VR4300
            {
                DivMode = 0b01, // Assuming this value as the CPU is clocked at 93.75 MHz, and the RCP would be clocked at 93.75 / 3 * 2 = 62.5 MHz.
                ReadSysAD = MemoryMaps.ReadWord,
                WriteSysAD = MemoryMaps.WriteWord
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
