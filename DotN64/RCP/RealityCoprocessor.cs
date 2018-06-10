using System.Collections.Generic;

namespace DotN64.RCP
{
    using Extensions;

    public partial class RealityCoprocessor
    {
        #region Fields
        private readonly Nintendo64 nintendo64;
        #endregion

        #region Properties
        public IReadOnlyList<MappingEntry> MemoryMaps { get; }

        public SignalProcessor SP { get; }

        public DisplayProcessor DP { get; }

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
            this.nintendo64 = nintendo64;
            SP = new SignalProcessor(this);
            DP = new DisplayProcessor(this);
            PI = new ParallelInterface(this);
            SI = new SerialInterface(this);
            AI = new AudioInterface(this);
            VI = new VideoInterface(this);
            MI = new MIPSInterface(this);
            RI = new RDRAMInterface(this);
            MemoryMaps = new[]
            {
                new MappingEntry(0x00000000, 0x03FFFFFF, false)
                {
                    Read = nintendo64.RAM.MemoryMaps.ReadWord,
                    Write = nintendo64.RAM.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04000000, 0x040FFFFF, false)
                {
                    Read = SP.MemoryMaps.ReadWord,
                    Write = SP.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04100000, 0x041FFFFF, false)
                {
                    Read = DP.MemoryMaps.ReadWord,
                    Write = DP.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04300000, 0x043FFFFF, false)
                {
                    Read = MI.MemoryMaps.ReadWord,
                    Write = MI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04400000, 0x044FFFFF, false)
                {
                    Read = VI.MemoryMaps.ReadWord,
                    Write = VI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04500000, 0x045FFFFF, false)
                {
                    Read = AI.MemoryMaps.ReadWord,
                    Write = AI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04600000, 0x046FFFFF, false)
                {
                    Read = PI.MemoryMaps.ReadWord,
                    Write = PI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04700000, 0x047FFFFF, false)
                {
                    Read = RI.MemoryMaps.ReadWord,
                    Write = RI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x04800000, 0x048FFFFF, false)
                {
                    Read = SI.MemoryMaps.ReadWord,
                    Write = SI.MemoryMaps.WriteWord
                },
                new MappingEntry(0x10000000, 0x1FBFFFFF, false)
                {
                    Read = PI.MemoryMaps.ReadWord
                },
                new MappingEntry(0x1FC00000, 0x1FC007FF, false)
                {
                    Read = nintendo64.PIF.MemoryMaps.ReadWord,
                    Write = nintendo64.PIF.MemoryMaps.WriteWord
                }
            };
        }
        #endregion
    }
}
