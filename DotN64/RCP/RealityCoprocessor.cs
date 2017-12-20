using System.Collections.Generic;

namespace DotN64.RCP
{
    using Extensions;

    public partial class RealityCoprocessor
    {
        #region Properties
        public IReadOnlyList<MappingEntry> MemoryMaps { get; }

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
            MemoryMaps = new[]
            {
                new MappingEntry(0x1FC00000, 0x1FC007BF, false) // PIF Boot ROM.
                {
                    Read = Nintendo64.PIF.MemoryMaps.ReadWord,
                    Write = Nintendo64.PIF.MemoryMaps.WriteWord
                },
                new MappingEntry(0x1FC007C0, 0x1FC007FF, false) // PIF (JoyChannel) RAM.
                {
                    Read = Nintendo64.PIF.MemoryMaps.ReadWord,
                    Write = Nintendo64.PIF.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04600000, 0x046FFFFF, false) // Peripheral interface (PI) registers.
                {
                    Read = PI.MemoryMaps.ReadWord,
                    Write = PI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04000000, 0x040FFFFF, false) // SP registers.
                {
                    Read = SP.MemoryMaps.ReadWord,
                    Write = SP.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04400000, 0x044FFFFF, false) // Video interface (VI) registers.
                {
                    Read = VI.MemoryMaps.ReadWord,
                    Write = VI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04500000, 0x045FFFFF, false) // Audio interface (AI) registers.
                {
                    Read = AI.MemoryMaps.ReadWord,
                    Write = AI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04300000, 0x043FFFFF, false) // MIPS interface (MI) registers.
                {
                    Read = MI.MemoryMaps.ReadWord,
                    Write = MI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04800000, 0x048FFFFF, false) // Serial interface (SI) registers.
                {
                    Read = SI.MemoryMaps.ReadWord,
                    Write = SI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x10000000, 0x1FBFFFFF, false) // Cartridge Domain 1 Address 2.
                {
                    Read = PI.MemoryMaps.ReadWord
                },
                new MappingEntry(0x04100000, 0x041FFFFF, false) // DP command registers.
                {
                    Read = DP.MemoryMaps.ReadWord,
                    Write = DP.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04700000, 0x047FFFFF, false) // RDRAM interface (RI) registers.
                {
                    Read = RI.MemoryMaps.ReadWord,
                    Write = RI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x00000000, 0x03EFFFFF, false) // RDRAM memory.
                {
                    Read = Nintendo64.RAM.MemoryMaps.ReadWord,
                    Write = Nintendo64.RAM.MemoryMaps.WriteWord
                },
                new MappingEntry(0x03F00000, 0x03FFFFFF, false) // RDRAM registers.
                {
                    Read = Nintendo64.RAM.MemoryMaps.ReadWord,
                    Write = Nintendo64.RAM.MemoryMaps.WriteWord
                }
            };
        }
        #endregion
    }
}
